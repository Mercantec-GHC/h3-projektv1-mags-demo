#include "SensorData.h"

SensorData sensorData;

void setup() {
  sensorData.begin();
}

void loop() {
  sensorData.readSensors();
  sensorData.printAndSendData();
}