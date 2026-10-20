namespace GilBeacon;
public static class Profit
{
    // HQ bonus is rounded upward per unit, before multiplying by stack quantity.
    public static ulong VendorUnit(uint nq, bool hq) => nq == 0 ? 0 : (ulong)nq + (hq ? ((ulong)nq + 9) / 10 : 0);
    public static ulong Calculate(uint sell, uint price, uint quantity, uint tax, bool hq = false)
    {
        if (sell == 0 || quantity == 0) return 0;
        // Invalid out-of-domain totals fail closed instead of wrapping.
        ulong unit = VendorUnit(sell, hq);
        if (unit > ulong.MaxValue / quantity) return 0;
        ulong proceeds = unit * quantity;
        ulong cost = (ulong)price * quantity + tax;
        return proceeds > cost ? proceeds - cost : 0;
    }
}
