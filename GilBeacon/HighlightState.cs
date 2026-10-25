namespace GilBeacon;

public readonly record struct VendorInfo(uint Price, bool CanHq, bool Materia);

public sealed class ItemPriceCache
{
    private uint itemId;
    private VendorInfo value;
    public void Clear() { itemId = 0; value = default; }
    public VendorInfo Get(uint id, Func<uint, VendorInfo> read)
    {
        if (id == 0) { Clear(); return default; }
        if (id == itemId) return value;
        Clear();
        var next = read(id); // Publish the key only after a successful read.
        value = next;
        itemId = id;
        return next;
    }
}

public sealed class HighlightFailures
{
    private readonly Func<long> clock;
    private long retryAt;
    public int Count { get; private set; }
    public string? Error { get; private set; }
    public bool Stopped => Count >= 3;
    public bool CanAttempt => !Stopped && clock() >= retryAt;
    public HighlightFailures(Func<long>? clock = null) => this.clock = clock ?? (() => Environment.TickCount64);
    public void Failed(Exception error) { Error = error.Message; Count++; retryAt = clock() + 5000; }
    public void Reset() { Count = 0; Error = null; retryAt = 0; }
}
