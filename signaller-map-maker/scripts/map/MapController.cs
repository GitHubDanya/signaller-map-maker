using System;
using Godot;

namespace signallerMap.Scripts.Map
{
    internal partial class MapController : Node
    {
        [Export] private Node2D map;
        [Export] public float ZoomSpeed = 0.1f;
        [Export] public float MinZoom = 0.1f;
        [Export] public float MaxZoom = 5.0f;
        [Export] public float DragThreshold = 5.0f; // Minimum pixels to move before dragging starts

        private bool _isMouseDown = false;
        private bool _mapBeingDragged = false;
        private Vector2 _mapDragStartMousePos;
        private Vector2 _mapDragStartMapPos;

        public override void _UnhandledInput(InputEvent @event)
        {
            HandleDragging(@event);
            HandleZooming(@event);
        }

        private void HandleDragging(InputEvent @event)
        {
            if (@event is InputEventMouseButton mouseButton && mouseButton.ButtonIndex == MouseButton.Left)
            {
                if (mouseButton.Pressed)
                {
                    _isMouseDown = true;
                    _mapDragStartMousePos = map.GetGlobalMousePosition();
                    _mapDragStartMapPos = map.Position;
                }
                else
                {
                    _isMouseDown = false;
                    _mapBeingDragged = false;
                }
            }

            if (@event is InputEventMouseMotion mouseMotion && _isMouseDown)
            {
                // Only start dragging if the mouse has moved beyond the threshold
                if (!_mapBeingDragged)
                {
                    float dist = mouseMotion.GlobalPosition.DistanceTo(_mapDragStartMousePos);
                    if (dist > DragThreshold)
                    {
                        _mapBeingDragged = true;
                    }
                }

                if (_mapBeingDragged)
                {
                    Vector2 mouseDelta = map.GetGlobalMousePosition() - _mapDragStartMousePos;
                    map.Position = _mapDragStartMapPos + mouseDelta;
                }
            }
        }

        private void HandleZooming(InputEvent @event)
        {
            if (@event is InputEventMouseButton wheelEvent && wheelEvent.Pressed)
            {
                float factor = 0;
                if (wheelEvent.ButtonIndex == MouseButton.WheelUp) factor = 1.0f + ZoomSpeed;
                else if (wheelEvent.ButtonIndex == MouseButton.WheelDown) factor = 1.0f - ZoomSpeed;

                if (factor != 0) ApplyZoom(factor);
            }
        }

        private void ApplyZoom(float factor)
        {
            Vector2 oldScale = map.Scale;
            Vector2 newScale = (oldScale * factor).Clamp(new Vector2(MinZoom, MinZoom), new Vector2(MaxZoom, MaxZoom));
            float actualFactor = newScale.X / oldScale.X;

            Vector2 mousePos = map.GetLocalMousePosition();
            map.Position -= mousePos * (actualFactor - 1.0f) * map.Scale;
            map.Scale = newScale;
        }
    }
}