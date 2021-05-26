using Covid_19_Tracker.Model;
using System.Windows;
using System.Windows.Controls;

namespace Covid_19_Tracker.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EndDatePicker.Visibility = InfectedTab.IsSelected ? Visibility.Visible : Visibility.Hidden;
            EndDatePickerLabel.Visibility = InfectedTab.IsSelected ? Visibility.Visible : Visibility.Hidden;
        }

        private void DataGridCountries_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                int counter = DataGridPickedCountries.Items.Count;
                var selectedCountry = (CountryVaccination)e.AddedItems[0];
                if (selectedCountry.IsPicked == true)
                {
                    selectedCountry.IsPicked = false;
                    counter--;
                    if(counter == 3)
                    {
                        //enable all
                    }
                }
                else
                {
                    selectedCountry.IsPicked = true;
                    counter++;
                }

                if(counter >= 4)
                {
                    var tt = DataGridCountries.RowStyle;
                    var test = sender;
                    //disable nevybrané
                }
            }

        }
    }
}
