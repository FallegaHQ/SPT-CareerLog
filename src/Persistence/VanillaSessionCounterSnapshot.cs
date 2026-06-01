using System.Collections.Generic;
using System.Reflection;
using EFT.Counters;
using Softwyx.CareerLog.Persistence.Models;

namespace Softwyx.CareerLog.Persistence;

internal static class VanillaSessionCounterSnapshot{
    private sealed class CounterSpec{
        public string                                                  Name;
        public SessionCountersClass.SessionCounterIdentifierValueClass Id;
        public CounterValueType                                        ValueType;
    }

    private static readonly CounterSpec[] AllCounters = DiscoverAllCounters();

    public static void Capture(SessionCountersClass sessionCounters, RaidRecord record){
        if(sessionCounters == null || record == null) return;

        SessionCounterTypesAbstractClass.Warmup();

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

        foreach(var field in typeof(SessionCounterTypesAbstractClass).GetFields(
                                                                                BindingFlags.Public
                                                                              | BindingFlags.Static
                                                                               )){
            if(field.FieldType != typeof(SessionCountersClass.SessionCounterIdentifierValueClass)) continue;

            var id = field.GetValue(null) as SessionCountersClass.SessionCounterIdentifierValueClass;

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
}
