namespace GilBeacon;
public static class MarketReadiness
{
    public static bool MatchesItem(uint searchItem, uint displayedItem) => searchItem != 0 && searchItem == displayedItem;
    public static bool MatchesRow(string unitText, string quantityText, bool shownHq, uint price, uint quantity, bool offerHq) =>
        shownHq == offerHq && MatchesNumbers(unitText, quantityText, price, quantity);
    public static bool UsableBounds(float x1, float y1, float x2, float y2) =>
        float.IsFinite(x1) && float.IsFinite(y1) && float.IsFinite(x2) && float.IsFinite(y2) &&
        x2 - x1 is >= 40 and <= 2000 && y2 - y1 is >= 8 and <= 150;
    // A partially populated result list is usable. Each rendered row is checked
    // against its current price and quantity before drawing, rather than waiting
    // for the entire UI list and proxy counts to match.
    public static bool CanDraw(uint item, uint count, int displayedCount,
        bool addonReady, bool updatePending, bool scrollPending) =>
        addonReady && item != 0 && count is > 0 and <= 100 && displayedCount is > 0 and <= 100 && !scrollPending;

    public static bool MatchesNumbers(string unitText, string quantityText, uint price, uint quantity) =>
        TryNumber(unitText, out var shownPrice) && TryNumber(quantityText, out var shownQuantity) &&
        shownPrice == price && shownQuantity == quantity;

    private static bool TryNumber(string text, out ulong value)
    {
        value = 0;
        bool any = false;
        foreach (char c in text)
        {
            if (c is < '0' or > '9') continue;
            if (value > (ulong.MaxValue - (uint)(c - '0')) / 10) return false;
            value = value * 10 + (uint)(c - '0');
            any = true;
        }
        return any;
    }

    // Sorted by unit price. Do not stop on an NQ loss if a later HQ row could
    // profit; use the greatest possible vendor value, even assuming zero tax.
    public static bool BeyondProfitRange(uint price, uint baseValue, bool canHq, bool materia) =>
        price >= Profit.VendorUnit(baseValue, canHq, materia);
}
