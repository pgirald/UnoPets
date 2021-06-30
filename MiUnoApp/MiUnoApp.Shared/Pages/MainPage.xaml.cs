using MiUnoApp.Components;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Navigation;
using MiUnoApp.Models;
using MiUnoApp.Services;
using static MiUnoApp.Services.CurrentTokenSingleton;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace MiUnoApp.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private bool _menuSwitch = true;

        public MainPage()
        {
            this.InitializeComponent();
            CurrentTokenSingleton.OnCurrentUserUpdated += OnCurrentUserUpdated;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            messageLbl.Text += " " + CurrentTokenSingleton.CurrentUser.Email;
            LocalFrame.Navigate(typeof(PetsListPage));
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }

        private async void OnExitImageTapped(object sender, TappedRoutedEventArgs e)
        {
            if(await Dialogs.ConfirmAsync("Wait", "Do you want to close session?"))
            {
                Loader loader=new Loader();
                HttpConsumer consumer = new HttpConsumer();
                loader.Show();
                try
                {
                    bool result = await consumer.LogOutAsync();
                    loader.Close();
                    if (!result)
                    {
                        await new MessageDialog("It was not possible to close session", "Error").ShowAsync();
                        return;
                    }
                    CurrentTokenSingleton.OnCurrentUserUpdated -= OnCurrentUserUpdated;
                    CurrentTokenSingleton.CurrentToken = null;
                    Frame.Navigate(typeof(LoginPage));
                }
                catch(Exception ex)
                {
                    loader.Close();
                    Dialogs.NotifyException(ex);
                }
            }
        }

        private void StackPanel_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (_menuSwitch)
            {
                return;
            }
            _menuSwitch = true;
            dogPnl.Background = (Brush)XamlBindingHelper.ConvertValue(typeof(Brush), "LightGray");
            userPnl.Background = (Brush)XamlBindingHelper.ConvertValue(typeof(Brush), "White");
            LocalFrame.Navigate(typeof(PetsListPage));
        }

        private void StackPane2_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (!_menuSwitch)
            {
                return;
            }
            _menuSwitch = false;
            dogPnl.Background = (Brush)XamlBindingHelper.ConvertValue(typeof(Brush), "White");
            userPnl.Background = (Brush)XamlBindingHelper.ConvertValue(typeof(Brush), "LightGray");
            LocalFrame.Navigate(typeof(UserInfoPage));
        }

        private void OnCurrentUserUpdated(object sender, OnCurrentUserUpdatedEventArgs args) 
        {
            messageLbl.Text = "Welcome dear " + args.CurrentUser.Email;
        }
    }
}
