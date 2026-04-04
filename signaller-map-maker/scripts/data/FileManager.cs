using Godot;
using System.Text.Json;
using System.Collections.Generic;
using signallerMap.Scripts.editor;

namespace signallerMap.Scripts.Data
{
    internal partial class FileManager : Node2D
    {
        [Export] Editor _editor;

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
                Callable.From<bool, string[], int>(SaveMapToFile)
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
                Callable.From<bool, string[], int>(LoadMapFromFile)
            );
        }

        private async void LoadMapFromFile(bool status, string[] selectedPaths, int filterIndex)
        {
            if (!status || selectedPaths.Length <= 0) return;

            string filePath = selectedPaths[0];

            string fileContent = ReadFile(filePath);
            if (string.IsNullOrEmpty(fileContent)) return;
            JsonDocument mapdata = JsonDocument.Parse(fileContent);

            _editor.CleanMap();
            await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);


            Callable.From(() =>
                    {
                        List<MapNode> nodes = MapConverters.Node.ParseFromJson(LoadJsonObject<JsonMapNode>(mapdata, "nodes"));
                        foreach (MapNode node in nodes) _editor.CreateNode(node);

                        List<MapEdge> edges = MapConverters.Edge.ParseFromJson(LoadJsonObject<JsonMapEdge>(mapdata, "edges"));
                        foreach (MapEdge edge in edges) _editor.CreateEdge(edge);

                        List<MapStation> stations = MapConverters.Station.ParseFromJson(LoadJsonObject<JsonMapStation>(mapdata, "stations"));
                        foreach (MapStation station in stations) _editor.CreateStation(station);

                        List<MapPlatform> platforms = MapConverters.Platform.ParseFromJson(LoadJsonObject<JsonMapPlatform>(mapdata, "platforms"));
                        foreach (MapPlatform platform in platforms) _editor.CreatePlatform(platform);

                        List<MapMovement> movements = MapConverters.Movement.ParseFromJson(LoadJsonObject<JsonMapMovement>(mapdata, "movements"));
                        foreach (MapMovement movement in movements) _editor.CreateMovement(movement);

                        List<MapSignal> signals = MapConverters.Signal.ParseFromJson(LoadJsonObject<JsonMapSignal>(mapdata, "signals"));
                        foreach (MapSignal signal in signals) _editor.CreateSignal(signal);

                        MapFactory.UpdateIndecies();
                    }).CallDeferred();


            CommandManager.Clear();
        }

        private string ReadFile(string filePath)
        {
            using var file = FileAccess.Open(filePath, FileAccess.ModeFlags.Read);

            if (file == null)
            {
                GD.PrintErr($"Could not open file at {filePath}. Error: {FileAccess.GetOpenError()}");
                return string.Empty;
            }

            return file.GetAsText();
        }

        private void SaveMapToFile(bool status, string[] selectedPaths, int filterIndex)
        {
            if (!status || selectedPaths.Length <= 0) return;

            string filePath = selectedPaths[0];

            using var file = FileAccess.Open(filePath, FileAccess.ModeFlags.Write);
            if (file == null) return;

            List<JsonMapNode> nodes = MapConverters.Node.ParseToJSON(MapData.Nodes);
            List<JsonMapEdge> edges = MapConverters.Edge.ParseToJSON(MapData.Edges);
            List<JsonMapStation> stations = MapConverters.Station.ParseToJSON(MapData.Stations);
            List<JsonMapPlatform> platforms = MapConverters.Platform.ParseToJSON(MapData.Platforms);
            List<JsonMapSignal> signals = MapConverters.Signal.ParseToJSON(MapData.Signals);

            HashSet<JsonMapMovement> movements = new();
            foreach (MapNode node in MapData.Nodes)
                movements.UnionWith(MapConverters.Movement.ParseToJSON(node.Movements));

            var data = new
            {
                nodes = nodes,
                edges = edges,
                stations = stations,
                platforms = platforms,
                signals = signals,
                movements = movements,
            };

            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(data, options);

            file.StoreString(jsonString);
        }

        private List<T> LoadJsonObject<T>(JsonDocument mapdata, string objName)
        {
            List<T> jsonObjects = new();
            foreach (var obj in mapdata.RootElement.GetProperty(objName).EnumerateArray())
            {
                T jsonObj = JsonSerializer.Deserialize<T>(obj.GetRawText());
                if (jsonObj != null) jsonObjects.Add(jsonObj);
            }
            return jsonObjects;
        }
    }
}
