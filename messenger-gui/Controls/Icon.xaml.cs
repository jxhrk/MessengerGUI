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

namespace messenger_gui.Controls
{
    /// <summary>
    /// Логика взаимодействия для Icon.xaml
    /// </summary>
    public partial class Icon : UserControl
    {
        public Icon()
        {
            InitializeComponent();
        }

        public static DependencyProperty ImageDataProperty = DependencyProperty.Register("ImageData", typeof(PathGeometry), typeof(IconButton), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
        public PathGeometry ImageData
        {
            get { return (PathGeometry)GetValue(ImageDataProperty); }
            set { SetValue(ImageDataProperty, value); }
        }

        //public static DependencyProperty ForegroundProperty = DependencyProperty.Register("Foreground", typeof(SolidColorBrush), typeof(IconButton));
        //public SolidColorBrush Foreground
        //{
        //    get { return (SolidColorBrush)GetValue(ForegroundProperty); }
        //    set { SetValue(ForegroundProperty, value); }
        //}
    }
}
