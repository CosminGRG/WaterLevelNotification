using System;
using System.Windows;

namespace WaterLevelNotification.NotificationHandler
{
	class MessageBoxHandler
	{
		public static readonly MessageBoxHandler instance = new MessageBoxHandler();

		public static MessageBoxHandler GetInstance()
		{
			return instance;
		}

		public void ShowErrorBox(String Caption, String Text)
		{
			MessageBoxButton button = MessageBoxButton.OK;
			MessageBoxImage icon = MessageBoxImage.Error;

			var dialog = MessageBox.Show(Text, Caption, button, icon, MessageBoxResult.OK);
			if (dialog == MessageBoxResult.OK)
			{
				Application.Current.Shutdown();
			}
		}

		public void ShowWarningBox(String Caption, String Text)
		{
			MessageBoxButton button = MessageBoxButton.OK;
			MessageBoxImage icon = MessageBoxImage.Warning;

			_ = MessageBox.Show(Text, Caption, button, icon, MessageBoxResult.OK);
		}
	}
}
