#include "SensorData.h"

SensorData sensorData("Fibernet-IA01021636", "hjKwucM");

void setup() {
  sensorData.begin();
}

void loop() {
  sensorData.readSensors();
  sensorData.printData();
  delay(5000); 
}