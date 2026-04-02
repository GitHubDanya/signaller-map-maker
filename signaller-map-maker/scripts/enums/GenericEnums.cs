namespace signallerMap.Scripts
{
    public enum SignalState
    {
        Proceed = 0,
        DoubleYellow = 1,
        Caution = 2,
        Danger = 3,
    }

    public enum TrainAccelerationState
    {
        Braking,
        Idle,
        Accelerating,
    }

    public enum EditorMode
    {
        BuildingMode,
        MovementMode,
    }

    public enum PlatformVerticalAlignment
    {
        Above,
        Below,
    }

    public enum EditorInputEvent
    {
        LMBClick,
        RMBClick,
        NodeClick,
        NodeHover,
        NodeUnhover,
        EdgeClick,
        EdgeHover,
        EdgeUnhover,
        SignalClick,
        SignalHover,
        SignalUnhover,
    }

    public enum EditorUiEvent
    {
        NodePrefixFieldChanged,
        NodeDeleteButtonPressed,
        EdgeCreateButtonPressed,
        EdgeStumpCheckboxToggled,
        EdgeLengthFieldChanged,
        EdgeSpeedFieldChanged,
        EdgeDeleteButtonPressed,
        CreateMovementPressed,
        CreateSignalPressed,
        CycleSignalPressed,
        DeleteSignalPressed,
        StationSelected,
        StationPlatformAbovePressed,
        StationPlatformBelowPressed,
        StationDeletePressed,
        JsonLoadButtonPressed,
        JsonSaveButtonPressed,
    }
}
