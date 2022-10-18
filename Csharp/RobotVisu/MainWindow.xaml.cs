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

namespace RobotVisu
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ReliableSerialPort serialPort1;
        public MainWindow()
        {
            InitializeComponent();
            serialPort1 = new ReliableSerialPort("COM9", 115200, Parity.None, 8, StopBits.One);
            serialPort1.Open();
        }

        bool toggle = false;
        private void buttonEnvoyer_Click(object sender, RoutedEventArgs e)
        {
            if (toggle)
            {
                SendMessage();
                //if (textBoxEmission.Text != "")
                //    textBoxReception.Text += "\nReçu : " + textBoxEmission.Text;
                //textBoxEmission.Text = "";
                //buttonEnvoyer.Background = Brushes.RoyalBlue;
            }
            //else
            //{
            //    buttonEnvoyer.Background = Brushes.Beige;
            //}

            //toggle = !toggle;
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
    }
}
