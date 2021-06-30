using MiUnoApp.Components;
using MiUnoApp.Modals;
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
using static MiUnoApp.Modals.CreatePetDialog;
using static MiUnoApp.Modals.UpdatePetDialog;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace MiUnoApp.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PetsListPage : Page
    {
        public ObservableCollection<Pet> Pets { get; set; }
        public object AddPetDialog { get; private set; }

        private Pet selectedPet;

        public PetsListPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            HttpConsumer consumer = new HttpConsumer();
            Loader loader = new Loader();
            loader.Show();
            try
            {
                Pets = new ObservableCollection<Pet>(await consumer.GetPetsAsync(CurrentTokenSingleton.CurrentUser));
                refreshList();
                loader.Close();
            }
            catch
            {
                loader.Close();
                Dialogs.NotifyException("Error", "There was a connection error");
            }
        }

        private void refreshList()
        {
            PetsList.ItemsSource = null;
            PetsList.Items.Clear();
            PetsList.ItemsSource = Pets;
        }

        private async void OnAddPedBtnClick()
        {
            CreatePetDialog createPetDialog = new CreatePetDialog();
            createPetDialog.OnPetAdded += OnPetAdded;
            await createPetDialog.ShowAsync();
        }

        private void OnPetAdded(object sender, OnPetAddedEventArgs args)
        {
            ((CreatePetDialog)sender).OnPetAdded -= OnPetAdded;
            Pets.Add(args.AddedPet);
            refreshList();
        }

        private async void DeleteTapped(object sender, TappedRoutedEventArgs args)
        {
            bool response = await Dialogs.ConfirmAsync("Wait", "Do you want to delete " + ((Pet)PetsList.SelectedItem).Name + "?");
            if (!response)
            {
                return;
            }
            HttpConsumer consumer = new HttpConsumer();
            Loader loader = new Loader();
            loader.Show();
            try
            {
                response = await consumer.DeletePetAsync(((Pet)PetsList.SelectedItem).Id);
                loader.Close();
                if (response)
                {
                    Pets.Remove((Pet)PetsList.SelectedItem);
                    refreshList();
                    await new MessageDialog("The selected pet was successfuly deleted", "Nice").ShowAsync();
                }
                else
                {
                    await new MessageDialog("The pet could not be deleted", "Error").ShowAsync();
                }
            }
            catch
            {
                loader.Close();
                await new MessageDialog("There was a connection error", "Error").ShowAsync();
            }
        }

        private async void EditTapped(object sender, TappedRoutedEventArgs args)
        {
            selectedPet = (Pet)PetsList.SelectedItem;
            UpdatePetDialog updatePetDialog = new UpdatePetDialog((Pet)PetsList.SelectedItem);
            updatePetDialog.OnPetUpdated += OnPetUpdated;
            await updatePetDialog.ShowAsync();
        }

        private void OnPetUpdated(object sender, OnPetUpdatedEventArgs args)
        {
            ((UpdatePetDialog)sender).OnPetUpdated -= OnPetUpdated;
            selectedPet.Kind = args.updatedPet.Kind;
            selectedPet.Name = args.updatedPet.Name;
            refreshList();
        }
    }
}
