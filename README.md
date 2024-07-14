# WaterLevelNotification
Windows desktop application used to receive notifications about the water level received from a water sensor installed on an Arduino board.
## Overview
WaterLevelNotification is a Windows desktop application that allows users to receive notifications about the state of water level received from a water sensor installed on an Arduino board through a SerialPort communication. The users can also set the threshold for notifications and choose one of the 3 operation modes (normal operations, above threshold, under threshold). The application is built using WPF, C# and C.


## Desktop app features
- Establish a serial connection with an arduino w/ water level sensor
- Send commands and receive data from the sensor
- Display notifications based on the water level readings
- Handle exceptions and provide feedback on communication status
- Multiple operation modes
- Adjustable threshold
- Checksum verification

## Arduino app featueres
- Pin configuration and initialization
- Serial data handling
- Sensor data reading
- Operation modes and notifications
- Checksum verification

## Technologies used
- WPF
- .NET Framework
- Serial Communication
- Arduino C/C++
- Microcontroller Hardware
