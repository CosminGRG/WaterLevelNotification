using System;
using System.IO.Ports;
using System.Text;
using WaterLevelNotification.NotificationHandler;
using static WaterLevelNotification.NotificationHandler.NotificationManager;

namespace SerialCommunication
{
	public class SerialPortConnection
	{
		public SerialPort SerialPort;

		private NotificationManager notificationManager;
		private MessageBoxHandler messageBoxHandler;

		bool bHeaderReceived;
		bool commStatus;

		public bool notificationsDisabled = false;

		public SerialPortConnection(string comPort)
		{
			this.notificationManager = new NotificationManager();
			this.messageBoxHandler = new MessageBoxHandler();

			commStatus = true;

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
					messageBoxHandler.ShowWarningBox("COM Port error", e.Message);
					commStatus = false;
				}
				catch (Exception e)
				{
					if (e is System.IO.IOException || e is System.UnauthorizedAccessException)
					{
						messageBoxHandler.ShowWarningBox("COM Port error", e.Message);
					}
					commStatus = false;
				}
			}
		}

		public bool SendCommand(byte operation, byte threshold)
		{
			bool status = false;

			byte[] buffer = new byte[] { 0x7E, operation, threshold, 0 };
			buffer[3] = checksum(buffer);
			try
			{
				SerialPort.Write(buffer, 0, 4);
				status = true;
			}
			catch (System.InvalidOperationException e)
			{
				messageBoxHandler.ShowWarningBox("Data write error", e.Message);
				status = false;
			}
			return status;
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

		public bool GetCommStatus()
		{
			return this.commStatus;
		}

		private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
		{
			var receivedData = (NotificationType_en)SerialPort.ReadByte();

			if (!notificationsDisabled)
			{
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
							notificationManager.AddNotification("Water level sensor notification", "Water level is at 1 cm!");
							break;
						case NotificationType_en.NormalOp2CmNotif:
							notificationManager.AddNotification("Water level sensor notification", "Water level is at 2 cm!");
							break;
						case NotificationType_en.NormalOp3CmNotif:
							notificationManager.AddNotification("Water level sensor notification", "Water level is at 3 cm!");
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
		public enum NotificationType_en
		{
			NormalOp0CmNotif = 1,
			NormalOp1CmNotif = 2,
			NormalOp2CmNotif = 3,
			NormalOp3CmNotif = 4,
			NormalOp4CmNotif = 5,
			NormalOpNotif = 10,
			AboveThresholdOpNotif = 20,
			UnderThresholdOpNotif = 30
		}
	}
}
