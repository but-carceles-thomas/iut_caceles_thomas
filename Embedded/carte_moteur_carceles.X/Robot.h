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
            float xPosFromOdometry;
            float yPosFromOdometry;
            float angleRadianFromOdometry;
            float xPosFromOdometry_1;
            float yPosFromOdometry_1;
            float angleRadianFromOdometry_1;
            float vitesseDroitFromOdometry;
            float vitesseGaucheFromOdometry;
            float vitesseLineaireFromOdometry;
            float vitesseAngulaireFromOdometry;
            float vitesseLineaireCommand;
            float vitesseAngulaireCommand;
            
            PidCorrector PidX;
            PidCorrector PidTheta;
            float xCorrectionVitessePourcent;
            float thetaCorrectionVitessePourcent;
            float PWMSetSpeedConsignePolaire;

        }
        ;
    }
    ;
} ROBOT_STATE_BITS;

extern volatile ROBOT_STATE_BITS robotState;
#endif /* ROBOT_H */