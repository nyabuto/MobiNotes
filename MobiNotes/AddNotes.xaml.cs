using MobiNotes.DataModel;
using System;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace MobiNotes
{
    public sealed partial class AddNotes : Page
    {
        private bool isViewing = false;
        private Note myNote;
        public AddNotes()
        {
            this.InitializeComponent();
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            Geopoint mypoint;
            if (e.Parameter == null)
            {
                // this is when the user is adding
                isViewing = false;
                var locator = new Geolocator();
                locator.DesiredAccuracyInMeters = 50;
                var position = await locator.GetGeopositionAsync();
                mypoint = position.Coordinate.Point;

            }
            else
            {
                // when the user is viewing/ deleting
                isViewing = true;
                myNote = (Note)e.Parameter;
                lblHeading.Text = "Edit Note";
                titleTextBox.Text = myNote.Title;
                noteTextBox.Text = myNote.Content;
                // dynamic changing of teh ui controls.
                addcommandbar.Content = "Delete";
                addcommandbar.Icon = new SymbolIcon(Symbol.Delete);
                // retrieve the position where the note was stored and display it on the map control
                var myPosition = new Windows.Devices.Geolocation.BasicGeoposition();
                myPosition.Latitude = myNote.Latitude;
                myPosition.Longitude = myNote.Longitude;
                mypoint = new Geopoint(myPosition);


            }
            await MyMap.TrySetViewAsync(mypoint, 19D);
        }
        private async void addButton_Click(object sender, RoutedEventArgs e)
        {

            if (isViewing)
            {
                addcommandbar.Content = "Delete";

                // Deleting message box
                var messageDialog = new Windows.UI.Popups.MessageDialog("Do you want to delete Note?");
                messageDialog.Commands.Add(new UICommand("Delete", new UICommandInvokedHandler(this.CommandInvokedHandler)));
                messageDialog.Commands.Add(new UICommand("Cancel", new UICommandInvokedHandler(this.CommandInvokedHandler)));
                // enable the command to be invoked by default
                messageDialog.DefaultCommandIndex = 0;
                //set command to be invoked when escape is pressed.
                messageDialog.CancelCommandIndex = 1;
                await messageDialog.ShowAsync();
                // enable delete option
                App.DataModel.DeleteMapNote(myNote);
                Frame.Navigate(typeof(MainPage));
            }
            else
            {
                if (String.IsNullOrEmpty(titleTextBox.Text) || String.IsNullOrEmpty(noteTextBox.Text) ||
                    comboCategories.SelectedItem == null)
                {
                    MessageDialog msgbox = new MessageDialog("Please Fill in all the note details");
                    msgbox.Commands.Clear();
                    msgbox.Commands.Add(new UICommand { Label = "Okay!", Id = 0 });

                    var res = await msgbox.ShowAsync();
                    // enable adding option
                    if ((int)res.Id == 0)
                    {
                        Frame.Navigate(typeof(AddNotes));
                    }
                }
                else
                {
                    var user = "092149-091459-091472";
                    Note newNote = new Note();
                    newNote.Title = titleTextBox.Text;
                    newNote.Content = noteTextBox.Text;
                    newNote.DateCreated = DateTime.Now;
                    newNote.Longitude = Math.Round(MyMap.Center.Position.Longitude, 1);
                    newNote.Latitude = Math.Round(MyMap.Center.Position.Latitude, 1);
                    newNote.Category = ((ComboBoxItem)comboCategories.SelectedItem).Content.ToString();
                    newNote.User = user;

                    // Code to save the note offline once the user hits addbutton
                    App.DataModel.AddNote(newNote);
                    Frame.Navigate(typeof(MainPage));

                    // code to save the note to an online APi as provided by lecturer
                    Task<Note> task = Task.Run<Note>(async () => await Proxy.PostNote(newNote));
                    Note noteResult = task.Result;
                    if (noteResult != null)
                    {
                        Frame.Navigate(typeof(MainPage));
                    }
                    else
                    {
                        MessageDialog msgbox = new MessageDialog("Oops! There is trouble saving the note");
                        msgbox.Commands.Clear();
                        msgbox.Commands.Add(new UICommand { Label = "Okay!", Id = 0 });
                        Frame.Navigate(typeof(AddNotes));
                    }
                }
            }
            Frame.Navigate(typeof(MainPage));
        }
        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));

        }
        private void CommandInvokedHandler(IUICommand command)
        {
            if (command.Label == "Delete")
            {
                App.DataModel.DeleteMapNote(myNote);
                Frame.Navigate(typeof(MainPage));
            }
        }


    }
}
