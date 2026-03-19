using Godot;
using signallerMap.Scripts.Data;
using signallerMap.Scripts.Graphics;
using signallerMap.Scripts.editor;
using System;

namespace signallerMap.Scripts.Input 
{
    public partial class uiInputHandler : Node2D
    {
        [Export] Button uiDeleteEdgeButton { get; set; }
        [Export] Button uiDeleteNodeButton { get; set; }
        [Export] Button uiCreateEdgeButton { get; set; }

        public override void _UnhandledInput(InputEvent @event)
        {
            HandleInput(@event);
        }

        private void HandleInput(InputEvent @event)
        {
            if (@event.IsActionPressed("create_edge"))
                uiCreateEdgeButton.EmitSignal(Button.SignalName.Pressed);
            else if (@event.IsActionPressed("delete_edge"))
                uiDeleteEdgeButton.EmitSignal(Button.SignalName.Pressed);
            else if (@event.IsActionPressed("delete_node"))
                uiDeleteNodeButton.EmitSignal(Button.SignalName.Pressed);
        }
    }
}