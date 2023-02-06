#include "asservissement.h"
#include "Robot.h"
#include "UART_Protocole.h"

double proportionelleMax;
double integralMax;
double deriveeMax;

void SetupPidAsservissement(volatile PidCorrector* PidCorr, double Kp, double Ki, double Kd, double pro){
    PidCorr->Kp = Kp;
    PidCorr->erreurProportionelleMax = proportionelleMax; //On limite la correction due au Kp
    PidCorr->Ki = Ki;
    PidCorr->erreurIntegraleMax = integralMax; //On limite la correction due au Ki
    PidCorr->Kd = Kd;
    PidCorr->erreurDeriveeMax = deriveeMax;
    
    //UartEncodeAndSendMessage();
}

