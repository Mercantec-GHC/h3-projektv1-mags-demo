#include "SensorData.h"

SensorData::SensorData(const char* ssid, const char* password) 
  : ssid(ssid), password(password) {}

void SensorData::begin() {
  carrier.noCase();
  carrier.begin();
  Serial.begin(9600); 
  connectWiFi();
  httpClient = new HttpClient(wifiClient, "h3-projekt2024.onrender.com", 443); 
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
  carrier.display.fillScreen(0x0000);
  Serial.println("");
  Serial.println("WiFi connected.");
  Serial.println("IP address: ");
  Serial.println(WiFi.localIP());
}

void SensorData::readSensors() {
  temperature = carrier.Env.readTemperature();
  humidity = carrier.Env.readHumidity();
  gasResistor = carrier.AirQuality.readGasResistor();
  volatileOrganicCompounds = carrier.AirQuality.readVOC();
  co2 = carrier.AirQuality.readCO2();
}

void SensorData::printData() {
  carrier.display.fillScreen(0x0000);
  Serial.print("Temperature: ");
  Serial.println(temperature);
  Serial.print("Humidity: ");
  Serial.println(humidity);
  Serial.print("Gas Resistor: ");
  Serial.println(gasResistor);
  Serial.print("VOC: ");
  Serial.println(volatileOrganicCompounds);
  Serial.print("CO2: ");
  Serial.println(co2);

  carrier.display.setTextSize(2);
  carrier.display.setCursor(30, 80); 
  carrier.display.print("Temp: ");
  carrier.display.print(temperature);
  carrier.display.setCursor(30, 100); 
  carrier.display.print("Humidity: ");
  carrier.display.print(humidity);
  carrier.display.setCursor(30, 120); 
  carrier.display.print("Gas: ");
  carrier.display.print(gasResistor);
  carrier.display.setCursor(30, 140); 
  carrier.display.print("VOC: ");
  carrier.display.print(volatileOrganicCompounds);
  carrier.display.setCursor(30, 160); 
  carrier.display.print("CO2: ");
  carrier.display.print(co2);

  sendData();
}

void SensorData::sendData() {
  String postData = "{\"deviceId\":\"10aed77d607b4428b05135cd9629d70f\",";
  postData += "\"temperature\":" + String(temperature) + ",";
  postData += "\"humidity\":" + String(humidity) + ",";
  postData += "\"gasResistor\":" + String(gasResistor) + ",";
  postData += "\"volatileOrganicCompounds\":" + String(volatileOrganicCompounds) + ",";
  postData += "\"cO2\":" + String(co2) + "}";

  httpClient->beginRequest();
  httpClient->post("/api/DeviceDatas");
  httpClient->sendHeader("Content-Type", "application/json");
  httpClient->sendHeader("Content-Length", postData.length());
  httpClient->sendHeader("accept", "text/plain");
  httpClient->beginBody();
  httpClient->print(postData);
  httpClient->endRequest();

  int statusCode = httpClient->responseStatusCode();
  String response = httpClient->responseBody();

  Serial.print("Status code: ");
  Serial.println(statusCode);
  Serial.print("Response: ");
  Serial.println(response);
}