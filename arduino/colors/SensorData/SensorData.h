#ifndef SENSORDATA_H
#define SENSORDATA_H

#include <Arduino_MKRIoTCarrier.h>
#include <SPI.h>
#include <WiFiNINA.h>
#include <ArduinoHttpClient.h>
#include <SD.h>

class SensorData {
  private:
    MKRIoTCarrier carrier;
    float temperature;
    float humidity;
    float gasResistor;
    float volatileOrganicCompounds;
    float co2;
    HttpClient* httpClient;
    WiFiSSLClient wifiClient;
    WiFiServer server;
    bool configMode;
    void loadWiFiCredentials();
    void saveWiFiCredentials(const String& ssid, const String& pass);
    const char* CONFIG_FILE = "/wifi_config.txt";  
    
  public:
    SensorData();
    void begin();
    void connectWiFi();
    void handleConfig();
    void readSensors();
    void printData();
    void sendData();
};

#endif // SENSORDATA_H
