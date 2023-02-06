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


#endif	/* ASSERVISSEMENT_H */

