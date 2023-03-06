#ifndef ROBOT_H
#define ROBOT_H

#include "asservissement.h"


typedef struct robotStateBITS {

    union {

        struct {
            unsigned char taskEnCours;
            float vitesseGaucheConsigne;
            float vitesseGaucheCommandeCourante;
            float vitesseDroiteConsigne;
            float vitesseDroiteCommandeCourante;
            float acceleration;
            float distanceTelemetreDroit;
            float distanceTelemetreCentre;
            float distanceTelemetreGauche;
            float distanceTelemetreExGauche;
            float distanceTelemetreExDroit;
            double xPosFromOdometry;
            double yPosFromOdometry;
            double angleRadianFromOdometry;
            double xPosFromOdometry_1;
            double yPosFromOdometry_1;
            double angleRadianFromOdometry_1;
            double vitesseDroitFromOdometry;
            double vitesseGaucheFromOdometry;
            double vitesseLineaireFromOdometry;
            double vitesseAngulaireFromOdometry;
            
            PidCorrector PidX;
            PidCorrector PidTheta;
            double xCorrectionVitessePourcent;
            double thetaCorrectionVitessePourcent;
            double PWMSetSpeedConsignePolaire;

        }
        ;
    }
    ;
} ROBOT_STATE_BITS;

extern volatile ROBOT_STATE_BITS robotState;
#endif /* ROBOT_H */