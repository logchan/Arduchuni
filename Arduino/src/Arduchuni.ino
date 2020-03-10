#define INDICATOR_START 8
#define INDICATOR_END 13
#define SENSOR_START 0
#define SENSOR_END 0
#define INDICATOR_BRIGHTNESS 128

const int num_sensors = SENSOR_END-SENSOR_START+1;

void setup() {
  for (int p = INDICATOR_START; p <= INDICATOR_END; ++p) {
    pinMode(p, OUTPUT);
  }
  Serial.begin(9600);
}

void loop() {
  for (int i = 0; i < num_sensors; ++i) {
    int pSensor = i + SENSOR_START;
    int pInd = i + INDICATOR_START;

    int val = analogRead(pSensor);
    int brightness = val > 1000 ? INDICATOR_BRIGHTNESS : 0;
    analogWrite(pInd, brightness);
    //Serial.println(val);
  }

  delay(10);
}
