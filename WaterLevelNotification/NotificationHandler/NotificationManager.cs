using Microsoft.Toolkit.Uwp.Notifications;
using System;
using WaterLevelNotificationButWpf;

namespace WaterLevelNotification.NotificationHandler
{
	class NotificationManager
	{
		public static readonly NotificationManager instance = new NotificationManager();

		public static NotificationManager GetInstance()
		{
			return instance;
		}

		public void AddNotification(string Title, string Content)
		{
			ToastButton dismissToastButton = new ToastButton("Dismiss", "argument");
			dismissToastButton.SetDismissActivation();
			ToastButtonSnooze snoozeToastButton = new ToastButtonSnooze();

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

			ToastNotificationManagerCompat.OnActivated += toastArgs =>
			{
				CheckInput(toastArgs);
			};
		}

		private void CheckInput(ToastNotificationActivatedEventArgsCompat e)
		{
			if (e.Argument == "snoozeNotifications")
			{

			}
		}
	}
}
