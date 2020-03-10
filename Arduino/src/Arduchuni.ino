#include "SevenSegmentLed.hpp"

#define INDICATOR_START 8
#define SENSOR_START 0
#define N_SENSORS 1
#define LED_START 22
#define INDICATOR_BRIGHTNESS 128

const int ledPins[4+7] = {
  LED_START, LED_START+1, LED_START+2, LED_START+3,
  LED_START+4, LED_START+5, LED_START+6, LED_START+7, LED_START+8, LED_START+9, LED_START+10
};
const SevenSegmentLed<4> led(ledPins, ledPins+4);

void setup() {
  for (int i = 0; i < N_SENSORS; ++i) {
    pinMode(i + INDICATOR_START, OUTPUT);
  }
  for (int i = 0; i < sizeof(ledPins)/sizeof(*ledPins), ++i) {
    pinMode(ledPins[i], OUTPUT);
  }
  Serial.begin(9600);
}

void loop() {
  for (int i = 0; i < N_SENSORS; ++i) {
    int pSensor = i + SENSOR_START;
    int pInd = i + INDICATOR_START;

    int val = analogRead(pSensor);
    int brightness = val > 1000 ? INDICATOR_BRIGHTNESS : 0;
    analogWrite(pInd, brightness);

    if (i == 0) {
      led.setNumber(val);
    }
  }

  delay(10);
}
