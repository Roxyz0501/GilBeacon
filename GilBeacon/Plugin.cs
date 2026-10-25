using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.Command;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Lumina.Excel.Sheets;
using Roxyz0501.Dalamud.CommonUi;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Info;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using Bounds = FFXIVClientStructs.FFXIV.Common.Math.Bounds;

namespace GilBeacon;

public sealed unsafe class Plugin : IDalamudPlugin
{
    private readonly IDalamudPluginInterface pi;
    private readonly ICommandManager commands;
    private readonly IGameGui gui;
    private readonly IDataManager data;
    private readonly Configuration config;
    private readonly LocalizedFonts fonts;
    private readonly ConfigStore settings;
    private readonly IFramework framework;
    private readonly IClientState client;
    private readonly IPluginLog log;
    private bool disposed;
    private string? reportedError;
    private bool open;
    private readonly ItemPriceCache prices = new();
    private readonly HighlightFailures highlightFailures = new();
    private readonly Func<uint, VendorInfo> readVendor;
    private readonly CommandInfo command;

    public Plugin(IDalamudPluginInterface pi, ICommandManager commands, IGameGui gui, IDataManager data, IClientState client, IFramework framework, IPluginLog log)
    {
        this.pi = pi; this.commands = commands; this.gui = gui; this.data = data;
        readVendor = ReadVendor;
        this.framework = framework; this.client = client; this.log = log;
        settings = new ConfigStore(pi.ConfigFile.FullName);
        var loaded = settings.Current;
        config = new Configuration { Version = loaded.Version, Language = loaded.Language, Enabled = loaded.Enabled };
        Loc.CheckCompleteness();
        if (!Loc.IsSupported(config.Language))
        {
            Dalamud.Game.ClientLanguage? game;
            try { game = client.ClientLanguage; } catch { game = null; }
            config.Language = Loc.FromGame(game) ?? Loc.Normalize(pi.UiLanguage) ?? "en";
            Changed();
        }
        fonts = new LocalizedFonts(pi.UiBuilder.FontAtlas, []);
        command = new CommandInfo((_, _) => Open()) { HelpMessage = T("CommandHelp") };
        commands.AddHandler("/gilbeacon", command);
        pi.UiBuilder.Draw += Draw;
        pi.UiBuilder.OpenMainUi += Open;
        pi.UiBuilder.OpenConfigUi += Open;
        framework.Update += Update;
        client.Logout += Logout;
        if (settings.LoadNotice != null || settings.LoadBlocked) open = true;
    }
    private string T(string key) => Loc.Get(config.Language, key);
    private void Open() { if (!disposed) open = true; }
    private void Changed() => settings.Change(new(config.Version, config.Language, config.Enabled));
    private void Update(IFramework _) { if (!disposed) { settings.Tick(); ReportSaveError(); } }
    private void Logout(int type, int code) { if (!disposed) { settings.Flush(); ReportSaveError(); } }
    private void ReportSaveError()
    {
        if (settings.Error == null) { reportedError = null; return; }
        if (reportedError != null && !settings.Stopped) return;
        var state = settings.Stopped ? "stopped" : "retrying";
        if (reportedError == state) return;
        reportedError = state;
        open = true;
        log.Warning("Gil Beacon settings {State}: {Error}", state, settings.Error);
    }
    private void Draw()
    {
        if (disposed) return;
        if (highlightFailures.CanAttempt)
        {
            try { DrawHighlights(); highlightFailures.Reset(); }
            catch (Exception ex)
            {
                prices.Clear();
                highlightFailures.Failed(ex);
                if (highlightFailures.Count == 1 || highlightFailures.Stopped)
                { log.Warning(ex, "Gil Beacon highlight read failed ({Count}/3).", highlightFailures.Count); open = true; }
            }
        }
        if (!open) return;
        using var font = fonts.Push(config.Language);
        ImGui.SetNextWindowSize(new Vector2(650, 330), ImGuiCond.FirstUseEver);
        if (ImGui.Begin("Gil Beacon###GilBeacon", ref open))
        {
            if (ImGui.BeginTabBar("Tabs"))
            {
                StandardPluginTabs.DrawStandardTrailingTabs(T("Settings"), "Settings", () =>
                {
                    var enabled = config.Enabled;
                    if (ImGui.Checkbox(T("HighlightEnabled") + "###Enabled", ref enabled))
                    { config.Enabled = enabled; Changed(); }
                    ImGui.TextWrapped(T("Explanation"));
                    if (highlightFailures.Error != null)
                    {
                        ImGui.TextWrapped(T("HighlightError"));
                        ImGui.TextWrapped(highlightFailures.Error);
                        if (ImGui.Button(T("RetryHighlight") + "###RetryHighlight"))
                        { prices.Clear(); highlightFailures.Reset(); }
                    }
                    ImGui.TextWrapped(T("SavePolicy"));
                    if (settings.LoadNotice is { } notice) ImGui.TextWrapped(T(notice));
                    if (settings.Error != null)
                    {
                        ImGui.TextWrapped(T(settings.LoadBlocked ? "LoadBlocked" : settings.Stopped ? "SaveStopped" : "SaveFailed"));
                        ImGui.TextWrapped(settings.Error);
                        if (!settings.LoadBlocked && ImGui.Button(T("RetrySave") + "###RetrySave"))
                        { settings.Retry(); ReportSaveError(); }
                    }
                }, T("LanguageSettings"), () =>
                {
                    var changed = StandardPluginTabs.DrawLanguageSelector(T("Language"), T("LanguageHelp"), config.Language!);
                    if (changed != null)
                    {
                        config.Language = changed; Changed();
                        command.HelpMessage = T("CommandHelp");
                    }
                }, T("Support"), () => StandardPluginTabs.DrawSupportContent(T("SupportHeading"), T("SupportText"), T("SupportRecipient"), T("OpenKoFi")));
                ImGui.EndTabBar();
            }
        }
        ImGui.End();
    }
    private VendorInfo ReadVendor(uint id)
    {
        if (!data.GetExcelSheet<Item>().TryGetRow(id, out var item)) return default;
        return new(item.PriceLow, item.CanBeHq, item.ItemUICategory.RowId == 58);
    }
    private void DrawHighlights()
    {
        if (!config.Enabled) return;
        var addon = gui.GetAddonByName<AddonItemSearchResult>("ItemSearchResult");
        if (addon == null || !addon->IsVisible || !addon->IsReady || addon->Results == null) { prices.Clear(); return; }
        var module = InfoModule.Instance();
        if (module == null) return;
        var proxy = module->GetInfoProxyItemSearch();
        if (proxy == null) return;
        var list = addon->Results;
        if (!MarketReadiness.CanDraw(proxy->SearchItemId, proxy->ListingCount, list->ListLength,
            addon->IsReady, list->IsUpdatePending, list->IsScrollRefreshPending)) return;
        var agent = AgentItemSearch.Instance();
        if (agent == null || !MarketReadiness.MatchesItem(proxy->SearchItemId, agent->ResultItemId)) return;
        uint cachedItem = proxy->SearchItemId;
        var vendor = prices.Get(cachedItem, readVendor);
        uint sellPrice = vendor.Price;
        bool canHq = vendor.CanHq, materia = vendor.Materia;
        if (sellPrice == 0 || list->NumVisibleItems is < 1 or > 100) return;
        int first = Math.Max(0, list->FirstVisibleItemIndex);
        int count = Math.Min((int)proxy->ListingCount, list->ListLength);
        int end = Math.Min(count, first + list->NumVisibleItems);
        // Sort only locally available row indices. No request or wait for missing pages.
        Span<int> indices = stackalloc int[100];
        for (int n = 0; n < count; n++)
        {
            int at = n;
            while (at > 0 && proxy->Listings[indices[at - 1]].UnitPrice > proxy->Listings[n].UnitPrice)
            { indices[at] = indices[at - 1]; at--; }
            indices[at] = n;
        }
        for (int n = 0; n < count; n++)
        {
            int i = indices[n];
            var offer = proxy->Listings[i];
            if (offer.ListingId == 0 || offer.ItemId != cachedItem || offer.IsMannequin || offer.IsSellingAsSet) continue;
            if (MarketReadiness.BeyondProfitRange(offer.UnitPrice, sellPrice, canHq, materia)) break;
            if (i < first || i >= end) continue;
            var renderer = list->GetItemRenderer(i);
            if (renderer == null || renderer->ListItemIndex != i || renderer->OwnerNode == null) continue;
            var hqNode = renderer->GetImageNodeById(3);
            var priceNode = renderer->GetTextNodeById(5);
            var quantityNode = renderer->GetTextNodeById(6);
            if (hqNode == null || priceNode == null || quantityNode == null || !MarketReadiness.MatchesRow(
                priceNode->NodeText.ToString(), quantityNode->NodeText.ToString(), hqNode->AtkResNode.IsVisible(), offer.UnitPrice, offer.Quantity, offer.IsHqItem)) continue;
            ulong profit = Profit.Calculate(sellPrice, offer.UnitPrice, offer.Quantity, offer.TotalTax, offer.IsHqItem, materia);
            if (profit == 0) continue;
            var node = &renderer->OwnerNode->AtkResNode;
            if (!node->IsVisible()) continue;
            Bounds bounds = default;
            node->GetBounds(&bounds);
            if (!MarketReadiness.UsableBounds(bounds.Pos1.X, bounds.Pos1.Y, bounds.Pos2.X, bounds.Pos2.Y)) continue;
            var min = new Vector2(bounds.Pos1.X, bounds.Pos1.Y);
            var max = new Vector2(bounds.Pos2.X, bounds.Pos2.Y);
            var draw = ImGui.GetForegroundDrawList();
            draw.AddRectFilled(min, max, 0x2240DD60, 2);
            draw.AddRect(min, max, 0xFF40DD60, 2, ImDrawFlags.None, 2);
            var label = "+" + profit.ToString("N0", System.Globalization.CultureInfo.InvariantCulture) + " G";
            var size = ImGui.CalcTextSize(label);
            var position = new Vector2(max.X + 8, min.Y + Math.Max(0, (max.Y - min.Y - size.Y) / 2));
            draw.AddRectFilled(position - new Vector2(4, 2), position + size + new Vector2(4, 2), 0xDF181818, 3);
            draw.AddText(position, 0xFF80EF90, label);
            // Passive overlay: no game memory writes, input capture, purchase, or requests.
        }
    }
    public void Dispose()
    {
        if (disposed) return;
        disposed = true;
        framework.Update -= Update;
        client.Logout -= Logout;
        pi.UiBuilder.Draw -= Draw;
        pi.UiBuilder.OpenMainUi -= Open;
        pi.UiBuilder.OpenConfigUi -= Open;
        commands.RemoveHandler("/gilbeacon");
        try { settings.Dispose(); ReportSaveError(); }
        finally { fonts.Dispose(); }
    }
}

