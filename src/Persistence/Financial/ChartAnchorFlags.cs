using System;

namespace Softwyx.CareerLog.Persistence.Financial;

[Flags]
internal enum ChartAnchorFlags{
    None     = 0,
    First    = 1 << 0,
    Last     = 1 << 1,
    MinWorth = 1 << 2,
    MaxWorth = 1 << 3,

    DayAnchors      = First | Last | MinWorth | MaxWorth,
    DayInPeriod     = First | Last,
    LifetimeAnchors = First | Last | MinWorth | MaxWorth
}
