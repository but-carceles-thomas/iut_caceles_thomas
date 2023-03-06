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
    
    //For Debug only
    double corrP;
    double corrI;
    double corrD;
} PidCorrector;

#define SET_ASSERVISSEMENT 0x0070

void SetupPidAsservissement(volatile PidCorrector* PidCorr, double Kp, double Ki, double Kd, double proportionelleMax);
void DisplayPid(volatile PidCorrector* PidCorr, double Kp, double Ki, double Kd, double proportionelleMax);
void DisplayAsservConstant();

#endif	/* ASSERVISSEMENT_H */

