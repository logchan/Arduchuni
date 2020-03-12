#include "SevenSegmentLed.hpp"

#define INDICATOR_START 22
#define SENSOR_START A0
#define N_SENSORS 2
#define LED_START 28
#define INDICATOR_BRIGHTNESS 128
#define LED_EVERY 100

const int ledPins[4+7] = {
  LED_START, LED_START+1, LED_START+2, LED_START+3, LED_START+4, LED_START+5, LED_START+6,
  LED_START+7, LED_START+8, LED_START+9, LED_START+10
};
SevenSegmentLed<4> led(ledPins+7, ledPins);

void setup() {
  for (int i = 0; i < N_SENSORS; ++i) {
    pinMode(i + INDICATOR_START, OUTPUT);
  }
  for (int i = 0; i < sizeof(ledPins)/sizeof(*ledPins); ++i) {
    pinMode(ledPins[i], OUTPUT);
  }
  Serial.begin(9600);
}

int led_counter = 0;

void loop() {
  for (int i = 0; i < N_SENSORS; ++i) {
    int pSensor = i + SENSOR_START;
    int pInd = i + INDICATOR_START;

    int val = analogRead(pSensor);
    int brightness = val > 1000 ? INDICATOR_BRIGHTNESS : 0;
    analogWrite(pInd, brightness);

    if (led_counter % LED_EVERY == 0) {
      led_counter = 0;
      led.setNumber(val);
    }
    led_counter += 1;
  }

  led.loop();
  delay(5);
}
