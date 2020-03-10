#pragma once

#include <Arduino.h>

/*
    Seven segment LED controller
    N: number of digits, 0 is the right-most digit

    Counter-clockwise ordering of segments
       --  0
    5 |  | 1
       --  6
    4 |  | 2
       --  3
*/
template<int N>
class SevenSegmentLed {
    public:
    int numberData[10] = {
        0b0111111, 0b1011011, 0b1011011, 0b1001111, 0b1100110, 
        0b1101101, 0b1111101, 0b0000111, 0b1111111, 0b1101111
    }

    SevenSegmentLed(int digitPins[N], int dataPins[7]) {
        for (int i = 0; i < N; ++i) {
            m_digitPins[i] = digitPins[i];
        }
        for (int i = 0; i < 7; ++i) {
            m_dataPins[i] = dataPins[i];
        }
    }

    void setDigit(int digit, int data) {
        for (int i = 0; i < N; ++i) {
            digitalWrite(m_digitPins[i], digit == i ? HIGH : LOW);
        }
        for (int i = 0; i < 7; ++i) {
            digitalWrite((data & 1) > 0 ? LOW : HIGH);
            data >>= 1;
        }
    }

    void setEmpty(int digit) {
        setDigit(digit, 0);
    }

    void setNumber(int digit, int n) {
        setDigit(digit, numberData[n]);
    }

    void setNumber(int n, bool leadingZeros=false) {
        for (int i = 0; i < N; ++i) {
            if (n == 0) {
                if (leadingZeros) {
                    setNumber(i, 0);
                }
                else {
                    setEmpty(i);
                }
            } else {
                setNumber(i, n % 10);
            }
            n /= 10;
        }
    }

    private:
    int m_digitPins[N];
    int m_dataPins[7];
}