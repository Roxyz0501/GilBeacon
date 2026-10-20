namespace GilBeacon;

public static class MarketReadiness
{
    // InfoProxyItemSearch.WaitingForListings remains set even for fully populated
    // ItemSearchResult windows in the observed live client. It is not a ready gate.
    public static bool CanDraw(uint item, uint count, int displayedCount,
        bool addonReady, bool updatePending, bool scrollPending) =>
        addonReady && item != 0 && count is > 0 and <= 100 && displayedCount == count &&
        !updatePending && !scrollPending;
}
