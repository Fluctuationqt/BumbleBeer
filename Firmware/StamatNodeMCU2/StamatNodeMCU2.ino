#include <ESP8266WiFi.h>
#include <WiFiClient.h> 
#include <ESP8266WebServer.h>
#include <ESP8266HTTPClient.h>
#include <Servo.h> 

/* ############# < PIN MAP > ################# */
#define D3 0  // pitch servo pin
#define D1 5  // yaw servo pin
#define D6 12 // charge caps pin
#define D8 15 // discharge caps (fire) pin
#define D0 16 // cup sensor input pin

/* ############# < WIFI SETUP > ############## */
const char *ssid = "BeerPongNetwork"; // WiFi name
const char *password = "beerpong#"; // WiFi password
//const char getRequest[] PROGMEM = "192.168.137.1/Beerpong/public/hit?id=%ld&pulse=%s"; // hit notification request

/* ######## < SERVO STARTUP DEG > ############ */
int curPitch = 90;
int curYaw = 90;

/* ######## < LIBRARY OBJECTS > ############ */
Servo yawServo;  
Servo pitchServo;
ESP8266WebServer server(80);


// On Startup
void setup() 
{
  SetupSerial();
  WiFiConnect();
  ConfigureServer();
  ConnectWithStamat();
  Serial.println("Ready!");
}

// Main Loop
void loop() 
{
  if (WiFi.status() == WL_CONNECTED) { //Check WiFi connection status
    server.handleClient();
  } else {
    Serial.println("wifi connection error");
	  //WiFiConnect();
  }
}

// Set up serial communication
void SetupSerial()
{
  delay(1000);
  Serial.begin(115200);
  Serial.println();
}

// Connect to given(ssid, password) wifi network
void WiFiConnect()
{
  WiFi.begin(ssid, password);
  while (WiFi.status() != WL_CONNECTED) {
    delay(1000);
	Serial.println("Connecting to WiFi..");
  }
  Serial.println("WiFi connected");
  Serial.print("Robot IP address is: ");  
  Serial.println(WiFi.localIP());
  HTTPClient http;
  String setIpRequest = "http://192.168.137.1/robot_ip_set.php?ip=" + WiFi.localIP().toString();
  http.begin(setIpRequest);
  int httpCode = http.GET();
  if(httpCode > 0) {
    // HTTP header has been send and Server response header has been handled
    Serial.printf("[HTTP] GET... code: %d\n", httpCode);
        // file found at server
        if(httpCode == HTTP_CODE_OK) {
            String payload = http.getString();
            Serial.println(payload);
        }
    } else {
        Serial.printf("[HTTP] GET... failed, error: %s\n", http.errorToString(httpCode).c_str());
    }

    http.end();
}

// Set up http server with root, /MoveTo and /Fire handlers
void ConfigureServer()
{
  server.on("/", handleRoot);
  server.on("/MoveTo", handleMoveTo); // Set up Get Request with specific args at http://192.168.4.1/MoveTo?pitch=13&yaw=196
  server.on("/Fire", handleFire);     // Set up Get Request with specific args at http://192.168.4.1/Charge?power=1000
  server.begin();
  Serial.println("HTTP server started");
}

// Attach servos and set up pins for Coilgun
void ConnectWithStamat()
{
  yawServo.attach(D1); 
  pitchServo.attach(D3);
  Serial.println("Servos attached");
  pinMode(D6, OUTPUT);
  pinMode(D8, OUTPUT);
  digitalWrite(D6, LOW);
  digitalWrite(D8, LOW);
  delay(50);
  Serial.println("Shooting Mechanism attached");
  pinMode(D0, INPUT);
  Serial.println("Cup Sensors attached");
}

// Charges the capacitors for given ms
void Charge(int ms)
{
  digitalWrite(D8, LOW);
  delay(50);
  digitalWrite(D6, HIGH);   
  delay(ms);               
  digitalWrite(D6, LOW);
  delay(500);
}

// Discharges capacitors in the coil
void Fire()
{
  digitalWrite(D6, LOW);
  delay(50);
  digitalWrite(D8, HIGH);
  delay(500);
  digitalWrite(D8, LOW);
  delay(50);
  Serial.println("Fired a shot!");
}

bool ListenForHit(){
  Serial.println("Listening for hit...");
  int sensorPulse = pulseIn(D0, HIGH, 5000000); 
  if(sensorPulse != 0){ 
    Serial.println("Hit detected.");
    return true;
  }else{
    Serial.println("No hit detected.");
    return false;
  }
}

// Slowly moves Pitch servo to given degrees argument
void PitchTo(int deg)
{
    if(curPitch < deg)
    {
      for(int y = curPitch; y < deg; y++)
      {
        pitchServo.write(y);
        delay(15);
      } 
    }
    else if(curPitch > deg)
    {
      for(int y = curPitch; y > deg; y--)
      {
        pitchServo.write(y);
        delay(15);
      } 
    }
    curPitch = deg;
}

// Slowly moves Yaw servo to given degrees argument.
void YawTo(int deg)
{
    if(curYaw < deg)
    {
      for(int x = curYaw; x < deg; x++)
      {
        yawServo.write(x);
        delay(15);
      } 
    }
    else if(curYaw > deg)
    {
      for(int x = curYaw; x > deg; x--)
      {
        yawServo.write(x);
        delay(15);
      } 
    }
    curYaw = deg;
}

// Handles requests to http://192.168.4.1/ 
void handleRoot() {
  server.send(200, "text/html", "<h1>You are connected to stamat</h1>");
}

// Handles request to http://192.168.4.1/MoveTo
// with arguments ?pitch=xx&yaw=xx
void handleMoveTo()
{
  int recievedPitch = curPitch;
  int recievedYaw = curYaw;
  recievedPitch = server.arg("pitch").toInt();
  recievedYaw = server.arg("yaw").toInt();
  PitchTo(recievedPitch);
  YawTo(recievedYaw);
  String out ="";
  out += curPitch;
  out +=',';
  out += curYaw;
  server.send(200, "text/plain", out);
}

// Handles request to http://192.168.4.1/Fire
// with argument ?power=xx
// response body is 1 if hit, 0 if miss
void handleFire()
{
  int power = 0;
  power = server.arg("power").toInt();
  Charge(power);
  Fire();
  String out = "";
  if(ListenForHit()) out += "1";
  else out += "0";
  server.send(200, "text/plain", out);
}




