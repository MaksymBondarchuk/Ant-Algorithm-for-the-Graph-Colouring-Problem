using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.VisualStudio.Modeling.Diagrams;
using Microsoft.Win32;

// For button click functions call
namespace System.Windows.Controls
{
    /// <summary>
    /// For allow perform button click 
    /// </summary>
    public static class MyExt
    {
        public static void PerformClick(this Button btn)
        {
            btn.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        }
    }
}

namespace T8_AI_Lab1_Ants
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        /// <summary>
        /// Brush for borders
        /// </summary>
        private readonly SolidColorBrush _brushBlack = Brushes.Black;
        /// <summary>
        /// Brush for text
        /// </summary>
        private readonly SolidColorBrush _brushWhite = Brushes.White;
        /// <summary>
        /// Brush for selected node fill
        /// </summary>
        private readonly SolidColorBrush _brushGrey = Brushes.LightGray;

        /// <summary>
        /// Graph
        /// </summary>
        private readonly Graph _graph = new Graph();
        /// <summary>
        /// For generate random numbers
        /// </summary>
        private readonly Random _rand = new Random();

        /// <summary>
        /// Index of a node where pointer is (-1 for not on a node)
        /// </summary>
        private int _onNode = -1;
        /// <summary>
        /// Flag for check is graph prepered for algorithm
        /// </summary>
        private bool _isGraphPrepared;
        /// <summary>
        /// Flag for show visual part (draw graph or work only in console)
        /// </summary>
        private bool _drawGraph;

        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// For Collection file selection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOpenFile_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                DefaultExt = ".col",
                Filter = "Collection File (.col)|*.col"
            };

            // Show open file dialog box
            var result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                ButtonClear.PerformClick();

                // Open document
                var filename = dlg.FileName;
                _graph.ParseFile(filename);

                // If graph is too big ask does user sure about to draw it
                if (50 < _graph.Nodes.Count)
                {
                    var res = MessageBox.Show("Graph has more nodes than 50." +
                                              "\nDo you want to draw it?" +
                                              "\nIf no, it still will be colored.",
                        "Graph is too big", MessageBoxButton.YesNo);
                    _drawGraph = res == MessageBoxResult.Yes;
                }
                else
                    _drawGraph = true;

                if (_drawGraph)
                    DrawGraph();

                _graph.AntsNumber = _graph.Nodes.Count / 3;
                TextBoxAntsNumber.Text = _graph.AntsNumber.ToString();
            }
        }

        /// <summary>
        /// Draws graph on canvas
        /// </summary>
        private void DrawGraph()
        {
            var n = Convert.ToInt32(Math.Ceiling(Math.Sqrt(_graph.Nodes.Count)));
            if (_graph.Nodes.Count == 8)
                n = 4;
            if (_graph.Nodes.Count == 20)
                n = 10;
            //n = Convert.ToInt32(n * 1.5);

            var width = (WindowMain.ActualWidth - 400) / n;
            var height = WindowMain.ActualHeight / n;

            for (var i = 0; i < _graph.Nodes.Count; i++)
            {
                var x = i % n;
                var y = i / n;

                _graph.Nodes[i].ColorNumber = _rand.Next(_graph.ChromaticNumber);
                AddNotVisual(new Point(width * x + width * .5, height * y + height * .5), i);
            }

            for (var i = 0; i < _graph.Nodes.Count; i++)
            {
                var i1 = i;
                foreach (var t in _graph.Nodes[i].ConnectedWith.Where(t => i1 < t))
                    ConnectNotVisual(i, t);
            }

            RecolorNodes();
        }

        /// <summary>
        /// Updates color of nodes
        /// </summary>
        private void RecolorNodes()
        {
            for (var i = 0; i < _graph.Nodes.Count; i++)
                FillNode(i, new SolidColorBrush(GetColor(_graph.Nodes[i].ColorNumber)));
        }

        /// <summary>
        /// Adds node to canvas
        /// </summary>
        /// <param name="pos">Location on canvas</param>
        /// <param name="idx">Index of node (for text on it)</param>
        private void AddNotVisual(Point pos, int idx)
        {
            var grid = new Grid
            {
                Height = Node.NodeSize,
                Width = Node.NodeSize,
                Margin = new Thickness(pos.X - Node.NodeSize * .5, pos.Y - Node.NodeSize * .5, 0, 0)
            };

            var circle = new Ellipse
            {
                Height = Node.NodeSize,
                Width = Node.NodeSize,
                StrokeThickness = 2,
                Stroke = _brushBlack,
                Fill = _brushWhite
            };
            grid.Children.Add(circle);

            var textBlock = new TextBlock
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                TextAlignment = TextAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = Node.NodeFontSize,
                Text = (idx + 1).ToString()
            };
            grid.Children.Add(textBlock);
            CanvasMain.Children.Add(grid);
            Panel.SetZIndex(CanvasMain.Children[CanvasMain.Children.Count - 1], 2);

            _graph.Nodes[idx].Location = pos;
            _graph.Nodes[idx].CanvasIdx = CanvasMain.Children.Count - 1;
        }

        /// <summary>
        /// Connects two nodes on canvas
        /// </summary>
        /// <param name="idx1">First node index</param>
        /// <param name="idx2">Second node index</param>
        private void ConnectNotVisual(int idx1, int idx2)
        {
            var conn = new Line
            {
                X1 = _graph.Nodes[idx1].Location.X,
                Y1 = _graph.Nodes[idx1].Location.Y,
                X2 = _graph.Nodes[idx2].Location.X,
                Y2 = _graph.Nodes[idx2].Location.Y,
                StrokeThickness = 2,
                Stroke = _brushBlack
            };

            CanvasMain.Children.Add(conn);
            Panel.SetZIndex(CanvasMain.Children[CanvasMain.Children.Count - 1], 1);

            _graph.Connections.Add(new Connection
            {
                P1 = _graph.Nodes[idx1].Location,
                Node1 = idx1,
                P2 = _graph.Nodes[idx2].Location,
                Node2 = idx2,
                CanvasIdx = CanvasMain.Children.Count - 1
            });

            //_graph.Nodes[idx1].ConnectedWith.Add(idx2);
            _graph.Nodes[idx1].ConnectedBy.Add(_graph.Connections.Count - 1);
            //_graph.Nodes[idx2].ConnectedWith.Add(idx1);
            _graph.Nodes[idx2].ConnectedBy.Add(_graph.Connections.Count - 1);

        }

        /// <summary>
        /// Fills node on canvas
        /// </summary>
        /// <param name="nodeIdx">Index of node</param>
        /// <param name="color">Color to fill node in</param>
        private void FillNode(int nodeIdx, Brush color)
        {
            var grid = (Grid)CanvasMain.Children[_graph.Nodes[nodeIdx].CanvasIdx];
            var nodeGray = (Ellipse)grid.Children[0];
            nodeGray.Fill = color;
        }

        /// <summary>
        /// Calculates color
        /// </summary>
        /// <param name="number">Number from set</param>
        /// <returns>Color with number</returns>
        private Color GetColor(int number)
        {
            var dColor = new HslColor(number * 239 / _graph.ChromaticNumber, 240, 100).ToRgbColor();
            return Color.FromArgb(dColor.A, dColor.R, dColor.G, dColor.B);
        }

        private void ButtonColor_Click(object sender, RoutedEventArgs e)
        {
            _graph.Color();
            if (_drawGraph)
                RecolorNodes();
            _isGraphPrepared = false;
            MessageBox.Show($"Done in {_graph.IterationNumber} iterations");
        }

        private void TextBoxAntsNumber_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            int result;
            e.Handled = !int.TryParse(e.Text, out result);
        }

        private void WindowMain_MouseMove(object sender, MouseEventArgs e)
        {
            var mousePos = Mouse.GetPosition(CanvasMain);

            if (Mouse.LeftButton == MouseButtonState.Pressed && _onNode != -1)
            {
                Panel.SetZIndex(CanvasMain.Children[_graph.Nodes[_onNode].CanvasIdx], 4);
                MoveNode(mousePos);
            }

            if (_drawGraph)
                for (var i = 0; i < _graph.Nodes.Count; i++)
                FillNode(i,
                    _graph.Nodes[i].IsMyPoint(mousePos)
                        ? _brushGrey
                        : new SolidColorBrush(GetColor(_graph.Nodes[i].ColorNumber)));
        }

        private void WindowMain_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_drawGraph)
                for (var i = 0; i < _graph.Nodes.Count; i++)
                if (_graph.Nodes[i].IsMyPoint(Mouse.GetPosition(CanvasMain)))
                {
                    _onNode = i;
                    break;
                }
        }

        private void WindowMain_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _onNode = -1;
        }

        /// <summary>
        /// Changes node location
        /// </summary>
        /// <param name="mousePos">New location</param>
        private void MoveNode(Point mousePos)
        {
            // Moving node
            var grid = (Grid)CanvasMain.Children[_graph.Nodes[_onNode].CanvasIdx];
            grid.Margin = new Thickness(mousePos.X - grid.Height * .5, mousePos.Y - grid.Width * .5, 0, 0);

            // Moving node connections
            foreach (var conn in _graph.Nodes[_onNode].ConnectedBy)
                if (_graph.Nodes[_onNode].Location == _graph.Connections[conn].P1)
                {
                    _graph.Connections[conn].P1 = mousePos;

                    var line = (Line)CanvasMain.Children[_graph.Connections[conn].CanvasIdx];
                    line.X1 = mousePos.X;
                    line.Y1 = mousePos.Y;
                }
                else if (_graph.Nodes[_onNode].Location == _graph.Connections[conn].P2)
                {
                    _graph.Connections[conn].P2 = mousePos;

                    var line = (Line)CanvasMain.Children[_graph.Connections[conn].CanvasIdx];
                    line.X2 = mousePos.X;
                    line.Y2 = mousePos.Y;
                }

            _graph.Nodes[_onNode].Location = mousePos;
        }

        private void ButtonPrepare_Click(object sender, RoutedEventArgs e)
        {
            _graph.AntsNumber = Convert.ToInt32(TextBoxAntsNumber.Text);
            _graph.PrepareToColor();
            if (_drawGraph)
                UpdateNodesInfo();
            _isGraphPrepared = true;
        }

        private void ButtonOneIteration_Click(object sender, RoutedEventArgs e)
        {
            if (!_isGraphPrepared)
                ButtonPrepare.PerformClick();

            _graph.OneIteration();
            if (_drawGraph)
            {
                UpdateNodesInfo();
                RecolorNodes();
            }
            if (_graph.GetIsColored())
            {
                _isGraphPrepared = false;
                MessageBox.Show($"Done in {_graph.IterationNumber} iterations");
            }
        }

        /// <summary>
        /// Updates text on nodes (location of ants, conflicts number)
        /// </summary>
        private void UpdateNodesInfo()
        {
            for (var i = 0; i < _graph.Nodes.Count; i++)
            {
                var grid = (Grid)CanvasMain.Children[_graph.Nodes[i].CanvasIdx];
                var textBlock = (TextBlock)grid.Children[1];

                var text = $"{i} c{_graph.Nodes[i].ConflictsNumber} ";

                for (var j = 0; j < _graph.Ants.Count; j++)
                    if (_graph.Ants[j] == i)
                        text += $"a{j} ";

                textBlock.Text = text;
            }
        }

        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            CanvasMain.Children.Clear();
            _graph.Clear();
        }

        private void ButtonResetGraph_Click(object sender, RoutedEventArgs e)
        {
            foreach (var t in _graph.Nodes)
                t.ColorNumber = _rand.Next(_graph.ChromaticNumber);
            if (_drawGraph)
                RecolorNodes();
        }
    }
}
