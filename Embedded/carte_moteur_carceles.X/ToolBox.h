/* 
 * File:   ToolBox.h
 * Author: Table2
 *
 * Created on 12 septembre 2022, 09:36
 */

#ifndef TOOLBOX_H
#define	TOOLBOX_H
#define PI 3.41592653589793

float Abs(float value);
float Max(float value, float value2);
float Min(float value, float value2);
float LimitToInterval(float value, float lowLimit, float highLimit);
float RadianToDegree(float value);
float DegreeToRadian(float value);

#endif	/* TOOLBOX_H */

