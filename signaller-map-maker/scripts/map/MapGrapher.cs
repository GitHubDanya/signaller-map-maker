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
        private Node2D edgesContainer;
        private Node2D nodesContainer;
        private Editor _editor;
        public const float LineWidth = 3.5f;
        public const int StumpLength = 10;
        public const int SignalRadius = 5;
        public const string LineColor = "FFFFFF";
        public const string SelectedNodeColor = "ffce1c";
        public const string SecondSelectedNodeColor = "ffde66";
        public const string SelectedEdgeColor = "fcb653";

        public override void _Ready()
        {
            edgesContainer = GetNode<Node2D>("EdgeContainer");
            nodesContainer = GetNode<Node2D>("NodeContainer");
            _editor = GetNode<Editor>("/root/Map/Editor");
        }
        
        public void DrawMap()
        {
            LoadNodes();
            //LoadEdges();
            //LoadStumps();
            // LoadStations();
            // LoadSignals();
        }

        public void LoadEdges()
        {
            List<MapEdge> edges = MapData.Edges;
            
            foreach (Node child in edgesContainer.GetChildren())
            {
                child.QueueFree();
            }

            foreach (var edge in edges)
            {
                DrawEdge(edge);
            }
        }

        public void DrawEdge(MapEdge edge)
        {
            if (edge.From == null || edge.To == null) return;

            ColorRect line = new ColorRect();
            line.Color = Color.FromHtml(LineColor);
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
                        _editor.SelectEdge(edge);
                    }
                }
            };

            edge.Sprite = line;
            edgesContainer.AddChild(line);
        }

        public void SelectEdge(MapEdge edge)
        {
            if (edge == null || edge.Sprite == null) return;
            edge.Sprite.Color = Color.FromHtml(SelectedEdgeColor);
        }

        public void DeselectEdge(MapEdge edge)
        {
            if (edge == null || edge.Sprite == null) return;
            edge.Sprite.Color = Color.FromHtml(LineColor);
        }

        public void LoadStumps()
        {
            List<MapEdge> edges = MapData.Edges;
            foreach (MapEdge edge in edges)
            {
                if (edge.From.IncomingEdges.Count == 0)
                    DrawStump(edge, edge.From);
                else if (edge.To.OutgoingEdges.Count == 0) 
                    DrawStump(edge, edge.To);
            }
        }

        private void DrawStump(MapEdge edge, MapNode node)
        {
            if (edge == null || node == null) return;

            Vector2 stumpDir = (edge.From.Position - edge.To.Position).Orthogonal().Normalized() * StumpLength;
            
            Line2D stump = new();
            stump.Width = LineWidth;
            stump.DefaultColor = Color.FromHtml(LineColor);
            stump.Antialiased = true;
            
            Vector2 stumpTopEdge = node.Position + (stumpDir / 2);
            Vector2 stumpBottomEdge = node.Position - (stumpDir / 2);

            stump.AddPoint(stumpTopEdge);
            stump.AddPoint(stumpBottomEdge);

            edgesContainer.AddChild(stump);
        }

        public void LoadNodes()
        {
            foreach (MapNode node in MapData.Nodes)
            {
                DrawNode(node);
            }
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
                    _editor.SelectNode(node);
            };

            nodesContainer.AddChild(sprite);
        }

        public void SelectNodePair(MapNode[] node)
        {
            if (node[0]?.Sprite != null) node[0].Sprite.Modulate = Color.FromHtml(SelectedNodeColor);
            if (node[1]?.Sprite != null) node[1].Sprite.Modulate = Color.FromHtml(SecondSelectedNodeColor);
        }

        public void DeselectNode(MapNode node)
        {
            if (node == null || node.Sprite == null) return;
            node.Sprite.Modulate = Color.FromHtml(LineColor);
        }
        
        public void Undo()
        {
            
        }

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

        // public void DrawSignal(MapSignal signal, float yOffset = 20)
        // {
        //     if (signalsContainer.GetNodeOrNull<SignalSprite>(signal.Id) != null) return;

        //     MapNode signalNode = signal.Node;

        //     int signalIndex = signalNode.Signals.IndexOf(signal) + 1;
        //     yOffset *= signalIndex;
    
        //     SignalSprite signalSprite = new SignalSprite();
        //     signalSprite.Radius = SignalRadius;
        //     signalSprite.Position = signalNode.Position;
        //     signalSprite.Name = signal.Id;
        //     signalSprite.Signal = signal;

        //     string signalColor = SignalGreenColor;

        //     switch (signal.State)
        //     {
        //         case SignalState.Danger: signalColor = SignalRedColor; break;
        //         case SignalState.Amber:
        //         case SignalState.DoubleYellow: signalColor = SignalYellowColor; break;
        //         case SignalState.Proceed: signalColor = SignalGreenColor; break;
        //     }

        //     signalSprite.Color = Color.FromHtml(signalColor);

        //     Vector2 offset = new Vector2(0, yOffset);
        //     if (signal.DrawBelowEdge) signalSprite.Position += offset;
        //     else signalSprite.Position -= offset; 

        //     signalsContainer.AddChild(signalSprite);

        //     MapSignalMovement movement = signal.SignalMovement;
        //     Polygon2D signalDirectionSprite = new Polygon2D();
        //     signalDirectionSprite.Polygon = new Vector2[]
        //     {
        //             new Vector2(0, -SignalRadius),
        //             new Vector2(0, SignalRadius),
        //             new Vector2(SignalRadius * 2f, 0)
        //     };

        //     Vector2 SourcePos = signal.Node.Position;
        //     Vector2 TargetPos;

        //     if (movement.From.To.Id == signal.Node.Id) // the movement is left to right
        //     {
        //         // the movement vector is towards the end of the right node
        //         TargetPos = movement.To.To.Position;
        //     }
        //     else if (movement.To.To.Id == signal.Node.Id) // the movement is right to left
        //     {
        //         // the movement vector is towards the start of the left node
        //         TargetPos = movement.To.From.Position;
        //     }
        //     else return;

        //     Vector2 signalDirection = TargetPos - SourcePos;
        //     signalDirectionSprite.Rotation = signalDirection.Angle();

        //     signalDirectionSprite.Color = signalSprite.Color;
        //     signalSprite.AddChild(signalDirectionSprite);
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
        //     sectionEdge.Sprite.DefaultColor = Color.FromHtml(LineColor);
        // }
        
    }

    internal partial class UiMapEdge : Line2D
    {
        public MapEdge Data { get; set; }
    }

}
