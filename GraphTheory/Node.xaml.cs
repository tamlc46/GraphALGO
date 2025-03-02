using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GraphTheory
{
    /// <summary>
    /// Interaction logic for Node.xaml
    /// </summary>
    public partial class Node : UserControl
    {
        public Node()
        {
            InitializeComponent();
            degree = 0;
        }
        //private String _nodeName="B";
        public Char nodeName
        {
            get {
                return (char)GetValue(nodeNameProperty) ;
            }
            set
            {
                SetValue(nodeNameProperty, (char)value);
            }
        }
        public int degree
        {
            get { return (int)GetValue(DegreeProperty); }
            set
            {
                SetValue(DegreeProperty, value);
                this.ToolTip = "Degree: " + GetValue(DegreeProperty).ToString();
            }
        }
        public static DependencyProperty nodeNameProperty = DependencyProperty.Register("nodeName", typeof(char), typeof(Node));
        public static readonly DependencyProperty DegreeProperty =
            DependencyProperty.Register("degree", typeof(int), typeof(Node));
    }
}
