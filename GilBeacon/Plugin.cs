using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.Command;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Lumina.Excel.Sheets;
using Roxyz0501.Dalamud.CommonUi;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Info;
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
    private bool open;
    private uint cachedItem;
    private uint sellPrice;
    private readonly CommandInfo command;

    public Plugin(IDalamudPluginInterface pi, ICommandManager commands, IGameGui gui, IDataManager data, IClientState client)
    {
        this.pi = pi; this.commands = commands; this.gui = gui; this.data = data;
        config = pi.GetPluginConfig() as Configuration ?? new();
        Loc.CheckCompleteness();
        if (!Loc.IsSupported(config.Language))
        {
            Dalamud.Game.ClientLanguage? game;
            try { game = client.ClientLanguage; } catch { game = null; }
            config.Language = Loc.FromGame(game) ?? Loc.Normalize(pi.UiLanguage) ?? "en";
            pi.SavePluginConfig(config);
        }
        fonts = new LocalizedFonts(pi.UiBuilder.FontAtlas, []);
        command = new CommandInfo((_, _) => Open()) { HelpMessage = T("CommandHelp") };
        commands.AddHandler("/gilbeacon", command);
        pi.UiBuilder.Draw += Draw;
        pi.UiBuilder.OpenMainUi += Open;
        pi.UiBuilder.OpenConfigUi += Open;
    }
    private string T(string key) => Loc.Get(config.Language, key);
    private void Open() => open = true;
    private void Draw()
    {
        DrawHighlights();
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
                    { config.Enabled = enabled; pi.SavePluginConfig(config); }
                    ImGui.TextWrapped(T("Explanation"));
                }, T("LanguageSettings"), () =>
                {
                    var changed = StandardPluginTabs.DrawLanguageSelector(T("Language"), T("LanguageHelp"), config.Language!);
                    if (changed != null)
                    {
                        config.Language = changed; pi.SavePluginConfig(config);
                        command.HelpMessage = T("CommandHelp");
                    }
                }, T("Support"), () => StandardPluginTabs.DrawSupportContent(T("SupportHeading"), T("SupportText"), T("SupportRecipient"), T("OpenKoFi")));
                ImGui.EndTabBar();
            }
        }
        ImGui.End();
    }
    private void DrawHighlights()
    {
        if (!config.Enabled) return;
        var addon = gui.GetAddonByName<AddonItemSearchResult>("ItemSearchResult");
        if (addon == null || !addon->IsVisible || !addon->IsReady || addon->Results == null) { cachedItem = 0; return; }
        var module = InfoModule.Instance();
        if (module == null) return;
        var proxy = module->GetInfoProxyItemSearch();
        if (proxy == null || proxy->WaitingForListings || proxy->SearchItemId == 0 || proxy->ListingCount > 100) return;
        var list = addon->Results;
        if (list->IsUpdatePending || list->IsScrollRefreshPending || list->ListLength != proxy->ListingCount) return;
        if (cachedItem != proxy->SearchItemId)
        {
            cachedItem = proxy->SearchItemId;
            sellPrice = data.GetExcelSheet<Item>().TryGetRow(cachedItem, out var item) ? item.PriceLow : 0;
        }
        if (sellPrice == 0 || list->NumVisibleItems is < 1 or > 100) return;
        int first = Math.Max(0, list->FirstVisibleItemIndex);
        int end = Math.Min((int)proxy->ListingCount, first + list->NumVisibleItems);
        for (int i = first; i < end; i++)
        {
            var offer = proxy->Listings[i];
            if (offer.ListingId == 0 || offer.ItemId != cachedItem || offer.IsMannequin || offer.IsSellingAsSet) continue;
            ulong profit = Profit.Calculate(sellPrice, offer.UnitPrice, offer.Quantity, offer.TotalTax, offer.IsHqItem);
            if (profit == 0) continue;
            var renderer = list->GetItemRenderer(i);
            if (renderer == null || renderer->ListItemIndex != i || renderer->OwnerNode == null) continue;
            var node = &renderer->OwnerNode->AtkResNode;
            if (!node->IsVisible()) continue;
            Bounds bounds = default;
            node->GetBounds(&bounds);
            if (bounds.Width is < 40 or > 2000 || bounds.Height is < 8 or > 150) continue;
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
        pi.UiBuilder.Draw -= Draw;
        pi.UiBuilder.OpenMainUi -= Open;
        pi.UiBuilder.OpenConfigUi -= Open;
        commands.RemoveHandler("/gilbeacon");
        fonts.Dispose();
    }
}

