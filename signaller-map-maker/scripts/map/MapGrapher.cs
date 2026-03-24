using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using signallerMap.Scripts.Data;
using signallerMap.Scripts.editor;

namespace signallerMap.Scripts.Graphics
{
    internal partial class MapGrapher : Node2D
    {
        [Export] Texture2D dangerSignalTexture { get; set; }
        [Export] Texture2D cautionSignalTexture { get; set; }
        [Export] Texture2D preliminaryCautionSignalTexture { get; set; }
        [Export] Texture2D proceedSignalTexture { get; set; }
        private Node2D edgesContainer;
        private Node2D nodesContainer;
        private Node2D signalsContainer;
        private Editor _editor;
        public readonly float LineWidth = 3.5f;
        public readonly int StumpLength = 10;
        public readonly int SignalXOffset = 10;
        public readonly float SignalYOffset = 5.5f;
        public readonly Color LineColor = Color.FromHtml("FFFFFF");
        public GrapherColors colors = new();

        public override void _Ready()
        {
            edgesContainer = GetNode<Node2D>("EdgeContainer");
            nodesContainer = GetNode<Node2D>("NodeContainer");
            signalsContainer = GetNode<Node2D>("SignalContainer");
            _editor = GetNode<Editor>("/root/Map/Editor");
        }

        public void DrawEdge(MapEdge edge)
        {
            if (edge.From == null || edge.To == null) return;

            ColorRect line = new ColorRect();
            line.Color = LineColor;
            line.ZIndex = 1;

            Vector2 startPos = edge.From.Position;
            Vector2 endPos = edge.To.Position;
            Vector2 direction = endPos - startPos;

            float length = direction.Length();
            float angle = direction.Angle();

            line.Size = new Vector2(length, LineWidth);

            line.PivotOffset = new Vector2(0, LineWidth / 2f);

            line.Position = startPos - new Vector2(0, LineWidth / 2f);
            line.Rotation = angle;

            line.MouseFilter = Control.MouseFilterEnum.Pass;
            line.GuiInput += (inputEvent) =>
            {
                if (inputEvent is InputEventMouseButton mouse && mouse.Pressed)
                {
                    if (mouse.ButtonIndex == MouseButton.Left)
                    {
                        _editor.EdgeClick(edge);
                    }
                }
            };

            line.MouseEntered += () => _editor.MouseEnterEdgeEvent(edge);
            line.MouseExited += () => _editor.MouseExitEdgeEvent(edge);

            edge.Sprite = line;
            edgesContainer.AddChild(line);
        }

        public void SelectEdgePair(List<MapEdge> edges)
        {
            if (edges.Count > 0 && edges[0]?.Sprite != null) edges[0].Sprite.Color = colors.SelectedEdgeColor;
            if (edges.Count > 1 && edges[1]?.Sprite != null) edges[1].Sprite.Color = colors.SecondSelectedEdgeColor;
        }
        
        public void SelectEdge(MapEdge edge)
        {
            if (edge == null || !IsInstanceValid(edge.Sprite)) return;
            edge.Sprite.Color = colors.SelectedEdgeColor;
        }

        public void ChangeEdgeColor(MapEdge edge, Color color)
        {
            if (edge == null) return;
            edge.Sprite.Color = color;
        }

        public void DeselectEdge(MapEdge edge)
        {
            if (edge == null || edge.Sprite == null) return;
            edge.Sprite.Color = LineColor;
        }

        public void DrawNode(MapNode node)
        {
            IEnumerable<Sprite2D> childrenNodes = nodesContainer.GetChildren().Where(c => c is Sprite2D).Cast<Sprite2D>();
            var children = nodesContainer.GetChildren();
            if (childrenNodes.Any(
                n => n.Position.IsEqualApprox(node.Position)
            )) return;

            Sprite2D sprite = new Sprite2D()
            {
                Name = node.Id,
                Texture = GD.Load<Texture2D>("res://assets/background_center.png"),
                Position = Vector2.Zero,
                GlobalPosition = node.Position,
                Scale = new Vector2(0.35f, 0.35f),
                ZIndex = 10
            };
            node.Sprite = sprite;

            var area = new Area2D();
            area.ZIndex = 10;
            var collision = new CollisionShape2D
            {
                Shape = new RectangleShape2D { Size = sprite.Texture.GetSize() }
            };

            area.AddChild(collision);
            sprite.AddChild(area);

            area.InputEvent += (viewport, @event, shapeIdx) =>
            {
                if (@event is InputEventMouseButton mb && mb.Pressed && mb.ButtonIndex == MouseButton.Left)
                    _editor.NodeClick(node);
            };

            nodesContainer.AddChild(sprite);
        }

        public void SelectNodePair(List<MapNode> nodes)
        {
            if (nodes.Count > 0 && nodes[0]?.Sprite != null) nodes[0].Sprite.Modulate = colors.SelectedNodeColor;
            if (nodes.Count > 1 && nodes[1]?.Sprite != null) nodes[1].Sprite.Modulate = colors.SecondSelectedNodeColor;
        }

        public void SelectNode(MapNode node)
        {
            if (node == null) return;
            node.Sprite.Modulate = colors.SelectedNodeColor;
        }

        public void DeselectNode(MapNode node)
        {
            if (node == null || node.Sprite == null) return;
            node.Sprite.Modulate = LineColor;
        }

        public void DrawSignal(MapSignal signal)
        {
            MapMovement movement = signal.Movement;
            MapNode node = movement.GetNode();
            MapNode sourceNode = movement.GetSourceNode();

            Texture2D texture = signal.State switch
            {
                SignalState.Danger => dangerSignalTexture,
                SignalState.DoubleYellow => preliminaryCautionSignalTexture,
                SignalState.Caution => cautionSignalTexture,
                _ => proceedSignalTexture
            };

            Vector2 direction = (node.Position - sourceNode.Position).Normalized();
            Vector2 position = node.Position - (direction * SignalXOffset);
            float angle = (float)Math.Atan2(direction.Y, direction.X);
            Vector2 offset = new Vector2(0, -SignalYOffset).Rotated(angle);
            position += offset;

            
            Sprite2D sprite = new()
            {
                Name = signal.Id,
                Position = position,
                Texture = texture,
                Rotation = angle,
                TextureFilter = TextureFilterEnum.Nearest,
                Scale = new Vector2(0.5f, 0.5f) 
            };

            signal.Sprite = sprite;
            signalsContainer.AddChild(sprite);
        }

        // public void LoadStumps()
        // {
        //     List<MapEdge> edges = MapData.Edges;
        //     foreach (MapEdge edge in edges)
        //     {
        //         if (edge.From.IncomingEdges.Count == 0)
        //             DrawStump(edge, edge.From);
        //         else if (edge.To.OutgoingEdges.Count == 0) 
        //             DrawStump(edge, edge.To);
        //     }
        // }

        // private void DrawStump(MapEdge edge, MapNode node)
        // {
        //     if (edge == null || node == null) return;

        //     Vector2 stumpDir = (edge.From.Position - edge.To.Position).Orthogonal().Normalized() * StumpLength;

        //     Line2D stump = new();
        //     stump.Width = LineWidth;
        //     stump.DefaultColor = LineColor;
        //     stump.Antialiased = true;

        //     Vector2 stumpTopEdge = node.Position + (stumpDir / 2);
        //     Vector2 stumpBottomEdge = node.Position - (stumpDir / 2);

        //     stump.AddPoint(stumpTopEdge);
        //     stump.AddPoint(stumpBottomEdge);

        //     edgesContainer.AddChild(stump);
        // }

        // public void LoadStations()
        // {
        //     List<MapStation> stations = MapData.Stations;
        //     Node stationsContainer = GetNode<Node>("StationsContainer");
        //     foreach (Node child in stationsContainer.GetChildren())
        //     {
        //         child.QueueFree();
        //     }

        //     foreach (MapStation station in stations)
        //     {
        //         DrawStation(station);
        //     }
        // }

        // public void DrawStation(MapStation station)
        // {
        //     Node stationsContainer = GetNode<Node>("StationsContainer");
        //     Node2D stationNode = new Node2D();
        //     stationNode.Name = station.Id;

        //     stationsContainer.AddChild(stationNode);


        //     foreach (MapStationPlatform platform in station.Platforms)
        //     {
        //         stationNode.AddChild(platform.Sprite);
        //     }
        //     DrawStationLabels(station, stationNode);
        // }

        // private void DrawStationLabels(MapStation station, Node2D stationNode)
        // {
        //     Label stationTitle = new Label();
        //     stationTitle.Text = station.Name;
        //     stationTitle.HorizontalAlignment = HorizontalAlignment.Center;
        //     stationTitle.VerticalAlignment = VerticalAlignment.Center;
        //     stationTitle.AddThemeFontSizeOverride("font_size", 16);
        //     stationTitle.Size = new Vector2(stationNode.Scale.X, stationTitle.Size.Y);
        //     stationTitle.AddThemeColorOverride("font_color", Color.FromHtml("#FFFFFF"));

        //     float furthestX = float.MinValue;
        //     float nearestX = float.MaxValue;
        //     float highestY = float.MaxValue;

        //     foreach (Node node in stationNode.GetChildren())
        //     {
        //         if (node is not ColorRect child) continue;

        //         float xRight = child.Position.X + child.Size.X;
        //         float xLeft = child.Position.X;
        //         float y = child.Position.Y;

        //         if (xRight > furthestX) furthestX = xRight;
        //         if (xLeft < nearestX) nearestX = xLeft;
        //         if (y < highestY) highestY = y;
        //     }

        //     float stationX = (furthestX + nearestX) / 2;

        //     Vector2 centeredPosition = new Vector2(stationX, highestY);

        //     stationTitle.Position = centeredPosition - new Vector2(stationTitle.Size.X / 2, 50);

        //     stationNode.AddChild(stationTitle);
        // }

        // private void LoadSignals()
        // {
        //     List<MapNode> nodes = MapData.Nodes;
        //     foreach (Node child in signalsContainer.GetChildren())
        //     {
        //         child.QueueFree();
        //     }

        //     foreach (MapNode node in nodes)
        //     {
        //         int signalCount = 1;
        //         foreach (MapSignal signal in node.Signals)
        //         {
        //             DrawSignal(signal);
        //             signalCount++;
        //         }
        //     }
        // }



        // public void UpdateAllSections()
        // {
        //     foreach (MapEdge section in MapData.Edges)
        //     {
        //         UpdateSection(section);
        //     }
        // }

        // public void UpdateSection(MapEdge section)
        // {
        //     Train train = section.Train;

        //     if (train == null)
        //     {
        //         UnoccupySection(section);
        //         RemoveTrainColor(section);
        //     }
        //     else
        //     {
        //         OccupySection(section);

        //         bool trainTRTS = train.OccupiedSections.Last().Edge.Platform != null && train.Speed == 0 && !train.LoadingPassengers;
        //         bool trainSPAD = train.SPAD;

        //         if (trainTRTS) SetTrainTRTS(section);
        //         if (trainSPAD) SetTrainSPAD(section);
        //         if (!trainTRTS && !trainSPAD) RemoveTrainColor(section);

        //         int index = train.Route.IndexOf(section);
        //         if (index > 0)
        //         {
        //             MapEdge previousSection = train.Route[index - 1];
        //             UpdateSection(previousSection);
        //         }
        //     }
        // }

        // public void OccupySection(MapEdge sectionEdge)
        // {
        //     sectionEdge.Sprite.Width = OccupiedLineWidth;
        // }

        // private void UnoccupySection(MapEdge sectionEdge)
        // {
        //     sectionEdge.Sprite.Width = LineWidth;
        // }

        // private void SetTrainTRTS(MapEdge sectionEdge)
        // {
        //     sectionEdge.Sprite.DefaultColor = Color.FromHtml(LineTRTScolor);
        // }
        // private void SetTrainSPAD(MapEdge sectionEdge)
        // {
        //     sectionEdge.Sprite.DefaultColor = Color.FromHtml(SignalRedColor);
        // }

        // private void RemoveTrainColor(MapEdge sectionEdge)
        // {
        //     sectionEdge.Sprite.DefaultColor = LineColor;
        // }

    }

    internal partial class UiMapEdge : Line2D
    {
        public MapEdge Data { get; set; }
    }

    internal class GrapherColors
    {
        public Color SelectedNodeColor { get; set; } = Color.FromHtml("ffce1c");
        public Color SelectedEdgeColor { get; set; } = Color.FromHtml("fcb653");
        public Color SecondSelectedNodeColor { get; set; } = Color.FromHtml("ffde66");
        public Color SecondSelectedEdgeColor { get; set; } = Color.FromHtml("f7be6d");
    }
}
