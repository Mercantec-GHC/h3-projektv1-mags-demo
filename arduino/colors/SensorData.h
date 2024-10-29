#ifndef SENSORDATA_H
#define SENSORDATA_H

#include <Arduino_MKRIoTCarrier.h>
#include <SPI.h>
#include <WiFiNINA.h>
#include <ArduinoHttpClient.h>

class SensorData {
  private:
    MKRIoTCarrier carrier;
    float temperature;
    float humidity;
    float gasResistor;
    float volatileOrganicCompounds;
    float co2;
    const char* ssid;
    const char* password;
    HttpClient* httpClient;
    WiFiSSLClient wifiClient; 

  public:
    SensorData(const char* ssid, const char* password);
    void begin();
    void connectWiFi();
    void readSensors();
    void printData();
    void sendData();
    void printAndSendData();
};

#endif // SENSORDATA_H