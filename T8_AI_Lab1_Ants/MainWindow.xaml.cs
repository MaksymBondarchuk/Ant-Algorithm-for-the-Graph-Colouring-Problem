using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.VisualStudio.Modeling.Diagrams;

namespace T8_AI_Lab1_Ants
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly SolidColorBrush _brushBlack = Brushes.Black;
        private readonly SolidColorBrush _brushWhite = Brushes.White;

        private const int NodeFontSize = 20;
        private readonly Graph _graph = new Graph();
        private readonly Random _rand = new Random();



        public MainWindow()
        {
            InitializeComponent();
        }

        private void buttonOpenFile_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".col",
                Filter = "Collection File (.col)|*.col"
            };

            // Show open file dialog box
            var result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                var filename = dlg.FileName;
                _graph.ParseFile(filename);
                DrawGraph();
            }

        }

        private void DrawGraph()
        {
            var n = Convert.ToInt32(Math.Ceiling(Math.Sqrt(_graph.Nodes.Count)));
            //n = Convert.ToInt32(n * 1.5);

            var width = (WindowMain.ActualWidth - 75) / n;
            var height = WindowMain.ActualHeight / n;

            for (var i = 0; i < _graph.Nodes.Count; i++)
            {
                var x = i % n;
                var y = i / n;

                AddNotVisual(new Point(width * x + width * .5, height * y + height * .5), i);
            }

            for (var i = 0; i < _graph.Nodes.Count; i++)
                foreach (var t in _graph.Nodes[i].ConnectedWith)
                    ConnectNotVisual(i, t);

            for (var i = 0; i < _graph.Nodes.Count; i++)
            {
                _graph.Nodes[i].ColorNumber = _rand.Next(_graph.ChromaticNumber);
                FillNode(i, new SolidColorBrush(GetColor(_graph.Nodes[i].ColorNumber)));
            }
        }

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
                Fill = _brushWhite,
            };
            grid.Children.Add(circle);

            var textBlock = new TextBlock
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                TextAlignment = TextAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = NodeFontSize,
                Text = (idx + 1).ToString()
            };
            grid.Children.Add(textBlock);
            CanvasMain.Children.Add(grid);
            Panel.SetZIndex(CanvasMain.Children[CanvasMain.Children.Count - 1], 2);

            _graph.Nodes[idx].Location = pos;
            _graph.Nodes[idx].CanvasIdx = CanvasMain.Children.Count - 1;
        }

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
        }

        private void FillNode(int nodeIdx, Brush color)
        {
            var grid = (Grid)CanvasMain.Children[_graph.Nodes[nodeIdx].CanvasIdx];
            var nodeGray = (Ellipse)grid.Children[0];
            nodeGray.Fill = color;
            grid.Children[0] = nodeGray;
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

        }
    }
}
