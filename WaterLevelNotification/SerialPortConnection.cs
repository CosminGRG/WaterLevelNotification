using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WaterLevelNotification.NotificationHandler;
using Windows.Devices.SerialCommunication;
using static WaterLevelNotification.NotificationHandler.NotificationManager;

namespace SerialCommunication
{
    public class SerialPortConnection
    {
        public SerialPort SerialPort;

        private NotificationManager notificationManager;

        bool bHeaderReceived;

        public SerialPortConnection(string comPort)
        {
            this.notificationManager = new NotificationManager();

            SerialPort = new SerialPort(comPort);
            SerialPort.BaudRate = 9600;
            SerialPort.Parity = Parity.None;
            SerialPort.StopBits = StopBits.One;
            SerialPort.DataBits = 8;
            SerialPort.Handshake = Handshake.None;
            SerialPort.Encoding = ASCIIEncoding.ASCII;

            SerialPort.DataReceived += new SerialDataReceivedEventHandler(OnDataReceived);

            if (!SerialPort.IsOpen)
            {
                try
                {
                    SerialPort.Open();
                }
                catch (System.IO.IOException e)
                {
                    /* TODO: Do something here */
                }
            }
        }

        public void SendCommand()
        {
            byte[] buffer = new byte[] { 0x7E, 20, 3, 0 };
            buffer[3] = checksum(buffer);
            SerialPort.Write(buffer, 0, 4);
        }

        public byte checksum(byte[] buffer)
        {
            byte result = 0;
            byte sum = 0;

            for (int i = 0; i < 3; i++)
            {
                sum += buffer[i];

            }
            result = (byte)(sum & 0xFF);

            return result;
        }
        private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //var receivedData1 = SerialPort.ReadByte();
            var receivedData = (NotificationType_en)SerialPort.ReadByte();

            if (!bHeaderReceived)
			{
                switch (receivedData)
                {
                    case NotificationType_en.NormalOpNotif:
                        bHeaderReceived = true;
                        break;
                    case NotificationType_en.AboveThresholdOpNotif:
                        notificationManager.AddNotification("Water level sensor notification", "Water level is above set threshold!");
                        break;
                    case NotificationType_en.UnderThresholdOpNotif:
                        notificationManager.AddNotification("Water level sensor notification", "Water level is under set threshold!");
                        break;
                    default:
                        break;
                }
            }
            else
			{
                bHeaderReceived = false;
                switch (receivedData)
				{
                    case NotificationType_en.NormalOp0CmNotif:
                        notificationManager.AddNotification("Water level sensor notification", "Water level just started rising!");
                        break;
                    case NotificationType_en.NormalOp1CmNotif:
                        notificationManager.AddNotification("Water level sensor notification", "Water level just reached 1 cm!");
                        break;
                    case NotificationType_en.NormalOp2CmNotif:
                        notificationManager.AddNotification("Water level sensor notification", "Water level just reached 2 cm!");
                        break;
                    case NotificationType_en.NormalOp3CmNotif:
                        notificationManager.AddNotification("Water level sensor notification", "Water level just reached 3 cm!");
                        break;
                    case NotificationType_en.NormalOp4CmNotif:
                        notificationManager.AddNotification("Water level sensor notification", "Water level sensor is exceeding maximum readable value!");
                        break;
                    default:
                        break;
				}
			}
        }
    }
}
