using IMG_Server.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using System.Drawing;
using System.Windows.Interop;
using System.Windows.Controls;
using System.Threading;
using IMG_Server.View;

namespace IMG_Server.ViewModel
{
    internal class MainViewModel : BaseViewModel
    {
        public RelayCommand Click_Btn { get; set; }
        private MainWindow _MW { get; set; }
        private bool isok { get; set; } = false;
        public MainViewModel(MainWindow MW) {
            Click_Btn = new RelayCommand(CanClick,AlwaysTrue);
            _MW= MW;

        }

        private BitmapImage GetBitmapImageFromBytes(byte[] bytes)
        {
            BitmapImage btm;
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                btm = new BitmapImage();
                btm.BeginInit();
                btm.StreamSource = ms;
                // Below code for caching is crucial.
                btm.CacheOption = BitmapCacheOption.OnLoad;
                btm.EndInit();
                btm.Freeze();
            }
            return btm;
        }

        private void S1()
        {
            var idAddres = IPAddress.Loopback;
            var port = 27001;
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                MessageBox.Show(idAddres.ToString());
                var EP = new IPEndPoint(idAddres, port);
                socket.Bind(EP);
                socket.Listen(10);
                //   Console.WriteLine($"Listen ever {socket.LocalEndPoint}");
                while (true)
                {
                    var client = socket.Accept();
                    Task.Run(() =>
                    {
                        var length = 0;
                        var bytes = new byte[1000000];
                        do
                        {
                            length = client.Receive(bytes);
                                _MW.Dispatcher.Invoke(() =>
                                {
                                    BitmapImage img = GetBitmapImageFromBytes(bytes);
                                    UC uc = new UC(new ImageBrush(img));
                                    uc.Width= 250;
                                    uc.Height= 180;
                                    _MW.Nihad_list.Items.Add(uc);
                                    _MW.Nihad_list.ScrollIntoView(_MW.Nihad_list.Items[_MW.Nihad_list.Items.Count-1]);
                                });
 
                        } while (true);
                    });
                }
            }
        }

        private void CanClick(object parameter)
        {
            if (!isok)
            {
                isok = true;
                Task t1 = new Task(S1);
                t1.Start();
                _MW.Grids_1.Children.Remove(_MW.btn);
            }
        }
        private bool AlwaysTrue(object parametr) => true;
    }
}
