namespace T8_AI_Lab1_Ants
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void buttonOpenFile_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                //FileName = "Document",
                DefaultExt = ".col",
                Filter = "Collection File (.col)|*.col"
            };
            // Default file name
            // Default file extension
            // Filter files by extension

            // Show open file dialog box
            var result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                var filename = dlg.FileName;
            }

        }
    }
}
