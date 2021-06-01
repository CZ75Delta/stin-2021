using Covid_19_Tracker.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Covid_19_Tracker.ViewModel;

namespace Covid_19_Tracker.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private LinkedList<CountryVaccination> VaccinationQueue { get; set; }

        public MainWindow()
        {
            VaccinationQueue = new LinkedList<CountryVaccination>();
            InitializeComponent();
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EndDatePicker.Visibility = InfectedTab.IsSelected ? Visibility.Visible : Visibility.Hidden;
            EndDatePickerLabel.Visibility = InfectedTab.IsSelected ? Visibility.Visible : Visibility.Hidden;
            if (!VaccinatedTab.IsSelected) return;
            var vm = (MainViewModel)DataContext;
            vm.InfectedInitCommand.Execute(null);
        }

        private void DataGridCountries_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count <= 0) return;
            var selectedCountry = (CountryVaccination)e.AddedItems[0];
            if (selectedCountry is {IsPicked: true})
            {
                VaccinationQueue.Remove(VaccinationQueue.First(s => s == selectedCountry));
                selectedCountry.IsPicked = false;
            }
            else
            {
                if (VaccinationQueue.Count >= 4)
                {
                    var endOfQueue = VaccinationQueue.First(x => !x.Name.Equals("Czechia"));
                    endOfQueue.IsPicked = false;
                    VaccinationQueue.Remove(endOfQueue);
                }   
                if (VaccinationQueue.Count > 3) return;
                if (selectedCountry == null) return;
                selectedCountry.IsPicked = true;
                VaccinationQueue.AddLast(selectedCountry);
            }
            CountriesGrid.Items.Refresh();
            CountriesGrid.SelectedItem = null;
        }

        /// <summary>
        /// Působí jako filtr zemí jenž zobrazí odpovídající výsledky dle textu v poli
        /// Reaguje na každou změnu v poli
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tb_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var text = Tb.Text;
            
            if (string.IsNullOrEmpty(text))
            {
                CountriesGrid.Items.Filter = null;
            }
            else
            {
                var customFilter = new Predicate<object>(item => !(item is CountryVaccination country) || country.Name.ToLowerInvariant().Contains(text.ToLowerInvariant()));
                CountriesGrid.Items.Filter = customFilter;
            }
            CountriesGrid.Items.Refresh();
        }

        /// <summary>
        /// Vymaže všechen text z filtrovacího pole
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonClearTextBoxFilter(object sender, RoutedEventArgs e)
        {
            Tb.Text = "";
        }
    }
}
