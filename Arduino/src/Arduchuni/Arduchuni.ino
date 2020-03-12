#define DEV_VERSION

#define LOOP_DELAY 5
#define N_SENSORS 6
const int sensorPins[N_SENSORS] = { A0, A1, A2, A3, A4, A5 };

#ifdef DEV_VERSION

#define INDICATOR_BRIGHTNESS 128
#define INDICATOR_THRESHOLD 1000
const int indicatorPins[N_SENSORS] = { 22, 23, 24, 25, 26, 27 };

#include "SevenSegmentLed.hpp"

#define LED_START 28
#define LED_EVERY 100
int led_counter = 0;
const int ledPins[4+7] = {
  LED_START, LED_START+1, LED_START+2, LED_START+3, LED_START+4, LED_START+5, LED_START+6,
  LED_START+7, LED_START+8, LED_START+9, LED_START+10
};
SevenSegmentLed<4> led(ledPins+7, ledPins);
#endif

void setup() {
  Serial.begin(9600);

#ifdef DEV_VERSION
  for (int i = 0; i < N_SENSORS; ++i) {
    pinMode(indicatorPins[i], OUTPUT);
  }
  for (int i = 0; i < sizeof(ledPins)/sizeof(*ledPins); ++i) {
    pinMode(ledPins[i], OUTPUT);
  }
#endif
}

void loop() {
  int values[N_SENSORS];

  for (int i = 0; i < N_SENSORS; ++i) {
    values[i] = analogRead(sensorPins[i]);
  }

  Serial.write((void*)values, sizeof(values));
  Serial.flush();

#ifdef DEV_VERSION
  for (int i = 0; i < N_SENSORS; ++i) {
    analogWrite(indicatorPins[i], values[i] > INDICATOR_THRESHOLD ? INDICATOR_BRIGHTNESS : 0);
  }

  if (led_counter % LED_EVERY == 0) {
    led_counter = 0;
    led.setNumber(values[0]);
  }
  led_counter += 1;
  led.loop();
#endif

  delay(LOOP_DELAY);
}
