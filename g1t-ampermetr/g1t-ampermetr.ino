
const int analogIn = A0;
int mVperAmp = 100; // use 185 for 5A Module, 100 for 20A Module, 66 for 30A Module
int RawValue= 0;
int ACSoffset = 2500; 
double Voltage = 0;
double Amps = 0;

void setup(){ 
 Serial.begin(9600);
}

void loop(){
 
 RawValue = analogRead(analogIn);
 Voltage = (RawValue / 1024.0) * 5000; // Gets you mV
 Amps = ((Voltage - ACSoffset) / mVperAmp);
 //Serial.print("Raw Value = " );
 //Serial.print(RawValue);
 //Serial.print("\t mV = "); 
 //Serial.print(Voltage);
 //Serial.print("\t Amps = ");
 Serial.println(Amps,2);
 delay(100); 
 
}
