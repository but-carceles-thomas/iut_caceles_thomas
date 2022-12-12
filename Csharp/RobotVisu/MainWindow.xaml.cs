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
                DecodeMessage(c);
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
        byte[] byteList2;
        byte[] byteList3;
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

            byteList = new byte[3];
            byteList[0] = 20;
            byteList[1] = 30;
            byteList[2] = 40;
            UartEncodeAndSendMessage((int)SuperVision.DistanceIR, 3, byteList);

            byteList2 = new byte[2];
            byteList2[0] = 20;
            byteList2[1] = 30;
            UartEncodeAndSendMessage((int)SuperVision.ConsigneVitesse, 2, byteList2);

            byte[] led = { 0x01, 0x01 };
            UartEncodeAndSendMessage((int)SuperVision.ReglageLED, 2, led);



        }

        private void buttontestFalse_Click(object sender, RoutedEventArgs e)
        {
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


        void UartEncodeAndSendMessageWithError(int msgFunction, int msgPayloadLength, byte[] msgPayload)
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
            message[3] = 0x23;
            serialPort1.Write(message, 0, pos);
        }

        public enum StateReception
        {
            Waiting,
            FunctionMSB,
            FunctionLSB,
            PayloadLengthMSB,
            PayloadLengthLSB,
            Payload,
            CheckSum
        }

        StateReception rcvState = StateReception.Waiting;
        int msgDecodedFunction = 0;
        int msgDecodedPayloadLength = 0;
        byte[] msgDecodedPayload;
        int msgDecodedPayloadIndex = 0;

        private void DecodeMessage(byte c)
        {
            switch (rcvState)
            {
                case StateReception.Waiting:
                    if (c == 0xFE)
                    {
                        rcvState = StateReception.FunctionMSB;
                    }
                    else rcvState = StateReception.Waiting;
                    break;
                case StateReception.FunctionMSB:
                    msgDecodedFunction = c << 8;
                    rcvState = StateReception.FunctionLSB;
                    break;
                case StateReception.FunctionLSB:
                    msgDecodedFunction += c << 0;
                    rcvState = StateReception.PayloadLengthMSB;
                    break;
                case StateReception.PayloadLengthMSB:
                    msgDecodedPayloadLength = c << 8;
                    rcvState = StateReception.PayloadLengthLSB;
                    break;
                case StateReception.PayloadLengthLSB:
                    msgDecodedPayloadLength += c << 0;
                    if (msgDecodedPayloadLength == 0)
                    {
                        rcvState = StateReception.CheckSum;
                    }
                    else if (msgDecodedPayloadLength >= 1024)
                    {
                        rcvState = StateReception.Waiting;
                    }
                    else
                    {
                        msgDecodedPayload = new byte[msgDecodedPayloadLength];
                        msgDecodedPayloadIndex = 0;
                        rcvState = StateReception.Payload;
                    }
                    break;
                case StateReception.Payload:
                        msgDecodedPayload[msgDecodedPayloadIndex++] = c;
                        if (msgDecodedPayloadIndex == msgDecodedPayloadLength)
                        {
                            rcvState = StateReception.CheckSum;
                            SuperVision supervision;
                        }
                    break;
                case StateReception.CheckSum:
                    byte calculatedChecksum = CalculateChecksum(msgDecodedFunction, msgDecodedPayloadLength, msgDecodedPayload);
                    if (calculatedChecksum == c)
                    {
                        //Sucess, on a message valide
                        textBoxReception.Text += "OK : ";
                        ProcessDecodedMessage(msgDecodedFunction, msgDecodedPayloadLength, msgDecodedPayload);
                    }
                    rcvState = StateReception.Waiting;
                    break;
                default:
                    rcvState = StateReception.Waiting;
                    break;
            }
        }

        public enum SuperVision
        {
            TransmissionTexte = 0x0080,
            ReglageLED = 0x0020,
            DistanceIR = 0x0030,
            ConsigneVitesse = 0x0040
        }

        void ProcessDecodedMessage(int msgFunction, int msgPayloadLength, byte[] msgPayload)
        {
            switch((SuperVision)msgFunction)
            {
                case SuperVision.DistanceIR:
                    labelIRGauche.Content = "IR Gauche : " + msgPayload[0].ToString() + " cm";
                    labelIRCentre.Content = "IR Centre : " + msgPayload[1].ToString() + " cm";
                    labelIRDroit.Content = "IR Droit : " + msgPayload[2].ToString() + " cm";
                    break;

                case SuperVision.ConsigneVitesse:
                    labelVitesseGauche.Content = "Vitesse Gauche :" + msgPayload[0].ToString() + "%";
                    labelVitesseDroit.Content = "Vitesse Droit :" + msgPayload[1].ToString() + "%";
                    break;

                case SuperVision.ReglageLED:
                    switch (msgPayload[0])
                    {
                        case 0:
                            if(msgPayload[1] == 0)
                                checkBoxLed1.IsChecked = false;
                            else
                                checkBoxLed1.IsChecked = true;
                            break;
                        case 1:
                            if (msgPayload[1] == 0)
                                CheckBoxLed2.IsChecked = false;
                            else
                                CheckBoxLed2.IsChecked = true;
                            break;
                        case 2:
                            if (msgPayload[1] == 0)
                                checkBoxLed3.IsChecked = false;
                            else
                                checkBoxLed3.IsChecked = true;
                            break;
                    }
                    break;
            }
        }
    }
}
