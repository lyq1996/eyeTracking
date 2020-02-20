using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static DebugServer.ServerAsync;

namespace DebugServer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        string clientKey = string.Empty;
        ServerAsync server = new ServerAsync();
        private void Form1_Load(object sender, EventArgs e)
        {
           
           
        }

        private void button_start_Click(object sender, EventArgs e)
        {

                button_start.Enabled = false;
                server = new ServerAsync();
                SetText("[info] > Started" + "\r\n");
                server.Completed += new Action<string, EnSocketAction>((key, enAction) =>
                {

                    switch (enAction)
                    {
                        case EnSocketAction.Connect:
                            SetText("[info] > " + key + " Connected" + "\r\n");
                            clientKey = key;
                            break;
                        case EnSocketAction.SendMsg:
                            //textBox1.AppendText("[Send] > " + key);
                            break;
                        case EnSocketAction.Close:
                            //SetText("[Info] > " + key + " Disconnected" + "\r\n");
                            break;
                        default:
                            break;
                    }
                });

                server.Received += new Action<string, string>((key, msg) =>
                {
                    SetText("[Receive] > "  + msg + "\r\n");

                });

                server.StartAsync(8080);
          

        }

        private void Button_Click(object sender, EventArgs e)
        {

            server.SendAsync(clientKey, ((Button)sender).Text.Substring(0, 1));

        }



        private delegate void SetTextCallback(string text);
       
        private void SetText(string text)
        {
            
            if (this.textBox1.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.textBox1.AppendText( text);
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            server.Close();
        }
    }
}
