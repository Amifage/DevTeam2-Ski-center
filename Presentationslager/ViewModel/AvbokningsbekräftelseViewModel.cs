using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Entitetslager;

namespace Presentationslager.ViewModel
{
    public class AvbokningsbekräftelseViewModel : INotifyPropertyChanged
    {
        public int BokningsNummer { get; }

        public string KundText { get; }

        public string AvbokningsdatumText { get; }

        public ObservableCollection<LogiRad> LogiRader { get; }

        public ObservableCollection<UtrustningRad> UtrustningRader { get; }

        public ObservableCollection<KonferensRad> KonferensRader { get; }

        public ObservableCollection<SkidlektionRad> SkidlektionRader { get; }


        public bool HarLogi =>
            LogiRader.Any();

        public bool HarUtrustning =>
            UtrustningRader.Any();

        public bool HarKonferens =>
            KonferensRader.Any();

        public bool HarSkidlektion =>
            SkidlektionRader.Any();


        public string TotalBeloppText =>
            $"{LogiRader.Sum(x => x.LogiBelopp) +
               UtrustningRader.Sum(x => x.UtrustningBelopp) +
               KonferensRader.Sum(x => x.KonferensBelopp) +
               SkidlektionRader.Sum(x => x.SkidlektionBelopp):N0} kr";

        public string KrediteringText =>
            $"-{LogiRader.Sum(x => x.LogiBelopp) +
               UtrustningRader.Sum(x => x.UtrustningBelopp) +
               KonferensRader.Sum(x => x.KonferensBelopp) +
               SkidlektionRader.Sum(x => x.SkidlektionBelopp):N0} kr";

        public string TotaltKvarText => "0 kr";


        public AvbokningsbekräftelseViewModel(
            int bokningsNummer,
            string kundText,
            DateTime avbokningsdatum,
            IEnumerable<LogiRad> logiRader,
            IEnumerable<UtrustningRad> utrustningRader,
            IEnumerable<KonferensRad> konferensRader,
            IEnumerable<SkidlektionRad> skidlektionRader)
        {
            BokningsNummer = bokningsNummer;
            KundText = kundText;

            AvbokningsdatumText =
                avbokningsdatum.ToString("yyyy-MM-dd");

            LogiRader =
                new ObservableCollection<LogiRad>(logiRader);

            UtrustningRader =
                new ObservableCollection<UtrustningRad>(utrustningRader);

            KonferensRader =
                new ObservableCollection<KonferensRad>(konferensRader);

            SkidlektionRader =
                new ObservableCollection<SkidlektionRad>(skidlektionRader);
        }


        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(
            [CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(propertyName));
        }
    }
}