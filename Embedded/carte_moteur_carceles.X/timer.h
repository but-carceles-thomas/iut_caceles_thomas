#ifndef TIMER_H
#define TIMER_H

extern unsigned long timestamp ;

void InitTimer23 (void) ;
void InitTimer1 (void) ;
void initTimer4 (void) ;

void __attribute__((interrupt, no_auto_psv)) _T3Interrupt(void);
void __attribute__((interrupt, no_auto_psv)) _T1Interrupt(void);
void __attribute__((interrupt, no_auto_psv)) _T4Interrupt(void);

#endif /*TIMER_H*/