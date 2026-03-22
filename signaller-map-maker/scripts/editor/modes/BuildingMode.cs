using Godot;
using System;
using System.Linq;
using signallerMap.Scripts.Data;
using System.Collections.Generic;
using signallerMap.Scripts.Graphics;
using System.Xml.Serialization;

namespace signallerMap.Scripts.editor
{
    internal class BuildingMode : IEditorMode
    {
        // This mode is responsible for creating, removing and manipulating map objects.

        private Editor _editor;
        private List<MapNode> selectedNodes;
        private List<MapEdge> selectedEdges;
        
        internal BuildingMode(Editor editor)
        {
            _editor = editor;
            _editor.SetSelectableNodeCount(2);
            _editor.SetSelectableEdgeCount(1);
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
                    _editor.SelectNode(nodeArgs.Node); break;
                case EditorInputEvent.EdgeClick when args is EditorInputOnEdgeArgs edgeArgs:
                    _editor.SelectEdge(edgeArgs.Edge); break;
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

            MapNode node = CreateNodeFromPosition(position);

            CreateNodeCommand command = new(_editor, node);
            CommandManager.ExecuteCommand(command);
            
            _editor.SelectNode(node);
        }
        
        private MapNode CreateNodeFromPosition(Vector2 position)
        {
            _editor.RefreshCurrentNodeID();
            
            string prefix = _editor.NextNodePrefix;
            if (!_editor.NodeIds.ContainsKey(prefix)) _editor.NodeIds[prefix] = 1;
            int serial = _editor.NodeIds[_editor.NextNodePrefix];

            MapNode node = new()
            {
                Serial = serial,
                Prefix = prefix,
                Id = prefix + serial.ToString(),
                Position = position
            };

            return node;
        }

        private void DeleteNode(MapNode node = null)
        {
            if (node == null && selectedNodes.Count > 0) node = selectedNodes[0];
            if (node == null) return;

            var command = new DeleteNodeCommand(_editor, node);
            CommandManager.ExecuteCommand(command);
        }

        private void uiCreateEdge(EditorUiCreateEdgeArgs args)
        {
            if (selectedNodes[0] == null || selectedNodes.Count < 1 || selectedNodes[1] == null) return;
            if (int.TryParse(args.EdgeLength, out int el) == false || int.TryParse(args.EdgeSpeed, out int esl) == false) return;
            CreateEdge(selectedNodes[0], selectedNodes[1], el, esl, args.IsStump);
        }

        public void CreateEdge(MapNode from, MapNode to, int length, int maxSpeed, bool stumps = false)
        {
            int fromId = from.Serial;
            int toId = to.Serial;
            string id = from.Prefix + Math.Min(fromId, toId) + Math.Max(fromId, toId);
            if (from.Prefix != to.Prefix) id += 'N';

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

            _editor.SelectEdge(edge);
        }

        private void DeleteEdge(MapEdge edge = null)
        {
            if (edge == null && selectedEdges.Count > 0) edge = selectedEdges[0];
            if (edge == null) return;
            CommandManager.ExecuteCommand(new DeleteEdgeCommand(_editor, selectedEdges[0]));
        }
    }
}