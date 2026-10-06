using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Entitetslager;
using Servicelager;
using Presentationslager.Command;

namespace Presentationslager.ViewModel
{
    public class UtrustningViewModel : INotifyPropertyChanged
    {
        private readonly UtrustningController _utrustningController;

        private ObservableCollection<Entitetslager.Utrustning> _utrustningsLista;
        public ObservableCollection<Entitetslager.Utrustning> UtrustningsLista
        {
            get => _utrustningsLista;
            set { _utrustningsLista = value; OnPropertyChanged(); }
        }

        private Entitetslager.Utrustning _valdUtrustning;
        public Entitetslager.Utrustning ValdUtrustning
        {
            get => _valdUtrustning;
            set { _valdUtrustning = value; OnPropertyChanged(); }
        }

        private string _sokUtrustningNummer;
        public string SokUtrustningNummer
        {
            get => _sokUtrustningNummer;
            set { _sokUtrustningNummer = value; OnPropertyChanged(); }
        }

        private string _sokStatus;
        public string SokStatus
        {
            get => _sokStatus;
            set { _sokStatus = value; OnPropertyChanged(); }
        }

        private ObservableCollection<string> _artikelTyper;
        public ObservableCollection<string> ArtikelTyper
        {
            get => _artikelTyper;
            set { _artikelTyper = value; OnPropertyChanged(); }
        }

        private string _valdArtikelTyp;
        public string ValdArtikelTyp
        {
            get => _valdArtikelTyp;
            set { _valdArtikelTyp = value; OnPropertyChanged(); }
        }


        public ICommand SokCommand { get; }
        public ICommand RensaCommand { get; }
        public ICommand UppdateraStatusCommand { get; }
        public ICommand TillbakaCommand { get; set; }

        public UtrustningViewModel()
        {
            _utrustningController = new UtrustningController();

            UtrustningsLista = new ObservableCollection<Entitetslager.Utrustning>();
            ArtikelTyper = new ObservableCollection<string>();

            SokCommand = new RelayCommand(UtforSokning);
            RensaCommand = new RelayCommand(RensaFalt);
            UppdateraStatusCommand = new RelayCommand(UppdateraStatus, CanUppdateraStatus);
            TillbakaCommand = new RelayCommand(TillbakaTillMeny);
            LaddaData();
        }

        private void LaddaData()
        {
            var utrustningar = _utrustningController.HamtaAllaUtrustningar();
            UtrustningsLista.Clear();
            if (utrustningar != null)
            {
                foreach (Entitetslager.Utrustning item in utrustningar)
                {
                    UtrustningsLista.Add(item);
                }
            }

            var typer = _utrustningController.HamtaAllaArtikelTyper();
            ArtikelTyper.Clear();
            if (typer != null)
            {
                
                var filtreradeTyper = typer
                    .Where(t => t != null && !string.IsNullOrWhiteSpace(t.TypNamn))
                    .Select(t => t.TypNamn)
                    .Where(namn =>
                        !namn.StartsWith("Paket", StringComparison.OrdinalIgnoreCase) &&
                        !namn.EndsWith("paket", StringComparison.OrdinalIgnoreCase) &&
                        !namn.Contains("LGH.I", StringComparison.OrdinalIgnoreCase) &&
                        !namn.Contains("Camp", StringComparison.OrdinalIgnoreCase) &&
                        !namn.Contains("LGH.II", StringComparison.OrdinalIgnoreCase) && 
                        !namn.Contains("Konferens", StringComparison.OrdinalIgnoreCase) &&
                        !namn.Contains("Skidlektion", StringComparison.OrdinalIgnoreCase) &&
                        !namn.Contains("Lektion", StringComparison.OrdinalIgnoreCase));

                foreach (var typ in filtreradeTyper)
                {
                    ArtikelTyper.Add(typ);
                }
            }
        }

        private void UtforSokning(object obj)
        {
            var resultat = _utrustningController.SokUtrustning(
                SokUtrustningNummer,
                SokStatus,
                ValdArtikelTyp
            );

            UtrustningsLista.Clear();
            if (resultat != null)
            {
                foreach (Entitetslager.Utrustning item in resultat)
                {
                    UtrustningsLista.Add(item);
                }
            }
        }

        private void RensaFalt(object obj)
        {
            SokUtrustningNummer = string.Empty;
            SokStatus = string.Empty;
            ValdArtikelTyp = null;

            LaddaData();
        }

        private bool CanUppdateraStatus(object obj)
        {
            return ValdUtrustning != null;
        }

        private void UppdateraStatus(object obj)
        {
            if (ValdUtrustning == null) return;

            string nyStatus = ValdUtrustning.Status == "Tillgänglig" ? "Ej tillgänglig" : "Tillgänglig";

            bool lyckades = _utrustningController.UppdateraUtrustningsStatus(ValdUtrustning.UtrustningNummer, nyStatus);

            if (lyckades)
            {
                ValdUtrustning.Status = nyStatus;

                int index = UtrustningsLista.IndexOf(ValdUtrustning);
                if (index >= 0)
                {
                    UtrustningsLista[index] = ValdUtrustning;
                }

                MessageBox.Show($"Status för utrustning {ValdUtrustning.UtrustningNummer} uppdaterades till: {nyStatus}",
                                "Status uppdaterad", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Det gick inte att uppdatera statusen för den valda utrustningen.",
                                "Fel", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void TillbakaTillMeny(object obj)
        {
            Meny menyFonster = new Meny();
            menyFonster.Show();

            // Stäng det nuvarande fönstret
            if (obj is Window nuvarandeFonster)
            {
                nuvarandeFonster.Close();
            }
        }
    }
}