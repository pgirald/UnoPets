using MiUnoApp.Components;
using MiUnoApp.Modals;
using MiUnoApp.Models;
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
using static MiUnoApp.Modals.CreateAccount;
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace MiUnoApp.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginPage : Page
    {
        public LoginPage()
        {
            this.InitializeComponent();
        }

        private async void LoginBtnClick()
        {
            HttpConsumer consumer = new HttpConsumer();
            Loader loader = new Loader();
            loader.Show();
            try
            {
                ResponseToken currentToken = await consumer.LoginAsync(emailTxt.Text, passwordTxt.Password);
                if (currentToken != null)
                {
                    CurrentTokenSingleton.CurrentToken = currentToken;
                    Frame.Navigate(typeof(MainPage));
                }
                else
                {
                    await new MessageDialog("Email or password incorrect", "Error").ShowAsync();
                }
                loader.Close();
            }
            catch
            {
                loader.Close();
                Dialogs.NotifyException("Error", "There was a connection error");
            }

        }

        private async void CreateAccountBtnClick()
        {
            CreateAccount createAccountDialog = new CreateAccount();
            createAccountDialog.OnUserRegisterd += OnUserRegistered;
            await createAccountDialog.ShowAsync();
        }

        private void OnUserRegistered(object obj, OnUserRegisteredEventArgs args)
        {
            CurrentTokenSingleton.CurrentToken = args.CurrentToken;
            Frame.Navigate(typeof(MainPage));
            ((CreateAccount)obj).OnUserRegisterd -= OnUserRegistered;
        }
    }
}
