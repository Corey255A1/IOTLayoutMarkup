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

namespace IOTClientWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SerialInterface si;
        Dictionary<UIElement, byte> ControlsToIDs = new Dictionary<UIElement, byte>();
        Dictionary<byte, UIElement> IDsToControls = new Dictionary<byte, UIElement>();
        public MainWindow()
        {
            InitializeComponent();
            si = new SerialInterface();
            si.AddCtrlEvent += AddControlEvent;
            si.UpdateCtrlEvent += UpdateControlEvent;
        }

        private void getPage_Click(object sender, RoutedEventArgs e)
        {
            ControlCanvas.Children.Clear();
            ControlsToIDs.Clear();
            IDsToControls.Clear();
            si.GetPage(0);
        }

        private void AddControlEvent(PacketHeaders.AddCtrl ac)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                if (IDsToControls.ContainsKey(ac.ID)) return;
                switch (ac.Type)
                {
                    case 1:
                        {
                            Button b = new Button();
                            b.Click += B_Click;
                            b.Width = ac.Width;
                            b.Height = ac.Height;
                            b.Content = ac.Label;
                            ControlsToIDs.Add(b, ac.ID);
                            IDsToControls.Add(ac.ID, b);

                            ControlCanvas.Children.Add(b);
                            Canvas.SetLeft(b, ac.X);
                            Canvas.SetTop(b, ac.Y);
                        }
                        break;
                    case 2:
                        {
                            TextBox b = new TextBox();
                            b.Width = ac.Width;
                            b.Height = ac.Height;
                            b.IsReadOnly = true;
                            ControlsToIDs.Add(b, ac.ID);
                            IDsToControls.Add(ac.ID, b);

                            ControlCanvas.Children.Add(b);
                            Canvas.SetLeft(b, ac.X);
                            Canvas.SetTop(b, ac.Y);
                        }
                        break;
                }
            }
            ));
        }

        private void UpdateControlEvent(PacketHeaders.CtrlData uc)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                if (IDsToControls.ContainsKey(uc.ID))
                {
                    Type t = IDsToControls[uc.ID].GetType();
                    if(t == typeof(TextBox))
                    {
                        TextBox tb = IDsToControls[uc.ID] as TextBox;
                        tb.Text = Encoding.UTF8.GetString(uc.Data).Trim('\0');
                    }
                }
            }
            ));
        }

        private void B_Click(object sender, RoutedEventArgs e)
        {
            if (ControlsToIDs.ContainsKey((UIElement)sender))
            {
                byte[] b = new byte[1];
                b[0] = 1;
                si.SetControl(0,(byte)ControlsToIDs[(UIElement)sender], b);
            }
        }
    }
}
