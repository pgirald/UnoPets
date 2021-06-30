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
	public sealed partial class UpdatePetDialog : ContentDialog
    {
        private ObservableCollection<Specie> Species;

        private Pet _selectedPet;

        public EventHandler<OnPetUpdatedEventArgs> OnPetUpdated;

        public UpdatePetDialog(Pet selectedPet)
		{
			this.InitializeComponent();
            _selectedPet = selectedPet;
		}

        private async void OnDialogOpened(object sender, ContentDialogOpenedEventArgs args)
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
            nameTxt.Text = _selectedPet.Name;
            specieCbx.SelectedItem = Species.First(s => s.Id == _selectedPet.Kind.Id);
        }

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Loader loader = new Loader();
            HttpConsumer consumer = new HttpConsumer();
            Pet changedPet = new Pet
            {
                Id = _selectedPet.Id
                ,
                Kind = (Specie)specieCbx.SelectedItem
                ,
                Name = nameTxt.Text
            };
            loader.Show();
            try
            {
                changedPet = await consumer.UpdatePetAsync(changedPet);
                changedPet.Owner = _selectedPet.Owner;
                loader.Close();
                if (changedPet == null)
                {
                    await new MessageDialog("The pet data was not updated", "Error").ShowAsync();
                    return;
                }
                await new MessageDialog("The pet data was updated", "Nice").ShowAsync();
                OnPetUpdated(this, new OnPetUpdatedEventArgs { updatedPet = changedPet });
            }
            catch
            {
                loader.Close();
                await new MessageDialog("There was a connection error", "Error").ShowAsync();
            }
        }

		private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
		{
		}

        public class OnPetUpdatedEventArgs
        {
            public Pet updatedPet { get; set; }
        }
	}
}
