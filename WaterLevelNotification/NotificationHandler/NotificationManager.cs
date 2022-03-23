using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Windows.Threading;
using WaterLevelNotificationButWpf;

namespace WaterLevelNotification.NotificationHandler
{
	class NotificationManager
	{
		public static readonly NotificationManager instance = new NotificationManager();

		DispatcherTimer dispatcherTimer = new DispatcherTimer();
		bool isSnoozed = false;

		public static NotificationManager GetInstance()
		{
			return instance;
		}

		public NotificationManager()
		{
			dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);

			ToastNotificationManagerCompat.OnActivated += toastArgs =>
			{
				CheckInput(toastArgs);
			};
		}

		public void AddNotification(string Title, string Content)
		{
			if (!isSnoozed)
			{
				ToastButton dismissToastButton = new ToastButton("Dismiss", "argument");
				dismissToastButton.SetDismissActivation();
				ToastButton snoozeToastButton = new ToastButton("Snooze", "snoozeNotifications");

				var logoUri = "file:///" + System.IO.Path.GetFullPath("Images/logo.png");

				new ToastContentBuilder()
					.AddArgument("action", "viewConversation")
					.AddArgument("conversationId", 9813)
					.AddButton(dismissToastButton)
					.AddButton(snoozeToastButton)
					.AddAppLogoOverride(new Uri(logoUri), ToastGenericAppLogoCrop.Circle)
					.AddText(Title)
					.AddText(Content)
					.Show();
			}
		}

		private void CheckInput(ToastNotificationActivatedEventArgsCompat e)
		{
			if (e.Argument == "snoozeNotifications")
			{
				isSnoozed = true;

				if (!dispatcherTimer.IsEnabled)
				{
					dispatcherTimer.Interval = new TimeSpan(0, 0, 20);
					dispatcherTimer.Start();
				}
			}
		}

		private void dispatcherTimer_Tick(object sender, EventArgs e)
		{
			isSnoozed = false;

			dispatcherTimer.Stop();
		}
	}
}
