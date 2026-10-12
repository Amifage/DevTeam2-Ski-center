using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Entitetslager;

namespace Presentationslager
{
    public class ÄndringsbekräftelseViewModel : INotifyPropertyChanged
    {
        public int BokningsNummer { get; }

        public string KundText { get; }

        public string ÄndringsdatumText { get; }

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

        public decimal TidigareTotalBelopp { get; }

        public decimal NyTotalBelopp =>
            LogiRader.Sum(x => x.LogiBelopp) +
            UtrustningRader.Sum(x => x.UtrustningBelopp) +
            KonferensRader.Sum(x => x.KonferensBelopp) +
            SkidlektionRader.Sum(x => x.SkidlektionBelopp);

        public decimal SkillnadBelopp =>
            NyTotalBelopp - TidigareTotalBelopp;

        public string TidigareTotalBeloppText =>
            $"{TidigareTotalBelopp:N0} kr";

        public string NyTotalBeloppText =>
            $"{NyTotalBelopp:N0} kr";

        public bool ÄrKreditering =>
            SkillnadBelopp < 0;

        public bool ÄrÖkning =>
            SkillnadBelopp > 0;

        public string SkillnadText =>
            SkillnadBelopp < 0
                ? $"{SkillnadBelopp:N0} kr"
                : $"+{SkillnadBelopp:N0} kr";

        public string FörändringRubrik =>
            SkillnadBelopp < 0
                ? "Kreditering:"
                : SkillnadBelopp > 0
                    ? "Ökning:"
                    : "Förändring:";


        public ÄndringsbekräftelseViewModel(
            int bokningsNummer,
            string kundText,
            DateTime ändringsdatum,
            decimal tidigareTotalBelopp,
            IEnumerable<LogiRad> logiRader,
            IEnumerable<UtrustningRad> utrustningRader,
            IEnumerable<KonferensRad> konferensRader,
            IEnumerable<SkidlektionRad> skidlektionRader)
        {
            BokningsNummer = bokningsNummer;

            KundText = kundText;

            ÄndringsdatumText =
                ändringsdatum.ToString("yyyy-MM-dd");

            TidigareTotalBelopp = tidigareTotalBelopp;

            LogiRader =
                new ObservableCollection<LogiRad>(
                    logiRader);

            UtrustningRader =
                new ObservableCollection<UtrustningRad>(
                    utrustningRader);

            KonferensRader =
                new ObservableCollection<KonferensRad>(
                    konferensRader);

            SkidlektionRader =
                new ObservableCollection<SkidlektionRad>(
                    skidlektionRader);
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