using Godot;
using signallerMap.Scripts.Data;
using signallerMap.Scripts.Graphics;
using signallerMap.Scripts.editor;
using System;

namespace signallerMap.Scripts.Input 
{
    public partial class InputHandler : Node2D
    {
        Editor _editor;
        MapGrapher mapGrapher;

        public override void _Ready()
        {
            mapGrapher = GetNode<MapGrapher>("/root/Map/MapGrapher");
            _editor = GetNode<Editor>("/root/Map/Editor");
        }

        public override void _UnhandledInput(InputEvent @event)
        {
            HandleInput(@event);
        }

        private void HandleInput(InputEvent @event)
        {
            if (@event is InputEventMouseButton mouseButton && mouseButton.Pressed)
                MouseClicked(mouseButton.ButtonIndex == MouseButton.Right);
            else if (@event.IsActionPressed("undo"))
                CommandManager.Undo();
            else if (@event.IsActionPressed("redo"))
                CommandManager.Redo();
            else if (@event.IsActionPressed("change_mode_movement"))
                _editor.SetEditorMode(new MovementMode(_editor));
            else if (@event.IsActionPressed("change_mode_build"))
                _editor.SetEditorMode(new BuildingMode(_editor));
            else if (@event.IsActionPressed("create_movement"))
                _editor.FireUiEvent(EditorUiEvent.CreateMovementPressed);
            else if (@event.IsActionPressed("create_signal"))
                _editor.FireUiEvent(EditorUiEvent.CreateSignalPressed);
        }

        private void MouseClicked(bool right = false)
        {
            Vector2 pos = mapGrapher.ToLocal(GetGlobalMousePosition());

            _editor.FireInputEvent
            (right ? EditorInputEvent.RMBClick : EditorInputEvent.LMBClick,
            new EditorInputMouseClickArgs() { Position = pos } );
        }
    }
}