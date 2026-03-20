using Godot;
using System;

namespace signallerMap.Scripts.editor
{
    internal interface IEditorMode
    {
        void OnInputEvent(EditorInputEvent inputEvent, EditorInputEventArgs args);
        void OnUiEvent(EditorUiEvent uiEvent, EditorUiEventArgs args);
    }
}