#define sensorPwrPin  2
#define sensorReadPin A0
#define ledPin        3

#define sensor0CmSubmerged 0
#define sensor1CmSubmerged 220
#define sensor2CmSubmerged 300
#define sensor3CmSubmerged 355
#define sensor4CmSubmerged 390

#define NORMAL_OP          10
#define ABOVE_THRESHOLD_OP 20
#define UNDER_THRESHOLD_OP 30

uint16_t val;
uint16_t analogValue;
uint16_t analogThresholdTable[5] = { sensor0CmSubmerged, sensor1CmSubmerged, sensor2CmSubmerged, sensor3CmSubmerged, sensor4CmSubmerged };
uint8_t operationMode = 10;
uint8_t threshold = 2;

uint16_t currentMillis = 0;
uint16_t sendTimeMillis = 0;
uint16_t notificationSendDelay = 10000;

const uint8_t header = 0x7E;
const uint8_t bufferSize = 4;

uint8_t buffer[bufferSize];
uint8_t readCounter;
uint8_t isHeader;
uint8_t firstTimeHeader;
uint8_t ok;

uint16_t readSensor();
void normaloperation(uint8_t analogValue);
uint8_t verifyChecksum(uint8_t originalResult);
bool inRange(uint16_t low, uint16_t high, uint16_t number);
void aboveThresholdOp(uint8_t analogValue, uint8_t thresholdIndex);
void underThresholdOp(uint8_t analogValue, uint8_t thresholdIndex);

void setup()
{
  // Set sensor power pin as output
  pinMode(sensorPwrPin, OUTPUT);
  pinMode(ledPin, OUTPUT);
  
  // Set to LOW so no power flows through the sensor
  digitalWrite(sensorPwrPin, LOW);
  digitalWrite(ledPin, LOW);

  readCounter = 0;
  isHeader = 0;
  firstTimeHeader = 0;

  while(!Serial);
  Serial.begin(9600);
}   

void loop()
{
  currentMillis = millis();
  
  //Check if there is any data available to read
  if(Serial.available())
  {
    //read only one byte at a time
    uint8_t c = Serial.read();
    
    //Check if header is found
    if(c == header)
    {
      if(!firstTimeHeader)
      {
        isHeader = 1;
        readCounter = 0;
        firstTimeHeader = 1;
      }
    }
    
    //store received byte, increase readCounter
    buffer[readCounter] = c;
    
    readCounter++;
   
    //prior overflow, we have to restart readCounter
    if(readCounter >= bufferSize){
      readCounter = 0;
      
      //if header was found
      if(isHeader)
      {       
        //get checksum value from buffer's last value, according to defined protocol
        uint8_t checksumValue = buffer[3];
        
        //perform checksum validation, it's optional but really suggested
        if(verifyChecksum(checksumValue))
        {        
          digitalWrite(ledPin, HIGH);
          ok = 1;
        }
        else { ok = 0; }
        //restart header flag
        isHeader = 0;
        firstTimeHeader = 0;
      }
    }
  }
  if (ok)
  {
    operationMode = buffer[1];
    if ((operationMode == ABOVE_THRESHOLD_OP) ||
        (operationMode == UNDER_THRESHOLD_OP))
    {
      threshold = buffer[2];
    }
  }

  if ((currentMillis - sendTimeMillis) > notificationSendDelay)
  {
    sendTimeMillis += notificationSendDelay;
     
    switch(operationMode)
    {
      case NORMAL_OP:
        normalOperation(analogValue);
        break;
      case ABOVE_THRESHOLD_OP:
        aboveThresholdOp(analogValue, threshold);
        break;
      case UNDER_THRESHOLD_OP:
        underThresholdOp(analogValue, threshold);
        break;
    }
  }
  
  analogValue = readSensor();

  delay(400);
}

void normalOperation(uint16_t analogValue)
{
  uint8_t normalOpSendBuffer[2] = { 10, 0 };
  
  if ((analogValue > sensor0CmSubmerged) &&
      (analogValue < sensor1CmSubmerged))
  {
    normalOpSendBuffer[1] = 1;
    Serial.write(normalOpSendBuffer, 2);
    sendTimeMillis = millis();
  }
  else if ((analogValue > sensor1CmSubmerged) &&
      (analogValue < sensor2CmSubmerged))
  {
    normalOpSendBuffer[1] = 2;
    Serial.write(normalOpSendBuffer, 2);
    sendTimeMillis = millis();
  }
  else if ((analogValue > sensor2CmSubmerged) &&
      (analogValue < sensor3CmSubmerged))
  {
    normalOpSendBuffer[1] = 3;
    Serial.write(normalOpSendBuffer, 2);
    sendTimeMillis = millis();
  }
  else if ((analogValue > sensor3CmSubmerged) &&
      (analogValue < sensor4CmSubmerged))
  {
    normalOpSendBuffer[1] = 4;
    Serial.write(normalOpSendBuffer, 2);
    sendTimeMillis = millis();
  }
  else if (analogValue > sensor4CmSubmerged)
  {
    normalOpSendBuffer[1] = 5;
    Serial.write(normalOpSendBuffer, 2);
    sendTimeMillis = millis();
  }
}

void aboveThresholdOp(uint16_t analogValue, uint8_t thresholdIndex)
{
  if (analogValue > analogThresholdTable[thresholdIndex])
  {
    Serial.write(20);
  }
}

void underThresholdOp(uint16_t analogValue, uint8_t thresholdIndex)
{
  if (analogValue < analogThresholdTable[thresholdIndex])
  {
    Serial.write(30);
  }
}

uint16_t readSensor() {
  digitalWrite(sensorPwrPin, HIGH); // Turn the sensor ON
  delay(10);                        // wait 10 milliseconds
  val = analogRead(sensorReadPin);  // Read the analog value form sensor
  digitalWrite(sensorPwrPin, LOW);  // Turn the sensor OFF
  return val;                       // send current reading
}

uint8_t verifyChecksum(uint8_t originalResult)
{
  uint8_t result = 0;
  uint16_t sum = 0;
  
  for(uint8_t i = 0; i < (bufferSize - 1); i++)
  {
    sum += buffer[i];
  }
  result = sum & 0xFF;
  
  if(originalResult == result){
     return 1;
  }else{
     return 0;
  }
}
