using Presentationslager.Command;
using Servicelager;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace Presentationslager.ViewModel
{
    public class StatistikViewModel : INotifyPropertyChanged
    {
        private readonly StatistikController _statistikController;

        public ICommand VisaStatistikCommand { get; set; }
        public ICommand TillbakaCommand { get; set; }

        public StatistikViewModel()
        {
            _statistikController = new StatistikController();

            VisaStatistikCommand = new RelayCommand(VisaStatistik);
            TillbakaCommand = new RelayCommand(TillbakaTillMeny);
        }

        private DateTime? _startDatum;
        public DateTime? StartDatum
        {
            get { return _startDatum; }
            set
            {
                _startDatum = value;
                OnPropertyChanged(nameof(StartDatum));
            }
        }

        private DateTime? _slutDatum;
        public DateTime? SlutDatum
        {
            get { return _slutDatum; }
            set
            {
                _slutDatum = value;
                OnPropertyChanged(nameof(SlutDatum));
            }
        }

        private StatistikResultat? _resultat;
        public StatistikResultat? Resultat
        {
            get { return _resultat; }
            set
            {
                _resultat = value;
                OnPropertyChanged(nameof(Resultat));
            }
        }

        private void VisaStatistik(object obj)
        {
            // Ett ogiltigt datumintervall ska inte accepteras
            if (!_statistikController.ÄrGiltigtIntervall(StartDatum, SlutDatum, out string felmeddelande))
            {
                Resultat = null;
                MessageBox.Show(felmeddelande, "Ogiltigt datumintervall",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                Resultat = _statistikController.HämtaStatistikMedJämförelse(StartDatum.Value, SlutDatum.Value);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kunde inte hämta statistik:\n" + (ex.InnerException?.Message ?? ex.Message));
            }
        }

        private void TillbakaTillMeny(object obj)
        {
            Meny menyFonster = new Meny();
            menyFonster.Show();

            if (obj is Window nuvarandeFonster)
            {
                nuvarandeFonster.Close();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}