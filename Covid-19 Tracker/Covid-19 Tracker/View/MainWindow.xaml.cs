using Covid_19_Tracker.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;

namespace Covid_19_Tracker.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Queue<CountryVaccination> VaccinationQueue { get; private set; }

        public MainWindow()
        {
            VaccinationQueue = new Queue<CountryVaccination>();            
            InitializeComponent();
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EndDatePicker.Visibility = InfectedTab.IsSelected ? Visibility.Visible : Visibility.Hidden;
            EndDatePickerLabel.Visibility = InfectedTab.IsSelected ? Visibility.Visible : Visibility.Hidden;
        }

        private void DataGridCountries_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            if (e.AddedItems.Count <= 0) return;
            var selectedCountry = (CountryVaccination)e.AddedItems[0];
            if (selectedCountry != null && selectedCountry.IsPicked)
            {
                VaccinationQueue = new Queue<CountryVaccination>(VaccinationQueue.Where(s => s != selectedCountry));
                selectedCountry.IsPicked = false;
            }
            else
            {
                if (VaccinationQueue.Count >= 4)
                {
                    var endOfQueue = VaccinationQueue.Dequeue();
                    endOfQueue.IsPicked = false;
                }
                if (VaccinationQueue.Count > 3) return;
                if (selectedCountry == null) return;
                selectedCountry.IsPicked = true;
                VaccinationQueue.Enqueue(selectedCountry);

            }
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
                var customFilter = new Predicate<object>(item => !(item is CountryVaccination country) || country.Name.Contains(text));
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
