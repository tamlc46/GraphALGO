using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using System.IO;
using System.Text.RegularExpressions;

namespace GraphTheory
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = Graphbox;
            Invalidated();
            fileName = "New Graph";
        }
        private string _fileName;
        private string fileName
        {
            get { return _fileName; }
            set
            {
                _fileName = value;
                FileStatus.Text = "Working On: " + _fileName;
            }
        }
        private string filePath;
        private bool Changed = false;
        private void resizeElement(object sender, SizeChangedEventArgs e)
        {
            mnuAlgorithm.Width = mnuGeneral.Width = this.RenderSize.Width / 2 - 10;
        }
        private void UnCheck(object sender, RoutedEventArgs e)
        {
            RadioButton obj = (RadioButton)sender;
            if (obj.IsChecked == true)
            {
                obj.IsChecked = false;
                e.Handled = true;
            }
        }
        private void Graph_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (BtnAddNode.IsChecked == true) { Graphbox.AddNode(); this.BtnKruskal.IsEnabled = true; }
            if (BtnAddConnect.IsChecked == true || BtnAddNode.IsChecked == true || BtnDelConnect.IsChecked == true || BtnDelNode.IsChecked == true) Changed = true;
            int n = Graphbox.NodesCount, edge = Graphbox.EdgesCount;
            if (n == 0) { BtnDelNode.IsChecked = false; BtnDelNode.IsEnabled = false; }
            else BtnDelNode.IsEnabled = true;
            if (n < 2 || edge >= (n * (n - 1)) / 2) { BtnAddConnect.IsChecked = false; BtnAddConnect.IsEnabled = false; }
            else BtnAddConnect.IsEnabled = true;
            if (n < 26) BtnAddNode.IsEnabled = true;
            else { BtnAddNode.IsChecked = false; BtnAddNode.IsEnabled = false; }
            if (edge == 0) { BtnDelConnect.IsChecked = false; BtnDelConnect.IsEnabled = false; }
            else BtnDelConnect.IsEnabled = true;
            if (!Graphbox.RunningAlgorithm && !BtnAddConnect.IsChecked==true && !BtnAddNode.IsChecked==true && !BtnDelConnect.IsChecked==true && !BtnDelNode.IsChecked==true && e.ChangedButton!=MouseButton.XButton1)
                Graphbox.Invalidated();
        }
        private void ShowInfo(object sender, RoutedEventArgs e)
        {
            AboutBox aboutBox = new AboutBox();
            aboutBox.Owner = this;
            aboutBox.ShowDialog();
        }
        private void NewGraph(object sender, RoutedEventArgs e)
        {
            if (Graphbox.NodesCount!=0 && Changed)
            {
                MessageBoxResult res = MessageBox.Show("Do you want to save your work?", "New Graph...", MessageBoxButton.YesNoCancel, MessageBoxImage.Exclamation);
                if (res == MessageBoxResult.Yes) SaveGraph(this, new RoutedEventArgs());
                else if (res == MessageBoxResult.Cancel) return;
            }
            Graphbox.Clear();
            fileName = "New Graph";
            filePath = string.Empty;
            Graph_MouseDown(this, new MouseButtonEventArgs(Mouse.PrimaryDevice, 1, MouseButton.Left));
            Graphbox.Invalidated();
            this.BtnKruskal.IsEnabled = false;
        }
        private void ShowHelp(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("No Document!!!");
        }
        public void Invalidated()
        {
            Graphbox.Invalidated();
        }
        private void SetDrawMode(object sender, RoutedEventArgs e)
        {
            if (BtnDrawConnect.IsChecked==true)
                Graphbox.drawConnectionMode = true;
            else Graphbox.drawConnectionMode = false;
        }
        private void SaveImage(object sender, RoutedEventArgs e)
        {
            SaveFileDialog imgFile = new SaveFileDialog();
            imgFile.Title = "Save Image to";
            imgFile.Filter = "PNG (*.png)|*.png|JPEG (*.jpg; *jpeg)|*.jpg; *.jpeg";
            imgFile.OverwritePrompt = true;
            imgFile.DefaultExt = ".png";
            imgFile.RestoreDirectory = true;
            imgFile.FileName = fileName;
            if (imgFile.ShowDialog(this)==true && imgFile.FileName != string.Empty)
            {
                UserControl control = this.Graphbox;
                RenderTargetBitmap rtb = new RenderTargetBitmap(
                    (int)control.ActualWidth, 
                    (int)control.ActualHeight, 
                    96, 96, 
                    PixelFormats.Pbgra32);
                Rect bounds = VisualTreeHelper.GetDescendantBounds(control);
                DrawingVisual dv = new DrawingVisual();
                using (DrawingContext ctx = dv.RenderOpen())
                {
                    VisualBrush vb = new VisualBrush(control);
                    ctx.DrawRectangle(vb, null, new Rect(new Point(), bounds.Size));
                }
                rtb.Render(dv);
                BitmapEncoder encoder = new PngBitmapEncoder();
                if (Path.GetExtension(imgFile.FileName) == ".png")
                    encoder = new PngBitmapEncoder();
                else if (Path.GetExtension(imgFile.FileName) == ".jpg" || Path.GetExtension(imgFile.FileName) == ".jpeg")
                    encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(rtb));
                using (Stream fileStream = new FileStream(imgFile.FileName, FileMode.Create))
                    encoder.Save(fileStream);
            }
        }
        private void SaveMatrix(object sender, RoutedEventArgs e)
        {
            Graphbox.CreateMaxtrix();
            SaveLog(this, new RoutedEventArgs());
        }
        private void ClearLog(object sender, RoutedEventArgs e)
        {
            LogViewer.Clear();
        }
        private void SaveLog(object sender, RoutedEventArgs e)
        {
            SaveFileDialog logFile = new SaveFileDialog();
            logFile.Title = "Save Log to";
            logFile.Filter = "Text Format (*.txt)|*.txt|All Type (*.*)|*.*";
            logFile.OverwritePrompt = true;
            logFile.DefaultExt = ".txt";
            logFile.FileName = fileName+"(Log)";
            logFile.RestoreDirectory = true;
            if (logFile.ShowDialog(this)==true && logFile.FileName != string.Empty)
                File.WriteAllText(logFile.FileName, LogViewer.Text, Encoding.Unicode);
        }
        private void OpenGraph(object sender, RoutedEventArgs e)
        {
            if (Graphbox.NodesCount != 0 && Changed)
            {
                MessageBoxResult res = MessageBox.Show("Do you want to save your work?", "New Graph...", MessageBoxButton.YesNoCancel, MessageBoxImage.Exclamation);
                if (res == MessageBoxResult.Yes) SaveGraph(this, new RoutedEventArgs());
                else if (res == MessageBoxResult.Cancel) return;
            }
            FileDialog graphF = new OpenFileDialog();
            graphF.Title = "Open Graph";
            graphF.Filter = "Graph File (*.graph)|*.graph|Text File (*.txt)|*.txt|All File (*.*)|*.*";
            graphF.DefaultExt = ".graph";
            graphF.RestoreDirectory = true;
            graphF.FileName = "New Graph";
            if (graphF.ShowDialog(this) == true)
            {
                try
                {
                    Graphbox.Clear();
                    using (StreamReader sr = new StreamReader(graphF.FileName))
                    {
                        string[] number = Regex.Split(sr.ReadLine(), " ");
                        int N = int.Parse(number[0]), E = int.Parse(number[1]);
                        for (int i = 0; i < N; i++)
                        {
                            number = Regex.Split(sr.ReadLine(), " ");
                            Graphbox.AddNode(new Point(double.Parse(number[0]), double.Parse(number[1])));
                        }
                        for (int i = 0; i < E; i++)
                        {
                            number = Regex.Split(sr.ReadLine(), " ");
                            Graphbox.AddEdge(int.Parse(number[0]), int.Parse(number[1]),int.Parse(number[2]));
                        }
                        fileName = graphF.SafeFileName.Replace(graphF.SafeFileName.Substring(graphF.SafeFileName.IndexOf('.')), "");
                        filePath = graphF.FileName;
                        Graphbox.CheckConnect();
                        Graphbox.Invalidated();
                        this.BtnKruskal.IsEnabled = true;
                        Changed = false;
                    }
                }
                catch (Exception ex)
                {
                    this.NewGraph(this, new RoutedEventArgs());
                    MessageBox.Show("This file cannot be read!!!\nError Code: " + ex.Message.ToString(), "File Incorrect!", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                Graph_MouseDown(this, new MouseButtonEventArgs(Mouse.PrimaryDevice,1,MouseButton.Left));
                //graphStream.Dispose();
                //graphStream = new FileStream(graphF.FileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            }
        }
        private void SaveGraph(object sender, RoutedEventArgs e)
        {
            if (File.Exists(filePath))
            {
                string dataSave = Graphbox.CreateData();
                File.WriteAllText(filePath, dataSave);
            }
            else SaveGraphAs(this, new RoutedEventArgs());
            Changed = false;
        }
        private void SaveGraphAs(object sender, RoutedEventArgs e)
        {
            string dataSave = Graphbox.CreateData();
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Title = "Save Graph As";
            saveDialog.Filter = "Graph File (*.graph)|*.graph|Text File (*.txt)|*.txt|All File (*.*)|*.*";
            saveDialog.DefaultExt = ".graph";
            saveDialog.RestoreDirectory = true;
            saveDialog.FileName = fileName;
            if (saveDialog.ShowDialog(this) == true)
            {
                File.WriteAllText(saveDialog.FileName, dataSave);
                fileName = saveDialog.SafeFileName.Replace(saveDialog.SafeFileName.Substring(saveDialog.SafeFileName.IndexOf('.')), "");
                filePath = saveDialog.FileName;
                Changed = false;
            }
        }
        private void ToggleMode(object sender, ExecutedRoutedEventArgs e)
        {
            var rb = e.Parameter as RadioButton;
            rb.IsChecked = !rb.IsChecked;
        }
        private void CanToggle(object sender, CanExecuteRoutedEventArgs e)
        {
            var rb = e.Parameter as RadioButton;
            if (rb.IsEnabled==true)
                e.CanExecute = true;
            else e.CanExecute = false;
        }
        private void ToggleDraw(object sender, ExecutedRoutedEventArgs e)
        {
            var tb = e.Parameter as System.Windows.Controls.Primitives.ToggleButton;
            tb.IsChecked = !tb.IsChecked;
        }
        private void TextboxScroll(object sender, TextChangedEventArgs e)
        {
            LogViewer.ScrollToEnd();
        }
        private void TurnOffOtherMode(object sender, RoutedEventArgs e)
        {
            if (Graphbox.NodesCount < 1) {((RadioButton)sender).IsChecked = false;return; }
            BtnOpen.IsEnabled = BtnNew.IsEnabled = BtnSave.IsEnabled = BtnSaveAs.IsEnabled = BtnImageExport.IsEnabled = BtnMatrixExport.IsEnabled =
                BtnAddConnect.IsEnabled = BtnAddNode.IsEnabled = BtnDelConnect.IsEnabled = BtnDelNode.IsEnabled =
                BtnDrawConnect.IsEnabled = false;
                BtnAddNode.IsChecked = BtnAddConnect.IsChecked = BtnDelConnect.IsChecked = BtnDelNode.IsChecked =
                BtnDrawConnect.IsChecked = false;
            BtnppAlgo.IsEnabled = BtnStopAlgo.IsEnabled = true;
        }
        private void TurnOnOtherMode(object sender, RoutedEventArgs e)
        {
            if (Graphbox.RunningAlgorithm && ((RadioButton)sender).IsChecked == false) { ((RadioButton)sender).IsChecked = true; return; }
            BtnOpen.IsEnabled = BtnNew.IsEnabled = BtnSave.IsEnabled = BtnSaveAs.IsEnabled = BtnImageExport.IsEnabled = BtnMatrixExport.IsEnabled =
                BtnAddConnect.IsEnabled = BtnAddNode.IsEnabled = BtnDelConnect.IsEnabled = BtnDelNode.IsEnabled =
                BtnDFS.IsEnabled = BtnBFS.IsEnabled = BtnDijktra.IsEnabled = BtnDrawConnect.IsEnabled = true;
            BtnppAlgo.IsEnabled = BtnStopAlgo.IsEnabled = false;
            Graph_MouseDown(this, new MouseButtonEventArgs(Mouse.PrimaryDevice, 1, MouseButton.XButton1));
        }
        private void StopAlgoritm(object sender, RoutedEventArgs e)
        {
            if (Graphbox.RunningAlgorithm)
            {
                Graphbox.RunningAlgorithm = false;
                Graphbox.startBFS = Graphbox.startDFS = Graphbox.startDijktra = Graphbox.startKruskal = false;
                System.Threading.Thread.Sleep(1500);
                if (Graphbox.PauseAlgorithm)
                {
                    Graphbox.PauseAlgorithm = false;
                    txtPlayPauseAlgo.Text = "Pause";
                    iconPlayPauseAlgo.Source = new BitmapImage(new Uri(@"\Resource\Pause.png", UriKind.Relative)); ;
                }
                Graphbox.Invalidated();
            }
        }
        private void PPAlgoritm(object sender, RoutedEventArgs e)
        {
            if (Graphbox.RunningAlgorithm)
            {
                if (txtPlayPauseAlgo.Text == "Play")
                {
                    txtPlayPauseAlgo.Text = "Pause";
                    iconPlayPauseAlgo.Source = new BitmapImage(new Uri(@"\Resource\Pause.png",UriKind.Relative)); ;
                }
                else { txtPlayPauseAlgo.Text = "Play"; iconPlayPauseAlgo.Source = new BitmapImage(new Uri(@"\Resource\Play.png",UriKind.Relative)); ; }
                Graphbox.PauseAlgorithm = !Graphbox.PauseAlgorithm;
            }
        }

        private void Kruskal(object sender, RoutedEventArgs e)
        {
            if (Graphbox.NodesCount > 0)
            {
                Graphbox.RunKruskal();
                TurnOffOtherMode(BtnKruskal, new RoutedEventArgs());
            }
            else BtnKruskal.IsChecked = false;
        }
    }
}
