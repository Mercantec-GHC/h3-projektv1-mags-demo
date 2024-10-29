#include "SensorData.h"

SensorData::SensorData() : server(80), configMode(false) {}

void SensorData::begin() {
  carrier.noCase();
  carrier.begin();
  Serial.begin(9600);
  
  // Initialiser SD kort
  if (!SD.begin(SD_CS)) {
    Serial.println("SD kort initialisering fejlede!");
    return;
  }
  
  connectWiFi();
  httpClient = new HttpClient(wifiClient, "h3test.mercantec.tech", 443);
}

void SensorData::loadWiFiCredentials() {
  if (SD.exists(CONFIG_FILE)) {
    File configFile = SD.open(CONFIG_FILE, FILE_READ);
    if (configFile) {
      String ssid = configFile.readStringUntil('\n');
      String pass = configFile.readStringUntil('\n');
      configFile.close();
      
      ssid.trim();
      pass.trim();
      
      if (ssid.length() > 0 && pass.length() > 0) {
        WiFi.begin(ssid.c_str(), pass.c_str());
        
        // Vent op til 10 sekunder på forbindelse
        int attempts = 0;
        while (WiFi.status() != WL_CONNECTED && attempts < 10) {
          delay(1000);
          attempts++;
        }
      }
    }
  }
}

void SensorData::saveWiFiCredentials(const String& ssid, const String& pass) {
  File configFile = SD.open(CONFIG_FILE, FILE_WRITE);
  if (configFile) {
    configFile.println(ssid);
    configFile.println(pass);
    configFile.close();
    Serial.println("WiFi oplysninger gemt");
  } else {
    Serial.println("Fejl ved gemning af WiFi oplysninger");
  }
}

void SensorData::connectWiFi() {
  // Prøv først at indlæse og forbinde med gemte oplysninger
  loadWiFiCredentials();
  
  if (WiFi.status() != WL_CONNECTED) {
    Serial.println("Starter AP mode");
    
    carrier.display.setTextSize(2);
    carrier.display.fillScreen(0x0000);
    carrier.display.setCursor(30, 80); 
    carrier.display.print("Forbind til:");
    carrier.display.setCursor(30, 100); 
    carrier.display.print("ArduinoAP");
    
    WiFi.beginAP("ArduinoAP");
    server.begin();
    configMode = true;
    
    while (configMode) {
      handleConfig();
    }
  }
  
  carrier.display.fillScreen(0x0000);
  Serial.println("WiFi forbundet");
  Serial.println("IP adresse: ");
  Serial.println(WiFi.localIP());
}

void SensorData::handleConfig() {
  WiFiClient client = server.available();
  if (client) {
    String currentLine = "";
    while (client.connected()) {
      if (client.available()) {
        char c = client.read();
        if (c == '\n') {
          if (currentLine.length() == 0) {
            client.println("HTTP/1.1 200 OK");
            client.println("Content-type:text/html");
            client.println();
            
            // Send web page
            client.println("<html><body>");
            client.println("<h1>WiFi Setup</h1>");
            client.println("<form method='get' action='/connect'>");
            client.println("SSID: <input type='text' name='ssid'><br>");
            client.println("Password: <input type='password' name='pass'><br>");
            client.println("<input type='submit' value='Connect'>");
            client.println("</form></body></html>");
            break;
          } else {
            if (currentLine.startsWith("GET /connect")) {
              // Parse SSID og password fra URL
              int ssidStart = currentLine.indexOf("ssid=") + 5;
              int ssidEnd = currentLine.indexOf("&", ssidStart);
              int passStart = currentLine.indexOf("pass=") + 5;
              int passEnd = currentLine.indexOf(" ", passStart);
              
              String ssid = currentLine.substring(ssidStart, ssidEnd);
              String pass = currentLine.substring(passStart, passEnd);
              
              WiFi.begin(ssid.c_str(), pass.c_str());
              
              if (WiFi.status() == WL_CONNECTED) {
                // Gem oplysningerne når forbindelsen lykkes
                saveWiFiCredentials(ssid, pass);
                configMode = false;
              }
            }
          }
          currentLine = "";
        } else if (c != '\r') {
          currentLine += c;
        }
      }
    }
    client.stop();
  }
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
  Serial.println(postData);
  
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
