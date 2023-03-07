using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace RobotVisu
{
    public class Robot
    {
        public string receivedText = "";
        public float distanceTelemetreDroit;
        public float distanceTelemetreCentre;
        public float distanceTelemetreGauche;
		public Queue<byte> byteListReceived = new Queue<byte>();
        public float timeStamp;
        public float positionXOdo;
        public float positionYOdo;
        public float angleRadOdo;
        public float vitLinéaireOdo;
        public float vitAngulaireOdo;
        public PID pidLin = new PID();
        public PID pidAng = new PID();

        public Robot()
        {
            
		}
    }

    public class PID
    {
        //Constants
        public double Kp;
        public double Ki;
        public double Kd;
        public double proportionelleMax;
        public double integralMax;
        public double deriveeMax;

        //Variables
        public float Erreur;
        public float Mesure;
        public float CorrP;
        public float CorrI;
        public float CorrD;
        public float Command;

    }
}
