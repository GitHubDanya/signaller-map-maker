using System;
using Godot;
using System.Text.Json;
using System.Collections.Generic;
using signallerMap.Scripts.editor;

namespace signallerMap.Scripts.Data
{
    internal partial class FileManager : Node2D
    {
        [Export] Editor _editor;
        private List<JsonMapEdge> jsonMapEdges;
        private List<JsonMapNode> jsonMapNodes;
        private JsonParser parser = new();
        
        public void LoadData()
        {
            OpenNativeLoadDialog();
        }
        public void SaveData()
        {
            OpenNativeSaveDialog();
        }

        public void OpenNativeSaveDialog()
        {
            string[] filters = { "*.json ; JSON Files" };

            DisplayServer.FileDialogShow(
                "Save signaller Map",         // Title
                "",                       // Initial directory (empty = default)
                "map.json",         // Default filename
                false,                    // Filter indexing (usually false)
                DisplayServer.FileDialogMode.SaveFile,
                filters,
                Callable.From<bool, string[], int>(OnNativeSaveFileSelected)
            );
        }
        public void OpenNativeLoadDialog()
        {
            string[] filters = { "*.json ; JSON Files" };

            DisplayServer.FileDialogShow(
                "Open signaller Map",
                "",
                "map.json",
                false,
                DisplayServer.FileDialogMode.OpenFile,
                filters,
                Callable.From<bool, string[], int>(OnNativeLoadFileSelected)
            );
        }

        private void OnNativeSaveFileSelected(bool status, string[] selectedPaths, int filterIndex)
        {
            if (status && selectedPaths.Length > 0)
            {
                string fullPath = selectedPaths[0];
                SaveJSONToPath(fullPath);
            }
        }

        private void OnNativeLoadFileSelected(bool status, string[] selectedPaths, int filterIndex)
        {
            if (status && selectedPaths.Length > 0)
            {
                string fullPath = selectedPaths[0];
                LoadMapFromJSON(fullPath);
            }
        }

        private void SaveJSONToPath(string path)
        {
            jsonMapNodes = parser.ConvertToJsonMapNodes(MapData.Nodes);
            jsonMapEdges = parser.ConvertToJsonMapEdges(MapData.Edges);

            var data = new { nodes = jsonMapNodes, edges = jsonMapEdges };
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(data, options);

            using var file = FileAccess.Open(path, FileAccess.ModeFlags.Write);
            if (file != null)
            {
                file.StoreString(jsonString);
            }
        }

        private void SaveMapToJSON()
        {
            
        }

        private async void LoadMapFromJSON(string path)
        {
            using var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);

            if (file == null)
            {
                GD.PrintErr($"Could not open file at {path}. Error: {FileAccess.GetOpenError()}");
                return;
            }

            string jsonContent = file.GetAsText();
            JsonDocument mapdata = JsonDocument.Parse(jsonContent);

            _editor.CleanMap();
            await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

            Callable.From(() =>
            {
                List<MapNode> nodes = parser.ConvertToMapNodes(loadJsonNodesFromData(mapdata));
                foreach (MapNode node in nodes) _editor.CreateNode(node);
                List<MapEdge> edges = parser.ConvertToMapEdges(loadJsonEdgesFromData(mapdata));
                foreach (MapEdge edge in edges) _editor.CreateEdge(edge);
            }).CallDeferred();

            CommandManager.Clear();
        }

        private List<JsonMapNode> loadJsonNodesFromData(JsonDocument mapdata)
        {
            List<JsonMapNode> jsonMapNodes = new();
            foreach (var node in mapdata.RootElement.GetProperty("nodes").EnumerateArray())
            {
                JsonMapNode jsonNode = JsonSerializer.Deserialize<JsonMapNode>(node.GetRawText());
                if (jsonNode != null) jsonMapNodes.Add(jsonNode);
            }
            return jsonMapNodes;
        } 

        private List<JsonMapEdge> loadJsonEdgesFromData(JsonDocument mapdata)
        {
            List<JsonMapEdge> jsonMapEdges = new();
            foreach (var edge in mapdata.RootElement.GetProperty("edges").EnumerateArray())
            {
                JsonMapEdge jsonEdge = JsonSerializer.Deserialize<JsonMapEdge>(edge.GetRawText());
                if (jsonEdge != null) jsonMapEdges.Add(jsonEdge);
            }
            return jsonMapEdges;
        } 
    }
}