using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace GraphTheory
{
    /// <summary>
    /// Interaction logic for GraphBox.xaml
    /// </summary>
    public partial class GraphBox : UserControl
    {
        private string Reversed(string str)
        {
            return new string(new char[2] { str[1], str[0] });
        }
        public GraphBox()
        {
            InitializeComponent();
            EdgesCount = 0;
            GenerateColor();
            oldRenderSize = this.RenderSize; ;
        }
        public void Clear()
        {
            RunningAlgorithm = PauseAlgorithm = startBFS = startDFS = startDijktra = false;
            graphContent.Children.Clear();
            Nodes.Clear();
            Edges.Clear();
            EdgesTransform.Clear();
            PA = PB = NodesCount = EdgesCount = totalDegrees = 0;
            A = B = new Point(0, 0);
            Connect = drawConnectionMode = false;
            _Dragmode = null;
            log = string.Empty;
        }
        #region DependendcyPropertyDefine
        public static readonly DependencyProperty MouseProperty =
            DependencyProperty.Register("MouseCoordinate", typeof(string), typeof(GraphBox));
        public static readonly DependencyProperty NodesCountProperty =
            DependencyProperty.Register("NodesCount", typeof(int), typeof(GraphBox));
        public static readonly DependencyProperty EdgesCountProperty =
            DependencyProperty.Register("EdgesCount", typeof(int), typeof(GraphBox));
        public static readonly DependencyProperty AddNodeProperty =
            DependencyProperty.Register("addNodes", typeof(bool), typeof(GraphBox));
        public static readonly DependencyProperty DeleteNodeProperty =
            DependencyProperty.Register("delNodes", typeof(bool), typeof(GraphBox));
        public static readonly DependencyProperty AddEdgesProperty =
            DependencyProperty.Register("addEdges", typeof(bool), typeof(GraphBox));
        public static readonly DependencyProperty DeleteEdgeProperty =
            DependencyProperty.Register("delEdges", typeof(bool), typeof(GraphBox));
        public static readonly DependencyProperty ConnectionNumberProperty =
            DependencyProperty.Register("nConnection", typeof(int), typeof(GraphBox));
        public static readonly DependencyProperty DegreeProperty =
            DependencyProperty.Register("totalDegrees", typeof(int), typeof(GraphBox));
        public static readonly DependencyProperty DrawConnectionProperty =
            DependencyProperty.Register("drawConnectionMode", typeof(bool), typeof(GraphBox));
        public static readonly DependencyProperty DFSProperty =
            DependencyProperty.Register("startDFS", typeof(bool), typeof(GraphBox));
        public static readonly DependencyProperty BFSProperty =
            DependencyProperty.Register("startBFS", typeof(bool), typeof(GraphBox));
        public static readonly DependencyProperty DijktraProperty =
            DependencyProperty.Register("startDijktra", typeof(bool), typeof(GraphBox));
        public static readonly DependencyProperty KruskalProperty =
            DependencyProperty.Register("startKruskal", typeof(bool), typeof(GraphBox));
        public static readonly DependencyProperty StatusProperty =
            DependencyProperty.Register("status", typeof(string), typeof(GraphBox));
        public static readonly DependencyProperty LogProperty =
            DependencyProperty.Register("log", typeof(string), typeof(GraphBox));
        #endregion
        private List<Node> Nodes = new List<Node>();
        private SortedDictionary<string, Edge> Edges = new SortedDictionary<string, Edge>();
        private Dictionary<string, Brush> colorBox = new Dictionary<string, Brush>();
        private bool _algorithm;
        Random rnd = new Random();
        private Size oldRenderSize;
        public bool RunningAlgorithm
        {
            get => _algorithm;
            set => _algorithm = value;
        }

        public bool addNodes
        {
            get { return (bool)GetValue(AddNodeProperty); }
            set { SetValue(AddNodeProperty, value); }
        }
        public bool addEdges
        {
            get { return (bool)GetValue(AddEdgesProperty); }
            set { SetValue(AddEdgesProperty, value); }
        }
        public bool delNodes
        {
            get { return (bool)GetValue(DeleteNodeProperty); }
            set { SetValue(DeleteNodeProperty, value); }
        }
        public bool delEdges
        {
            get { return (bool)GetValue(DeleteEdgeProperty); }
            set { SetValue(DeleteEdgeProperty, value); }
        }
        public bool drawConnectionMode
        {
            get { return (bool)GetValue(DrawConnectionProperty); }
            set
            {
                SetValue(DrawConnectionProperty, value);
                DrawConnection();
            }
        }
        public bool startDFS
        {
            get { return (bool)GetValue(DFSProperty); }
            set { SetValue(DFSProperty, value); }
        }
        public bool startBFS
        {
            get { return (bool)GetValue(BFSProperty); }
            set { SetValue(BFSProperty, value); }
        }
        public bool startDijktra
        {
            get { return (bool)GetValue(DijktraProperty); }
            set { SetValue(DijktraProperty, value); }
        }
        public bool startKruskal
        {
            get { return (bool)GetValue(KruskalProperty); }
            set { SetValue(KruskalProperty, value); }
        }
        public bool PauseAlgorithm = false;
        public int NodesCount
        {
            get { return (int)GetValue(NodesCountProperty); }
            set
            {
                SetValue(NodesCountProperty, (int)value);
                CheckConnect();
            }
        }
        public int EdgesCount
        {
            get { return (int)GetValue(EdgesCountProperty); }
            set
            {
                SetValue(EdgesCountProperty, value);
            }
        }
        public int nConnection
        {
            get { return (int)GetValue(ConnectionNumberProperty); }
            set { SetValue(ConnectionNumberProperty, value); }
        }
        public int totalDegrees
        {
            get { return (int)GetValue(DegreeProperty); }
            set { SetValue(DegreeProperty, value); }
        }
        public string MouseCoordinate
        {
            get { return GetValue(MouseProperty).ToString(); }
            set { SetValue(MouseProperty, value.ToString()); }
        }
        public string status
        {
            get { return GetValue(StatusProperty).ToString(); }
            set { SetValue(StatusProperty, value); }
        }
        public string log
        {
            get { return GetValue(LogProperty).ToString(); }
            set { SetValue(LogProperty, value); }
        }
        public void AddNode()
        {
            Point pos = Mouse.GetPosition(this);
            if (pos.X < 15) pos.X = 15;
            else if (pos.X > this.RenderSize.Width - 15) pos.X = this.RenderSize.Width - 15;
            if (pos.Y < 15) pos.Y = 15;
            else if (pos.Y > this.RenderSize.Height - 15) pos.Y = this.RenderSize.Height - 15;
            Node node = new Node();
            node.nodeName = (Char)(Nodes.Count + 65);
            node.VerticalAlignment = VerticalAlignment.Top;
            node.HorizontalAlignment = HorizontalAlignment.Left;
            node.Margin = new Thickness(pos.X - 15, pos.Y - 15, 0, 0);
            node.MouseDown += new MouseButtonEventHandler(DeleteNode);
            node.MouseDown += new MouseButtonEventHandler(SetEdge);
            node.MouseDown += new MouseButtonEventHandler(DragModeEnter);
            node.MouseUp += new MouseButtonEventHandler(DragModeLeave);
            node.MouseEnter += new MouseEventHandler(NodeHover);
            node.GotFocus += new RoutedEventHandler(FocusColorChange);
            node.LostFocus += new RoutedEventHandler(FocusColorChange);
            Nodes.Add(node);
            //graphContent.Children.Add(node);
            NodesCount = Nodes.Count;
        }
        public void AddNode(Point pos)
        {
            //Point pos = Mouse.GetPosition(this);
            pos.X *= oldRenderSize.Width / 100;
            pos.Y *= oldRenderSize.Height / 100;
            if (pos.X > this.RenderSize.Width - 30) pos.X = this.RenderSize.Width - 60;
            if (pos.Y > this.RenderSize.Height - 30) pos.Y = this.RenderSize.Height - 60;
            Node node = new Node();
            node.nodeName = (Char)(Nodes.Count + 65);
            node.VerticalAlignment = VerticalAlignment.Top;
            node.HorizontalAlignment = HorizontalAlignment.Left;
            node.Margin = new Thickness(pos.X, pos.Y, 0, 0);
            node.MouseDown += new MouseButtonEventHandler(DeleteNode);
            node.MouseDown += new MouseButtonEventHandler(SetEdge);
            node.MouseDown += new MouseButtonEventHandler(DragModeEnter);
            node.MouseUp += new MouseButtonEventHandler(DragModeLeave);
            node.MouseEnter += new MouseEventHandler(NodeHover);
            node.GotFocus += new RoutedEventHandler(FocusColorChange);
            node.LostFocus += new RoutedEventHandler(FocusColorChange);
            Nodes.Add(node);
            //graphContent.Children.Add(node);
            NodesCount = Nodes.Count;
        }
        private int PA = 0, PB = 0;
        private Point A, B;
        Brush currNodeColor;
        private bool Connect = false;
        private Dictionary<Edge, RotateTransform> EdgesTransform = new Dictionary<Edge, RotateTransform>();
        private List<List<int>> Connection = new List<List<int>>();
        private void AddEdge()
        {
            if (Connect)
            {
                Edge edge = new Edge(A, B);
                if (!Edges.ContainsKey(Nodes[PA].nodeName.ToString() + Nodes[PB].nodeName.ToString()) && !Edges.ContainsKey(Nodes[PA].nodeName.ToString() + Nodes[PB].nodeName.ToString()))
                {
                    Edges.Add(Nodes[PA].nodeName.ToString() + Nodes[PB].nodeName.ToString(), edge);
                    Edges.Add(Nodes[PB].nodeName.ToString() + Nodes[PA].nodeName.ToString(), edge);
                    EdgesTransform.Add(edge, new RotateTransform(edge.RotateAngle));
                    edge.RenderTransform = EdgesTransform[edge];
                    edge.MouseDown += new MouseButtonEventHandler(DeleteEdge);
                    EdgesCount++;
                    Nodes[PA].degree++; Nodes[PB].degree++;
                    totalDegrees += 2;
                }
                else
                {
                    MessageBoxResult QA = MessageBox.Show("Edge " + Nodes[PA].nodeName + Nodes[PB].nodeName + " already exist!\n" +
                        "Do you want to Modify it?",
                        "Existed Edge", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                    if (QA == MessageBoxResult.Yes) Edges[Nodes[PA].nodeName.ToString() + Nodes[PB].nodeName.ToString()].ActiveTextbox();
                }
            }
            CheckConnect();
        }
        public void AddEdge(int A, int B, int W)
        {
            Edge edge = new Edge(new Point(Nodes[A].Margin.Left, Nodes[A].Margin.Top),
                new Point(Nodes[B].Margin.Left, Nodes[B].Margin.Top));
            if (!Edges.ContainsKey(Nodes[A].nodeName.ToString() + Nodes[B].nodeName.ToString()))
            {
                Edges.Add(Nodes[A].nodeName.ToString() + Nodes[B].nodeName.ToString(), edge);
                Edges.Add(Nodes[B].nodeName.ToString() + Nodes[A].nodeName.ToString(), edge);
                EdgesTransform.Add(edge, new RotateTransform(edge.RotateAngle));
                edge.RenderTransform = EdgesTransform[edge];
                edge.MouseDown += new MouseButtonEventHandler(DeleteEdge);
                EdgesCount++;
                Nodes[A].degree++; Nodes[B].degree++;
                totalDegrees += 2;
                Edges[Nodes[A].nodeName.ToString() + Nodes[B].nodeName.ToString()].Weight = W;
            }
            else
            {
                Edges[Nodes[A].nodeName.ToString() + Nodes[B].nodeName.ToString()].Weight = W; ;
            }
        }
        private void NodeHover(object sender, MouseEventArgs e)
        {
            Node obj = sender as Node;
            if (addNodes || addEdges || delNodes || delEdges) obj.Cursor = Cursors.Hand;
            else obj.Cursor = Cursors.ScrollAll;
        }
        private void DeleteNode(object sender, MouseButtonEventArgs e)
        {
            if (delNodes)
            {
                Node node = sender as Node;
                int pos = Nodes.IndexOf(node);
                Nodes.Remove(node);
                graphContent.Children.Remove(node);
                for (int i = pos; i < Nodes.Count; i++)
                    Nodes[i].nodeName = (Char)(i + 65);
                //DeleteEdge Connected with Node
                string oldname, newname;
                for (int i = pos; i <= NodesCount; i++)
                {
                    for (int j = 0; j <= NodesCount; j++)
                    {
                        if (j == i) j++;
                        oldname = ((char)(i + 65)).ToString() + ((char)(j + (j > pos ? 64 : 65))).ToString();
                        if (Edges.ContainsKey(oldname))
                        {
                            if (i == pos)
                            {
                                Nodes[(j > pos ? j - 2 : j)].degree--;
                                graphContent.Children.Remove(Edges[oldname]);
                                Edges.Remove(oldname);
                                Edges.Remove(Reversed(oldname));
                                EdgesCount--;
                                totalDegrees -= 2;
                            }
                            else
                            {
                                newname = Nodes[i - 1].nodeName.ToString() + ((char)(j + (j > pos ? 64 : 65))).ToString();
                                Edges.Add(newname, Edges[oldname]);
                                Edges.Add(Reversed(newname), Edges[oldname]);
                                Edges.Remove(oldname);
                                Edges.Remove(Reversed(oldname));
                            }
                        }
                    }
                }
                NodesCount--;
            }
        }
        private void DeleteEdge(object sender, MouseButtonEventArgs e)
        {
            if (delEdges)
            {
                Edge obj = sender as Edge;
                graphContent.Children.Remove(obj);
                foreach (string K in Edges.Keys)
                    if (Edges[K] == obj)
                    {
                        Nodes[(int)K.First() - 65].degree--;
                        Nodes[(int)K.Last() - 65].degree--;
                        Edges.Remove(K);
                        Edges.Remove(Reversed(K));
                        EdgesCount--;
                        totalDegrees -= 2;
                        break;
                    }
                CheckConnect();
            }
        }
        private void SetEdge(object sender, MouseButtonEventArgs e)
        {
            if (addEdges)
            {
                Node node = sender as Node;
                int pos = Nodes.IndexOf(node);
                if (!Connect)
                {
                    PA = pos;
                    currNodeColor = Nodes[PA].nodeStroke.Stroke;
                    A = new Point(node.Margin.Left, node.Margin.Top);
                    Nodes[PA].nodeStroke.Stroke = colorBox["setConnect"];
                    Connect = true;
                }
                else
                {
                    PB = pos;
                    B = new Point(node.Margin.Left, node.Margin.Top);
                    Nodes[PA].nodeStroke.Stroke = currNodeColor;
                    if (PA != PB)
                    {
                        if (PA > PB) { int tmp = PA; PA = PB; PB = tmp; }
                        if (A.X > B.X) { Point tmp = A; A = B; B = tmp; }
                        AddEdge();
                    }
                    PA = PB = 0; Connect = false;
                }
            }
        }
        public void Invalidated()
        {
            DrawConnection();
            SortedSet<string> Added = new SortedSet<string>();
            graphContent.Children.Clear();
            for (int i = 0; i < Nodes.Count; i++)
            {
                for (int j = 0; j < Nodes.Count; j++)
                {
                    string name = Nodes[i].nodeName.ToString() + Nodes[j].nodeName.ToString();
                    if (Edges.ContainsKey(name) && !Added.Contains(Reversed(name)))
                    {
                        Edges[name].Visibility = Visibility.Visible;
                        graphContent.Children.Add(Edges[name]);
                        Added.Add(name); Added.Add(Reversed(name));
                    }
                }
                Nodes[i].Visibility = Visibility.Visible;
                graphContent.Children.Add(Nodes[i]);
            }
        }
        private void GenerateColor()
        {
            colorBox["Default"] = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            colorBox["GotFocus"] = new SolidColorBrush(Color.FromRgb(0, 0, 255));
            colorBox["setConnect"] = new SolidColorBrush(Color.FromRgb(0, 140, 240));
            //Connection Color
            /*
            colorBox["Connection1"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#90f85a"));
            colorBox["Connection2"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#b2d838"));
            colorBox["Connection3"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3d42fc"));
            colorBox["Connection4"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4c294b"));
            colorBox["Connection5"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#e48f26"));
            colorBox["Connection6"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#f33b77"));
            colorBox["Connection7"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#fce4c3"));
            colorBox["Connection8"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#513d7e"));
            colorBox["Connection9"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#9b6d8b"));
            colorBox["Connection10"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#c63a4a"));
            colorBox["Connection11"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#95bac4"));
            colorBox["Connection12"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#cf1e8b"));
            colorBox["Connection13"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#c479f6"));
            colorBox["Connection14"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3e564a"));
            colorBox["Connection15"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#5b8a6e"));
            colorBox["Connection16"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2a0236"));
            colorBox["Connection17"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4f75db"));
            colorBox["Connection18"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ca06b6"));
            colorBox["Connection19"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#9cdadf"));
            colorBox["Connection20"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#b62060"));
            colorBox["Connection21"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#19b28e"));
            colorBox["Connection22"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#a8773d"));
            colorBox["Connection23"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#c1e3d0"));
            colorBox["Connection24"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#738925"));
            colorBox["Connection25"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#5a5ef9"));
            colorBox["Connection26"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#e78976"));
            */
            byte R = 2, B = 0, G = 0;
            byte[] color = new byte[3] { 21, 125, 201 };
            for (int i = 1; i <= 26; i++)
            {
                colorBox["Connection" + i.ToString()] = new SolidColorBrush(Color.FromRgb(color[R], color[G], color[B]));
                colorBox["EConnection" + i.ToString()] = new SolidColorBrush(Color.FromRgb((byte)(color[R] - 15), (byte)(color[G] - 15), (byte)(color[B] - 15)));
                B++;
                if (B > 2)
                {
                    B = 0; G++;
                    if (G > 2) { G = 0; R++;
                        if (R > 2) R = 0;
                    }
                }
            }
        }
        private void FocusColorChange(object sender, RoutedEventArgs e)
        {
            Node node = sender as Node;
            if (node.IsFocused == true)
            {
                currNodeColor = node.nodeStroke.Stroke;
                node.nodeStroke.Stroke = colorBox["GotFocus"];
            }
            else node.nodeStroke.Stroke = (drawConnectionMode ? currNodeColor : colorBox["Default"]);
        }
        public void CheckConnect()
        {
            status = "";
            Connection.Clear(); nConnection = 0;
            bool[] Visited = new bool[Nodes.Count], hasConnect = new bool[Nodes.Count];
            int Last = 0;
            List<int> Pos = new List<int>();
            for (int i = 0; i < Nodes.Count; i++)
                if (!Visited[i])
                {
                    Connection.Add(new List<int>());
                    Pos.Add(i);
                    Connection[nConnection].Add(i);
                    while (Pos.Count != 0)
                    {
                        Last = Pos.Last();
                        if (!Visited[Last])
                        {
                            Connection[nConnection].Add(Last);
                            status += Last.ToString() + " ";
                            Visited[Last] = true;
                            for (int j = 0; j < NodesCount; j++)
                                if (!Visited[j] && Edges.ContainsKey(Nodes[Last].nodeName.ToString() + Nodes[j].nodeName.ToString()))
                                    Pos.Add(j);
                        }
                        else
                            Pos.RemoveAt(Pos.Count - 1);
                    }
                    nConnection++;
                    status += ";";
                }
            Invalidated();
        }
        public void DrawConnection()
        {
            if (_Dragmode == null)
            {
                SortedSet<string> drawed = new SortedSet<string>();
                for (int i = 0; i < nConnection; i++)
                {
                    for (int j = 0; j < Connection[i].Count; j++)
                    {
                        if (drawConnectionMode) Nodes[Connection[i][j]].nodeStroke.Stroke = colorBox["Connection" + (i + 1).ToString()];
                        else Nodes[Connection[i][j]].nodeStroke.Stroke = colorBox["Default"];
                        for (int x = 0; x < NodesCount; x++)
                            if (Edges.ContainsKey(Nodes[x].nodeName.ToString() + Nodes[Connection[i][j]].nodeName.ToString()))
                                if (drawConnectionMode)
                                {
                                    if (!drawed.Contains(Nodes[x].nodeName.ToString() + Nodes[Connection[i][j]].nodeName.ToString()))
                                    {
                                        Edges[Nodes[x].nodeName.ToString() + Nodes[Connection[i][j]].nodeName.ToString()].Line.Fill = colorBox["EConnection" + (i + 1).ToString()];
                                        Edges[Nodes[x].nodeName.ToString() + Nodes[Connection[i][j]].nodeName.ToString()].BeginStoryboard(Edges[Nodes[x].nodeName.ToString() + Nodes[Connection[i][j]].nodeName.ToString()].ResizeLine);
                                        drawed.Add(Nodes[x].nodeName.ToString() + Nodes[Connection[i][j]].nodeName.ToString());
                                        drawed.Add(Nodes[Connection[i][j]].nodeName.ToString() + Nodes[x].nodeName.ToString());
                                    }
                                }
                                else Edges[Nodes[x].nodeName.ToString() + Nodes[Connection[i][j]].nodeName.ToString()].Line.Fill = colorBox["Default"];
                    }
                }
            }
            //Invalidated();
        }
        private async void RunDFS(int StartNode)
        {
            prepareAlgorithm(); //Tắt hết các chế độ khác để chạy Algorithm
            log = "Running Depth First Search Algoritm.";//Khởi tạo log(Khung log được bind thẳng vào thằng này nên thằng này thay đổi, khung log cũng thay đổi, ko cần phải cập nhật bằng code-behind)
            log += Environment.NewLine + " Start with " + Nodes[StartNode].nodeName;
            Stack<int> stack = new Stack<int>(); //Stack bla bla bla
            Brush currEdgeColor; //tạo biến lưu màu hiện tại của cạnh đang xét, đỉnh thì có biến global currNodeColor r`
            bool[] visited = new bool[NodesCount]; //Mảng Visited bla bla bla 
            int[] pos = new int[NodesCount]; //Lưu vị trí quét hiện tại của mỗi đỉnh, bỏ qua các đỉnh đã xét, xét tiếp những đỉnh kế tiếp
            List<string> SearchOrder = new List<string>();
            stack.Push(StartNode); //Thêm điểm bắt đầu vào stack (t xử lý phần chọn đỉnh rồi, lúc chạy chỉ cần chọn Algorithm rồi chọn đỉnh bắt đầu thôi)
            //Nodes[StartNode].Visibility = Visibility.Visible; //Hiện đỉnh đầu tiên
            while (stack.Count > 0) //While
            {
                if (!RunningAlgorithm) return;
                while (PauseAlgorithm) { await Task.Run(() => { System.Threading.Thread.Sleep(200); }); }
                if (!visited[stack.Peek()]) //Check Visit
                {
                    Nodes[stack.Peek()].nodeStroke.Stroke = Brushes.Red; //Tô màu đỉnh trong stack (nhánh đang duyệt)
                    log += Environment.NewLine + "Explore " + Nodes[stack.Peek()].nodeName + ":"; // Thêm thông tin vào Log
                    visited[stack.Peek()] = true;  //stack.Peek=stack.Top() bên C++ (hàm Pop cũng trả về phần tử đầu, nhưng đồng thơi xóa luôn đỉnh đó như C++)
                    if (!SearchOrder.Contains(Nodes[stack.Peek()].nodeName.ToString())) SearchOrder.Add(Nodes[stack.Peek()].nodeName.ToString());
                    for (int i = pos[stack.Peek()]; i < NodesCount; i++, pos[stack.Peek()]++)    //Tìm đỉnh kề
                    {
                        string name = Nodes[stack.Peek()].nodeName.ToString() + Nodes[i].nodeName.ToString(); //Gán name bằng tên cạnh, để gọi cạnh trong Edges gọn hơn
                        if (Edges.ContainsKey(name)) //Kiểm tra coi đỉnh stack.Peek() có nối với đỉnh i ko
                        {
                            currNodeColor = Nodes[i].nodeStroke.Stroke; //Lưu màu của đỉnh đang xét
                            currEdgeColor = Edges[name].Line.Fill; //Lưu màu của cạnh đang xét
                            Nodes[i].nodeStroke.Stroke = Brushes.Yellow; //Tô màu đỉnh đang xét
                            Edges[name].Line.Fill = Brushes.Yellow; //Tô màu cạnh đang xét
                            await Task.Run(() => { System.Threading.Thread.Sleep(1000); }); //Hiệu ứng dừng trong 1sec
                            if (!visited[i] && !stack.Contains(i)) //Kiểm tra đỉnh kề
                            //điều kiện: chưa visit, đỉnh đó không tồn tại trong stack, kiểm tra danh sách Edges có chứa cạnh nối từ đỉnh đang xét tới đỉnh i không
                            //stack.Peek() là đỉnh đang xét, Nodes[stack.Peek()].nodeName.ToString*() là lấy tên đỉnh chuyển từ char sang string + với tên đỉnh của i (cách lấy tương tự)
                            //VD: stack.Peek() = 0 Nodes[0].nodeName = (string)A + i = 1 Nodes[i].nodeName = (string)B => cạnh AB
                            //hoặc có thể lấy tên bằng cách khác gọi ((char)(i+65)).ToString()
                            {
                                log += Environment.NewLine + "    Check " + Nodes[i].nodeName + ": not Visited,"; //Update Log
                                pos[stack.Peek()]++; //Tăng pos của đỉnh hiện tại lên, tránh trường hợp xét lại (thử cho dòng này thành comment rồi chạy thử sẽ rõ)
                                Edges[name].Line.Fill = Brushes.DarkBlue; //Tô màu cạnh đã duyệt
                                visited[stack.Peek()] = false; //Nếu đỉnh stack.Peek() có liên thông với đỉnh khác thì xem như chưa visit để lát quay lại
                                await Task.Run(() => //"""START""" animation bằng 1 thread khác
                                {
                                    Dispatcher.Invoke(new Action(() => //Do nó tác động đến 1 đối tượng khác nên phải gọi hàm trong nó (dùng Lambda Syntax để tạo 1 hàm mới thực hiện ngay trong object này)
                                           {
                                               Edges[name].BeginStoryboard(Edges[name].ResizeLine);  //Chạy Animation (mấy thuật toán sau cứ ghi lại từ START -> END không cần thay đổi, gán name cho đúng cạnh thôi :v
                                           }));
                                    System.Threading.Thread.Sleep(1000); //Tắt Thread hiện tại đi 1,1s để đợi Animation chạy xong
                                }); //End Animation
                                log += " add to Stack."; //Update Log
                                stack.Push(i); //Đẩy đỉnh i vào stack
                                break; //Khi biết có đỉnh kề thì break, tránh tình trạng dồn thêm đỉnh kề vào stack thành duyệt BFS
                            }
                            else //Nếu đỉnh i có nối với stack.Peek() nhưng đã visit
                            {
                                log += Environment.NewLine + "    Check " + Nodes[i].nodeName + ": Visited, Continue."; //Update Log
                                Edges[name].Line.Fill = currEdgeColor; //Trả lại màu cho cạnh
                                Nodes[i].nodeStroke.Stroke = currNodeColor; //Trả lại màu cho đỉnh
                            }
                            if (stack.Contains(i)) Nodes[i].nodeStroke.Stroke = Brushes.Red; //Nếu i có trong stack(tức đang duyệt nhánh có chứa i) thì tô màu Đỏ
                            //else Nodes[i].nodeStroke.Stroke = currNodeColor;
                        }
                    }
                }
                else
                {
                    Nodes[stack.Peek()].nodeStroke.Stroke = Brushes.Blue;
                    log += Environment.NewLine + "Finished explore " + Nodes[stack.Pop()].nodeName + ". Remove from Stack.";
                    await Task.Run(() => { System.Threading.Thread.Sleep(500); });
                    //stack.Pop();//Nếu cạnh đầu tiên trong stack đã visit thì đẩy khỏi stack
                    if (stack.Count > 0)
                        log += Environment.NewLine + "Go back to " + Nodes[stack.Peek()].nodeName + ".";
                    else
                    {
                        log += Environment.NewLine + "All Nodes Visited.";
                        log += Environment.NewLine + "Depth First Search Finished!";
                        log += Environment.NewLine + "Search Order:" + Environment.NewLine;
                        foreach (string N in SearchOrder)
                        {
                            log += N;
                            if (N != SearchOrder.Last()) log += " -> ";
                        }
                        startDFS = RunningAlgorithm = false; //Tắt DFS (BFS với Dijktra cũng phải tắt, tên tương tự : startBFS, startDijktra
                    }
                    //await Task.Run(() => { System.Threading.Thread.Sleep(1000); }) ;
                }
            }
        }
        private async void RunBFS(int StartNode)
        {
            prepareAlgorithm(); // Hàm xóa các đỉnh và các cạnh
            //Hàm BFS ở đây
            Queue<int> bfs = new Queue<int>();
            List<string> SearchOrder = new List<string>();
            bool[] visited = new bool[NodesCount];
            Brush currEdgeColor;
            bfs.Enqueue(StartNode);
            visited[StartNode] = true;
            Nodes[StartNode].nodeStroke.Stroke = Brushes.DarkRed;
            log = "Running Breadth First Search Algorithm.";
            log += Environment.NewLine + "Start with " + Nodes[StartNode].nodeName + ". Add to Queue.";
            while (bfs.Count > 0)
            {
                if (!RunningAlgorithm) return;
                while (PauseAlgorithm) { await Task.Run(() => { System.Threading.Thread.Sleep(200); }); }
                string name; int Last = bfs.Dequeue();
                SearchOrder.Add(Nodes[Last].nodeName.ToString());
                Nodes[Last].nodeStroke.Stroke = Brushes.DarkGreen;
                log += Environment.NewLine + "Get " + Nodes[Last].nodeName + " from Queue, Explore " + Nodes[Last].nodeName + ":";
                await Task.Run(() => { System.Threading.Thread.Sleep(750); });
                for (int i = 0; i < NodesCount; i++)
                {
                    name = Nodes[Last].nodeName.ToString() + Nodes[i].nodeName.ToString();
                    if (Edges.ContainsKey(name))
                    {
                        currNodeColor = Nodes[i].nodeStroke.Stroke;
                        Nodes[i].nodeStroke.Stroke = Brushes.Yellow;
                        currEdgeColor = Edges[name].Line.Fill;
                        Edges[name].Line.Fill = Brushes.Yellow;
                        log += Environment.NewLine + "    Check " + Nodes[i].nodeName + ": ";
                        await Task.Run(() => { System.Threading.Thread.Sleep(750); });
                        if (!visited[i])
                        {
                            bfs.Enqueue(i);
                            visited[i] = true;
                            Nodes[i].nodeStroke.Stroke = Brushes.Red;
                            Edges[name].Line.Fill = Brushes.DarkBlue;
                            log += " not Visited, Add to Queue.";
                            await Task.Run(() => //"""START""" animation bằng 1 thread khác
                            {
                                Dispatcher.Invoke(new Action(() => //Do nó tác động đến 1 đối tượng khác nên phải gọi hàm trong nó (dùng Lambda Syntax để tạo 1 hàm mới thực hiện ngay trong object này)
                                {
                                    Edges[name].BeginStoryboard(Edges[name].ResizeLine);  //Chạy Animation (mấy thuật toán sau cứ ghi lại từ START -> END không cần thay đổi, gán name cho đúng cạnh thôi :v
                                }));
                                System.Threading.Thread.Sleep(1000); //Tắt Thread hiện tại đi 1,1s để đợi Animation chạy xong
                            }); //End Animation
                        }
                        else
                        {
                            Nodes[i].nodeStroke.Stroke = currNodeColor;
                            //await Task.Run(() => { System.Threading.Thread.Sleep(500); });
                            Edges[name].Line.Fill = currEdgeColor;
                            log += " Visited, Continue.";
                        }
                    }
                }
                Nodes[Last].nodeStroke.Stroke = Brushes.Blue;
                log += Environment.NewLine + "Finished Explore " + Nodes[Last].nodeName + ".";
            }
            log += Environment.NewLine + "All Nodes Visited." + Environment.NewLine + "Breadth First Search Complete.";
            log += Environment.NewLine + "Search Order:" + Environment.NewLine;
            foreach (string N in SearchOrder)
            {
                log += N;
                if (N != SearchOrder.Last()) log += " -> ";
            }
            RunningAlgorithm = false;
            startBFS = false;
        }
        private static int CompareWeight (KeyValuePair<int, string> a, KeyValuePair<int, string> b) {
            return a.Key.CompareTo(b.Key);
        }
        private async void RunDijktra(int StartNode)
        {
            prepareAlgorithm();
            log = "Dijktra Algorithm" + Environment.NewLine + "Shortest Path from " + Nodes[StartNode].nodeName + " to other";
            int[] dis = new int[NodesCount];
            int[] before = new int[NodesCount];
            bool[] visited = new bool[NodesCount];
            for (int i = 0; i < NodesCount; i++) { dis[i] = int.MaxValue; before[i] = -1; }
            dis[StartNode] = 0;
            int u;
            int du;
            for (int s = 0; s < NodesCount; s++)
            {
                if (!RunningAlgorithm) return;
                u = du = int.MaxValue;
                for (int j = 0; j < NodesCount; j++) if (dis[j] < du && !visited[j]) { u = j; du = dis[j]; }
                if (u < NodesCount)
                {
                    visited[u] = true;
                    for (int j = 0; j < NodesCount; j++)
                    {

                        while (PauseAlgorithm) { await Task.Run(() => { System.Threading.Thread.Sleep(200); }); }
                        string name = Nodes[u].nodeName.ToString() + Nodes[j].nodeName.ToString();
                        if (j != u && Edges.ContainsKey(name) && (dis[j] > (du + Edges[name].Weight)))
                        {
                            dis[j] = du + Edges[name].Weight;
                            log += Environment.NewLine;
                            if (before[j] != -1)
                            {
                                log += "Update ";
                                Edges[Nodes[j].nodeName.ToString() + Nodes[before[j]].nodeName.ToString()].Line.Fill = Brushes.Black;
                            }
                            before[j] = u;
                            log += "Shortest path from " + Nodes[StartNode].nodeName.ToString() + " to " + Nodes[j].nodeName + " is " + dis[j].ToString();
                            await Task.Run(() => //"""START""" animation bằng 1 thread khác
                            {
                                Dispatcher.Invoke(new Action(() => //Do nó tác động đến 1 đối tượng khác nên phải gọi hàm trong nó (dùng Lambda Syntax để tạo 1 hàm mới thực hiện ngay trong object này)
                                {
                                    Edges[name].Line.Fill = Brushes.Blue;
                                    Nodes[u].nodeStroke.Stroke = Nodes[j].nodeStroke.Stroke = Brushes.Blue;
                                    Edges[name].BeginStoryboard(Edges[name].ResizeLine);  //Chạy Animation (mấy thuật toán sau cứ ghi lại từ START -> END không cần thay đổi, gán name cho đúng cạnh thôi :v
                                }));
                                System.Threading.Thread.Sleep(1000); //Tắt Thread hiện tại đi 1,1s để đợi Animation chạy xong
                            }); //End Animation
                        }
                    }
                }
            }
            for (int i = 0;i<NodesCount;i++)
            {
                if (!visited[i]) log += Environment.NewLine + "No path to " + Nodes[i].nodeName + ".";
            }
            RunningAlgorithm = false;
            startDijktra = false;
            log += Environment.NewLine + "Finished Dijktra.";
        }
        public async void RunKruskal()
        {
            RunningAlgorithm = true;
            List<KeyValuePair<int, string>> SortedEdgesList = new List<KeyValuePair<int, string>>();
            int MinSpanningTreeValue = 0;
            int[] Parent = new int[NodesCount];
            log = "Running Kruskal Algoritm.";
            log += Environment.NewLine + "Push all Edges to List:"+Environment.NewLine+"[";
            for (int i = 0; i < NodesCount; i++) Parent[i] = i;
            foreach (string K in Edges.Keys)
                if (!SortedEdgesList.Contains(new KeyValuePair<int, string>(Edges[K].Weight, K)) && !SortedEdgesList.Contains(new KeyValuePair<int, string>(Edges[K].Weight, Reversed(K)))) {
                    SortedEdgesList.Add(new KeyValuePair<int, string>(Edges[K].Weight, K));
                    log += "(" + Edges[K].Weight.ToString() + "," + K + "),";
                }
            log.Remove(log.Length - 1);
            log += "]";
            await Task.Run(() => { System.Threading.Thread.Sleep(500); });
            log += Environment.NewLine + "Sort Edges List by edge's Weight:" + Environment.NewLine + "[";
            SortedEdgesList.Sort(Comparer<KeyValuePair<int,string>>.Create(CompareWeight));
            foreach(var edge in SortedEdgesList) log += "(" + edge.Key + "," + edge.Value + "),";
            log.Remove(log.Length - 1);
            log += "]" + Environment.NewLine + "Foreach Edges in Sorted Edges List:";
            foreach (KeyValuePair<int,string> edge in SortedEdgesList)
            {
                if (!RunningAlgorithm) return;
                while (PauseAlgorithm)
                {
                    await Task.Run(() => { System.Threading.Thread.Sleep(200); });
                }
                string name = edge.Value;
                int A = (int)name[0] - 65, B = (int)name[1] - 65;
                Edges[name].Line.Fill = Brushes.LightBlue;
                Nodes[A].nodeStroke.Stroke = Nodes[B].nodeStroke.Stroke = Brushes.DarkBlue;
                log += Environment.NewLine + "Check " + name +":";
                int sourceParent = A, destParent = B;
                while (Parent[sourceParent]!=sourceParent)
                {
                    sourceParent = Parent[sourceParent];
                }
                while (Parent[destParent] != destParent)
                {
                    destParent = Parent[destParent];
                }
                await Task.Run(() => { System.Threading.Thread.Sleep(750); });
                if (sourceParent != destParent)
                {
                    Parent[sourceParent] = destParent;
                    MinSpanningTreeValue += Edges[name].Weight;
                    Edges[name].Line.Fill = Brushes.Red;
                    log += " Accepted. Add to Minimum Spanning Tree.";
                    await Task.Run(() => //"""START""" animation bằng 1 thread khác
                    {
                        Dispatcher.Invoke(new Action(() => //Do nó tác động đến 1 đối tượng khác nên phải gọi hàm trong nó (dùng Lambda Syntax để tạo 1 hàm mới thực hiện ngay trong object này)
                        {
                            Edges[name].BeginStoryboard(Edges[name].ResizeLine);  //Chạy Animation (mấy thuật toán sau cứ ghi lại từ START -> END không cần thay đổi, gán name cho đúng cạnh thôi :v
                        }));
                        System.Threading.Thread.Sleep(1000); //Tắt Thread hiện tại đi 1,1s để đợi Animation chạy xong
                    }); //End Animation
                }
                else
                {
                    log += " Created a Cycle. Delete.";
                    await Task.Run(() => { System.Threading.Thread.Sleep(500); });
                    graphContent.Children.Remove(Edges[name]);
                    Edges[name].Line.Fill = Brushes.LightGray;
                }
                Nodes[A].nodeStroke.Stroke = Nodes[B].nodeStroke.Stroke = Brushes.DarkRed;
            }
            log += Environment.NewLine + "Kruskal Algorithm Finished!";
            log += Environment.NewLine + "Total Minimum Spanning Tree Weight: " + MinSpanningTreeValue.ToString(); 
            RunningAlgorithm = false;
            startKruskal = false;
        }
        private void prepareAlgorithm()
        {
            //foreach (string K in Edges.Keys) Edges[K].Visibility = Visibility.Hidden;
            //foreach (Node node in Nodes) node.Visibility = Visibility.Hidden;
            drawConnectionMode = addEdges = addNodes = delEdges = delNodes = false;
            RunningAlgorithm = true;
        }
        public void CreateMaxtrix()
        {
            log = " ======= Matrix Exporter ======= "+Environment.NewLine;
            log += "Created on " + DateTime.Now.ToString() + Environment.NewLine;
            string name;
            for (int i = 0; i < Nodes.Count; i++)
            {
                for (int j = 0; j < Nodes.Count; j++)
                {
                    name = Nodes[i].nodeName.ToString() + Nodes[j].nodeName.ToString();
                    if (j > 0) log += "\t";
                    if (Edges.ContainsKey(name))
                        log += Edges[name].Weight.ToString();
                    else log += "0";
                }
                log += Environment.NewLine;
            }
            log += "Total: " + NodesCount + " Nodes & " + EdgesCount + " Edges.";
        }
        public string CreateData()
        {
            SortedSet<string> Added = new SortedSet<string>();
            string res,name;
            Point p;
            res = NodesCount.ToString() + " " + EdgesCount.ToString();
            for (int i = 0; i < Nodes.Count; i++)
            {
                p=getPercent(Nodes[i].Margin);
                res += Environment.NewLine + p.X.ToString() + " " + p.Y.ToString();
            }
            for (int i = 0; i < Nodes.Count; i++)
            {
                for (int j = 0; j < Nodes.Count; j++)
                {
                    name = Nodes[i].nodeName.ToString() + Nodes[j].nodeName.ToString();
                    if (Edges.ContainsKey(name) && !Added.Contains(name))
                    {
                        res += Environment.NewLine + i.ToString() + ' ' + j.ToString() + ' ' + Edges[name].Weight.ToString();
                        Added.Add(name); Added.Add(Reversed(name));
                    }
                }
            }
            return res;
        }
        /*
        public Point MousePosition()
        {
            System.Drawing.Point point = System.Windows.Forms.Control.MousePosition;
            return new Point(point.X, point.Y);
        } 
        */
        #region DraggingSetting
        private Node _Dragmode;
        private void DragModeEnter(object sender, MouseButtonEventArgs e)
        {
            Node node = sender as Node;
            if (!addNodes && !addEdges && !delEdges && !delNodes && !startBFS && !startDFS && !startDijktra)
            {
                _Dragmode = node;
                graphContent.Children.Remove(node);
                graphContent.Children.Add(node);
                _Dragmode.Focus();
            }
            if (!RunningAlgorithm)
                if (startBFS) {  RunBFS(Nodes.IndexOf(node)); }
                else if (startDFS) { RunDFS(Nodes.IndexOf(node)); }
                else if (startDijktra) { RunDijktra(Nodes.IndexOf(node)); }
        }
        private Point getPercent(Thickness inp)
        {
            return new Point(inp.Left * 100 / oldRenderSize.Width, inp.Top * 100 / oldRenderSize.Height);
        }
        private void RelocatedElement(object sender, SizeChangedEventArgs e)
        {
            Size currentSize = this.RenderSize;
            foreach (Node node in Nodes)
            {
                Point p = getPercent(node.Margin);
                p.X *= currentSize.Width / 100;
                p.Y *= currentSize.Height / 100;
                //if (p.X >= currentSize.Width - 30) p.X = currentSize.Width - 30;
                //if (p.Y >= currentSize.Height - 30) p.Y = currentSize.Height - 30;
                node.Margin = new Thickness( (p.X>currentSize.Width-30)?(currentSize.Width-30):p.X,
                    (p.Y>currentSize.Height-30)?(currentSize.Height-30):p.Y,
                    0, 0);
            }
            foreach (string K in Edges.Keys)
            {
                int nA=(int)((char)K[0]-65), nB = (int)((char)K[1] - 65);
                Point pA=new Point(Nodes[nA].Margin.Left,Nodes[nA].Margin.Top), pB = new Point(Nodes[nB].Margin.Left, Nodes[nB].Margin.Top);
                Edges[K].changePosition(pA,pB);
                EdgesTransform[Edges[K]] = new RotateTransform(Edges[K].RotateAngle);
                Edges[K].RenderTransform = EdgesTransform[Edges[K]];
            }
            oldRenderSize = currentSize;
        }
        private void getMouseCoordinate(object sender, MouseEventArgs e)
        {
            Point coor = Mouse.GetPosition(this);
            MouseCoordinate = ((Int32)coor.X).ToString() + "," + ((Int32)coor.Y).ToString();
            if (_Dragmode != null && coor.X >= 15 && coor.Y >= 15 &&
                coor.X <= this.RenderSize.Width - 15 && coor.Y <= this.RenderSize.Height - 15)
            {
                int pos = Nodes.IndexOf(_Dragmode);
                _Dragmode.Margin = new Thickness(coor.X - 15, coor.Y - 15, 0, 0);
                Point DragPoint = new Point(_Dragmode.Margin.Left, _Dragmode.Margin.Top);
                for (int i=0;i<Nodes.Count;i++)
                    if (Edges.ContainsKey(_Dragmode.nodeName.ToString()+Nodes[i].nodeName.ToString()))
                    {
                        Edges[_Dragmode.nodeName.ToString() + Nodes[i].nodeName.ToString()].changePosition(new Point(Nodes[i].Margin.Left, Nodes[i].Margin.Top),
                            DragPoint);
                        EdgesTransform[Edges[_Dragmode.nodeName.ToString() + Nodes[i].nodeName.ToString()]] = new RotateTransform(Edges[_Dragmode.nodeName.ToString() + Nodes[i].nodeName.ToString()].RotateAngle);
                        Edges[_Dragmode.nodeName.ToString() + Nodes[i].nodeName.ToString()].RenderTransform = EdgesTransform[Edges[_Dragmode.nodeName.ToString() + Nodes[i].nodeName.ToString()]];
                    }
            }
        }
        private void DragModeLeave(object sender, EventArgs e)
        {
            _Dragmode = null;
            this.Focus();
        }
        #endregion
    }
}