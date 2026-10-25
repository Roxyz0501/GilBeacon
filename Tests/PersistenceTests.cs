using GilBeacon;
using System.Text.Json;

static class PersistenceTests
{
    public static int Run(string directory)
    {
        Directory.CreateDirectory(directory);
        int checks = 0;
        var metrics = new List<object>();
        void Check(bool ok) { checks++; if (!ok) throw new Exception($"Persistence check {checks} failed"); }
        string FileAt(string name) => Path.Combine(directory, name + ".json");
        void Seed(string path, ConfigValues value) => File.WriteAllBytes(path, JsonSerializer.SerializeToUtf8Bytes(value));
        ConfigValues Read(string path) => JsonSerializer.Deserialize<ConfigValues>(File.ReadAllBytes(path))!;
        var original = new ConfigValues(1, "ja", true);
        long now = 0;
        var p = FileAt("normal"); Seed(p, original);
        using (var store = new ConfigStore(p, () => now))
        {
            for (int i = 0; i < 480; i++) { now += 60000; store.Tick(); }
            Check(store.SaveCount == 0 && store.PayloadBytes == 0 && store.Attempts == 0);
            for (int i = 0; i < 600; i++) store.Change(original with { Enabled = i % 2 != 0 });
            store.Change(original with { Enabled = false });
            Check(store.SaveCount == 0);
            now += 59999; store.Tick(); Check(store.SaveCount == 0);
            now++; store.Tick(); Check(store.SaveCount == 1 && !store.Pending);
            var before = store.SaveCount;
            store.Change(original); store.Change(original with { Enabled = false });
            now += 60000; store.Tick(); Check(store.SaveCount == before);
            store.Change(original with { Language = "en" }); store.Flush();
            Check(store.SaveCount == 2 && Read(p).Language == "en");
            store.Flush(); Check(store.SaveCount == 2);
            store.Change(original with { Language = "de" }); store.Dispose();
            Check(store.SaveCount == 3 && Read(p).Language == "de");
            store.Change(original); now += 60000; store.Tick(); store.Flush(); store.Retry(); store.Dispose();
            Check(store.SaveCount == 3 && Read(p).Language == "de");
            metrics.Add(new { scenario = "8h idle + 600 edits + logout + dispose", store.SaveCount, store.PayloadBytes, store.Attempts });
        }

        p = FileAt("generations"); Seed(p, original);
        using (var store = new ConfigStore(p, () => now))
        {
            foreach (var language in new[] { "en", "de", "fr", "ko" }) { store.Change(original with { Language = language }); store.Flush(); }
            var backups = Directory.GetFiles(directory, "generations.json.bak.*").OrderByDescending(x => x).ToArray();
            Check(backups.Length == 3);
            Check(backups.Select(x => Read(x).Language).SequenceEqual(new[] { "fr", "de", "en" }));
            File.WriteAllText(backups[0], "broken newest backup");
            File.WriteAllText(p, "broken main");
        }
        using (var recovered = new ConfigStore(p))
        {
            Check(recovered.Current.Language == "de" && recovered.LoadNotice == "Recovered");
            Check(recovered.SaveCount == 1 && Read(p).Language == "de");
            Check(Directory.GetFiles(directory, "generations.json.corrupt-*").Length == 1);
            Check(Directory.GetFiles(directory, "generations.json.bak.*").Count(x => !x.Contains(".corrupt-")) == 2);
            Check(Directory.GetFiles(directory, "generations.json.bak.*.corrupt-*").Length == 1);
            metrics.Add(new { scenario = "corrupt main and newest backup recovery", recovered.SaveCount, recovered.PayloadBytes });
        }
        using (var restart = new ConfigStore(p)) Check(restart.SaveCount == 0 && restart.Current.Language == "de");

        foreach (var stage in new[] { "write", "validate", "replace" })
        {
            p = FileAt("failure-" + stage); Seed(p, original);
            bool fail = true; int invoked = 0; long time = 0;
            using var store = new ConfigStore(p, () => time, at => { if (at == stage && fail) { invoked++; throw new IOException("Injected " + stage); } });
            store.Change(original with { Enabled = false });
            for (int i = 0; i < 3; i++)
            {
                time += 60000; store.Tick();
                for (int frame = 0; frame < 1000; frame++) { store.Tick(); store.Flush(); }
                Check(invoked == i + 1 && store.Pending);
                Check(Read(p) == original && Directory.GetFiles(directory, "failure-" + stage + ".json.bak.*").Length == 0);
            }
            time += 600000; store.Tick(); store.Flush(); Check(invoked == 3 && store.Stopped);
            Check(Directory.GetFiles(directory, "failure-" + stage + ".json.tmp-*").Length == 0);
            fail = false; store.Retry();
            Check(!store.Pending && !store.Stopped && store.SaveCount == 1);
            metrics.Add(new { scenario = stage + " failure x3 then manual retry", store.SaveCount, store.PayloadBytes, store.Attempts });
        }

        p = FileAt("locked"); Seed(p, original);
        using (var store = new ConfigStore(p, () => now))
        {
            store.Change(original with { Language = "fr" });
            using (var held = new FileStream(p, FileMode.Open, FileAccess.Read, FileShare.None)) store.Flush();
            Check(store.Failures == 1 && store.SaveCount == 0 && Read(p) == original);
            store.Retry(); Check(store.SaveCount == 1 && Read(p).Language == "fr");
        }
        using (var held = new FileStream(p, FileMode.Open, FileAccess.Read, FileShare.None))
        using (var blocked = new ConfigStore(p))
        { Check(blocked.LoadBlocked); blocked.Change(original); blocked.Retry(); blocked.Flush(); Check(blocked.Attempts == 0); }

        p = FileAt("exit-after-error"); Seed(p, original); bool error = true;
        using (var store = new ConfigStore(p, fault: stage => { if (stage == "write" && error) throw new IOException("Injected disk full"); }))
        {
            store.Change(original with { Language = "fr" }); store.Flush();
            error = false; store.Dispose();
            Check(store.Attempts == 2 && Read(p).Language == "fr");
            store.Flush(); Check(store.Attempts == 2);
        }

        p = FileAt("first-launch"); long firstTime = 0;
        using (var store = new ConfigStore(p, () => firstTime))
        {
            store.Change(original); firstTime = 59999; store.Tick(); Check(!File.Exists(p));
            firstTime++; store.Tick(); Check(Read(p) == original && store.SaveCount == 1);
            store.Dispose(); Check(store.SaveCount == 1);
        }
        p = FileAt("revert-error"); Seed(p, original);
        using (var store = new ConfigStore(p, fault: _ => throw new IOException("Injected failure")))
        {
            store.Change(original with { Enabled = false }); store.Flush();
            Check(store.Failures == 1);
            store.Change(original); store.Flush();
            Check(!store.Pending && store.Error == null && store.SaveCount == 0 && store.Attempts == 1);
        }

        p = FileAt("legacy");
        File.WriteAllText(p, "{\"$type\":\"GilBeacon.Configuration, GilBeacon\",\"Version\":1,\"Language\":\"zh-Hant\",\"Enabled\":false}");
        using (var legacy = new ConfigStore(p)) { Check(legacy.Current.Language == "zh-Hant" && !legacy.Current.Enabled); legacy.Flush(); Check(legacy.SaveCount == 0); }
        p = FileAt("invalid-language");
        File.WriteAllText(p, "{\"Version\":1,\"Language\":\"Auto\",\"Enabled\":false}");
        using (var legacy = new ConfigStore(p))
        {
            Check(legacy.Current.Language == null && !legacy.Current.Enabled);
            legacy.Change(legacy.Current with { Language = "ja" }); legacy.Dispose();
            Check(Read(p).Language == "ja" && !Read(p).Enabled);
        }
        p = FileAt("invalid"); File.WriteAllText(p, "{\"Version\":\"oops\",\"Enabled\":true}");
        using (var invalid = new ConfigStore(p)) { Check(invalid.LoadNotice == "ResetConfig"); invalid.Change(original); invalid.Flush(); Check(Read(p) == original); }
        Check(Directory.GetFiles(directory, "invalid.json.corrupt-*").Length == 1);

        p = FileAt("reentrant"); Seed(p, original);
        ConfigStore? reentrant = null; bool once = true;
        using (reentrant = new ConfigStore(p, () => now, stage =>
        {
            if (stage != "write" || !once) return;
            once = false;
            reentrant!.Change(original with { Language = "ko" });
            reentrant.Flush(); // Must not recurse into another writer.
        }))
        {
            reentrant.Change(original with { Language = "en" }); reentrant.Flush();
            Check(reentrant.Pending && reentrant.SaveCount == 1 && Read(p).Language == "en");
            reentrant.Flush(); Check(!reentrant.Pending && reentrant.SaveCount == 2 && Read(p).Language == "ko");
        }

        p = FileAt("parallel"); Seed(p, original);
        using (var store = new ConfigStore(p))
        {
            store.Change(original with { Enabled = false });
            Parallel.For(0, 32, _ => store.Flush());
            Check(store.SaveCount == 1 && store.Attempts == 1);
        }

        p = FileAt("during-save"); Seed(p, original);
        using var entered = new ManualResetEventSlim(); using var release = new ManualResetEventSlim();
        using (var store = new ConfigStore(p, fault: stage => { if (stage == "write") { entered.Set(); if (!release.Wait(5000)) throw new IOException("Test gate timeout"); } }))
        {
            store.Change(original with { Language = "en" });
            var writer = Task.Run(store.Flush); Check(entered.Wait(5000));
            var change = Task.Run(() => store.Change(original with { Language = "fr" }));
            release.Set(); Task.WaitAll(writer, change);
            Check(store.Pending && Read(p).Language == "en");
            store.Dispose(); Check(Read(p).Language == "fr" && !store.Pending);
            Check(writer.IsCompleted && change.IsCompleted);
        }
        p = FileAt("future-schema");
        string future = "{\"Version\":2,\"Language\":\"en\",\"Enabled\":false,\"FutureData\":42}";
        File.WriteAllText(p, future);
        Seed(p + ".bak.0000000000000000001", original);
        using (var store = new ConfigStore(p))
        {
            Check(store.LoadBlocked && store.Error != null);
            store.Change(original); store.Retry(); store.Dispose();
            Check(store.SaveCount == 0 && File.ReadAllText(p) == future);
        }
        p = FileAt("generation-overflow"); Seed(p, original);
        Seed(p + ".bak.9223372036854775807", original);
        using (var store = new ConfigStore(p))
        {
            store.Change(original with { Enabled = false }); store.Flush();
            Check(store.Failures == 1 && store.Pending && Read(p) == original);
            Check(!Directory.GetFiles(directory, "generation-overflow.json.bak.*").Any(x => x.Contains(".-")));
        }
        File.WriteAllText(Path.Combine(directory, "metrics.json"), JsonSerializer.Serialize(metrics, new JsonSerializerOptions { WriteIndented = true }));
        Console.WriteLine($"Persistence: {checks} checks; metrics: {Path.Combine(directory, "metrics.json")}");
        return checks;
    }
}
