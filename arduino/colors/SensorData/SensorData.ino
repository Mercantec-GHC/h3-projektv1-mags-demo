#include "SensorData.h"

SensorData sensorData;

void setup() {
  sensorData.begin();
}

void loop() {
  sensorData.readSensors();
  sensorData.printData();
  delay(60000); 
}