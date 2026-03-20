using System;
using System.Collections.Generic;
using System.Linq;
using Godot; 
using signallerMap.Scripts.Graphics;

namespace signallerMap.Scripts.editor
{
    internal class MovementMode : IEditorMode
    {
        private Editor _editor;
        private MapEdge[] selectedEdges;
        private MapNode[] selectedNodes;
        private Color movementInColor = Color.FromHtml("16e1f0");
        private Color movementOutColor = Color.FromHtml("f0e116");
        private Color movementInAndOutColor = Color.FromHtml("16f0e1");
        internal MovementMode(Editor editor)
        {
            _editor = editor;
            editor.SelectedEdges = new MapEdge[2];
            editor.SelectedNodes = new MapNode[2];
            selectedNodes = editor.SelectedNodes;
            selectedEdges = editor.SelectedEdges;

            MapGrapher grapher = _editor.mapGrapher;
            GrapherColors grapherColors = new()
            {
                SelectedNodeColor = Color.FromHtml("4370d1"),
                SelectedEdgeColor = Color.FromHtml("348feb"),
                SecondSelectedNodeColor = Color.FromHtml("688cd9"),
                SecondSelectedEdgeColor = Color.FromHtml("64a8ed"),
            };
            grapher.colors = grapherColors;
        }
        public void MouseClick(Vector2 position)
        {
            _editor.ClearSelection();
        }

        public void OnInputEvent(EditorInputEvent inputEvent, EditorInputEventArgs args)
        {
            switch (inputEvent)
            {
                case EditorInputEvent.EdgeClick when args is EditorInputOnEdgeArgs edgeArgs:
                    SelectEdge(edgeArgs.Edge); break;
                case EditorInputEvent.EdgeHover when args is EditorInputOnEdgeArgs edgeArgs:
                    DisplayEdgeMovements(edgeArgs.Edge); break;
                case EditorInputEvent.EdgeUnhover when args is EditorInputOnEdgeArgs edgeArgs:
                    HideEdgeMovements(edgeArgs.Edge); break;

            }
        }
        public void OnUiEvent(EditorUiEvent uiEvent, EditorUiEventArgs args)
        {
            switch (uiEvent)
            {
                case EditorUiEvent.CreateMovementPressed when args is null:
                    CreateAndLogMovementBetweenSelected(); break;
            }
        }

        private void SelectEdge(MapEdge edge)
        {
            if (edge == null) return;
            if (selectedEdges[0] == edge) edge = selectedEdges[1];
            _editor.mapGrapher.ClearEdgeColor(selectedEdges[1]);
            selectedEdges[1] = selectedEdges[0];
            selectedEdges[0] = edge;
            _editor.mapGrapher.SelectEdgePair(selectedEdges);
        }

        private void CreateAndLogMovementBetweenSelected()
        {
            if (selectedEdges[0] == null
            || selectedEdges[1] == null
            || selectedEdges[0] == selectedEdges[1]) return;

            MapNodeMovement movement = new()
            {
                from = selectedEdges[0],
                to = selectedEdges[1]
            };

            var command = new CreateNodeMovementCommand(_editor, movement.GetNode(), movement);
            CommandManager.ExecuteCommand(command);
        }

        private void DisplayEdgeMovements(MapEdge edge)
        {
            List<MapEdge> edgesIn = new();
            List<MapEdge> edgesOut = new();
            List<MapEdge> edgesInOut;

            var allMovements = edge.To.Movements.Concat(edge.From.Movements);

            foreach (MapNodeMovement movement in allMovements)
            {
                if (movement.from == edge) edgesOut.Add(movement.to);
                else if (movement.to == edge) edgesIn.Add(movement.from);
            }

            edgesInOut = edgesIn.Intersect(edgesOut).ToList();

            colorEdges(edgesIn, movementInColor);
            colorEdges(edgesOut, movementOutColor);
            colorEdges(edgesInOut, movementInAndOutColor);
        }
        
        private void HideEdgeMovements(MapEdge edge)
        {
            List<MapEdge> connectedEdges = edge.To.Movements
            .Concat(edge.From.Movements)
            .Select(m => m.from == edge ? m.to : m.from)
            .ToList();
            
            colorEdges(connectedEdges, _editor.mapGrapher.LineColor);
        }

        private void colorEdges(List<MapEdge> edges, Color color)
        {
            foreach (MapEdge _edge in edges)
                if (selectedEdges.Contains(_edge)) break;
                else _editor.mapGrapher.ChangeEdgeColor(_edge, movementInColor);
        }

        
    }
}