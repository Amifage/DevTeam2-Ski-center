using Entitetslager;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Presentationslager.ViewModel
{
    public class BokningsbekräftelseViewModel : INotifyPropertyChanged
    {
        public string KundText { get; }

        public string BokningsdatumText { get; }

        public ObservableCollection<LogiRad> LogiRader { get; }
        public ObservableCollection<UtrustningRad> UtrustningRader { get; }

        public string TotalBeloppText =>
            $"{LogiRader.Sum(x => x.LogiBelopp) +
            UtrustningRader.Sum(x => x.UtrustningBelopp):N0} kr";

        public BokningsbekräftelseViewModel(
            string kundText,
            DateTime bokningsdatum,
            IEnumerable<LogiRad> logiRader,
            IEnumerable<UtrustningRad> utrustningRader)
        {
            KundText = kundText;
            BokningsdatumText = bokningsdatum.ToString("yyyy-MM-dd");


            LogiRader = new ObservableCollection<LogiRad>(logiRader);
            UtrustningRader = new ObservableCollection<UtrustningRad>(utrustningRader);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}