# Gil Beacon

![icon](GilBeacon/images/icon.png)

Roxyz0501 / 1.0.0.2 / Dalamud API 15 / Apache-2.0

マーケット出品一覧で、購入手数料込みでもNPCへの売却で利益が出る行を緑色でハイライトします。行の右側に **+1,234 G** の形式で、その出品全個数の利益総額を表示します。HQにも対応しています。

**公開プレビュー:** 実ゲームでの行対応、ソート・スクロール、DPI、画面重なり、売却時の受取額は未検証です。画面右端では利益ラベルが欠ける場合があります。

## インストール

Dalamudのカスタムリポジトリに次のURLを登録し、Gil Beaconを検索してください。

`https://raw.githubusercontent.com/Roxyz0501/DalamudPluginRepo/main/repo.json`

設定コマンド: `/gilbeacon`。自動購入・自動売却は行いません。ほかのプラグインへの依存はありません。

## 計算

`利益 = NPC売却単価 × 個数 − (出品単価 × 個数 + 出品全体の購入手数料)`

- 通常のNQ: ゲームItemシートのPriceLow。NQだから減額する処理はありません。
- マテリア商品: PriceLowに10%を加算し端数切り上げ。144→159、151→167、197→217。品質のない商品すべてにこの補正を適用するわけではありません。
- HQ: NQ価格に10%を加算し、1個ごとに端数切り上げ。例えば296→326、11→13。単価を丸めてから個数を掛けます。
- 1ギル以上の利益のみ表示。手数料はゲーム側のTotalTaxを使用します。
- NPC売却額0、マネキン・セット販売、読込中・不整合の結果は除外します。
- 装着マテリアによる加算・特殊買取ボーナスは含めません。

HQ計算については実装例に相違があるため、単純にコードを転載せず、[Lodestoneの実売却価格の報告（296のHQが326）](https://jp.finalfantasyxiv.com/lodestone/character/29950599/blog/4547134)と照合し端数切り上げを採用しました。実クライアントでの売却額照合は未実施です。

## 言語

日本語・英語・ドイツ語・フランス語・韓国語・簡体字・繁体字。設定右側の言語設定タブから変更できます。初回はゲーム言語→Dalamud公開UiLanguage→英語で決定し保存します。保存済みの選択は起動時に上書きしません。公開されたランチャー言語APIは利用していません。ゲーム由来アイテム名を独自翻訳しません。フォントはDalamud提供Noto CJKを使用。ライブでの字形・長文表示は未検証です。

## Build and checks

.NET 10 SDK and Dalamud API 15 development libraries are required.

```powershell
dotnet build GilBeacon/GilBeacon.csproj -c Release
dotnet run --project Tests/Tests.csproj -c Release
```

20,064 offline checks cover NQ/HQ arithmetic, rounding before multiplication, tax and break-even boundaries, overflow rejection, random BigInteger comparisons, localization completeness and language normalization. These checks do not establish live-game UI correctness.

## Load budget

No external HTTP requests or market requests. No database. At most one local Item-sheet row lookup on item change; unchanged redraws perform zero sheet lookups. Cache holds one item and is invalidated on close. At most 100 locally available row indices are sorted by price per draw. Profit checks stop at the maximum possible resale value (including HQ when supported); only visible profitable candidates have their renderer text checked and are drawn immediately in that loop. Partial results do not wait for count equality or the global update flag. Price and quantity must match the displayed row. No requests are added; game-side listing delivery can still delay availability. Config writes occur only for initial language selection and actual user changes. These are code-derived limits, not live-game measurements.

## Sources and license

New standalone plugin, Apache-2.0. Author: Roxyz0501.

- [Bait Beacon](https://github.com/Roxyz0501/BaitBeacon), Roxyz0501, Apache-2.0: selected localization and font utilities reused and adapted. No fishing data is included.
- Shared tab implementation: canonical Roxyz0501 StandardPluginTabs.
- [FFXIVClientStructs](https://github.com/aers/FFXIVClientStructs), aers and contributors, MIT: host-provided native API definitions for market data and list renderers. No host libraries are bundled.
- HQ formula investigation consulted [LootView](https://github.com/GitHixy/LootView), GitHixy, GPL-3.0, and [CriticalCommonLib](https://github.com/Critical-Impact/CriticalCommonLib), Critical-Impact. Only pricing behavior was investigated; their source implementation was not copied.
- Original icon generated for Gil Beacon. No third-party or game artwork reused.

Optional support: [Roxyz0501 on Ko-fi](https://ko-fi.com/roxyz0501).

## 1.0.0.1 fix

Live read-only diagnostics found the SDK WaitingForListings flag set while 35 listings, 10 visible renderers and the addon were ready. Removed this incorrect blocking condition. Readiness now uses the populated list count, addon readiness and UI update/scroll state; each listing still must match the search item. The user-reported 100-gil listing versus 217-gil NPC value is covered by regression tests (112 gil profit for one item with 5 gil tax). Actual rendered output after this update still needs in-game confirmation.

## 1.0.0.2 fix

マテリア商品の買取額を修正。画像の例は159 − (99 + 4) = **56ギル**。根拠: [公式メガマテリジャ159](https://jp.finalfantasyxiv.com/lodestone/playguide/db/item/eee0f70611d/)、[エクスマテリジャ167](https://jp.finalfantasyxiv.com/lodestone/playguide/db/item/cd92a6cfb05/)。通常の品質なし商品には一律加算しません。

受信済みの出品を安い順に判定し、利益が出る表示行をその場で描画します。全件受信や件数一致を待ちません。後続のHQ利益を見落とさないよう、最高売却額でも利益が出ない価格に達したら終了します。実ゲームでの更新後の表示速度・行対応は未確認です。

[ECommons](https://github.com/NightmareXIV/ECommons), NightmareXIV, MIT: ItemSearchResult price/quantity node IDs were consulted as native UI layout reference. No implementation copied or library bundled.
