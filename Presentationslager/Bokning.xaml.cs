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



        private void LaddaLogi()
        {
            LogiController logiController = new LogiController();

            cmbLogi.ItemsSource = logiController.HämtaAllaLogi();
        }

        private void cmbLogi_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbAntalPersonerValtLogi.Items.Clear();
            txtLogiPris.Text = "0 kr";

            if (cmbLogi.SelectedItem is Logi valtLogi)
            {
                if (valtLogi.LogiKapacitet.HasValue)
                {
                    for (int i = 1; i <= valtLogi.LogiKapacitet.Value; i++)
                    {
                        cmbAntalPersonerTotalt.Items.Add(i);
                    }
                }

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

                cmbAntalPersonerValtLogi.Items.Clear();

                if (valtLogi.LogiKapacitet.HasValue)
                {
                    for (int i = 1; i <= valtLogi.LogiKapacitet.Value; i++)
                    {
                        cmbAntalPersonerValtLogi.Items.Add(i);
                    }
                }
            }

        }

        private void Datum_Changed(object sender, SelectionChangedEventArgs e)
        {
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

            MessageBox.Show("Bokningen har sparats.");
        }
    }
}
