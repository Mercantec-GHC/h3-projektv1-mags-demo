#include "SensorData.h"

SensorData sensorData("MAGS-OLC", "Merc1234!");

void setup() {
  sensorData.begin();
}

void loop() {
  sensorData.readSensors();
  sensorData.printData();
  delay(5000); 
}