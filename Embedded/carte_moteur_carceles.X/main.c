#include <stdio.h>
#include <stdlib.h>
#include <xc.h>
#include "ChipConfig.h"
#include "IO.h"
#include "timer.h"
#include "PWM.h"
#include "adc.h"   
#include "Robot.h"
#include "ToolBox.h"

unsigned int ADCValue0;
unsigned int ADCValue1;
unsigned int ADCValue2;
unsigned int ADCValue3;
unsigned int ADCValue4;

int main(void) {
    /***************************************************************************************************/
    //Initialisation de l?oscillateur
    /****************************************************************************************************/
    InitOscillator();

    /****************************************************************************************************/
    // Configuration des entrées sorties
    /****************************************************************************************************/
    InitIO();

    //LED_BLANCHE = 1;
   // LED_BLEUE = 1;
   // LED_ORANGE = 1;

    InitTimer1();
    InitTimer23();

    InitPWM();
    InitADC1();
    /****************************************************************************************************/
    // Boucle Principale
    /****************************************************************************************************/
    while (1) {
        if (ADCIsConversionFinished()) {
            ADCClearConversionFinishedFlag();
            unsigned int * result = ADCGetResult();
            ADCValue0 = result[0];
            ADCValue1 = result[1];
            ADCValue2 = result[2];
            ADCValue3 = result[3];
            ADCValue4 = result[4];
        }
        if(ADCValue0 > 0x015D){
            LED_ORANGE = 1 ;
        }
        else {
            LED_ORANGE = 0 ;
        }
        if(ADCValue1 > 0x015D){
            LED_ORANGE = 1 ;
        }
        else {
            LED_ORANGE = 0 ;
        }
        if(ADCValue2 > 0x015D){
            LED_BLEUE = 1 ;
        }
        else {
            LED_BLEUE = 0 ;
        }
        if(ADCValue3 > 0x015D){
            LED_BLANCHE = 1 ;
        }
        else {
            LED_BLANCHE = 0 ;
        }
        if(ADCValue4 > 0x015D){
            LED_BLANCHE = 1 ;
        }
        else {
            LED_BLANCHE = 0 ;
        }
    }
} // fin main
