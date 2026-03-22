using Godot;
using System;

namespace signallerMap.Scripts.editor
{
    internal class EditorInputEventArgs { }
    internal class EditorUiEventArgs { }

    internal class EditorUiCreateEdgeArgs : EditorUiEventArgs
    {
        public string EdgeLength;
        public string EdgeSpeed;
        public bool IsStump;
    }

    internal class EditorUiDeleteEdgeArgs : EditorUiEventArgs
    {
        public string id;
    }

    internal class EditorInputMouseClickArgs : EditorInputEventArgs
    {
        public Vector2 Position;
    }

    internal class EditorInputOnNodeArgs : EditorInputEventArgs
    {
        public MapNode Node;
    }

    internal class EditorInputOnEdgeArgs : EditorInputEventArgs
    {
        public MapEdge Edge;
    }

    internal class EditorInputOnSignalArgs : EditorInputEventArgs
    {
        public MapSignal Signal;
    }
}