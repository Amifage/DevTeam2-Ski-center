using Entitetslager;
using Servicelager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Presentationslager
{
    /// <summary>
    /// Interaction logic for Bokning.xaml
    /// </summary>
    public partial class Bokning : Window
    {
        private Kund? _valdKund;
        private List<LogiRad> _logiRader = new List<LogiRad>();

        public Bokning()
        {
            InitializeComponent();
            LaddaKunder();
            LaddaAntalPersoner();
            LaddaLogi();

        }

        private void LaddaKunder()
        {
            KundController kundController = new KundController();

            cmbKund.ItemsSource = kundController.HämtaAllaKunder();
        }

        private void cmbKund_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _valdKund = cmbKund.SelectedItem as Kund;
        }

        private bool ValideraDatum()
        {
            if (dpStartDatum.SelectedDate == null || dpSlutDatum.SelectedDate == null)
            {
                MessageBox.Show("Du måste välja både startdatum och slutdatum.");
                return false;
            }

            if (dpSlutDatum.SelectedDate < dpStartDatum.SelectedDate)
            {
                MessageBox.Show("Slutdatum kan inte vara före startdatum.");
                return false;
            }

            return true;
        }

        private void Datum_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (cmbBoendeTyp.SelectedItem != null)
            {
                cmbBoendeTyp_SelectionChanged(cmbBoendeTyp, null);
            }
        }



        private void LaddaAntalPersoner()
        {
            for (int i = 1; i <= 50; i++)
            {
                cmbAntalPersonerTotalt.Items.Add(i);
            }
        }

        private void cmbAntalPersonerTotalt_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UppdateraBoendePlacering();
        }

        private void UppdateraBoendePlacering()
        {
            int totalt = cmbAntalPersonerTotalt.SelectedItem is int antal
                ? antal
                : 0;

            int placerade = _logiRader.Sum(x => x.AntalPersoner);

            txtBoendePlacering.Text = $"{placerade} av {totalt} personer placerade";
        }



        private void cmbBoendeTyp_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbBoendeTyp.SelectedItem is not ComboBoxItem valtTyp)
                return;

            string typ = valtTyp.Content.ToString();

            LogiController logiController = new LogiController();
            var allaLogi = logiController.HämtaAllaLogi();

            if (dpStartDatum.SelectedDate == null || dpSlutDatum.SelectedDate == null)
            {
                return;
            }

            var upptagnaLogi = logiController.HämtaUpptagnaLogi(
                dpStartDatum.SelectedDate.Value,
                dpSlutDatum.SelectedDate.Value);


            if (typ == "Lägenhet")
            {
                cmbLogi.ItemsSource = allaLogi
                        .Where(l => l.ArtikelTypNummer == 1 || l.ArtikelTypNummer == 2)
                        .Where(l => !_logiRader.Any(r => r.LogiNummer == l.LogiNummer))
                        .Where(l => !upptagnaLogi.Contains(l.LogiNummer))
                        .OrderBy(l => AvståndTillValdaLogi(l))
                        .ThenBy(l => HämtaLogiNummer(l))
                        .ToList();
            }
            else if (typ == "Camping")
            {
                cmbLogi.ItemsSource = allaLogi
                        .Where(l => l.ArtikelTypNummer == 3)
                        .Where(l => !_logiRader.Any(r => r.LogiNummer == l.LogiNummer))
                        .Where(l => !upptagnaLogi.Contains(l.LogiNummer))
                        .OrderBy(l => HämtaLogiNummer(l))
                        .ToList();
            }
        }




        private void LaddaLogi()
        {
            LogiController logiController = new LogiController();

            cmbLogi.ItemsSource = logiController.HämtaAllaLogi();
        }

        private int HämtaLogiNummer(Logi logi)
        {
            string siffror = new string(logi.LogiNummer
                .Where(char.IsDigit)
                .ToArray());

            return int.TryParse(siffror, out int nummer)
                ? nummer
                : int.MaxValue;
        }

        private int AvståndTillValdaLogi(Logi logi)
        {
            if (!_logiRader.Any())
                return 0;

            int aktuelltNummer = HämtaLogiNummer(logi);

            return _logiRader
                .Where(r => r.LogiNummer != null)
                .Select(r =>
                {
                    string siffror = new string(r.LogiNummer
                        .Where(char.IsDigit)
                        .ToArray());

                    return int.TryParse(siffror, out int nummer)
                        ? Math.Abs(aktuelltNummer - nummer)
                        : int.MaxValue;
                })
                .Min();
        }


        private void cmbLogi_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbAntalPersonerValtLogi.Items.Clear();
            txtLogiPris.Text = "0 kr";

            if (cmbLogi.SelectedItem is Logi valtLogi)
            {

                if (dpStartDatum.SelectedDate != null &&
                    dpSlutDatum.SelectedDate != null &&
                    valtLogi.ArtikelTypNummer != null)
                {
                    PrisController prisController = new PrisController();

                    Pris? pris = prisController.HämtaAktuelltPris(
                        valtLogi.ArtikelTypNummer.Value,
                        dpStartDatum.SelectedDate.Value,
                        dpSlutDatum.SelectedDate.Value);

                    if (pris != null)
                    {
                        txtLogiPris.Text = $"{pris.PrisBelopp:N0} kr";
                    }
                }

                int totaltAntal = cmbAntalPersonerTotalt.SelectedItem is int totalt
                    ? totalt
                    : 0;

                int redanPlacerade = _logiRader.Sum(x => x.AntalPersoner);

                int återstående = totaltAntal - redanPlacerade;
                if (valtLogi.LogiKapacitet.HasValue)
                {
                    int maxAntal = Math.Min(valtLogi.LogiKapacitet.Value, återstående);

                    for (int i = 1; i <= maxAntal; i++)
                    {
                        cmbAntalPersonerValtLogi.Items.Add(i);
                    }
                }
                else
                {
                    // Camping saknar fast kapacitet
                    for (int i = 1; i <= återstående; i++)
                    {
                        cmbAntalPersonerValtLogi.Items.Add(i);
                    }
                }
            }

        }

       


        private void LäggTillLogi_Click(object sender, RoutedEventArgs e)
        {
            if (cmbLogi.SelectedItem is not Logi valtLogi)
            {
                MessageBox.Show("Du måste välja ett boende.");
                return;
            }

            if (cmbAntalPersonerValtLogi.SelectedItem == null)
            {
                MessageBox.Show("Du måste välja antal personer för boendet.");
                return;
            }

            if (!ValideraDatum())
            {
                return;
            }

            int antalPersoner = (int)cmbAntalPersonerValtLogi.SelectedItem;

            if (cmbAntalPersonerTotalt.SelectedItem == null)
            {
                MessageBox.Show("Du måste välja totalt antal personer.");
                return;
            }

            int totaltAntal = (int)cmbAntalPersonerTotalt.SelectedItem;
            int redanPlacerade = _logiRader.Sum(x => x.AntalPersoner);

            if (redanPlacerade + antalPersoner > totaltAntal)
            {
                MessageBox.Show("Du kan inte placera fler personer än det totala antalet i bokningen.");
                return;
            }

            PrisController prisController = new PrisController();

            Pris? pris = prisController.HämtaAktuelltPris(
                valtLogi.ArtikelTypNummer!.Value,
                dpStartDatum.SelectedDate!.Value,
                dpSlutDatum.SelectedDate!.Value);

            if (pris == null)
            {
                MessageBox.Show("Kunde inte hitta något pris för det valda boendet.");
                return;
            }

            LogiRad logiRad = new LogiRad
            {
                StartDatum = dpStartDatum.SelectedDate.Value,
                SlutDatum = dpSlutDatum.SelectedDate.Value,
                LogiBelopp = pris.PrisBelopp,
                AntalPersoner = antalPersoner,
                LogiNummer = valtLogi.LogiNummer,
                LogiDisplayText = valtLogi.DisplayText,
                SenastUppdaterad = DateTime.Now,
            };

            _logiRader.Add(logiRad);
            lstLogiRader.ItemsSource = null;
            lstLogiRader.ItemsSource = _logiRader;
            UppdateraTotalBelopp();
            cmbLogi.SelectedItem = null;
            cmbAntalPersonerValtLogi.Items.Clear();
            txtLogiPris.Text = "0 kr";

            UppdateraBoendePlacering();

            if (cmbBoendeTyp.SelectedItem is ComboBoxItem valtTyp)
            {
                cmbBoendeTyp_SelectionChanged(cmbBoendeTyp, null);
            }
        }

        #region Totalbelopp
        private void UppdateraTotalBelopp()
        {
            decimal total = _logiRader.Sum(x => x.LogiBelopp);

            txtTotalBelopp.Text = $"{total:N0} kr";
        }
        #endregion



        private void SkapaBokning_Click(object sender, RoutedEventArgs e)
        {
            if (_valdKund == null)
            {
                MessageBox.Show("Du måste välja en kund.");
                return;
            }

            if (!ValideraDatum())
            {
                return;
            }

            Entitetslager.Bokning nyBokning = new Entitetslager.Bokning
            {
                KundNummer = _valdKund.KundNummer,
                BokningsDatum = DateTime.Now,
                Status = "Aktiv",
                LogiRader = _logiRader
            };

            BokningController bokningController = new BokningController();

            bokningController.SkapaBokning(nyBokning);

            string kundText = _valdKund switch
            {
                PrivatKund privatKund => privatKund.DisplayText,
                FöretagsKund företagsKund => företagsKund.DisplayText,
                _ => _valdKund.KundNummer.ToString()
            };

            Bokningsbekräftelse bekräftelseFönster =
            new Bokningsbekräftelse(
                kundText,
                DateTime.Now,
                _logiRader);

            bekräftelseFönster.Owner = this;

            bekräftelseFönster.ShowDialog();

            ÅterställFormulär();
        }

        private void ÅterställFormulär()
        {
            cmbKund.SelectedItem = null;

            dpStartDatum.SelectedDate = null;
            dpSlutDatum.SelectedDate = null;

            cmbAntalPersonerTotalt.SelectedItem = null;

            cmbLogi.SelectedItem = null;
            cmbAntalPersonerValtLogi.Items.Clear();

            txtLogiPris.Text = "0 kr";
            txtTotalBelopp.Text = "0 kr";
            txtBoendePlacering.Text = "0 av 0 personer placerade";

            _valdKund = null;
            _logiRader.Clear();

            lstLogiRader.ItemsSource = null;

            cmbBoendeTyp.SelectedItem = null;
            cmbLogi.ItemsSource = null;
        }
    }
}
