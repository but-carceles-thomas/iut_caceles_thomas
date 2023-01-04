#include <xc.h>
#include "UART_Protocole.h"
#include "CB_TX1.h"
#include "Robot.h"

extern unsigned char autoControl;
extern unsigned char stateRobot;


enum StateReception {
    Waiting,
    FunctionMSB,
    FunctionLSB,
    PayloadLengthMSB,
    PayloadLengthLSB,
    Payload,
    CheckSum
};

unsigned char UartCalculateChecksum(int msgFunction,
        int msgPayloadLength, unsigned char* msgPayload) {
    //Fonction prenant entrée la trame et sa longueur pour calculer le checksum
    unsigned char Checksum = 0;
    Checksum ^= 0xFE;
    Checksum ^= msgFunction >> 8;
    Checksum ^= msgFunction >> 0;
    Checksum ^= msgPayloadLength >> 8;
    Checksum ^= msgPayloadLength >> 0;
    int i;
    for (i = 0; i < msgPayloadLength; i++) {
        Checksum ^= msgPayload[i];
    }
    return Checksum;
}

void UartEncodeAndSendMessage(int msgFunction,
        int msgPayloadLength, unsigned char* msgPayload) {
    //Fonction d?encodage et d?envoi d?un message
    unsigned char message[6 + msgPayloadLength];
    int pos = 0;
    message[pos++] = 0xFE;
    message[pos++] = msgFunction >> 8;
    message[pos++] = msgFunction >> 0;
    message[pos++] = msgPayloadLength >> 8;
    message[pos++] = msgPayloadLength >> 0;

    int i;
    for (i = 0; i < msgPayloadLength; i++) {
        message[pos++] = msgPayload[i];
    }
    message[pos++] = UartCalculateChecksum(msgFunction, msgPayloadLength, msgPayload);
    SendMessage(message, pos);
}

int msgDecodedFunction = 0;
int msgDecodedPayloadLength = 0;
unsigned char msgDecodedPayload[128];
int msgDecodedPayloadIndex = 0;
enum StateReception rcvState = Waiting;

void UartDecodeMessage(unsigned char c) {
    //Fonction prenant en entrée un octet et servant à reconstituer les trames
    switch (rcvState) {
        case Waiting:
            if (c == 0xFE)
                rcvState = FunctionMSB;
            break;
        case FunctionMSB:
            msgDecodedFunction = c << 8;
            rcvState = FunctionLSB;
            break;
        case FunctionLSB:
            msgDecodedFunction += c << 0;
            rcvState = PayloadLengthMSB;
            break;
        case PayloadLengthMSB:
            msgDecodedPayloadLength = c << 8;
            rcvState = PayloadLengthLSB;
            break;
        case PayloadLengthLSB:
            msgDecodedPayloadLength += c << 0;
            if (msgDecodedPayloadLength == 0) {
                rcvState = CheckSum;
            } else if (msgDecodedPayloadLength >= 128) {
                rcvState = Waiting;
            } else {
                msgDecodedPayloadIndex = 0;
                rcvState = Payload;
            }
            break;
        case Payload:
            msgDecodedPayload[msgDecodedPayloadIndex++] = c;
            if (msgDecodedPayloadIndex == msgDecodedPayloadLength) {
                rcvState = CheckSum;
            }
            break;
        case CheckSum:
        {
            unsigned char calculatedChecksum = UartCalculateChecksum(msgDecodedFunction, msgDecodedPayloadLength, msgDecodedPayload);
            if (calculatedChecksum == c) {
                //Sucess, on a message valide
                UartProcessDecodedMessage(msgDecodedFunction, msgDecodedPayloadLength, msgDecodedPayload);
            }
            rcvState = Waiting;
        }
            break;


        default:
            rcvState = Waiting;
            break;
    }
}

void UartProcessDecodedMessage(unsigned char function,
        unsigned char payloadLength, unsigned char* payload) {
    //Fonction appelée après le décodage pour exécuter l'action
    //correspondant au message reçu
    switch (function) {
        case SET_ROBOT_STATE:
            SetRobotState(payload[0]);
            break;
        case SET_ROBOT_MANUAL_CONTROL:
            SetRobotAutoControlState(payload[0]);
            break;
        default:
            break;
    }
}

void SetRobotState(unsigned char state){
    stateRobot = state;
}

void SetRobotAutoControlState(unsigned char state){
    autoControl = state;
}

//*************************************************************************/
//Fonctions correspondant aux messages
//*************************************************************************/