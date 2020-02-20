using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using Path = System.IO.Path;
using SuperSocket;
using SuperSocket.ClientEngine;
using System.Net;
using System.Text;
using System.Windows.Media;
using System.Windows.Threading;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Windows.Media.Imaging;
using System.Drawing;
using EyeKeyboard.Utils;
using EyeKeyboard.Properties;
using System.Windows.Controls.Primitives;
using System.Reflection;

namespace EyeKeyboard
{




    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MetroWindow
    {

        #region 变量与对象
        private Button[,] btns;
        private int x = 0;
        private int y = 0;
        private string ip = string.Empty;
        private string port = string.Empty;
        AsyncTcpSession tcpClient = null;
        Mat imde = new Mat();
        Mat imde2 = new Mat();
        #endregion

        /// <summary>
        /// 构造方法
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

        }




        #region 控件事件

        /// <summary>
        /// 主窗口载入事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {

            ReadConfig();
            CloseState();
        }

        /// <summary>
        /// 帮助按钮单击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Btn_help_Click(object sender, RoutedEventArgs e)
        {
            MessageDialogResult clickresult = await this.ShowMessageAsync(this.Title, "\r\nProgrammed by SanShuiFeiBing.\r\nDesigned by lyq1996 and Dashan Deng.\r\n\r\nBlog: www.sanshuifeibing.cn", MessageDialogStyle.Affirmative);

            //if (clickresult == MessageDialogResult.Negative) //取消
            //{
            //    return;
            //}
            //else //确认
            //{
            //    //确认后的处理
            //}


        }

        /// <summary>
        /// 键盘按钮单击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnKey_Click(object sender, RoutedEventArgs e)
        {

            if (((Button)sender).Name != "btn_Space" && ((Button)sender).Name != "btn_Del" && ((Button)sender).Name != "btn_sqm" && ((Button)sender).Name != "btn_cm" && ((Button)sender).Name != "btn_qm" && ((Button)sender).Name != "btn_fs")
            {
                tb_output.AppendText(((Button)sender).Name.Replace("btn_", ""));
            }
            else if (((Button)sender).Name == "btn_Space")
            {
                tb_output.AppendText(" ");
            }
            else if (((Button)sender).Name == "btn_Del")
            {
                if (tb_output.Text != string.Empty)
                {
                    tb_output.Text = tb_output.Text.Substring(0, tb_output.Text.Length - 1);
                }
            }
            else if (((Button)sender).Name == "btn_sqm")
            {
                tb_output.AppendText("'");
            }
            else if (((Button)sender).Name == "btn_cm")
            {
                tb_output.AppendText(",");
            }
            else if (((Button)sender).Name == "btn_qm")
            {
                tb_output.AppendText("?");
            }
            else if (((Button)sender).Name == "btn_fs")
            {
                tb_output.AppendText(".");
            }
        }

        /// <summary>
        /// Open按钮单击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_open_Click(object sender, RoutedEventArgs e)
        {


            ip = tb_ip.Text;
            port = tb_port.Text;
            try
            {
                tcpClient = new AsyncTcpSession();
                tcpClient.ReceiveBufferSize = 800 * 1024;
                tcpClient.Connect(new IPEndPoint(IPAddress.Parse(ip), int.Parse(port)));


            }
            catch (Exception error)
            {
                AddMessage("[Error] > " + error.Message);
                CloseState();
                Log.Error("[Error] > " + error.Message);
            }


            tcpClient.DataReceived += TcpClient_DataReceived;
            tcpClient.Error += TcpClient_Error;
            tcpClient.Closed += TcpClient_Closed;
            tcpClient.Connected += TcpClient_Connected;




        }

        /// <summary>
        /// Close按钮单击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_close_Click(object sender, RoutedEventArgs e)
        {
            tcpClient.Close();
            CloseState();
        }

        /// <summary>
        /// 博客链接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_link_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.sanshuifeibing.cn");
        }

        /// <summary>
        /// 主窗口关闭事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MetroWindow_Closed(object sender, EventArgs e)
        {

            WriteConfig();
        }
# endregion

        #region 其他方法与事件



        /// <summary>
        /// 客户端断开连接事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TcpClient_Closed(object sender, EventArgs e)
        {


            AddMessage("[Info] > Disconnect");
            Log.Info("[Info] > Disconnect");
            CloseState();
        }

        /// <summary>
        /// 连接发生异常事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TcpClient_Error(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
        {

            AddMessage("[Error] > " + e.Exception.Message);
            CloseState();
            Log.Error("[Error] > " + e.Exception.Message);
        }

        /// <summary>
        /// 收到服务器消息事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TcpClient_DataReceived(object sender, DataEventArgs e)
        {
            byte[] strBuffer = new byte[1];
            byte[] imgBuffer = new byte[e.Length - 1];
            byte[] imgBuffer2 = new byte[e.Length - 1];
            byte[] sendBuffer = new byte[1];
            string translate = string.Empty;
            Array.Copy(e.Data, strBuffer, 1);
            string message = Encoding.UTF8.GetString(strBuffer).Substring(0, 1);

            SelectButtonInit();
            //0.center 1.right 2.left 3.ensure 4.init 5.eye-img 6.camera-img 7.up/serve-notice 8.down/enter-notice
            switch (message)
            {
                case "1":
                    translate = "right";


                    this.Dispatcher.BeginInvoke(new Action(delegate
                    {
                        img_right.Source = FormatConvert.BitmapToBitmapImage(Properties.Resources.on);
                        img_left.Source = FormatConvert.BitmapToBitmapImage(Properties.Resources.off);
                        img_center.Source = FormatConvert.BitmapToBitmapImage(Properties.Resources.round_off);
                        img_up.Source = FormatConvert.BitmapToBitmapImage(Properties.Resources.off);
                        img_down.Source = FormatConvert.BitmapToBitmapImage(Properties.Resources.off);
                        SolidColorBrush defaultBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0xFF, 0x00, 0x96, 0x88));
                        if (y < 9)
                        {
                            if (x == 4)
                            {
                                if (y == 8)
                                {
                                    btns[x, y].Background = defaultBrush;
                                    y = 0;
                                    btns[x, y].Background = System.Windows.Media.Brushes.Orange;

                                }
                                else
                                {
                                    btns[x, 0].Background = defaultBrush;
                                    y = 8;
                                    btns[x, y].Background = System.Windows.Media.Brushes.Orange;
                                }
                            }
                            else
                            {
                                y++;
                                btns[x, y].Background = System.Windows.Media.Brushes.Orange;
                                btns[x, y - 1].Background = defaultBrush;
                            }
                        }
                        else
                        {
                            btns[x, y].Background = defaultBrush;
                            y = 0;
                            btns[x, y].Background = System.Windows.Media.Brushes.Orange;

                        }

                    }));


                    sendBuffer = Encoding.UTF8.GetBytes("7");
                    tcpClient.Send(sendBuffer, 0, sendBuffer.Length);
                    break;
                case "2":

                    translate = "left";
                    this.Dispatcher.BeginInvoke(new Action(delegate
                    {


                        img_right.Source = FormatConvert.BitmapToBitmapImage(Properties.Resources.off);
                        img_left.Source = FormatConvert.BitmapToBitmapImage(Properties.Resources.on);
                        img_center.Source = FormatConvert.BitmapToBitmapImage(Properties.Resources.round_off);
                        img_up.Source = FormatConvert.BitmapToBitmapImage(Properties.Resources.off);
                        img_down.Source = FormatConvert.BitmapToBitmapImage(Properties.Resources.off);
                        SolidColorBrush defaultBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0xFF, 0x00, 0x96, 0x88));
                        if (y > 0)
                        {

                            if (x == 4)//若位于第四行
                            {
                                if (y > 7)
                                {
                                    btns[x, 8].Background = defaultBrush;
                                    y = 0;
                                    btns[x, y].Background = System.Windows.Media.Brushes.Orange;
                                }
                                else
                                {
                                    btns[x, 0].Background = defaultBrush;
                                    y = 8;
                                    btns[x, y].Background = System.Windows.Media.Brushes.Orange;
                                }
                            }
                            else
                            {
                                y--;
                                btns[x, y].Background = System.Windows.Media.Brushes.Orange;
                                btns[x, y + 1].Background = defaultBrush;
                            }
                        }
                        else
                        {
                            if (x==4)
                            {
                                btns[x, y].Background = defaultBrush;
                                y = 8;
                                btns[x, y].Background = System.Windows.Media.Brushes.Orange;
                            }
                            else
                            {
                                btns[x, y].Background = defaultBrush;
                                y = 9;
                                btns[x, y].Background = System.Windows.Media.Brushes.Orange;
                            }
                               
                        }
                    }));
                    sendBuffer = Encoding.UTF8.GetBytes("7");
                    tcpClient.Send(sendBuffer, 0, sendBuffer.Length);
                    break;
                case "3":

                    translate = "ensure";

                    this.Dispatcher.BeginInvoke(new Action(delegate
                    {


                        ClickHelper.PerformClick(btns[x, y]);

                    }));
                    sendBuffer = Encoding.UTF8.GetBytes("7");
                    tcpClient.Send(sendBuffer, 0, sendBuffer.Length);
                    break;

                case "0":
                    this.Dispatcher.BeginInvoke(new Action(delegate
                    {


                        img_right.Source = FormatConvert.BitmapToBitmapImage(Properties.Resources.off);
                        img_left.Source = FormatConvert.BitmapToBitmapImage(Properties.Resources.off);
                        img_center.Source = FormatConvert.BitmapToBitmapImage(Properties.Resources.round_on);
                        img_up.Source = FormatConvert.BitmapToBitmapImage(Properties.Resources.off);
                        img_down.Source = FormatConvert.BitmapToBitmapImage(Properties.Resources.off);

                    }));
                    translate = "center";
                    sendBuffer = Encoding.UTF8.GetBytes("7");
                    tcpClient.Send(sendBuffer, 0, sendBuffer.Length);
                    break;
                case "4":
                    translate = "init";
                    this.Dispatcher.BeginInvoke(new Action(delegate
                    {

                        img_lamp.Source = FormatConvert.BitmapToBitmapImage(Properties.Resources.init_on);


                    }));
                    sendBuffer = Encoding.UTF8.GetBytes("7");
                    tcpClient.Send(sendBuffer, 0, sendBuffer.Length);
                    break;
                case "5":
                    Array.Copy(e.Data, 1, imgBuffer, 0, e.Length - 1);
                    imde = Cv2.ImDecode(imgBuffer, ImreadModes.Color);
                    this.img_video.Dispatcher.BeginInvoke(new Action(delegate
                    {
                        img_video.Source = FormatConvert.MatToBitmapImage(imde);
                        Cv2.WaitKeyEx(1);
                        imde.Release();

                    }));
                    sendBuffer = Encoding.UTF8.GetBytes("7");
                    tcpClient.Send(sendBuffer, 0, sendBuffer.Length);
                    break;
                case "6":
                    Array.Copy(e.Data, 1, imgBuffer2, 0, e.Length - 1);
                    imde2 = Cv2.ImDecode(imgBuffer2, ImreadModes.Color);


                    this.Dispatcher.BeginInvoke(new Action(delegate
                    {
                        Cv2.ImShow("Camera", imde2);

                        if (Cv2.WaitKey(100) == 13)
                        {
                            sendBuffer = Encoding.UTF8.GetBytes("8");
                            tcpClient.Send(sendBuffer, 0, sendBuffer.Length);
                            Cv2.DestroyAllWindows();


                        }
                        else
                        {
                            sendBuffer = Encoding.UTF8.GetBytes("7");
                            tcpClient.Send(sendBuffer, 0, sendBuffer.Length);
                        }

                        imde2.Release();


                    }));


                    break;

                case "7":
                    translate = "up";
                    this.Dispatcher.BeginInvoke(new Action(delegate
                    {
                        img_right.Source = FormatConvert.BitmapToBitmapImage(Properties.Resources.off);
                        img_left.Source = FormatConvert.BitmapToBitmapImage(Properties.Resources.off);
                        img_center.Source = FormatConvert.BitmapToBitmapImage(Properties.Resources.round_off);
                        img_up.Source = FormatConvert.BitmapToBitmapImage(Properties.Resources.on);
                        img_down.Source = FormatConvert.BitmapToBitmapImage(Properties.Resources.off);
                        SolidColorBrush defaultBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0xFF, 0x00, 0x96, 0x88));
                        if (x > 0)
                        {
                            x--;
                            btns[x, y].Background = System.Windows.Media.Brushes.Orange;
                            btns[x + 1, y].Background = defaultBrush;
                        }
                        else
                        {
                            btns[x, y].Background = defaultBrush;
                            x = 4;
                            btns[x, y].Background = System.Windows.Media.Brushes.Orange;
                            
                        }
                    }));
                    sendBuffer = Encoding.UTF8.GetBytes("7");
                    tcpClient.Send(sendBuffer, 0, sendBuffer.Length);
                    break;
                case "8":
                    translate = "down";
                    this.Dispatcher.BeginInvoke(new Action(delegate
                    {
                        img_right.Source = FormatConvert.BitmapToBitmapImage(Properties.Resources.off);
                        img_left.Source = FormatConvert.BitmapToBitmapImage(Properties.Resources.off);
                        img_center.Source = FormatConvert.BitmapToBitmapImage(Properties.Resources.round_off);
                        img_up.Source = FormatConvert.BitmapToBitmapImage(Properties.Resources.off);
                        img_down.Source = FormatConvert.BitmapToBitmapImage(Properties.Resources.on);
                        SolidColorBrush defaultBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0xFF, 0x00, 0x96, 0x88));
                        if (x < 4)
                        {
                            x++;
                            btns[x - 1, y].Background = defaultBrush;
                            btns[x, y].Background = System.Windows.Media.Brushes.Orange;
                        }
                        else
                        {
                            btns[x, y].Background = defaultBrush;
                            x = 0;
                            btns[x, y].Background = System.Windows.Media.Brushes.Orange;

                        }
                    }));
                    sendBuffer = Encoding.UTF8.GetBytes("7");
                    tcpClient.Send(sendBuffer, 0, sendBuffer.Length);
                    break;

            }

            if (message != "5" && message != "6")
            {

                AddMessage("[Receive] > " + message + " (" + translate + ")");
                Log.Debug("[Receive] > " + message + " (" + translate + ")");
            }







        }


        /// <summary>
        /// 连接到服务器事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TcpClient_Connected(object sender, EventArgs e)
        {
            AddMessage("[Info] > Successful connection");
            Log.Info("[Info] > Successful connection");
            OpenState();
        }

        /// <summary>
        /// 键盘按钮选择控制初始化
        /// </summary>
        private void SelectButtonInit()
        {
            btns = new Button[5, 10] {
                           {btn_1,btn_2,btn_3,btn_4,btn_5,btn_6,btn_7,btn_8,btn_9,btn_0},
                           {btn_Q,btn_W,btn_E,btn_R,btn_T,btn_Y,btn_U,btn_I,btn_O,btn_P},
                           {btn_A,btn_S,btn_D,btn_F,btn_G,btn_H,btn_J,btn_K,btn_L,btn_sqm},
                           {btn_Z,btn_X,btn_C,btn_V,btn_B,btn_N,btn_M,btn_cm,btn_fs,btn_qm },
                           {btn_Space,btn_Space,btn_Space,btn_Space,btn_Space,btn_Space,btn_Space,btn_Space,btn_Del,btn_Del}


            };


        }


        /// <summary>
        /// UI信息输出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void AddMessage(string str)
        {

            this.lb_log.Dispatcher.BeginInvoke(new Action(delegate
            {
                lb_log.Items.Add(str);

                //滚动条自动置底
                Decorator decorator = (Decorator)VisualTreeHelper.GetChild(lb_log, 0);
                ScrollViewer scrollViewer = (ScrollViewer)decorator.Child;
                scrollViewer.ScrollToEnd();


            }));



        }

        /// <summary>
        /// 打开连接时界面状态
        /// </summary>
        private void OpenState()
        {
            this.Dispatcher.BeginInvoke(new Action(delegate
            {
                btn_close.IsEnabled = true;
                btn_open.IsEnabled = false;



            }));
        }

        /// <summary>
        /// 关闭连接时界面状态
        /// </summary>
        private void CloseState()
        {


            this.Dispatcher.BeginInvoke(new Action(delegate
            {
                btn_close.IsEnabled = false;
                btn_open.IsEnabled = true;


                img_video.Source = null;
                img_lamp.Source = FormatConvert.BitmapToBitmapImage(Properties.Resources.init_off);
                img_right.Source = FormatConvert.BitmapToBitmapImage(Properties.Resources.off);
                img_left.Source = FormatConvert.BitmapToBitmapImage(Properties.Resources.off);
                img_center.Source = FormatConvert.BitmapToBitmapImage(Properties.Resources.round_off);
                img_up.Source = FormatConvert.BitmapToBitmapImage(Properties.Resources.off);
                img_down.Source = FormatConvert.BitmapToBitmapImage(Properties.Resources.off);

                SelectButtonInit();

                SolidColorBrush defaultBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0xFF, 0x00, 0x96, 0x88));
                btns[x, y].Background = defaultBrush;
                x = 0;
                y = 0;
                btn_1.Background = System.Windows.Media.Brushes.Orange;
                Cv2.DestroyAllWindows();

            }));


        }

        /// <summary>
        /// 读配置文件
        /// </summary>
        private void ReadConfig()
        {
            tb_ip.Text = Settings.Default.Ip;
            tb_port.Text = Settings.Default.Port;
        }

        /// <summary>
        /// 写配置文件
        /// </summary>
        private void WriteConfig()
        {
            Settings.Default.Ip = tb_ip.Text;
            Settings.Default.Port = tb_port.Text;
            Settings.Default.Save();
        }


        #endregion




      
    }
}
