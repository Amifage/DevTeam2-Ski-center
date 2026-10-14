using Entitetslager;
using Presentationslager.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Win32;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.IO;
using System.Text.Json;

namespace Presentationslager
{
    public class ÄndringsbekräftelseViewModel : INotifyPropertyChanged
    {
        public int BokningsNummer { get; }

        public string KundText { get; }

        public string KundNamnText =>
            KundText.Contains("|")
                ? KundText.Split('|')[0]
                : KundText;

        public string KundNummerText =>
            KundText.Contains("|")
                ? KundText.Split('|')[1]
                : "";

        public string ÄndringsdatumText { get; }

        public ICommand ExporteraJsonCommand { get; }
        public ICommand ExporteraPdfCommand { get; }
        public ICommand StängCommand { get; }
        public event Action? StängFönster;

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
             SkidlektionRader.Sum(x => x.SkidlektionBelopp) +
             (HarAvbeställningsskydd ? 300 : 0);

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
                 : SkillnadBelopp > 0
                     ? $"+{SkillnadBelopp:N0} kr"
                     : "";

        public string FörändringRubrik =>
            SkillnadBelopp < 0
                ? "Kreditering:"
                : SkillnadBelopp > 0
                    ? "Ökning:"
                    : "Ingen prisförändring";

        public bool HarAvbeställningsskydd { get; }

        public string AvbeställningsskyddText =>
            HarAvbeställningsskydd
                ? "Ja, 300 kr"
                : "Nej";

        public ÄndringsbekräftelseViewModel(
            int bokningsNummer,
            string kundText,
            DateTime ändringsdatum,
            bool harAvbeställningsskydd,
            decimal tidigareTotalBelopp,
            IEnumerable<LogiRad> logiRader,
            IEnumerable<UtrustningRad> utrustningRader,
            IEnumerable<KonferensRad> konferensRader,
            IEnumerable<SkidlektionRad> skidlektionRader)
        {
            BokningsNummer = bokningsNummer;

            KundText = kundText;
            HarAvbeställningsskydd = harAvbeställningsskydd;

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

            ExporteraJsonCommand = new RelayCommand(_ => ExporteraJson());

            ExporteraPdfCommand = new RelayCommand(_ => ExporteraPdf());

            StängCommand = new RelayCommand(_ => StängFönster?.Invoke());
        }

        private void ExporteraJson()
        {
            SaveFileDialog dialog = new SaveFileDialog
            {
                Filter = "JSON-fil (*.json)|*.json",
                FileName = $"Ändringsbekräftelse_{BokningsNummer}.json"
            };

            bool? resultat = dialog.ShowDialog();

            if (resultat != true)
                return;

            var exportData = new
            {
                BokningsNummer,
                KundNamn = KundNamnText,
                KundNummer = int.TryParse(KundNummerText, out int kundNummer)
                            ? kundNummer
                            : 0,
                Ändringsdatum = ÄndringsdatumText,

                HarAvbeställningsskydd,
                Avbeställningsskydd = AvbeställningsskyddText,

                TidigareTotalBelopp,
                Förändring = SkillnadBelopp,
                FörändringTyp = FörändringRubrik.Replace(":", ""),
                NyTotalBelopp,

                Logi = LogiRader.Select(r => new
                {
                    r.LogiNummer,
                    r.StartDatum,
                    r.SlutDatum,
                    r.AntalPersoner,
                    Belopp = r.LogiBelopp,
                    r.LogiDisplayText
                }).ToList(),

                Utrustning = UtrustningRader.Select(r => new
                {
                    r.UtrustningNummer,
                    r.UtrustningPaketNummer,
                    Exemplarnummer = r.ExemplarNummerText,
                    r.StartDatum,
                    r.SlutDatum,
                    Belopp = r.UtrustningBelopp,
                    r.UtrustningDisplayText
                }).ToList(),

                Konferens = KonferensRader.Select(r => new
                {
                    r.KonferensNummer,
                    r.StartTid,
                    r.SlutTid,
                    r.AntalPersoner,
                    Belopp = r.KonferensBelopp
                }).ToList(),

                Skidlektioner = SkidlektionRader.Select(r => new
                {
                    r.SkidlektionNummer,
                    r.Datum,
                    r.AntalPersoner,
                    Belopp = r.SkidlektionBelopp,
                    r.SkidlektionDisplayText
                }).ToList()
            };

            JsonSerializerOptions options =
                new JsonSerializerOptions
                {
                    WriteIndented = true
                };

            string json =
                JsonSerializer.Serialize(exportData, options);

            File.WriteAllText(
                dialog.FileName,
                json);
        }

        private void ExporteraPdf()
        {
            QuestPDF.Settings.License = LicenseType.Community;

            SaveFileDialog dialog = new SaveFileDialog
            {
                Filter = "PDF-fil (*.pdf)|*.pdf",
                FileName = $"Ändringsbekräftelse_{BokningsNummer}.pdf"
            };

            bool? resultat = dialog.ShowDialog();

            if (resultat != true)
                return;

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);

                    page.DefaultTextStyle(x =>
                        x.FontSize(11));

                    page.Header()
                        .Text("Ändringsbekräftelse")
                        .FontSize(22)
                        .Bold();

                    page.Content()
                        .PaddingVertical(20)
                        .Column(column =>
                        {
                            column.Spacing(8);

                            column.Item()
                                .Text($"Bokningsnummer: {BokningsNummer}");

                            column.Item()
                                .Text($"Kund: {KundNamnText}");

                            column.Item()
                                .Text($"Kundnummer: {KundNummerText}");

                            column.Item()
                                .Text($"Ändringsdatum: {ÄndringsdatumText}");

                            column.Item()
                                .Text($"Avbeställningsskydd: {AvbeställningsskyddText}");


                            if (LogiRader.Any())
                            {
                                column.Item()
                                    .PaddingTop(15)
                                    .Text("Boende")
                                    .FontSize(15)
                                    .Bold();

                                foreach (LogiRad rad in LogiRader)
                                {
                                    column.Item().Text(
                                        $"{rad.LogiDisplayText}\n" +
                                        $"Antal personer: {rad.AntalPersoner}\n" +
                                        $"Period: {rad.StartDatum:yyyy-MM-dd} - {rad.SlutDatum:yyyy-MM-dd}\n" +
                                        $"Belopp: {rad.LogiBelopp:N0} kr");
                                }
                            }


                            if (UtrustningRader.Any())
                            {
                                column.Item()
                                    .PaddingTop(15)
                                    .Text("Utrustning")
                                    .FontSize(15)
                                    .Bold();

                                foreach (UtrustningRad rad in UtrustningRader)
                                {
                                    column.Item().Text(
                                         $"{rad.UtrustningDisplayText}\n" +
                                         $"Exemplarnummer: {rad.ExemplarNummerText}\n" +
                                         $"Period: {rad.StartDatum:yyyy-MM-dd} - {rad.SlutDatum:yyyy-MM-dd}\n" +
                                         $"Belopp: {rad.UtrustningBelopp:N0} kr");
                                }
                            }


                            if (KonferensRader.Any())
                            {
                                column.Item()
                                    .PaddingTop(15)
                                    .Text("Konferens")
                                    .FontSize(15)
                                    .Bold();

                                foreach (KonferensRad rad in KonferensRader)
                                {
                                    column.Item().Text(
                                        $"{rad.KonferensNummer}\n" +
                                        $"Antal deltagare: {rad.AntalPersoner}\n" +
                                        $"Tid: {rad.StartTid:yyyy-MM-dd HH:mm} - {rad.SlutTid:yyyy-MM-dd HH:mm}\n" +
                                        $"Belopp: {rad.KonferensBelopp:N0} kr");
                                }
                            }


                            if (SkidlektionRader.Any())
                            {
                                column.Item()
                                    .PaddingTop(15)
                                    .Text("Skidlektion")
                                    .FontSize(15)
                                    .Bold();

                                foreach (SkidlektionRad rad in SkidlektionRader)
                                {
                                    column.Item().Text(
                                        $"{rad.SkidlektionDisplayText}\n" +
                                        $"Antal personer: {rad.AntalPersoner}\n" +
                                        $"Datum: {rad.Datum:yyyy-MM-dd}\n" +
                                        $"Belopp: {rad.SkidlektionBelopp:N0} kr");
                                }
                            }


                            column.Item()
                                .PaddingTop(20)
                                .LineHorizontal(1);

                            column.Item()
                                .PaddingTop(10)
                                .AlignRight()
                                .Text($"Tidigare totalsumma: {TidigareTotalBeloppText}");

                            column.Item()
                                .AlignRight()
                                .Text($"{FörändringRubrik} {SkillnadText}");

                            column.Item()
                                .AlignRight()
                                .Text($"Ny totalsumma: {NyTotalBeloppText}")
                                .FontSize(16)
                                .Bold();
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(text =>
                        {
                            text.Span("Sida ");
                            text.CurrentPageNumber();
                        });
                });
            })
            .GeneratePdf(dialog.FileName);
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