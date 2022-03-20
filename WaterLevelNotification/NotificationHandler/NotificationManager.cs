using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterLevelNotification.NotificationHandler
{
	class NotificationManager
	{
		public void AddNotification(string Title, string Content)
		{
            ToastButton toastButton = new ToastButton("ok bro ms ", "argument");

            var logoUri = "file:///" + System.IO.Path.GetFullPath("Images/logo.png");

            new ToastContentBuilder()
                .AddArgument("action", "viewConversation")
                .AddArgument("conversationId", 9813)
                .AddButton(toastButton)
                .AddAppLogoOverride(new Uri(logoUri), ToastGenericAppLogoCrop.Circle)
                .AddText(Title)
                .AddText(Content)
                .Show();
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
