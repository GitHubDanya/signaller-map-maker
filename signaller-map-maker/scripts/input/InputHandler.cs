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
        private Vector2 _clickStartPosition;
        private float _clickThreshold = 10.0f; // If moved more than this, it's a drag, not a draw

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
            // if (@event is InputEventMouseButton mouseButton && mouseButton.ButtonIndex == MouseButton.Left)
            // {
            //     if (mouseButton.Pressed)
            //     {
            //         _clickStartPosition = mouseButton.GlobalPosition;
            //     }
            //     else
            //     {
            //         float dragDistance = mouseButton.GlobalPosition.DistanceTo(_clickStartPosition);
                    
            //         if (dragDistance < _clickThreshold)
            //         {
            //             MouseClicked();
                        
            //         }
            //     }
            // }
            
            if (@event is InputEventMouseButton mouseButton && mouseButton.Pressed)
            {
                
                if (mouseButton.ButtonIndex == MouseButton.Right)
                {
                    MouseClicked(true);
                }
                else if (mouseButton.ButtonIndex == MouseButton.Left)
                {
                    //MouseClicked(false);
                }
            }
            
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
            Vector2 mousePos = mapGrapher.ToLocal(GetGlobalMousePosition());
            Vector2 nodePos = new Vector2(
                (float)Math.Round(mousePos.X / 25f) * 25f,
                (float)Math.Round(mousePos.Y / 25f) * 25f
            );
            if (right) _editor.RMBClickEvent(nodePos); else _editor.LMBClickEvent(nodePos);
        }
    }
}