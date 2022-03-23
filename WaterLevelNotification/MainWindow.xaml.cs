using SerialCommunication;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using WaterLevelNotification.NotificationHandler;
using WinForms = System.Windows.Forms;

namespace WaterLevelNotificationButWpf
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		const string commPort = "COM3";
		public SerialPortConnection SerialPortConnection;

		System.Windows.Forms.NotifyIcon notifyIcon = new System.Windows.Forms.NotifyIcon();

		DispatcherTimer dispatcherTimer = new DispatcherTimer();

		public MainWindow()
		{
			InitializeComponent();

			notifyIcon.Icon = new System.Drawing.Icon("Images/logo.ico");
			notifyIcon.Visible = true;
			notifyIcon.Text = "Water Level Notification";
			notifyIcon.DoubleClick +=
				delegate (object sender, EventArgs args)
				{
					this.Show();
					this.WindowState = WindowState.Normal;
				};
			notifyIcon.MouseDown +=
				new WinForms.MouseEventHandler(notifier_MouseDown);
			notifyIcon.BalloonTipClosed +=
				(sender, e) =>
				{
					var thisIcon = (System.Windows.Forms.NotifyIcon)sender;
					thisIcon.Visible = false;
					thisIcon.Dispose();
				};

			dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);

			OperationComboBox.SelectionChanged += new SelectionChangedEventHandler(OperationComboBox_SelectionChanged);

			if (OperationComboBox.SelectedIndex == 0)
			{
				ThresholdComboBox.IsEnabled = false;
			}
		}

		protected override void OnStateChanged(EventArgs e)
		{
			base.OnStateChanged(e);
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			e.Cancel = true;

			this.Hide();
			base.OnClosing(e);
		}

		void notifier_MouseDown(object sender, WinForms.MouseEventArgs e)
		{
			if (e.Button == WinForms.MouseButtons.Right)
			{
				ContextMenu menu = (ContextMenu)this.FindResource("NotifierContextMenu");

				menu.IsOpen = true;
			}
		}

		private void Menu_Open(object sender, RoutedEventArgs e)
		{
			this.Show();
		}

		private void Menu_Close(object sender, RoutedEventArgs e)
		{
			notifyIcon.Visible = false;
			Application.Current.Shutdown();
		}

		protected override void OnContentRendered(EventArgs e)
		{
			SerialPortConnection = new SerialPortConnection(commPort);

			if (SerialPortConnection.GetCommStatus() == true)
			{
				commStatus.Text = "Active";
				commStatus.Foreground = Brushes.Green;
			}
			else
			{
				commStatus.Text = "Inactive";
				commStatus.Foreground = Brushes.Red;
			}

			base.OnContentRendered(e);
		}

		private void ApplyButton_Click(object sender, RoutedEventArgs e)
		{
			String operationMode = OperationComboBox.SelectedValue.ToString();
			String threshold = ThresholdComboBox.SelectedValue.ToString();

			byte operationModeInt = (byte)Int32.Parse(operationMode);
			byte thresholdInt = (byte)Int32.Parse(threshold);

			bool status = SerialPortConnection.SendCommand(operationModeInt, thresholdInt);
			if (status == true)
			{
				msgStatus.Foreground = Brushes.Green;
				msgStatus.Text = "Sent";
			}
			else
			{
				msgStatus.Foreground = Brushes.Red;
				msgStatus.Text = "COM error.";
			}
		}

		private void SnoozeButton_Click(object sender, RoutedEventArgs e)
		{
			String snoozeTime = SnoozeComboBox.SelectedValue.ToString();

			SerialPortConnection.notificationsDisabled = true;
			notificationStatus.Text = "Disabled";
			notificationStatus.Foreground = Brushes.Red;
			
			int snoozeTimeInt = Int32.Parse(snoozeTime);
			if (!dispatcherTimer.IsEnabled)
			{
				dispatcherTimer.Interval = new TimeSpan(0, 0, snoozeTimeInt);
				dispatcherTimer.Start();
			}
		}

		private void dispatcherTimer_Tick(object sender, EventArgs e)
		{
			SerialPortConnection.notificationsDisabled = false;
			notificationStatus.Text = "Active";
			notificationStatus.Foreground = Brushes.Green;

			dispatcherTimer.Stop();
		}

		private void OperationComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (ThresholdComboBox != null)
			{
				if (OperationComboBox.SelectedIndex == 0)
				{
					ThresholdComboBox.IsEnabled = false;
				}
				else
				{
					ThresholdComboBox.IsEnabled = true;
				}
			}
		}
	}
}
