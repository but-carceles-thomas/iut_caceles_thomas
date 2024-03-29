#include <stdio.h>
#include <stdlib.h>
#include <xc.h>
#include <libpic30.h>
#include "ChipConfig.h"
#include "IO.h"
#include "timer.h"
#include "PWM.h"
#include "adc.h"   
#include "Robot.h"
#include "ToolBox.h"
#include "main.h"
#include "UART.h"
#include "CB_TX1.h"
#include "CB_RX1.h"
#include "UART_Protocole.h"
#include "QEI.h"

unsigned int ADCValue0;
unsigned int ADCValue1;
unsigned int ADCValue2;
unsigned int ADCValue3;
unsigned int ADCValue4;

unsigned char stateRobot;

int main(void) {
    /***************************************************************************************************/
    //Initialisation de l?oscillateur
    /****************************************************************************************************/
    InitOscillator();

    /****************************************************************************************************/
    // Configuration des entr�es sorties
    /****************************************************************************************************/
    InitIO();

    //LED_BLANCHE = 1;
    // LED_BLEUE = 1;
    // LED_ORANGE = 1;

    InitTimer1();
    initTimer4();
    InitTimer23();

    InitPWM();
    InitADC1();
    InitUART();

    InitQEI1();
    InitQEI2();
    /****************************************************************************************************/
    // Boucle Principale
    /****************************************************************************************************/

    while (1) {
        if (ADCIsConversionFinished() == 1) {
            ADCClearConversionFinishedFlag();
            unsigned int * result = ADCGetResult();
            ADCValue0 = result[0];
            ADCValue1 = result[1];
            ADCValue2 = result[2];
            ADCValue3 = result[3];
            ADCValue4 = result[4];

            float volts = ((float) result [0])*3.3 / 4096 * 3.2;
            robotState.distanceTelemetreDroit = 34 / volts - 5;
            volts = ((float) result [1])*3.3 / 4096 * 3.2;
            robotState.distanceTelemetreExDroit = 34 / volts - 5;
            volts = ((float) result [2])*3.3 / 4096 * 3.2;
            robotState.distanceTelemetreCentre = 34 / volts - 5;
            volts = ((float) result [3])*3.3 / 4096 * 3.2;
            robotState.distanceTelemetreGauche = 34 / volts - 5;
            volts = ((float) result [4])*3.3 / 4096 * 3.2;
            robotState.distanceTelemetreExGauche = 34 / volts - 5;

            if (robotState.distanceTelemetreDroit < 30 || robotState.distanceTelemetreExDroit < 30) {
                LED_ORANGE = 1;
            } else {
                LED_ORANGE = 0;
            }

            if (robotState.distanceTelemetreCentre < 30) {
                LED_BLEUE = 1;
            } else {
                LED_BLEUE = 0;
            }
            if (robotState.distanceTelemetreGauche < 30 || robotState.distanceTelemetreExGauche < 30) {
                LED_BLANCHE = 1;
            } else {
                LED_BLANCHE = 0;
            }
            unsigned char payload[] = {robotState.distanceTelemetreGauche, robotState.distanceTelemetreCentre, robotState.distanceTelemetreDroit};
            UartEncodeAndSendMessage(0x0030, 3, payload);
        }

        int i;
        for (i = 0; i < CB_RX1_GetDataSize(); i++) {
            unsigned char c = CB_RX1_Get();
            //SendMessage(&c, 1);
            UartDecodeMessage(c);
        }

        //        unsigned char payload[] = {'B', 'o', 'n', 'j', 'o', 'u', 'r'};
        //        UartEncodeAndSendMessage(0x0080, 7, payload);
        //        __delay32(40000000);
    }
}

unsigned char autoControl = 1;

void OperatingSystemLoop(void) {
    switch (stateRobot) {
        case STATE_ATTENTE:
            timestamp = 0;
            PWMSetSpeedConsigne(0, moteur_droit);
            PWMSetSpeedConsigne(0, moteur_gauche);
            stateRobot = STATE_ATTENTE_EN_COURS;

        case STATE_ATTENTE_EN_COURS:
            if (timestamp > 1000)
                stateRobot = STATE_AVANCE;
            break;

        case STATE_AVANCE:
            PWMSetSpeedConsigne(17, moteur_droit);
            PWMSetSpeedConsigne(17, moteur_gauche);
            stateRobot = STATE_AVANCE_EN_COURS;
            break;
        case STATE_AVANCE_EN_COURS:
            if (autoControl)
                SetNextRobotStateInAutomaticMode();
            break;

        case STATE_TOURNE_GAUCHE:
            PWMSetSpeedConsigne(15, moteur_droit);
            PWMSetSpeedConsigne(-15, moteur_gauche);
            stateRobot = STATE_TOURNE_GAUCHE_EN_COURS;
            break;
        case STATE_TOURNE_GAUCHE_EN_COURS:
            if (autoControl)
                SetNextRobotStateInAutomaticMode();
            break;

        case STATE_TOURNE_DROITE:
            PWMSetSpeedConsigne(-15, moteur_droit);
            PWMSetSpeedConsigne(15, moteur_gauche);
            stateRobot = STATE_TOURNE_DROITE_EN_COURS;
            break;
        case STATE_TOURNE_DROITE_EN_COURS:
            if (autoControl)
                SetNextRobotStateInAutomaticMode();
            break;

        case STATE_RECULE:
            timestamp = 0;
            PWMSetSpeedConsigne(-15, moteur_droit);
            PWMSetSpeedConsigne(-15, moteur_gauche);
            stateRobot = STATE_RECULE_EN_COURS;
            break;

        case STATE_RECULE_EN_COURS:
            if (autoControl)
                stateRobot = STATE_TOURNE_SUR_PLACE_DROITE;
            break;

        case STATE_TOURNE_SUR_PLACE_GAUCHE:
            PWMSetSpeedConsigne(15, moteur_droit);
            PWMSetSpeedConsigne(-15, moteur_gauche);
            stateRobot = STATE_TOURNE_SUR_PLACE_GAUCHE_EN_COURS;
            break;
        case STATE_TOURNE_SUR_PLACE_GAUCHE_EN_COURS:
            if (autoControl)
                SetNextRobotStateInAutomaticMode();
            break;

        case STATE_TOURNE_SUR_PLACE_DROITE:
            PWMSetSpeedConsigne(-15, moteur_droit);
            PWMSetSpeedConsigne(15, moteur_gauche);
            stateRobot = STATE_TOURNE_SUR_PLACE_DROITE_EN_COURS;
            break;
        case STATE_TOURNE_SUR_PLACE_DROITE_EN_COURS:
            if (autoControl)
                SetNextRobotStateInAutomaticMode();
            break;

        default:
            stateRobot = STATE_ATTENTE;
            break;

    }
}

//void SendStateSupervision() {
//    unsigned long timestampCourant = timestamp;
//    unsigned char payload[] = {stateRobot, (unsigned char) (timestampCourant >> 24), (unsigned char) (timestampCourant >> 16), (unsigned char) (timestampCourant >> 8), (unsigned char) (timestampCourant >> 0)};
//    UartEncodeAndSendMessage(0x0050, 5, payload);
//}

unsigned char nextStateRobot = 0;

void SetNextRobotStateInAutomaticMode(void) {
    unsigned char positionObstacle = PAS_D_OBSTACLE;

    //D�termination de la position des obstacles en fonction des t�l�m�tres
    if (robotState.distanceTelemetreDroit > 30 &&
            robotState.distanceTelemetreExDroit > 20 &&
            robotState.distanceTelemetreCentre > 30 &&
            robotState.distanceTelemetreExGauche > 20 &&
            robotState.distanceTelemetreGauche > 30) //pas d?obstacle
        positionObstacle = PAS_D_OBSTACLE;
    else if (robotState.distanceTelemetreCentre <= 30)
        positionObstacle = OBSTACLE_EN_FACE;
    else if (robotState.distanceTelemetreDroit < 30 ||
            robotState.distanceTelemetreExDroit < 20)
        positionObstacle = OBSTACLE_A_DROITE;
    else if (robotState.distanceTelemetreExGauche < 30 ||
            robotState.distanceTelemetreGauche < 20)
        positionObstacle = OBSTACLE_A_GAUCHE;


    //D�termination de l?�tat � venir du robot
    if (positionObstacle == PAS_D_OBSTACLE)
        nextStateRobot = STATE_AVANCE;
    else if (positionObstacle == OBSTACLE_A_DROITE)
        nextStateRobot = STATE_TOURNE_GAUCHE;
    else if (positionObstacle == OBSTACLE_A_GAUCHE)
        nextStateRobot = STATE_TOURNE_DROITE;
    else if (positionObstacle == OBSTACLE_EN_FACE)
        nextStateRobot = STATE_TOURNE_SUR_PLACE_DROITE;

    //Si l?on n?est pas dans la transition de l?�tape en cours
    if (nextStateRobot != stateRobot - 1) {
        stateRobot = nextStateRobot;
        //SendStateSupervision();
    }
}
