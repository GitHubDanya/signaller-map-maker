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

    public enum SelectionType
    {
        BuildingMode,
        MovementMode
    }

    public enum EditorInputEvent
    {
        LMBClick,
        RMBClick,
        NodeClick,
        EdgeClick,
        EdgeHover,
        EdgeUnhover
    }

    public enum EditorUiEvent
    {
        NodePrefixFieldChanged,
        NodeUpdateButtonPressed,
        NodeDeleteButtonPressed,
        EdgeCreateButtonPressed,
        EdgeStumpCheckboxToggled,
        EdgeLengthFieldChanged,
        EdgeSpeedFieldChanged,
        EdgeDeleteButtonPressed,
        CreateMovementPressed,
        JsonLoadButtonPressed,
        JsonSaveButtonPressed,
    }
}