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
        ["SavePolicy"] = ["設定は60秒ごとに変更時だけ保存し、ログアウト・正常終了時にも保存します。", "Changed settings save every 60 seconds and on logout or normal shutdown.", "Geänderte Einstellungen werden alle 60 Sekunden und beim Abmelden oder regulären Beenden gespeichert.", "Les modifications sont enregistrées toutes les 60 secondes, à la déconnexion et à la fermeture normale.", "변경된 설정은 60초마다, 로그아웃 및 정상 종료 시 저장됩니다.", "设置仅在有更改时每60秒保存，并在登出或正常退出时保存。", "設定僅在有變更時每60秒儲存，並在登出或正常結束時儲存。"],
        ["Recovered"] = ["破損した設定を正常なバックアップから復元しました。", "Settings were recovered from a valid backup.", "Einstellungen wurden aus einer gültigen Sicherung wiederhergestellt.", "Les paramètres ont été restaurés depuis une sauvegarde valide.", "정상 백업에서 설정을 복구했습니다.", "已从有效备份恢复设置。", "已從有效備份還原設定。"],
        ["ResetConfig"] = ["設定を読み取れず、正常なバックアップもありません。初期値を使用します。破損ファイルは保存時に退避します。", "Settings and backups were invalid. Defaults are in use; the damaged file will be preserved when saving.", "Einstellungen und Sicherungen waren ungültig. Standardwerte werden verwendet; die beschädigte Datei bleibt beim Speichern erhalten.", "Paramètres et sauvegardes invalides : valeurs par défaut utilisées. Le fichier endommagé sera conservé lors de l’enregistrement.", "설정과 백업이 손상되어 기본값을 사용합니다. 저장 시 손상 파일을 보존합니다.", "设置和备份均无效，现使用默认值。保存时将保留损坏文件。", "設定與備份皆無效，目前使用預設值。儲存時將保留損毀檔案。"],
        ["BackupCleanup"] = ["設定は保存できましたが、古いバックアップの整理に失敗しました。ファイルのアクセス権を確認してください。", "Settings saved, but old backups could not be removed. Check file permissions.", "Gespeichert, aber alte Sicherungen konnten nicht entfernt werden. Dateiberechtigungen prüfen.", "Enregistrement réussi, mais suppression des anciennes sauvegardes impossible. Vérifiez les permissions.", "설정은 저장했지만 오래된 백업을 삭제하지 못했습니다. 파일 권한을 확인하세요.", "设置已保存，但无法清理旧备份。请检查文件权限。", "設定已儲存，但無法清理舊備份。請檢查檔案權限。"],
        ["LoadBlocked"] = ["設定ファイルにアクセスできないため保存を停止しています。原因を解消してプラグインを再読み込みしてください。", "Saving is blocked because settings could not be accessed. Resolve the error and reload the plugin.", "Speichern gesperrt: kein Zugriff auf Einstellungen. Fehler beheben und Plugin neu laden.", "Enregistrement bloqué : paramètres inaccessibles. Corrigez l’erreur puis rechargez le plugin.", "설정 파일에 접근할 수 없어 저장을 중지했습니다. 원인을 해결한 후 플러그인을 다시 불러오세요.", "无法访问设置文件，已停止保存。请排除故障后重新加载插件。", "無法存取設定檔案，已停止儲存。請排除故障後重新載入外掛。"],
        ["SaveFailed"] = ["設定を保存できませんでした。未保存の変更は保持しています。60秒以上空けて再試行します（連続3回まで）。", "Settings could not be saved. Changes remain pending; retries are at least 60 seconds apart, up to three failures.", "Speichern fehlgeschlagen. Änderungen bleiben vorgemerkt; Wiederholungen frühestens nach 60 Sekunden, maximal drei Fehler.", "Échec de l’enregistrement. Modifications en attente ; essais espacés d’au moins 60 secondes, arrêt après trois échecs.", "설정을 저장하지 못했습니다. 변경 사항은 유지되며 60초 이상 간격으로 최대 3회 실패까지 재시도합니다.", "设置保存失败，未保存的更改仍被保留。重试间隔至少60秒，连续失败3次后停止。", "設定儲存失敗，未儲存的變更仍被保留。重試間隔至少60秒，連續失敗3次後停止。"],
        ["SaveStopped"] = ["保存が3回失敗したため自動保存を停止しました。原因を解消して「保存を再試行」を押してください。終了すると未保存の変更は失われます。", "Autosave stopped after three failures. Resolve the error and retry saving. Pending changes will be lost on exit.", "Automatisches Speichern nach drei Fehlern gestoppt. Fehler beheben und erneut speichern. Ungespeicherte Änderungen gehen beim Beenden verloren.", "Enregistrement automatique arrêté après trois échecs. Corrigez l’erreur puis réessayez. Les modifications en attente seront perdues à la fermeture.", "3회 실패하여 자동 저장을 중지했습니다. 원인을 해결하고 저장을 재시도하세요. 종료하면 미저장 변경 사항은 사라집니다.", "保存连续失败3次，自动保存已停止。请排除故障后重试。退出时未保存的更改将丢失。", "儲存連續失敗3次，自動儲存已停止。請排除故障後重試。結束時未儲存的變更將遺失。"],
        ["RetrySave"] = ["保存を再試行", "Retry saving", "Speichern wiederholen", "Réessayer l’enregistrement", "저장 재시도", "重试保存", "重試儲存"],
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