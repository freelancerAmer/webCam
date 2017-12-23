using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Security.Cryptography;
using System.Threading;
using Pfz.Remoting;
using RemotingArticleSamples.Common;
using System.Drawing.Imaging;
using System.IO;
using AForge.Video;
using AForge.Video.DirectShow;
using Pfz.Collections;


namespace webCam
{
    public partial class Form1 : Form
    {
        public string host;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string hostName = Dns.GetHostName();
            string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();
            label3.Text = hostName + "/ IP /" + myIP;
            //textBox1.Text = myIP;        
            //label4.Text = "Error " + " Can not make session";
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            string IPAddress = string.Empty;
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    IPAddress = ip.ToString();
                    label4.Text += IPAddress +"\n";
                    break;
                }
            }
            //Console.WriteLine(IPAddress);
            //Console.ReadKey();
            textBox1.Text = IPAddress;
            
        }
        private static void p_Server(object serverObject)
        {
            try
            {
                RemotingServer server = (RemotingServer)serverObject;

                // To this sample, I will require cryptography, and will only
                // accept RijndaelManaged and TripleDES. If I don't register a
                // valid cryptography, all cryptographies are valid.

                server.CryptographyMode = CryptographyMode.Required;
                server.RegisterAcceptedCryptography<RijndaelManaged>();
                server.RegisterAcceptedCryptography<TripleDESCryptoServiceProvider>();

                // Here I register the valid classes that the client can create
                // directly.
                //server.Register(typeof(IServer), typeof(SecureChat.Server.Server));
                server.Register(typeof(SecureChat.Common.IServer), typeof(SecureChat.Server.Server));
                // Here I start the server.
                server.Run(570);

            }
            catch (ObjectDisposedException)
            {
                // If during initialization the user press ENTER and disposes the
                // server, I will simple ignore the exception and finish the thread.

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (var server = new RemotingServer())
            {
                Thread thread = new Thread(p_Server);
                thread.Name = "Listener server thread";
                thread.Start(server);
                label4.Text = thread.Name + " Started";
                //textBox1.Text = myIP;

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string hostName = Dns.GetHostName();
            string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();
            
            hosts = textBox1.Text;
            label4.Text = "the client machine IP is " + hostName + " " + myIP + " " + hosts;
            //RemotingClient client = new RemotingClient(textBox1.Text, 8000);

            Application.Run(new SecureChat.Client.FormMain(hosts));

        }

        
    }

}



