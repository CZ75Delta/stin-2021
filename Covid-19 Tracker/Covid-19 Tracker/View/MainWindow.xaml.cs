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
            int counter = DataGridPickedCountries.Items.Count;
            if(counter > 2)
            {
                
            }
            if (e.AddedItems.Count > 0)
            {
                var SelectedCountry = (CountryVaccination)e.AddedItems[0];
                SelectedCountry.IsPicked = !SelectedCountry.IsPicked;
            }
        }
    }
}
