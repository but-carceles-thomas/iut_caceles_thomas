using ExtendedSerialPort;
using System;
using System.Collections.Generic;
using System.IO.Ports;
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
using System.Windows.Threading;

namespace RobotVisu
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ReliableSerialPort serialPort1;
        DispatcherTimer timerAffichage;
        Robot robot = new Robot();

        public MainWindow()
        {
            InitializeComponent();
            serialPort1 = new ReliableSerialPort("COM4", 115200, Parity.None, 8, StopBits.One);
            serialPort1.DataReceived += SerialPort1_DataReceived; ;
            serialPort1.Open();

            timerAffichage = new DispatcherTimer();
            timerAffichage.Interval = new TimeSpan(0, 0, 0, 0, 100);
            timerAffichage.Tick += TimerAffichage_Tick;
            timerAffichage.Start();
        }
        private void TimerAffichage_Tick(object sender, EventArgs e)
        {
            //if (robot.receivedText != "")
            //{
            //    textBoxReception.Text += robot.receivedText;
            //    robot.receivedText = "";
            //}

            while(robot.byteListReceived.Count()>0)
            {
                var c = robot.byteListReceived.Dequeue();
                textBoxReception.Text += "0x" + c.ToString("X2") + " ";
            }
        }

        private void SerialPort1_DataReceived(object sender, DataReceivedArgs e)
        {
            //robot.receivedText += Encoding.UTF8.GetString(e.Data, 0, e.Data.Length);
            foreach(var c in e.Data)
            {
                robot.byteListReceived.Enqueue(c);
            }
		}

        bool toggle = true;
        private void buttonEnvoyer_Click(object sender, RoutedEventArgs e)
        {
            if (toggle)
            {
                SendMessage();
               
            }
        }

        private void textBoxEmission_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SendMessage();
            }

           
        }
        void SendMessage()
        {
            serialPort1.WriteLine(textBoxEmission.Text);
            ///if (textBoxEmission.Text != "")
            ///   textBoxReception.Text += "Reçu : " + textBoxEmission.Text;
            textBoxEmission.Text = "";
        }
        
        private void buttonClear_Click(object sender, RoutedEventArgs e)
        {
            if (toggle)
            {
                textBoxReception.Text = "";
            }
        }

        byte[] byteList = new byte[20];
        private void buttontest_Click(object sender, RoutedEventArgs e)
        {
            //for (int i = 0; i < 20; i++)
            //{
            //    byteList[i] = (byte)(2 * i);
            //}
            //serialPort1.Write(byteList, 0, 20);

            string s = "Bonjour";
            byteList = Encoding.UTF8.GetBytes(s);
            UartEncodeAndSendMessage(0x0080, byteList.Length, byteList);
        }

        byte CalculateChecksum(int msgFunction, int msgPayloadLength, byte[] msgPayload)
        {
            byte Checksum = 0;
            Checksum ^= 0xFE;
            Checksum ^= (byte)(msgFunction >> 8);
            Checksum ^= (byte)(msgFunction >> 0);
            Checksum ^= (byte)(msgPayloadLength >> 8);
            Checksum ^= (byte)(msgPayloadLength >> 0);
            int i;
            for (i=0; i < msgPayloadLength; i++)
            {
                Checksum ^= msgPayload[i];
            }
            return Checksum;
        }

        void UartEncodeAndSendMessage (int msgFunction, int msgPayloadLength, byte[] msgPayload)
        {
            byte[] message = new byte[msgPayloadLength + 6];
            int pos = 0;
            message[pos++] = 0xFE;
            message[pos++] = (byte)(msgFunction >> 8);
            message[pos++] = (byte)(msgFunction >> 0);
            message[pos++] = (byte)(msgPayloadLength >> 8);
            message[pos++] = (byte)(msgPayloadLength >> 0);

            int i;
            for (i = 0; i < msgPayloadLength; i++)
            {
                message[pos++] = msgPayload[i];
            }
            message[pos++] = CalculateChecksum(msgFunction, msgPayloadLength, msgPayload);
            serialPort1.Write(message, 0, pos);
        }
    }
}
