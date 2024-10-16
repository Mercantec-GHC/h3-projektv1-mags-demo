#ifndef SENSORDATA_H
#define SENSORDATA_H

#include <Arduino_MKRIoTCarrier.h>
#include <SPI.h>
#include <WiFiNINA.h>

class SensorData {
  private:
    MKRIoTCarrier carrier;
    float temperature;
    float humidity;
    const char* ssid;
    const char* password;

  public:
    SensorData(const char* ssid, const char* password);
    void begin();
    void connectWiFi();
    void readSensors();
    void printData();
};

#endif // SENSORDATA_H