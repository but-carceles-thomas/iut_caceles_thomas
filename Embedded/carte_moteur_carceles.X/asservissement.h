/* 
 * File:   asservissement.h
 * Author: Table2
 *
 * Created on 6 février 2023, 14:20
 */

#ifndef ASSERVISSEMENT_H
#define	ASSERVISSEMENT_H

typedef struct _PidCorrector {
    double Kp;
    double Ki;
    double Kd;
    double erreurProportionelleMax;
    double erreurIntegraleMax;
    double erreurDeriveeMax;
    double erreurIntegrale;
    double epsilon_1;
    double erreur;
    double Consigne;
    
    //For Debug only
    double corrP;
    double corrI;
    double corrD;
} PidCorrector;

void SetAsservissement(unsigned char * payload);
void SetupPidAsservissement(volatile PidCorrector* PidCorr, double Kp, double Ki, double Kd, double proportionelleMax, double integralMax, double deriveeMax, double consigne);
void DisplayPid(volatile PidCorrector* PidCorr, double Kp, double Ki, double Kd, double proportionelleMax);
void DisplayAsservConstant();
void DisplayAsservVariables();
double Correcteur(volatile PidCorrector* PidCorr, double erreur);
void UpdateAsservissement();

#endif	/* ASSERVISSEMENT_H */

