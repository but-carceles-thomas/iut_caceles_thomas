/* 
 * File:   UART_Protocole.h
 * Author: Table2
 *
 * Created on 3 janvier 2023, 14:00
 */

#ifndef UART_PROTOCOLE_H
#define	UART_PROTOCOLE_H

unsigned char UartCalculateChecksum(int msgFunction,
        int msgPayloadLength, unsigned char* msgPayload);
void UartEncodeAndSendMessage(int msgFunction,
        int msgPayloadLength, unsigned char* msgPayload);
void UartDecodeMessage(unsigned char c);
void UartProcessDecodedMessage(unsigned char function,
        unsigned char payloadLength, unsigned char* payload);
void SetRobotState(unsigned char state);
void SetRobotAutoControlState(unsigned char state);

#define SET_ROBOT_STATE 0x0051
#define SET_ROBOT_MANUAL_CONTROL 0x0052


#endif	/* UART_PROTOCOLE_H */

