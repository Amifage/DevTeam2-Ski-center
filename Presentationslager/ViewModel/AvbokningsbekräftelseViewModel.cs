using Entitetslager;
using Microsoft.Win32;
using Presentationslager.Command;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows.Input;
using System.Windows.Input;

namespace Presentationslager.ViewModel
{
    public class AvbokningsbekräftelseViewModel : INotifyPropertyChanged
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
        public int KundNummer { get; }

        public string AvbokningsdatumText { get; }

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
                SkidlektionRader.Sum(x => x.SkidlektionBelopp) +
                (HarAvbeställningsskydd ? 300 : 0):N0} kr";

        public string KrediteringText =>
            $"-{LogiRader.Sum(x => x.LogiBelopp) +
               UtrustningRader.Sum(x => x.UtrustningBelopp) +
               KonferensRader.Sum(x => x.KonferensBelopp) +
               SkidlektionRader.Sum(x => x.SkidlektionBelopp) +
               (HarAvbeställningsskydd ? 300 : 0):N0} kr";

        public string TotaltKvarText => "0 kr";

        public bool HarAvbeställningsskydd { get; }

        public string AvbeställningsskyddText =>
            HarAvbeställningsskydd
                ? "Ja, 300 kr"
                : "Nej";

        public decimal AnvändRabattProcent { get; }

        public string RabattText =>
            AnvändRabattProcent > 0
                ? $"{AnvändRabattProcent:N0} % på logi"
                : "Ingen";

        public decimal MomsBelopp =>
            (LogiRader.Sum(x => x.LogiBelopp) +
                UtrustningRader.Sum(x => x.UtrustningBelopp) +
                KonferensRader.Sum(x => x.KonferensBelopp) +
                SkidlektionRader.Sum(x => x.SkidlektionBelopp) +
                (HarAvbeställningsskydd ? 300m : 0m))
            * 12m / 112m;

        public string MomsText =>
            $"{MomsBelopp:N2} kr";

        public bool Delbetala { get; }

        public DateTime BokningsDatum { get; }


        public AvbokningsbekräftelseViewModel(
             int bokningsNummer,
             string kundText,
             DateTime avbokningsDatum,
             DateTime bokningsDatum,
             bool harAvbeställningsskydd,
             bool delbetala,
             decimal användRabattProcent,
             IEnumerable<LogiRad> logiRader,
             IEnumerable<UtrustningRad> utrustningRader,
             IEnumerable<KonferensRad> konferensRader,
             IEnumerable<SkidlektionRad> skidlektionRader)
        {
            BokningsNummer = bokningsNummer;
            KundText = kundText;
            AvbokningsdatumText = avbokningsDatum.ToString("yyyy-MM-dd");
            HarAvbeställningsskydd = harAvbeställningsskydd;
            AnvändRabattProcent = användRabattProcent;
            Delbetala = delbetala;
            BokningsDatum = bokningsDatum;

            LogiRader =
                new ObservableCollection<LogiRad>(logiRader);

            UtrustningRader =
                new ObservableCollection<UtrustningRad>(utrustningRader);

            KonferensRader =
                new ObservableCollection<KonferensRad>(konferensRader);

            SkidlektionRader =
                new ObservableCollection<SkidlektionRad>(skidlektionRader);

            ExporteraJsonCommand = new RelayCommand(_ => ExporteraJson());

            ExporteraPdfCommand = new RelayCommand(_ => ExporteraPdf());

            StängCommand = new RelayCommand(_ => StängFönster?.Invoke());
        }


        public decimal LogiBelopp =>
             LogiRader.Sum(x => x.LogiBelopp);

        public decimal ÖvrigtBelopp =>
            UtrustningRader.Sum(x => x.UtrustningBelopp) +
            KonferensRader.Sum(x => x.KonferensBelopp) +
            SkidlektionRader.Sum(x => x.SkidlektionBelopp) +
            (HarAvbeställningsskydd ? 300m : 0m);

        public decimal FörstaDelbetalning =>
            Delbetala
                ? (LogiBelopp * 0.20m) + ÖvrigtBelopp
                : 0m;

        public decimal AndraDelbetalning =>
            Delbetala
                ? LogiBelopp * 0.80m
                : 0m;

        public string FörstaDelbetalningText =>
            $"{FörstaDelbetalning:N0} kr";

        public string AndraDelbetalningText =>
            $"{AndraDelbetalning:N0} kr";


        public DateTime FörstaFörfallodatum =>
     BokningsDatum.AddDays(30);

        public DateTime? AndraFörfallodatum =>
            LogiRader.Any()
                ? LogiRader.Min(x => x.StartDatum).AddDays(-30)
                : null;

        public string FörstaFörfallodatumText =>
            FörstaFörfallodatum.ToString("yyyy-MM-dd");

        public string AndraFörfallodatumText =>
            AndraFörfallodatum?.ToString("yyyy-MM-dd") ?? "-";


        private void ExporteraJson()
        {
            SaveFileDialog dialog = new SaveFileDialog
            {
                Filter = "JSON-fil (*.json)|*.json",
                FileName = $"Avbokningsbekräftelse_{BokningsNummer}.json"
            };

            bool? resultat = dialog.ShowDialog();

            if (resultat != true)
                return;

            decimal tidigareTotalBelopp =
                LogiRader.Sum(r => r.LogiBelopp) +
                UtrustningRader.Sum(r => r.UtrustningBelopp) +
                KonferensRader.Sum(r => r.KonferensBelopp) +
                SkidlektionRader.Sum(r => r.SkidlektionBelopp);

            var exportData = new
            {
                BokningsNummer,
                KundNamn = KundNamnText,
                KundNummer = int.TryParse(KundNummerText, out int kundNummer)
                            ? kundNummer
                            : 0,

                HarAvbeställningsskydd,
                Avbeställningsskydd = AvbeställningsskyddText,

                RabattProcent = AnvändRabattProcent,
                Rabatt = RabattText,

                MomsProcent = 12,
                MomsBelopp = MomsBelopp,

                Delbetala = Delbetala,
                FörstaFakturaBelopp = FörstaDelbetalning,
                FörstaFörfallodatum = FörstaFörfallodatumText,
                AndraFakturaBelopp = AndraDelbetalning,
                AndraFörfallodatum = AndraFörfallodatumText,

                Avbokningsdatum = AvbokningsdatumText,
                Status = "Avbokad",

                TidigareTotalBelopp = tidigareTotalBelopp,
                Kreditering = -tidigareTotalBelopp,
                TotaltKvar = 0,

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
                FileName = $"Avbokningsbekräftelse_{BokningsNummer}.pdf"
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
                        .Text("Avbokningsbekräftelse")
                        .FontSize(22)
                        .Bold();

                    page.Content()
                        .PaddingVertical(20)
                        .Column(column =>
                        {
                            column.Spacing(8);

                            column.Item()
                                .Text("Bokningen har avbokats")
                                .FontSize(16)
                                .Bold();

                            column.Item()
                                .Text($"Kund: {KundNamnText}");

                            column.Item()
                                .Text($"Kundnummer: {KundNummerText}");

                            column.Item()
                                .Text($"Avbokningsdatum: {AvbokningsdatumText}");

                            column.Item()
                                .PaddingTop(10)
                                .Text("Följande har avbokats:")
                                .FontSize(14)
                                .Bold();

                            column.Item()
                                .Text($"Avbeställningsskydd: {AvbeställningsskyddText}");

                            column.Item()
                                 .Text($"Rabatt: {RabattText}");

                            column.Item()
                                 .Text($"Varav moms 12 %: {MomsText}");

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
                                .Text($"Tidigare totalsumma: {TotalBeloppText}");

                            column.Item()
                                .AlignRight()
                                .Text($"Kreditering: {KrediteringText}");

                            column.Item()
                                .AlignRight()
                                .Text($"Totalt kvar: {TotaltKvarText}")
                                .FontSize(16)
                                .Bold();

                            if (Delbetala)
                            {
                                column.Item().Text("Delbetalning").Bold();

                                column.Item()
                                     .Text($"Första faktura (20 % logi + övrigt): {FörstaDelbetalningText}");

                                column.Item()
                                    .Text($"Förfallodatum: {FörstaFörfallodatumText}");

                                column.Item()
                                     .Text($"Andra faktura (80 % logi): {AndraDelbetalningText}");
            
                                column.Item()
                                    .Text($"Förfallodatum: {AndraFörfallodatumText}");
                            }
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