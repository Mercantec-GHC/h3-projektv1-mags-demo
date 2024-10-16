#include "SensorData.h"

SensorData::SensorData(const char* ssid, const char* password) 
  : ssid(ssid), password(password) {}

void SensorData::begin() {
  carrier.noCase();
  carrier.begin();
  Serial.begin(9600); 
  connectWiFi();
}

void SensorData::connectWiFi() {
  Serial.print("Connecting to ");
  Serial.println(ssid);

  carrier.display.setTextSize(2);
  carrier.display.setCursor(30, 80); 
  carrier.display.print("Connecting to ");
  carrier.display.setCursor(30, 100); 
  carrier.display.print(ssid);

  while (WiFi.begin(ssid, password) != WL_CONNECTED) {
    delay(1000);
    Serial.print(".");
  }

  Serial.println("");
  Serial.println("WiFi connected.");
  Serial.println("IP address: ");
  Serial.println(WiFi.localIP());

  carrier.display.fillScreen(0x07E0);
  carrier.display.setCursor(30, 80); 
  carrier.display.print("WiFi connected.");
  carrier.display.setCursor(30, 100); 
  carrier.display.print("IP: ");
  carrier.display.print(WiFi.localIP());
}

void SensorData::readSensors() {
  temperature = carrier.Env.readTemperature();
  humidity = carrier.Env.readHumidity();
}

void SensorData::printData() {
  Serial.print("Temperature: ");
  Serial.println(temperature);
  Serial.print("Humidity: ");
  Serial.println(humidity);

  carrier.display.fillScreen(0x07E0);
  carrier.display.setTextSize(2);
  carrier.display.setCursor(30, 80); 
  carrier.display.print("Temp: ");
  carrier.display.print(temperature);
  carrier.display.setCursor(30, 100); 
  carrier.display.print("Humidity: ");
  carrier.display.print(humidity);
}
