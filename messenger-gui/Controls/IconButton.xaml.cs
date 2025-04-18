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
    /// Логика взаимодействия для IconButton.xaml
    /// </summary>
    public partial class IconButton : UserControl
    {

        public IconButton()
        {
            InitializeComponent();
        }

       public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent(
       name: "Click",
       routingStrategy: RoutingStrategy.Bubble,
       handlerType: typeof(RoutedEventHandler),
       ownerType: typeof(IconButton));

        public event RoutedEventHandler Click
        {
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }

        void RaiseCustomRoutedEvent()
        {
            // Create a RoutedEventArgs instance.
            RoutedEventArgs routedEventArgs = new(routedEvent: ClickEvent);

            // Raise the event, which will bubble up through the element tree.
            RaiseEvent(routedEventArgs);
        }

        public static DependencyProperty ImageGeometryProperty = DependencyProperty.Register("ImageGeometry", typeof(PathGeometry), typeof(IconButton));
        public PathGeometry ImageGeometry
        {
            get { return (PathGeometry)GetValue(ImageGeometryProperty); }
            set { SetValue(ImageGeometryProperty, value); }
        }

        public static DependencyProperty MouseForegroundProperty = DependencyProperty.Register("MouseForeground", typeof(SolidColorBrush), typeof(IconButton));
        public SolidColorBrush MouseForeground
        {
            get { return (SolidColorBrush)GetValue(MouseForegroundProperty); }
            set { SetValue(MouseForegroundProperty, value); }
        }

        private static void OnChanged(DependencyObject sender,
            DependencyPropertyChangedEventArgs e)
        {
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            RaiseCustomRoutedEvent();
        }
    }
}
