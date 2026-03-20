using Godot;
using System;
using System.Linq;
using signallerMap.Scripts.Data;
using System.Collections.Generic;
using signallerMap.Scripts.Graphics;

namespace signallerMap.Scripts.editor
{
    internal class BuildingMode : IEditorMode
    {
        // All actions related to editing the map structure are defined here.
        // 

        private Editor _editor;
        private MapNode[] selectedNodes;
        private MapEdge[] selectedEdges;
        
        internal BuildingMode(Editor editor)
        {
            _editor = editor;
            editor.SelectedEdges = new MapEdge[2];
            editor.SelectedNodes = new MapNode[2];
            selectedNodes = editor.SelectedNodes;
            selectedEdges = editor.SelectedEdges;

            MapGrapher grapher = _editor.mapGrapher;
            GrapherColors grapherColors = new()
            {
                SelectedNodeColor = Color.FromHtml("ffce1c"),
                SelectedEdgeColor = Color.FromHtml("fcb653"),
                SecondSelectedNodeColor = Color.FromHtml("ffde66") 
            };
            grapher.colors = grapherColors;
        }

        

        public void OnInputEvent(EditorInputEvent inputEvent, EditorInputEventArgs args)
        {
            switch (inputEvent)
            {
                case EditorInputEvent.RMBClick when args is EditorInputMouseClickArgs clickArgs:
                    MouseClick(clickArgs.Position); break;
                case EditorInputEvent.NodeClick when args is EditorInputOnNodeArgs nodeArgs:
                    SelectNode(nodeArgs.Node); break;
                case EditorInputEvent.EdgeClick when args is EditorInputOnEdgeArgs edgeArgs:
                    SelectEdge(edgeArgs.Edge); break;
            }
        }

        public void OnUiEvent(EditorUiEvent uiEvent, EditorUiEventArgs args)
        {
            switch (uiEvent)
            {
                case EditorUiEvent.NodeDeleteButtonPressed:
                    DeleteNode(); break;
                case EditorUiEvent.EdgeCreateButtonPressed when args is EditorUiCreateEdgeArgs edgeArgs:
                    uiCreateEdge(edgeArgs); break;
                case EditorUiEvent.EdgeDeleteButtonPressed:
                    DeleteEdge(); break;
            }
        }

        public void MouseClick(Vector2 position)
        {
            var childrenNodes = _editor.nodeContainer.GetChildren().Where(c => c is Sprite2D).Cast<Sprite2D>();
            MapNode existingNode = MapData.Nodes.FirstOrDefault(n => n.Position.IsEqualApprox(position));

            if (existingNode != null) return;

            _editor.CreateNode(position);
            existingNode = MapData.Nodes.First(n => n.Position.IsEqualApprox(position));
            SelectNode(existingNode);
        }

        private void SelectNode(MapNode node)
        {
            if (node == null) return;
            if (selectedNodes[0] == node) node = selectedNodes[1];
            _editor.mapGrapher.DeselectNode(selectedNodes[1]);
            selectedNodes[1] = selectedNodes[0];
            selectedNodes[0] = node;
            _editor.mapGrapher.SelectNodePair(selectedNodes);
        }

        private void DeleteNode(MapNode node = null)
        {
            if (node == null && selectedNodes[0] != null) node = selectedNodes[0];
            else return;

            var command = new DeleteNodeCommand(_editor, node);
            CommandManager.ExecuteCommand(command);
            
            selectedNodes[0] = selectedNodes[1];
            SelectNode(selectedNodes[0]);
            selectedNodes[1] = null;
        }

        private void uiCreateEdge(EditorUiCreateEdgeArgs args)
        {
            if (selectedNodes[0] == null || selectedNodes[1] == null) return;
            if (int.TryParse(args.EdgeLength, out int el) == false || int.TryParse(args.EdgeSpeed, out int esl) == false) return;
            CreateEdge(selectedNodes[0], selectedNodes[1], el, esl, args.IsStump);
        }

        public void CreateEdge(MapNode from, MapNode to, int length, int maxSpeed, bool stumps = false)
        {
            int fromId = from.Serial;
            int toId = to.Serial;
            string id = from.Prefix + Math.Min(fromId, toId) + Math.Max(fromId, toId);

            MapEdge edge = new MapEdge()
            {
                Id = id,
                From = from,
                To = to,
                Length = length,
                MaxSpeed = maxSpeed
            };

            var command = new CreateEdgeCommand(_editor, edge);
            CommandManager.ExecuteCommand(command);

            SelectEdge(edge);
        }

        private void SelectEdge(MapEdge edge)
        {
            if (edge == null) return;
            DeselectEdge();
            selectedEdges[0] = edge;
            
            MapGrapher grapher = _editor.mapGrapher;
            grapher.SelectEdge(edge);
        }

        private void DeselectEdge()
        {
            if (selectedEdges[0] == null) return;
            _editor.mapGrapher.ClearEdgeColor(selectedEdges[0]);
            selectedEdges[0] = null;
        }

        private void DeleteEdge()
        {
            if (selectedEdges[0] == null) return;
            CommandManager.ExecuteCommand(new DeleteEdgeCommand(_editor, selectedEdges[0]));
        }
    }
}