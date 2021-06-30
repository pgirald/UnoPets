using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

namespace MiUnoApp.Components
{
    class Dialogs
    {
        public async static void NotifyException(Exception ex)
        {
            await new MessageDialog(ex.Message, "Error").ShowAsync();
        }

        public async static void NotifyException(string title, string content)
        {
            await new MessageDialog(content, title).ShowAsync();
        }

        public async static Task<bool> ConfirmAsync(string title, string content)
        {
            ContentDialog confirmDialog = new ContentDialog
            {
                Title = title,
                Content = content,
                PrimaryButtonText = "Yes",
                CloseButtonText = "No"
            };

            return (await confirmDialog.ShowAsync()) == ContentDialogResult.Primary;
        }
    }
}
