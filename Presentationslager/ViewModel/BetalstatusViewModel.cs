using Presentationslager.Command;
using Servicelager;
using Entitetslager; 
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Presentationslager.ViewModel
{
    public class BetalstatusViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly FakturaController fakturaController;

        public ICommand TillbakaCommand { get; set; }
        public ICommand SokCommand { get; set; }
        public ICommand RensaCommand { get; set; }

        public ICommand UppdateraStatusCommand { get; set; }

        private List<Faktura> _allaFakturor;

        private string _sokFakturaNummer;
        public string SokFakturaNummer
        {
            get { return _sokFakturaNummer; }
            set { _sokFakturaNummer = value; OnPropertyChanged(); }
        }

        public List<string> Status { get; set; }

        private string _valdStatus;
        public string ValdStatus
        {
            get { return _valdStatus; }
            set { _valdStatus = value; OnPropertyChanged(); }
        }

        private ObservableCollection<Faktura> _fakturaLista;
        public ObservableCollection<Faktura> FakturaLista
        {
            get { return _fakturaLista; }
            set { _fakturaLista = value; OnPropertyChanged(); }
        }

        private Faktura _valdFaktura;
        public Faktura ValdFaktura
        {
            get { return _valdFaktura; }
            set { _valdFaktura = value; OnPropertyChanged(); }
        }

        public BetalstatusViewModel()
        {
            fakturaController = new FakturaController();

            Status = new List<string> { "Alla", "Betald", "Obetald" };
            ValdStatus = "Alla";

            TillbakaCommand = new RelayCommand(TillbakaTillMeny);
            SokCommand = new RelayCommand(SokFakturor);
            RensaCommand = new RelayCommand(RensaSokning);
            UppdateraStatusCommand = new RelayCommand(UppdateraStatus);

            LaddaData();
        }

        private void LaddaData()
        {
            
            _allaFakturor = fakturaController.HamtaAllaFakturor();

            if (_allaFakturor != null)
            {
                FakturaLista = new ObservableCollection<Faktura>(_allaFakturor);
            }
            else
            {
                _allaFakturor = new List<Faktura>();
                FakturaLista = new ObservableCollection<Faktura>();
            }
        }

        private void SokFakturor(object obj)
        {
            var resultat = _allaFakturor.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SokFakturaNummer))
            {
                resultat = resultat.Where(f => f.FakturaNummer.ToString().Contains(SokFakturaNummer));
            }

            if (!string.IsNullOrWhiteSpace(ValdStatus) && ValdStatus != "Alla")
            {
                resultat = resultat.Where(f => f.Status == ValdStatus);
            }

            FakturaLista = new ObservableCollection<Faktura>(resultat);
        }

        private void RensaSokning(object obj)
        {
            SokFakturaNummer = string.Empty;
            ValdStatus = "Alla";

            FakturaLista = new ObservableCollection<Faktura>(_allaFakturor);
        }

        private void TillbakaTillMeny(object obj)
        {
            Meny window = new Meny();
            window.Show();

            if (obj is Window nuvarandeFonster)
            {
                nuvarandeFonster.Close();
            }
        }
        private void UppdateraStatus(object obj)
        {
            if (ValdFaktura != null)
            {
                if (ValdFaktura.Status == "Obetald")
                {
                    ValdFaktura.Status = "Betald";
                }
                else
                {
                    ValdFaktura.Status = "Obetald";
                }

                fakturaController.UppdateraFaktura(ValdFaktura);

                MessageBox.Show($"Status för faktura {ValdFaktura.FakturaNummer} har ändrats till: {ValdFaktura.Status}",
                                "Status uppdaterad",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);

                SokFakturor(null);
                
            }
            else
            {
                MessageBox.Show("Vänligen markera en faktura i listan först.",
                                "Ingen faktura vald",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}