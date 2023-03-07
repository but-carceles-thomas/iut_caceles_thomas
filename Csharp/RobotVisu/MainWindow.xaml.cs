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
using MouseKeyboardActivityMonitor.WinApi;
using MouseKeyboardActivityMonitor;
using System.Windows.Forms;
using WpfOscilloscopeControl;
using SciChart.Charting.Visuals;
using WpfAsservissementDisplay;

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
        private readonly KeyboardHookListener m_KeyboardHookManager;

        public MainWindow()
        {
            // Set this code once in App.xaml.cs or application startup
            SciChartSurface.SetRuntimeLicenseKey("oyxbgnRp6QRmRXs59rp8vFCzacIdcu5XdPKDtAe98UnygxSZwLV1fC1N4ZLXUVSAG2pa2RfEOZUJerlT0NqpzZ/dbDI5nsm+OONzitIZkuDR8cG23HbheqCsljLzREGKsETdl0P1waT+zsRWrUUkIAERV0HTfHbaWU6K44JYNL6iiSXV3ZhV9MFOly7eGxiF5UCZE1g4a/SU+8eypSlfEoe0/Gem2boAG8Gx3+rNPulu0u5XNchtlYsvVIvY1u97gDVTKa5bJMvLAz2OmvIREiC2EEzaGgQVcJE/6pshxR01aevAq1zL0Z0jP4k1DNVcpWecPNoNjJpvVZya7oMQR4WAGq/M1HhFmjju+P7OKk51IdAnt3iUNDYryBBwb87hSKNHYJ1/7h67iDyMGsf2eFx69YobjA9QSXaZ2nIZkGUWNb2gppQTAf384fykn3M3xwsyEgVm9ERZb9MODfclK0KgPgA03ec/raKLSvXQQhKhARYaA14VT1LpR4hx5jVZZA==");

            InitializeComponent();
            serialPort1 = new ReliableSerialPort("COM7", 115200, Parity.None, 8, StopBits.One);
            serialPort1.DataReceived += SerialPort1_DataReceived; ;
            serialPort1.Open();

            timerAffichage = new DispatcherTimer();
            timerAffichage.Interval = new TimeSpan(0, 0, 0, 0, 100);
            timerAffichage.Tick += TimerAffichage_Tick;
            timerAffichage.Start();

            m_KeyboardHookManager = new KeyboardHookListener(new GlobalHooker());
            m_KeyboardHookManager.Enabled = true;
            m_KeyboardHookManager.KeyDown += M_KeyboardHookManager_KeyDown; ;

            oscilloSpeed.AddOrUpdateLine(1, 200, "Ligne1");
            oscilloSpeed.ChangeLineColor(1, Color.FromArgb(0xFF, 0xFF, 0x00, 0x00));
        }

        bool autoControlActivated = false;
        private void M_KeyboardHookManager_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (autoControlActivated == false)
            {
                switch (e.KeyCode)
                {
                    case Keys.Left:
                        UartEncodeAndSendMessage(0x0051, 1, new byte[] {
                         (byte)StateRobot.STATE_TOURNE_SUR_PLACE_GAUCHE });
                        break;
                    case Keys.Right:
                        UartEncodeAndSendMessage(0x0051, 1, new byte[] {
                        (byte)StateRobot.STATE_TOURNE_SUR_PLACE_DROITE });
                        break;
                    case Keys.Up:
                        UartEncodeAndSendMessage(0x0051, 1, new byte[] {
                        (byte)StateRobot.STATE_AVANCE });
                        break;
                    case Keys.Down:
                        UartEncodeAndSendMessage(0x0051, 1, new byte[]
                        { (byte)StateRobot.STATE_RECULE });
                        break;
                    case Keys.PageDown:
                        UartEncodeAndSendMessage(0x0051, 1, new byte[]
                      { (byte)StateRobot.STATE_ARRET });
                        break;
                }
            }
        }
        
        private void TimerAffichage_Tick(object sender, EventArgs e)
        {
            //if (robot.receivedText != "")
            //{
            //    textBoxReception.Text += robot.receivedText;
            //    robot.receivedText = "";
            //}

            while (robot.byteListReceived.Count() > 0)
            {
                var c = robot.byteListReceived.Dequeue();
                //textBoxReception.Text += "0x" + c.ToString("X2") + " ";
                DecodeMessage(c);
            }
            double xvalue = robot.positionXOdo;
            double yvalue = robot.positionYOdo;
            oscilloSpeed.AddPointToLine(1, xvalue, yvalue);


            //asservSpeedDisplay.UpdatePolarOdometrySpeed(xvalue, yvalue);
            asservSpeedDisplay.UpdatePolarOdometrySpeed(robot.vitLinéaireOdo, robot.vitAngulaireOdo);
        }

        private void SerialPort1_DataReceived(object sender, DataReceivedArgs e)
        {
            //robot.receivedText += Encoding.UTF8.GetString(e.Data, 0, e.Data.Length);
            foreach (var c in e.Data)
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
            byteList[0] = 10;
            byteList[1] = 20;
            byteList[2] = 50;
            UartEncodeAndSendMessage((int)SuperVision.DistanceIR, 3, byteList);

            byteList2 = new byte[2];
            byteList2[0] = 50;
            byteList2[1] = 60;
            UartEncodeAndSendMessage((int)SuperVision.ConsigneVitesse, 2, byteList2);

            byte[] led = { 0x01, 0x01 };
            UartEncodeAndSendMessage((int)SuperVision.ReglageLED, 2, led);

            byteList3 = new byte[9*6];

            var t = BitConverter.GetBytes((float)10.0);
            Array.Copy(t, 0, byteList3, 0, 4);
            Array.Copy(t, 0, byteList3, 4, 4);
            Array.Copy(t, 0, byteList3, 8, 4);
            Array.Copy(t, 0, byteList3, 12, 4);
            Array.Copy(t, 0, byteList3, 16, 4);
            Array.Copy(t, 0, byteList3, 20, 4);
            Array.Copy(t, 0, byteList3, 24, 4);
            Array.Copy(t, 0, byteList3, 28, 4);
            Array.Copy(t, 0, byteList3, 32, 4);
            Array.Copy(t, 0, byteList3, 36, 4);
            Array.Copy(t, 0, byteList3, 40, 4);
            Array.Copy(t, 0, byteList3, 44, 4);
            UartEncodeAndSendMessage((int)SuperVision.PC_TO_R_PidAsservConstants, 48, byteList3);
        }

        //private void buttontestFalse_Click(object sender, RoutedEventArgs e)
        //{
        //    string s = "Bonjour";
        //    byteList = Encoding.UTF8.GetBytes(s);
        //    UartEncodeAndSendMessage(0x0080, byteList.Length, byteList);
        //}

        byte CalculateChecksum(int msgFunction, int msgPayloadLength, byte[] msgPayload)
        {
            byte Checksum = 0;
            Checksum ^= 0xFE;
            Checksum ^= (byte)(msgFunction >> 8);
            Checksum ^= (byte)(msgFunction >> 0);
            Checksum ^= (byte)(msgPayloadLength >> 8);
            Checksum ^= (byte)(msgPayloadLength >> 0);
            int i;
            for (i = 0; i < msgPayloadLength; i++)
            {
                Checksum ^= msgPayload[i];
            }
            return Checksum;
        }

        void UartEncodeAndSendMessage(int msgFunction, int msgPayloadLength, byte[] msgPayload)
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


        //void UartEncodeAndSendMessageWithError(int msgFunction, int msgPayloadLength, byte[] msgPayload)
        //{
        //    byte[] message = new byte[msgPayloadLength + 6];
        //    int pos = 0;
        //    message[pos++] = 0xFE;
        //    message[pos++] = (byte)(msgFunction >> 8);
        //    message[pos++] = (byte)(msgFunction >> 0);
        //    message[pos++] = (byte)(msgPayloadLength >> 8);
        //    message[pos++] = (byte)(msgPayloadLength >> 0);

        //    int i;
        //    for (i = 0; i < msgPayloadLength; i++)
        //    {
        //        message[pos++] = msgPayload[i];
        //    }
        //    message[pos++] = CalculateChecksum(msgFunction, msgPayloadLength, msgPayload);
        //    message[3] = 0x23;
        //    serialPort1.Write(message, 0, pos);
        //}

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
                        //textBoxReception.Text += ": OK ";
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
            ConsigneVitesse = 0x0040,
            RobotState = 0x0050,
            PositionData = 0x0061,
            PC_TO_R_PidAsservConstants = 0x0170,
            R_TO_PC_PidAsservConstants = 0x0070,
            R_TO_PC_PidAsservVariables = 0x0071
        }

        public enum StateRobot
        {
            STATE_ATTENTE = 0,
            STATE_ATTENTE_EN_COURS = 1,
            STATE_AVANCE = 2,
            STATE_AVANCE_EN_COURS = 3,
            STATE_TOURNE_GAUCHE = 4,
            STATE_TOURNE_GAUCHE_EN_COURS = 5,
            STATE_TOURNE_DROITE = 6,
            STATE_TOURNE_DROITE_EN_COURS = 7,
            STATE_TOURNE_SUR_PLACE_GAUCHE = 8,
            STATE_TOURNE_SUR_PLACE_GAUCHE_EN_COURS = 9,
            STATE_TOURNE_SUR_PLACE_DROITE = 10,
            STATE_TOURNE_SUR_PLACE_DROITE_EN_COURS = 11,
            STATE_ARRET = 12,
            STATE_ARRET_EN_COURS = 13,
            STATE_RECULE = 14,
            STATE_RECULE_EN_COURS = 15
        }

        void ProcessDecodedMessage(int msgFunction, int msgPayloadLength, byte[] msgPayload)
        {
            switch ((SuperVision)msgFunction)
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
                            if (msgPayload[1] == 0)
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

                case SuperVision.RobotState:
                    int instant = (((int)msgPayload[1]) << 24) + (((int)msgPayload[2]) << 16)
                        + (((int)msgPayload[3]) << 8) + ((int)msgPayload[4]);
                    textBoxReception.Text += "\nRobot␣State␣:␣" +
                        ((StateRobot)(msgPayload[0])).ToString() +
                        "␣-␣" + instant.ToString() + "␣ms";
                    break;

                case SuperVision.PositionData:

                    robot.timeStamp = BitConverter.ToSingle(msgPayload, 0);
                    textBoxReception.Text = "\ntemps : " + robot.timeStamp;

                    robot.positionXOdo = BitConverter.ToSingle(msgPayload, 4);
                    textBoxReception.Text = "\nposition x : " + robot.positionXOdo;

                    robot.positionYOdo = BitConverter.ToSingle(msgPayload, 8);
                    textBoxReception.Text += "\nposition y : " + robot.positionYOdo;

                    robot.angleRadOdo = BitConverter.ToSingle(msgPayload, 12);
                    textBoxReception.Text += "\nangle : " + robot.angleRadOdo;

                    robot.vitLinéaireOdo = BitConverter.ToSingle(msgPayload, 16);
                    textBoxReception.Text += "\nvit.linéaire : " + robot.vitLinéaireOdo;

                    robot.vitAngulaireOdo = BitConverter.ToSingle(msgPayload, 20);
                    textBoxReception.Text += "\nvit.angulaire : " + robot.vitAngulaireOdo;
                    break;

                case SuperVision.R_TO_PC_PidAsservConstants:
                    robot.pidLin.Kp = BitConverter.ToSingle(msgPayload, 0);                 
                    robot.pidLin.Ki = BitConverter.ToSingle(msgPayload, 4);
                    robot.pidLin.Kd = BitConverter.ToSingle(msgPayload, 8);

                    robot.pidLin.proportionelleMax = BitConverter.ToSingle(msgPayload, 12);
                    robot.pidLin.integralMax = BitConverter.ToSingle(msgPayload, 16);
                    robot.pidLin.deriveeMax = BitConverter.ToSingle(msgPayload, 20);

                    robot.pidAng.Kp = BitConverter.ToSingle(msgPayload, 24);
                    robot.pidAng.Ki = BitConverter.ToSingle(msgPayload, 28);
                    robot.pidAng.Kd = BitConverter.ToSingle(msgPayload, 32);

                    robot.pidAng.proportionelleMax = BitConverter.ToSingle(msgPayload, 36);
                    robot.pidAng.integralMax = BitConverter.ToSingle(msgPayload, 40);
                    robot.pidAng.deriveeMax = BitConverter.ToSingle(msgPayload, 44);
                    
                    asservSpeedDisplay.UpdatePolarSpeedCorrectionGains(robot.pidLin.Kp, robot.pidAng.Kp, robot.pidLin.Ki, robot.pidAng.Ki, robot.pidLin.Kd, robot.pidAng.Kd);
                    break;


                case SuperVision.R_TO_PC_PidAsservVariables:
                    robot.pidLin.Mesure = BitConverter.ToSingle(msgPayload, 0);
                    robot.pidLin.Erreur = BitConverter.ToSingle(msgPayload, 4);
                    robot.pidLin.Command = BitConverter.ToSingle(msgPayload, 8);

                    robot.pidLin.CorrP = BitConverter.ToSingle(msgPayload, 12);
                    robot.pidLin.CorrI = BitConverter.ToSingle(msgPayload, 16);
                    robot.pidLin.CorrD = BitConverter.ToSingle(msgPayload, 20);

                    robot.pidAng.Mesure = BitConverter.ToSingle(msgPayload, 24);
                    robot.pidAng.Erreur = BitConverter.ToSingle(msgPayload, 28);
                    robot.pidAng.Command = BitConverter.ToSingle(msgPayload, 32);

                    robot.pidAng.CorrP = BitConverter.ToSingle(msgPayload, 36);
                    robot.pidAng.CorrI = BitConverter.ToSingle(msgPayload, 40);
                    robot.pidAng.CorrD = BitConverter.ToSingle(msgPayload, 44);

                    
                    asservSpeedDisplay.UpdatePolarSpeedConsigneValues(robot.pidLin.Mesure, robot.pidAng.Mesure);
                    asservSpeedDisplay.UpdatePolarSpeedErrorValues(robot.pidLin.Erreur, robot.pidAng.Erreur);
                    break;
            }
        }

        //void SetupPidAsservissement(double PidCorrector, double Kp, double Ki, double Kd, double proportionelleMax, double integralMax, double deriveeMax)
        //{
        //    PidCorrector -> Kp = Kp;
        //}

        private void textBoxEmission_KeyUp_1(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SendMessage();
            }
        }

        void SetAutoControl(byte state)
        {
            byte[] etat = { state };
            if (state == 0)
            {
                autoControlActivated = false;
            }
            else
            {
                autoControlActivated = true;
            }
            UartEncodeAndSendMessage(0x0052, 1, etat);
        }

        void setRobotState(byte state)
        {
            byte[] etat = { state };
            UartEncodeAndSendMessage(0x0051, 1, etat);
        }

        byte state = 1;

        private void modeManuel_Checked(object sender, RoutedEventArgs e)
        {
            state = 0;
            SetAutoControl(state);
        }

        private void modeManuel_Unchecked(object sender, RoutedEventArgs e)
        {
            state = 1;
            SetAutoControl(state);
        }


    }
}
