using MiUnoApp.Components;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace MiUnoApp.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class UserInfoPage : Page
    {
        public UserInfoPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            RefreshFields();
        }

        private void RefreshFields()
        {
            emailTxt.Text = CurrentTokenSingleton.CurrentUser.Email;
            phoneTxt.Text = CurrentTokenSingleton.CurrentUser.PhoneNumber;
            lastNamesTxt.Text = CurrentTokenSingleton.CurrentUser.SecondNames;
            firstNamesTxt.Text = CurrentTokenSingleton.CurrentUser.FirstNames;
            userNameTxt.Text = CurrentTokenSingleton.CurrentUser.UserName;
        }

        private async void OnUpdateBtnClick()
        {
            if (MissingData)
            {
                await new MessageDialog("You must at least specify an email and a user name", "Error").ShowAsync();
                return;
            }
            Loader loader = new Loader();
            HttpConsumer consumer = new HttpConsumer();
            UserData changedData = new UserData
            {
                Email = emailTxt.Text
                ,
                PhoneNumber = phoneTxt.Text
                ,
                SecondNames = lastNamesTxt.Text
                ,
                FirstNames = firstNamesTxt.Text
                ,
                UserName = userNameTxt.Text
            };
            loader.Show();
            try
            {
                User changedUser = await consumer.UpdateUserAsync(changedData);
                loader.Close();
                if (changedData == null)
                {
                    await new MessageDialog("The user data was not updated", "Error").ShowAsync();
                    RefreshFields();
                    return;
                }
                CurrentTokenSingleton.CurrentUser = changedUser;
                await new MessageDialog("The user data was updated", "Nice").ShowAsync();
            }
            catch
            {
                loader.Close();
                await new MessageDialog("There was a connection error", "Error").ShowAsync();
            }
        }

        private bool MissingData => string.IsNullOrEmpty(emailTxt.Text)
                || string.IsNullOrEmpty(userNameTxt.Text);
    }
}
