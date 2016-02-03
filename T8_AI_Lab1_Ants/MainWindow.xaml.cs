using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace T8_AI_Lab1_Ants
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly SolidColorBrush _brushPressedButton = new SolidColorBrush(Color.FromRgb(196, 229, 246));
        private readonly SolidColorBrush _brushDisabled = Brushes.IndianRed;
        private readonly SolidColorBrush _brushActive = Brushes.Red;
        private readonly SolidColorBrush _brushBlack = Brushes.Black;
        private readonly SolidColorBrush _brushWhite = Brushes.White;
        private readonly SolidColorBrush _brushGrey = Brushes.LightGray;

        private const int NodeFontSize = 20;
        private Graph _graph = new Graph();



        public MainWindow()
        {
            InitializeComponent();
        }

        private void buttonOpenFile_Click(object sender, System.Windows.RoutedEventArgs e)
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
            }

        }

        private void AddNotVisual(Point pos, string id)
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
                Text = id
            };
            grid.Children.Add(textBlock);
            CanvasMain.Children.Add(grid);
            Panel.SetZIndex(CanvasMain.Children[CanvasMain.Children.Count - 1], 2);

            _graph.Nodes.Add(new Node
            {
                Location = pos,
                CanvasIdx = CanvasMain.Children.Count - 1
            });
        }
    }
}
