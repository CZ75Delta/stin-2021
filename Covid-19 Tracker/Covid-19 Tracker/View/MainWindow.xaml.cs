using Covid_19_Tracker.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
