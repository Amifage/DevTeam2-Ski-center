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

            ÄndringsbekräftelseViewModel viewModel =
             new ÄndringsbekräftelseViewModel(
                 bokningsNummer,
                 kundText,
                 ändringsdatum,
                 tidigareTotalBelopp,
                 logiRader,
                 utrustningRader,
                 konferensRader,
                 skidlektionRader);

            viewModel.StängFönster += Close;

            DataContext = viewModel;
        }

       
    }
}