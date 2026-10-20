using GilBeacon;
using System.Numerics;
int checks = 0;
void Check(bool value) { checks++; if (!value) throw new Exception($"Check {checks} failed"); }
Check(Profit.Calculate(100, 90, 10, 45) == 55);
Check(Profit.Calculate(100, 100, 10, 0) == 0);
Check(Profit.Calculate(100, 99, 1, 1) == 0);
Check(Profit.Calculate(100, 98, 1, 1) == 1);
Check(Profit.Calculate(0, 1, 99, 0) == 0);
Check(Profit.Calculate(100, 1, 0, 0) == 0);
Check(Profit.Calculate(uint.MaxValue, 0, uint.MaxValue, 0) == (ulong)uint.MaxValue * uint.MaxValue);
var random = new Random(42);
for (int i = 0; i < 10000; i++)
{
    uint sell = (uint)random.NextInt64(0, 1L << 32), price = (uint)random.NextInt64(0, 1L << 32);
    uint quantity = (uint)random.NextInt64(0, 1L << 32), tax = (uint)random.NextInt64(0, 1L << 32);
    var expected = BigInteger.Max(0, (BigInteger)sell * quantity - (BigInteger)price * quantity - tax);
    Check(Profit.Calculate(sell, price, quantity, tax) == (ulong)expected);
}
Loc.CheckCompleteness(); checks++;
foreach (var (raw, expected) in new (string?, string?)[] {("JA_jp", "ja"), ("en-US", "en"), ("de-DE", "de"), ("fr-FR", "fr"), ("ko-KR", "ko"), ("zh-CN", "zh-Hans"), ("zh-SG", "zh-Hans"), ("zh-TW", "zh-Hant"), ("zh-HK", "zh-Hant"), ("zh-MO", "zh-Hant"), ("zh", null), (null, null), ("xx", null) }) Check(Loc.Normalize(raw) == expected);
foreach (var (code, _) in Loc.Languages) Check(Loc.Get(code, "Explanation") != "Explanation");
Check(Loc.FromGame(null) == null);
Check(Loc.Get("invalid", "Settings") == "Settings");
Check(Profit.VendorUnit(296, true) == 326);
Check(Profit.VendorUnit(1, true) == 2);
Check(Profit.VendorUnit(10, true) == 11);
Check(Profit.VendorUnit(11, true) == 13);
Check(Profit.VendorUnit(0, true) == 0);
Check(Profit.Calculate(11, 10, 3, 1, true) == 8);
Check(Profit.Calculate(100, 104, 1, 5, true) == 1);
Check(Profit.Calculate(100, 104, 1, 5, false) == 0);
Check(Profit.Calculate(uint.MaxValue, 0, uint.MaxValue, 0, true) == 0);
for (int i = 0; i < 10000; i++)
{
    uint sell = (uint)random.Next(0, 10000000), price = (uint)random.Next(0, 10000000), q = (uint)random.Next(1,1000), tax = (uint)random.Next(0,1000000);
    var expected = BigInteger.Max(0, ((BigInteger)sell * 11 + 9) / 10 * q - (BigInteger)price * q - tax);
    Check(Profit.Calculate(sell, price, q, tax, true) == (ulong)expected);
}
// Regression: live client had 35 valid rows and ready UI while WaitingForListings stayed true.
Check(MarketReadiness.CanDraw(41757, 35, 35, true, false, false));
Check(MarketReadiness.CanDraw(41757, 35, 34, true, false, false));
Check(!MarketReadiness.CanDraw(41757, 35, 35, false, false, false));
Check(MarketReadiness.CanDraw(41757, 35, 35, true, true, false));
Check(!MarketReadiness.CanDraw(41757, 35, 35, true, false, true));
Check(!MarketReadiness.CanDraw(0, 35, 35, true, false, false));
Check(!MarketReadiness.CanDraw(41757, 0, 0, true, false, false));
Check(!MarketReadiness.CanDraw(41757, 101, 101, true, false, false));
Check(Profit.Calculate(217, 100, 1, 5) == 112);
Check(Profit.Calculate(217, 100, 10, 50) == 1120);
Check(Profit.Calculate(144, 99, 1, 4, materia: true) == 56);
Check(Profit.Calculate(144, 99, 10, 49, materia: true) == 551);
Check(Profit.VendorUnit(500, false) == 500); // Qualityless silver piece must not gain 10%.
Check(Profit.VendorUnit(1, false) == 1); // Qualityless iron ore.
Check(Profit.VendorUnit(27, false, true) == 30);
Check(Profit.VendorUnit(151, false, true) == 167);
Check(Profit.VendorUnit(197, false, true) == 217);
Check(MarketReadiness.MatchesNumbers("99 G", "1", 99, 1));
Check(!MarketReadiness.MatchesNumbers("99 G", "1", 199, 1));
Check(MarketReadiness.MatchesNumbers("1,234", "99", 1234, 99));
Check(!MarketReadiness.MatchesNumbers("", "1", 0, 1));
Check(!MarketReadiness.BeyondProfitRange(100, 100, true, false)); // HQ can still profit.
Check(MarketReadiness.BeyondProfitRange(110, 100, true, false));
Check(!MarketReadiness.BeyondProfitRange(150, 144, false, true));
Check(MarketReadiness.BeyondProfitRange(159, 144, false, true));
Console.WriteLine($"Passed {checks} checks.");

