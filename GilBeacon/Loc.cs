using Dalamud.Game;

namespace GilBeacon;

public static class Loc
{
    public static readonly (string Code, string Native)[] Languages =
    [
        ("ja", "日本語"), ("en", "English"), ("de", "Deutsch"), ("fr", "Français"),
        ("ko", "한국어"), ("zh-Hans", "简体中文"), ("zh-Hant", "繁體中文")
    ];

    private static readonly Dictionary<string, string[]> Text = new()
    {
        ["Explanation"] = ["利益の出る行を緑色で強調し、右側に手数料控除後の利益総額を表示します。HQはNPC単価を10%加算し、1個ごとに端数を切り上げます。マテリア商品にも買取額補正を適用します。装着マテリア加算・特殊買取・セット販売は対象外です。", "Green rows show total profit after purchase tax on the right. HQ NPC value includes a 10% bonus, rounded up per item. Materia items also receive the resale correction. Attached-materia bonuses, special vendors and set sales are excluded.", "Grüne Zeilen zeigen rechts den Gesamtgewinn nach Kaufsteuer. HQ: 10% NPC-Aufschlag, pro Stück aufgerundet. Materia-Gegenstände erhalten ebenfalls die Verkaufspreiskorrektur. Boni durch eingesetzte Materia, Sonderankäufe und Sets sind ausgeschlossen.", "Les lignes vertes affichent à droite le bénéfice total après taxe. HQ : bonus PNJ de 10%, arrondi au supérieur par objet. Le prix des objets matérias est aussi corrigé. Bonus des matérias serties, rachats spéciaux et ensembles exclus.", "이익인 행을 녹색으로 표시하고 오른쪽에 세금 차감 후 총이익을 표시합니다. HQ NPC 단가는 10% 가산 후 개당 올림합니다. 마테리아 아이템에도 매입가 보정을 적용합니다. 장착 마테리아 보너스, 특별 매입, 세트 판매는 제외됩니다.", "绿色行右侧显示扣税后的总利润。优质物品NPC单价增加10%，每件向上取整。魔晶石物品也进行收购价修正。不含镶嵌魔晶石加成、特殊收购和套装出售。", "綠色行右側顯示扣稅後的總利潤。優質物品NPC單價增加10%，每件向上取整。魔晶石物品也進行收購價修正。不含鑲嵌魔晶石加成、特殊收購和套裝出售。"],
        ["CommandHelp"] = ["Gil Beacon を開きます。", "Open Gil Beacon.", "Gil Beacon öffnen.", "Ouvrir Gil Beacon.", "Gil Beacon을 엽니다.", "打开 Gil Beacon。", "開啟 Gil Beacon。"],
        ["Settings"] = ["設定", "Settings", "Einstellungen", "Paramètres", "설정", "设置", "設定"],
        ["LanguageSettings"] = ["言語設定", "Language settings", "Spracheinstellungen", "Paramètres de langue", "언어 설정", "语言设置", "語言設定"],
        ["Support"] = ["支援", "Support", "Unterstützen", "Soutien", "후원", "支持", "支援"],
        ["Language"] = ["言語 / Language", "Language / 言語", "Sprache / Language", "Langue / Language", "언어 / Language", "语言 / Language", "語言 / Language"],
        ["LanguageHelp"] = ["表示言語を選択します。変更はすぐに反映され、次回起動後も保持されます。", "Choose the display language. Changes apply immediately and remain after restarting.", "Wähle die Anzeigesprache. Die Änderung gilt sofort und bleibt nach dem Neustart erhalten.", "Choisissez la langue d’affichage. Le changement s’applique aussitôt et reste enregistré après le redémarrage.", "표시 언어를 선택하세요. 변경 사항은 즉시 적용되며 다시 시작해도 유지됩니다.", "选择显示语言。更改会立即生效，并在重新启动后保留。", "選擇顯示語言。變更會立即生效，重新啟動後仍會保留。"],
        ["HighlightEnabled"] = ["ハイライトを有効にする", "Enable highlighting", "Hervorhebung aktivieren", "Activer le surlignage", "강조 표시 사용", "启用高亮", "啟用醒目提示"],
        ["SupportText"] = ["Roxyz0501 を任意で支援できます。", "You can optionally support Roxyz0501.", "Du kannst Roxyz0501 freiwillig unterstützen.", "Vous pouvez soutenir Roxyz0501 si vous le souhaitez.", "원한다면 Roxyz0501을 후원할 수 있습니다.", "你可以自愿支持 Roxyz0501。", "你可以自願支持 Roxyz0501。"],
        ["SupportHeading"] = ["任意の支援", "Optional support", "Freiwillige Unterstützung", "Soutien facultatif", "선택적 후원", "自愿支持", "自願支持"],
        ["SupportRecipient"] = ["支援先: Roxyz0501", "Recipient: Roxyz0501", "Empfänger: Roxyz0501", "Bénéficiaire : Roxyz0501", "후원 받는 사람: Roxyz0501", "收款人：Roxyz0501", "受款人：Roxyz0501"],
        ["OpenKoFi"] = ["Ko-fi を開く", "Open Ko-fi", "Ko-fi öffnen", "Ouvrir Ko-fi", "Ko-fi 열기", "打开 Ko-fi", "開啟 Ko-fi"],
    };

    public static IEnumerable<string> AllStrings => Text.Values.SelectMany(x => x);
    public static string FontText => string.Concat(AllStrings) + string.Concat(Languages.Select(x => x.Native))
        + "—…\u202F\u00A0";
    public static ClientLanguage GameDataLanguage(string? code) => code switch
    {
        "ja" => ClientLanguage.Japanese,
        "de" => ClientLanguage.German,
        "fr" => ClientLanguage.French,
        _ => ClientLanguage.English,
    };

    public static bool IsSupported(string? code) => Languages.Any(x => x.Code == code);
    public static string? FromGame(ClientLanguage? value) => value switch
    {
        ClientLanguage.Japanese => "ja",
        ClientLanguage.English => "en",
        ClientLanguage.German => "de",
        ClientLanguage.French => "fr",
        _ => null,
    };

    public static string? Normalize(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return null;
        var value = raw.Trim().Replace('_', '-').ToLowerInvariant();
        if (value is "ja" or "ja-jp") return "ja";
        if (value is "en" or "en-us" or "en-gb") return "en";
        if (value is "de" or "de-de") return "de";
        if (value is "fr" or "fr-fr") return "fr";
        if (value is "ko" or "ko-kr") return "ko";
        if (value is "zh-hans" or "zh-cn" or "zh-sg") return "zh-Hans";
        if (value is "zh-hant" or "zh-tw" or "zh-hk" or "zh-mo") return "zh-Hant";
        return null;
    }

    public static string Get(string? code, string key)
    {
        if (!Text.TryGetValue(key, out var values)) return key;
        var index = Array.FindIndex(Languages, x => x.Code == code);
        return values[index < 0 ? 1 : index];
    }

    public static void CheckCompleteness()
    {
        foreach (var (key, values) in Text)
        {
            if (values.Length != Languages.Length || values.Any(string.IsNullOrWhiteSpace))
                throw new InvalidOperationException($"Incomplete localization: {key}");
            var expected = CountParameters(values[1]);
            if (values.Any(x => CountParameters(x) != expected))
                throw new InvalidOperationException($"Localization format mismatch: {key}");
        }
    }

    private static int CountParameters(string value)
        => System.Text.RegularExpressions.Regex.Matches(value, @"\{\d+\}").Count;
}