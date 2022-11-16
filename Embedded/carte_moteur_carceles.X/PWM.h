/* 
 * File:   PWM.h
 * Author: Table2
 *
 * Created on 12 septembre 2022, 09:47
 */

#ifndef PWM_H
#define	PWM_H
#define moteur_droit 1
#define moteur_gauche 0

void InitPWM(void);
//void PWMSetSpeed(float vitesseEnPourcents, int mot);
void PWMUpdateSpeed();
void PWMSetSpeedConsigne(float vitesseEnPourcents, char moteur);
#endif	/* PWM_H */
