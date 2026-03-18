using System;

namespace signallerMap.Scripts
{
    public enum SignalState
    {
        Proceed = 0,
        DoubleYellow = 1,
        Amber = 2,
        Danger = 3
    }

    public enum TrainAccelerationState
    {
        Braking,
        Idle,
        Accelerating
    }
}