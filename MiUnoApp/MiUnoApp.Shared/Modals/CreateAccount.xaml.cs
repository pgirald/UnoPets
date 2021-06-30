using MiUnoApp.Components;
using MiUnoApp.Models;
using MiUnoApp.Pages;
using MiUnoApp.Services;
using MiUnoApp.Services.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace MiUnoApp.Modals
{
	public sealed partial class CreateAccount : ContentDialog
	{
		public CreateAccount()
		{
			this.InitializeComponent();
		}

		public event EventHandler<OnUserRegisteredEventArgs> OnUserRegisterd;

		private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
		{
			if (MissingData)
			{
				await new MessageDialog("You must specify at least an email, an user name and a password", "Error").ShowAsync();
				return;
			}
			if (passwordTxt.Password != passwordConfirmTxt.Password)
			{
				await new MessageDialog("The passwords you specified do not match", "Error").ShowAsync();
				return;
			}
			User newUser = new User
			{
				Email = emailTxt.Text
				,
				FirstNames = firstNamesTxt.Text
				,
				SecondNames = lastNamesTxt.Text
				,
				PhoneNumber = phoneTxt.Text
				,
				UserName = userNameTxt.Text
			};
			RegisterData data = new RegisterData { User = newUser, Password = passwordTxt.Password };
			Loader loader = new Loader();
			HttpConsumer consumer = new HttpConsumer();
			loader.Show();
			try
			{
				ResponseToken currentToken = await consumer.RegisterUserAsync(data);
				loader.Close();
				if (newUser == null)
				{
					await new MessageDialog("The user could not be regitered", "Error").ShowAsync();
					return;
				}
				OnUserRegisterd(this, new OnUserRegisteredEventArgs { CurrentToken = currentToken });
			}
			catch
			{
				loader.Close();
				await new MessageDialog("There was a connection error", "Error").ShowAsync();
			}
		}

		public class OnUserRegisteredEventArgs : EventArgs
        {
			public ResponseToken CurrentToken { get; set; }
        }

		private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
		{

		}

		private bool MissingData => string.IsNullOrEmpty(emailTxt.Text)
				|| string.IsNullOrEmpty(passwordTxt.Password)
				|| string.IsNullOrEmpty(passwordConfirmTxt.Password)
				|| string.IsNullOrEmpty(userNameTxt.Text);
	}
}
