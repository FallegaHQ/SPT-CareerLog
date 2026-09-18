using System.Collections.Generic;
using System.Reflection;
using EFT.Counters;
using Softwyx.CareerLog.Persistence.Models;

namespace Softwyx.CareerLog.Persistence;

internal static class VanillaSessionCounterSnapshot{
    private static readonly CounterSpec[] AllCounters = DiscoverAllCounters();

    public static void Capture(CountersCollection sessionCounters, RaidRecord record){
        if(sessionCounters == null || record == null) return;

        PredefinedCounters.Warmup();

        foreach(var counter in AllCounters){
            if(counter == null || counter.Id == null || string.IsNullOrEmpty(counter.Name)) continue;

            if(counter.ValueType == CounterValueType.Float)
                record.VanillaSessionCountersFloat[counter.Name] = sessionCounters.GetFloat(counter.Id);
            else
                // Treat everything else as long (Long / Undefined / etc).
                // SessionCounters stores values as long internally.
                record.VanillaSessionCountersLong[counter.Name] = sessionCounters.GetLong(counter.Id);
        }
    }

    private static CounterSpec[] DiscoverAllCounters(){
        var specs = new List<CounterSpec>(128);

        foreach(var field in typeof(PredefinedCounters).GetFields(
                                                                BindingFlags.Public
                                                              | BindingFlags.Static
                                                               )){
            if(field.FieldType != typeof(CountersCollection.Identifier)) continue;

            var id = field.GetValue(null) as CountersCollection.Identifier;

            if(id == null) continue;

            specs.Add(
                      new CounterSpec{
                                         Name      = field.Name,
                                         Id        = id,
                                         ValueType = id.ValueType
                                     }
                     );
        }

        return specs.ToArray();
    }

    private sealed class CounterSpec{
        public CountersCollection.Identifier Id;
        public string                      Name;
        public CounterValueType            ValueType;
    }
}
