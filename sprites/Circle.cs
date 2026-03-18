using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace signallerMap.Scripts.Graphics {
    internal partial class Circle : Sprite2D {
        public float Radius = 20;
        public Color Color = Colors.White;

        public override void _Draw() {
            DrawCircle(Vector2.Zero, Radius, Color);
        }

        // public override void _Input(InputEvent @event)
        // {
        //     if (Signal == null) return;
        //     if (@event is InputEventMouseButton mb && mb.Pressed && mb.ButtonIndex == MouseButton.Left)
        //     {
        //         Vector2 mouse = GetGlobalMousePosition();
        //         if (mouse.DistanceTo(GlobalPosition) <= Radius)
        //         {
        //             SignalHandler signalHandler = GetNode<SignalHandler>("/root/Map/SignalHandler");
        //             if (Signal.TargetState < SignalState.Danger) Signal.TargetState = SignalState.Danger;
        //             else Signal.TargetState = SignalState.Proceed;

        //             signalHandler.UpdateSignal(Signal);
        //         }
        //     }
        // }
    }
}