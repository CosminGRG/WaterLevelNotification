using Microsoft.Toolkit.Uwp.Notifications;
using SerialCommunication;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WaterLevelNotificationButWpf
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
        const string correctPort = "COM3";
        public SerialPortConnection SerialPortConnection;

        public int Score;

        public MainWindow()
        {
            InitializeComponent();
            Score = 0;
        }

        protected override void OnContentRendered(EventArgs e)
        {
            SerialPortConnection = new SerialPortConnection(correctPort);
            base.OnContentRendered(e);
        }

        private void SendSerial_Click(object sender, RoutedEventArgs e)
        {
            SerialPortConnection.SendCommand();
        }

		private void SendNotif_Click(object sender, RoutedEventArgs e)
		{
            ToastButton toastButton = new ToastButton("ok bro ms ", "argument");

            var logoUri = "file:///" + System.IO.Path.GetFullPath("Images/logo.png");

            new ToastContentBuilder()
                .AddArgument("action", "viewConversation")
                .AddArgument("conversationId", 9813)
                .AddButton(toastButton)
                .AddAppLogoOverride(new Uri(logoUri), ToastGenericAppLogoCrop.Circle)
                .AddText("Water level sensor notification")
                .AddText("Water level is above set treshold!")
                .Show();
        }
	}
}
