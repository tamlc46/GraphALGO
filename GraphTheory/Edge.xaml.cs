using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace GraphTheory
{
    /// <summary>
    /// Interaction logic for Edge.xaml
    /// </summary>
    public partial class Edge : UserControl
    {
        public Edge()
        {
            InitializeComponent();
            Weight = 1;
            tmpW = Weight;
        }
        public Edge(Point _A, Point _B)
        {
            InitializeComponent();
            A = _A; B = _B;
            Weight = 1;
            tmpW = Weight;
            InvalidatedEdge();
        }
        /*
        private bool animation;
        public bool Animation
        {
            get { return animation; }
            set
            {
                animation = value;
                if (Animation)
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        BeginStoryboard(ResizeLine);
                        Animation = false;
                    }));
                }
            }
        }*/
        private Point A, B;
        public Point getA
        {
            get { return A; }
        }
        public Point getB
        {
            get { return B; }
        }
        private double _angle;
        public double RotateAngle
        {
            get { return _angle; }
        }
        private void InvalidatedEdge()
        {
            this.Width = calLength();
            Line.Width = this.Width;
            _angle = calAngle();
            this.Margin = new Thickness(A.X+15, A.Y-2.5, 0, 0);
        }
        public void changePosition(Point nA,Point nB)
        {
            if (nA.X > nB.X)
            {
                B = nA; A = nB;
            }
            else
            {
                A = nA; B = nB;
            }
            InvalidatedEdge();
        }
        #region PropertiCalculate
        private double calLength()
        {
            double len = 0.0;
            len = Math.Sqrt(Math.Pow(A.X - B.X, 2) + Math.Pow(A.Y - B.Y, 2));
            return len;
        }
        private double calAngle()
        {
            double xDiff = B.X - A.X;
            double yDiff = B.Y - A.Y;
            return (Math.Atan2(yDiff,xDiff)*180.0/Math.PI);
        }
        #endregion
        public static DependencyProperty EdgeWeightProperty = DependencyProperty.Register("Weight", typeof(int), typeof(Edge));
        public int Weight
        {
            get { return (int)GetValue(EdgeWeightProperty); }
            set { SetValue(EdgeWeightProperty, value); }
        }
        private int tmpW;
        #region TextBox
        private void DeactiveTextBox(object sender, RoutedEventArgs e)
        {
            if (Weight > 100)
            {
                MessageBox.Show("Maximum value is 100!!!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                Weight = tmpW;
                ActiveTextbox();
            }
            else
            {
                tmpW = Weight;
                inputWeight.Visibility = Visibility.Hidden;
                inputWeight.Focusable = false;
                lblWeight.Visibility = Visibility.Visible;
                this.InvalidateVisual();
            }
        }
        private void CheckNumberic(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        private void checkKey(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) DeactiveTextBox(this, new RoutedEventArgs());
            else if (e.Key == Key.Escape) { Weight = tmpW; DeactiveTextBox(this, new RoutedEventArgs()); }
        }
        public void ActiveTextbox()
        {
                inputWeight.Visibility = Visibility.Visible;
                inputWeight.Focusable = true;
                lblWeight.Visibility = Visibility.Hidden;
                inputWeight.Focus();
                inputWeight.SelectAll();
        }
        private void tblckClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount==2)
            {
                ActiveTextbox();
            }
        }
        private void AnimationComplete(object sender, EventArgs e)
        {
            Line.BeginAnimation(Rectangle.WidthProperty, null);
        }
        /*
private void ShowLength(object sender, MouseButtonEventArgs e)
{
   MessageBox.Show("Sqrt((" + A.X.ToString() + "-" + B.X.ToString() + ")^2+(" + A.Y.ToString() + "-" + B.Y.ToString() + ")^2)=" + calLength().ToString() + "\n" + this.ActualWidth.ToString() + " " + this.RenderSize.Width.ToString());
}
*/
        private void ChangeWeightValue(object sender, MouseWheelEventArgs e)
        {
            if (Weight<=100 && Weight>=0)
            {
                Weight += e.Delta;
            }
            if (Weight <=1) Weight = 1;
            else if (Weight > 100) Weight = 100;
            tmpW = Weight;
        }
        #endregion
    }
}
