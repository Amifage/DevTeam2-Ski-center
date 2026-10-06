using Presentationslager.Command;
using Servicelager;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Presentationslager.ViewModel
{
    public class SkidlektionViewModel : INotifyPropertyChanged
    {
        private readonly SkidlektionController _controller = new SkidlektionController();
        private List<Entitetslager.Skidlektion> _alla = new List<Entitetslager.Skidlektion>();

        public ObservableCollection<Entitetslager.Skidlektion> VisadeLektioner { get; } = new ObservableCollection<Entitetslager.Skidlektion>();

        private string _söktAnställningsNummer = "";
        public string SöktAnställningsNummer
        {
            get { return _söktAnställningsNummer; }
            set { _söktAnställningsNummer = value; OnPropertyChanged(); }
        }

        public ICommand SökCommand { get; }
        public ICommand VisaAllaCommand { get; }
        public ICommand StängCommand { get; }

        public SkidlektionViewModel()
        {
            SökCommand = new RelayCommand(Sök);
            VisaAllaCommand = new RelayCommand(VisaAlla);
            StängCommand = new RelayCommand(Stäng);

            try
            {
                _alla = _controller.HämtaAllaSkidlektioner().ToList();
                VisaAlla(null);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kunde inte hämta skidlektioner:\n" + (ex.InnerException?.Message ?? ex.Message));
            }
        }

        private void VisaAlla(object? obj)
        {
            VisadeLektioner.Clear();
            foreach (var l in _alla) VisadeLektioner.Add(l);
        }

        private void Sök(object? obj)
        {
            if (!int.TryParse(SöktAnställningsNummer, out int nr))
            {
                VisaAlla(null);
                return;
            }

            VisadeLektioner.Clear();
            foreach (var l in _alla.Where(x => x.AnställningsNummer == nr)) VisadeLektioner.Add(l);
        }

        private void Stäng(object? obj)
        {
            if (obj is Window fönster) fönster.Close();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}