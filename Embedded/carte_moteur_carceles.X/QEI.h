/* 
 * File:   QEI.h
 * Author: Table2
 *
 * Created on 23 janvier 2023, 08:08
 */

#ifndef QEI_H
#define	QEI_H

#define DISTROUES 0.2185
#define FREQ_ECH_QEI  250.0
#define POSITION_DATA 0x0061
#define VITESSE_DATA 0x0062

void InitQEI1();
void InitQEI2();
void QEIUpdateData();
void SendPositionData();

#endif	/* QEI_H */

