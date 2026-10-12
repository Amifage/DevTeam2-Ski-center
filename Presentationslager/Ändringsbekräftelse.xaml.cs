using System;
using System.Collections.Generic;
using System.Windows;
using Entitetslager;

namespace Presentationslager
{
    public partial class Ändringsbekräftelse : Window
    {
        public Ändringsbekräftelse(
                int bokningsNummer,
                string kundText,
                DateTime ändringsdatum,
                decimal tidigareTotalBelopp,
                List<LogiRad> logiRader,
                List<UtrustningRad> utrustningRader,
                List<KonferensRad> konferensRader,
                List<SkidlektionRad> skidlektionRader)
        {
            InitializeComponent();

            DataContext = new ÄndringsbekräftelseViewModel(
                 bokningsNummer,
                 kundText,
                 ändringsdatum,
                 tidigareTotalBelopp,
                 logiRader,
                 utrustningRader,
                 konferensRader,
                 skidlektionRader);
        }

        private void Stäng_Click(
            object sender,
            RoutedEventArgs e)
        {
            Close();
        }
    }
}