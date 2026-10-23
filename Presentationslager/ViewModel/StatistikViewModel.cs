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

            DateTime förstaDennaMånad = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            StartDatum = förstaDennaMånad;
            SlutDatum = förstaDennaMånad.AddMonths(1).AddDays(-1);
            JämförStartDatum = förstaDennaMånad.AddMonths(-1);
            JämförSlutDatum = förstaDennaMånad.AddDays(-1);

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

        private DateTime? _jämförStartDatum;
        public DateTime? JämförStartDatum
        {
            get { return _jämförStartDatum; }
            set
            {
                _jämförStartDatum = value;
                OnPropertyChanged(nameof(JämförStartDatum));
            }
        }

        private DateTime? _jämförSlutDatum;
        public DateTime? JämförSlutDatum
        {
            get { return _jämförSlutDatum; }
            set
            {
                _jämförSlutDatum = value;
                OnPropertyChanged(nameof(JämförSlutDatum));
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
            if (!_statistikController.ArGiltigtIntervall(StartDatum, SlutDatum, out string felmeddelande))
            {
                Resultat = null;
                MessageBox.Show(felmeddelande, "Ogiltigt datumintervall",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!_statistikController.ArGiltigtIntervall(JämförStartDatum, JämförSlutDatum, out string jämförFel))
            {
                Resultat = null;
                MessageBox.Show("Jämförelseperiod: " + jämförFel, "Ogiltig jämförelseperiod",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                Resultat = _statistikController.HämtaStatistikMedJämförelse(
                    StartDatum.Value, SlutDatum.Value,
                    JämförStartDatum.Value, JämförSlutDatum.Value);
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