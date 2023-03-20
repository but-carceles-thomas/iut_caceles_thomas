#include "asservissement.h"
#include "Robot.h"
#include "UART_Protocole.h"
#include "QEI.h"
#include "Utilities.h"
#include "ToolBox.h"

double proportionelleMax;
double integralMax;
double deriveeMax;

void SetupPidAsservissement(volatile PidCorrector* PidCorr, double Kp, double Ki, double Kd, double proportionelleMax, double integralMax, double deriveeMax, double consigne) {
    PidCorr->Kp = Kp;
    PidCorr->erreurProportionelleMax = proportionelleMax; //On limite la correction due au Kp
    PidCorr->Ki = Ki;
    PidCorr->erreurIntegraleMax = integralMax; //On limite la correction due au Ki
    PidCorr->Kd = Kd;
    PidCorr->erreurDeriveeMax = deriveeMax;
    PidCorr->Consigne = consigne;
}

void SetAsservissement(unsigned char * payload) {
    // Réglage PID linéaire
    robotState.PidX.Kp = getFloat(payload, 0);
    robotState.PidX.Ki = getFloat(payload, 4);
    robotState.PidX.Kd = getFloat(payload, 8);

    robotState.PidX.erreurProportionelleMax = getFloat(payload, 12);
    robotState.PidX.erreurIntegraleMax = getFloat(payload, 16);
    robotState.PidX.erreurDeriveeMax = getFloat(payload, 20);

    // Réglage PID angulaire
    robotState.PidTheta.Kp = getFloat(payload, 24);
    robotState.PidTheta.Ki = getFloat(payload, 28);
    robotState.PidTheta.Kd = getFloat(payload, 32);

    robotState.PidTheta.erreurProportionelleMax = getFloat(payload, 36);
    robotState.PidTheta.erreurIntegraleMax = getFloat(payload, 40);
    robotState.PidTheta.erreurDeriveeMax = getFloat(payload, 44);

}

void DisplayAsservConstant() {
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
    UartEncodeAndSendMessage(R_TO_PC_ASSERVISSEMENT_CONSTANTS, 48, asservDisplay);

}

void DisplayAsservVariables() {
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
    UartEncodeAndSendMessage(R_TO_PC_ASSERVISSEMENT_VARIABLES, 48, asservDisplay);
}

double Correcteur(volatile PidCorrector* PidCorr, double erreur) {
    PidCorr->erreur = erreur;
    double erreurProportionnelle = LimitToInterval(PidCorr->erreur, -robotState.PidX.erreurProportionelleMax / PidCorr->Kp, robotState.PidX.erreurProportionelleMax / PidCorr->Kp);
    PidCorr->corrP = PidCorr->Kp * erreurProportionnelle;
    PidCorr->erreurIntegrale += erreur / FREQ_ECH_QEI;
    PidCorr->erreurIntegrale = LimitToInterval(PidCorr->erreurIntegrale, -robotState.PidX.erreurIntegraleMax / PidCorr->Ki, robotState.PidX.erreurIntegraleMax / PidCorr->Ki);
    PidCorr->corrI = PidCorr->Ki * PidCorr->erreurIntegrale;
    double erreurDerivee = (erreur - PidCorr->epsilon_1) * FREQ_ECH_QEI;
    double deriveeBornee = LimitToInterval(erreurDerivee, -PidCorr->erreurDeriveeMax / PidCorr->Kd, PidCorr->epsilon_1 = erreur);
    PidCorr->corrD = deriveeBornee * PidCorr->Kd;

    return PidCorr->corrP + PidCorr->corrI + PidCorr->corrD;
}

void UpdateAsservissement() {
    robotState.PidX.erreur = robotState.PidX.Consigne - robotState.vitesseLineaireFromOdometry;
    robotState.PidTheta.erreur = robotState.PidX.Consigne - robotState.vitesseAngulaireFromOdometry;

    robotState.xCorrectionVitessePourcent = Correcteur(&robotState.PidX, robotState.PidX.erreur);
    robotState.thetaCorrectionVitessePourcent = Correcteur(&robotState.PidTheta, robotState.PidTheta.erreur);
    
    double coeff = 50;
    robotState.vitesseGaucheConsigne = coeff * (robotState.xCorrectionVitessePourcent - DISTROUES / 2 * robotState.thetaCorrectionVitessePourcent);
    robotState.vitesseDroiteConsigne = coeff * (robotState.xCorrectionVitessePourcent + DISTROUES / 2 * robotState.thetaCorrectionVitessePourcent);
}