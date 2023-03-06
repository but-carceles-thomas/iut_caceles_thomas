#include "asservissement.h"
#include "Robot.h"
#include "UART_Protocole.h"
#include "QEI.h"

double proportionelleMax;
double integralMax;
double deriveeMax;

void SetupPidAsservissement(volatile PidCorrector* PidCorr, double Kp, double Ki, double Kd, double proportionelleMax) {
    PidCorr->Kp = Kp;
    PidCorr->erreurProportionelleMax = proportionelleMax; //On limite la correction due au Kp
    PidCorr->Ki = Ki;
    PidCorr->erreurIntegraleMax = integralMax; //On limite la correction due au Ki
    PidCorr->Kd = Kd;
    PidCorr->erreurDeriveeMax = deriveeMax;


    //UartEncodeAndSendMessage();
}

void DisplayAsservConstant()
{
    unsigned char asservDisplay[48];
    getBytesFromFloat(asservDisplay, 0, (float) (robotState.PidX.Kp));
    getBytesFromFloat(asservDisplay, 4, (float) (robotState.PidX.Ki));
    getBytesFromFloat(asservDisplay, 8, (float) (robotState.PidX.Kd));
    getBytesFromFloat(asservDisplay, 12, (float) (robotState.PidX.erreurProportionelleMax));
    getBytesFromFloat(asservDisplay, 16, (float) (robotState.PidX.erreurIntegraleMax));
    getBytesFromFloat(asservDisplay, 20, (float) (robotState.PidX.erreurDeriveeMax));
    
    getBytesFromFloat(asservDisplay, 24, (float) (robotState.PidTheta.Kp));
    getBytesFromFloat(asservDisplay, 28, (float) (robotState.PidTheta.Ki));
    getBytesFromFloat(asservDisplay, 32, (float) (robotState.PidTheta.Kd));
    getBytesFromFloat(asservDisplay, 36, (float) (robotState.PidTheta.erreurProportionelleMax));
    getBytesFromFloat(asservDisplay, 40, (float) (robotState.PidTheta.erreurIntegraleMax));
    getBytesFromFloat(asservDisplay, 44, (float) (robotState.PidTheta.erreurDeriveeMax));
    UartEncodeAndSendMessage(SET_ASSERVISSEMENT, 48, asservDisplay);
    
}

void DisplayAsservVariables()
{
    unsigned char asservDisplay[48];
    getBytesFromFloat(asservDisplay, 0, (float) (robotState.PidX.erreurIntegrale));
    getBytesFromFloat(asservDisplay, 4, (float) (robotState.PidX.epsilon_1));
    getBytesFromFloat(asservDisplay, 8, (float) (robotState.PidX.erreur));
    getBytesFromFloat(asservDisplay, 12, (float) (robotState.PidX.corrP));
    getBytesFromFloat(asservDisplay, 16, (float) (robotState.PidX.corrI));
    getBytesFromFloat(asservDisplay, 20, (float) (robotState.PidX.corrD));
    
    getBytesFromFloat(asservDisplay, 24, (float) (robotState.PidTheta.erreurIntegrale));
    getBytesFromFloat(asservDisplay, 28, (float) (robotState.PidTheta.epsilon_1));
    getBytesFromFloat(asservDisplay, 32, (float) (robotState.PidTheta.erreur));
    getBytesFromFloat(asservDisplay, 36, (float) (robotState.PidTheta.corrP));
    getBytesFromFloat(asservDisplay, 40, (float) (robotState.PidTheta.corrI));
    getBytesFromFloat(asservDisplay, 44, (float) (robotState.PidTheta.corrD));
    UartEncodeAndSendMessage(SET_ASSERVISSEMENT, 48, asservDisplay);
    //DisplayPid(robotState.PidX.Kp, robotState.PidX.Ki, robotState.PidX.Kd, robotState.PidX.erreurProportionelleMax);
}
        

//void DisplayPid(double Kp, double Ki, double Kd, double proportionelleMax) {
//    Kp = PidCorr->Kp;
//    Ki = PidCorr->Ki;
//    Kd = PidCorr->Kd;
//    
//    proportionelleMax  = PidCorr->erreurProportionelleMax;
//    integralMax = PidCorr->erreurIntegraleMax;
//    deriveeMax = PidCorr->erreurDeriveeMax;
//}

//double Correcteur(volatile PidCorrector* PidCorr, double erreur) {
//    PidCorr->erreur = erreur;
//    double erreurProportionnelle = LimitToInterval(erreur, -erreurProportionelleMax / PidCorr->Kp, erreurProportionelleMax / PidCorr->Kp);
//    PidCorr->corrP = PidCorr.Kp * erreurProportionnelle;
//    PidCorr->erreurIntegrale += integralMax;
//    PidCorr->erreurIntegrale = LimitToInterval(...);
//    PidCorr->corrI = ...;
//    double erreurDerivee = (erreur - PidCorr->epsilon_1) * FREQ_ECH_QEI;
//    double deriveeBornee = LimitToInterval(erreurDerivee, -PidCorr->erreurDeriveeMax / PidCorr->Kd, PidCorr->epsilon_1 = erreur;
//            PidCorr->corrD = deriveeBornee * PidCorr->Kd;
//
//    return PidCorr->corrP + PidCorr->corrI + PidCorr->corrD;
//}

//void UpdateAsservissement() {
//    robotState.PidX.erreur = ...;
//            robotState.PidTheta.erreur = ...;
//            robotState.xCorrectionVitessePourcent =
//            Correcteur(&robotState.PidX, robotState.PidX.erreur);
//            robotState.thetaCorrectionVitessePourcent = ...;
//            PWMSetSpeedConsignePolaire(robotState.xCorrectionVitessePourcent,
//            robotState.thetaCorrectionVitessePourcent);
//}