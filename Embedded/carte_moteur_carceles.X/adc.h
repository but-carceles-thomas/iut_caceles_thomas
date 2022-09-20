/* 
 * File:   adc.h
 * Author: Table2
 *
 * Created on 20 septembre 2022, 15:06
 */

#ifndef ADC_H
#define	ADC_H

void InitADC1();
void __attribute__((interrupt, no_auto_psv)) _AD1Interrupt();
void ADC1StartConversionSequence();
unsigned int * ADCGetResult();
unsigned char ADCIsConversionFinished();
void ADCClearConversionFinishedFlag();

#endif
