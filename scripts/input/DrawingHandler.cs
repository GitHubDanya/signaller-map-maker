using Godot;
using signallerMap.Scripts.Data;
using signallerMap.Scripts.Graphics;
using System;

namespace signallerMap.Scripts.Input 
{
    public partial class DrawingHandler : Node2D
    {
        MapGrapher mapGrapher;
        Node2D nodesContainer;
        private Vector2 _clickStartPosition;
        private float _clickThreshold = 10.0f; // If moved more than this, it's a drag, not a draw

        public override void _Ready()
        {
            mapGrapher = GetNode<MapGrapher>("/root/Map/MapGrapher");
            nodesContainer = mapGrapher.GetNode<Node2D>("NodeContainer");
        }

        public override void _Input(InputEvent @event)
        {
            HandleInput(@event);
        }

        private void HandleInput(InputEvent @event)
        {
            if (@event is InputEventMouseButton mouseButton && mouseButton.ButtonIndex == MouseButton.Left)
            {
                if (mouseButton.Pressed)
                {
                    _clickStartPosition = mouseButton.GlobalPosition;
                }
                else
                {
                    float dragDistance = mouseButton.GlobalPosition.DistanceTo(_clickStartPosition);
                    
                    if (dragDistance < _clickThreshold)
                    {
                        SpawnNode();
                        
                    }
                }
            }
        }

        private void SpawnNode()
        {
            Vector2 mousePos = mapGrapher.ToLocal(GetGlobalMousePosition());
            Vector2 nodePos = new Vector2(
                (float)Math.Round(mousePos.X / 25f) * 25f,
                (float)Math.Round(mousePos.Y / 25f) * 25f
            );

            string id = MapData.Nodes.Count.ToString("D3");

            MapNode node = new()
            {
                Id = id,
                Position = nodePos
            };

            

            
        }
    }
}