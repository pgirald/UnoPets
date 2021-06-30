using MiUnoApp.Components;
using MiUnoApp.Models;
using MiUnoApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public sealed partial class CreatePetDialog : ContentDialog
    {
        private ObservableCollection<Specie> Species;

        public EventHandler<OnPetAddedEventArgs> OnPetAdded;

        public CreatePetDialog()
        {
            InitializeComponent();
        }

        private async void OnDialogOpened(object sender,ContentDialogOpenedEventArgs args)
        {
            Loader loader = new Loader();
            HttpConsumer consumer = new HttpConsumer();
            loader.Show();
            try
            {
                Species = new ObservableCollection<Specie>(await consumer.GetSpeciesAsync());
                specieCbx.Items.Clear();
                specieCbx.ItemsSource = Species;
                loader.Close();
            }
            catch
            {
                loader.Close();
                await new MessageDialog("There was a connection error", "Error").ShowAsync();
                Hide();
            }
        }

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Loader loader = new Loader();
            HttpConsumer consumer = new HttpConsumer();
            if (DataMissing)
            {
                await new MessageDialog("You must specify the pet name and its specie", "Error").ShowAsync();
                return;
            }
            Pet newPet = new Pet
            {
                Name = nameTxt.Text
                ,
                Kind = (Specie)specieCbx.SelectedItem
            };
            loader.Show();
            newPet = await consumer.AddPetAsync(newPet);
            loader.Close();
            if (newPet == null)
            {
                await new MessageDialog("The pet could not be added", "Error").ShowAsync();
                return;
            }
            OnPetAdded(this, new OnPetAddedEventArgs { AddedPet = newPet });
        }

        private bool DataMissing => string.IsNullOrEmpty(nameTxt.Text) || specieCbx.SelectedItem == null;

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        public class OnPetAddedEventArgs : EventArgs
        {
            public Pet AddedPet { get; set; }
        }
    }
}
