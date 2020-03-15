#define N_SENSORS 6
const int sensorPins[N_SENSORS] = { A0, A1, A2, A3, A4, A5 };

void setup() {
  Serial.begin(19200);
}

void loop() {
  int values[N_SENSORS+2];

  for (int i = 0; i < N_SENSORS; ++i) {
    values[i] = analogRead(sensorPins[i]);
  }
  values[N_SENSORS] = 0x00;
  values[N_SENSORS+1] = 0x00;

  if (Serial.read() == 0x42) {
    Serial.write((unsigned char*)values, sizeof(values));
  }
}
