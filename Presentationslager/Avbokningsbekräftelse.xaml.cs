using Entitetslager;
using Presentationslager.ViewModel;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Presentationslager
{
    public partial class Avbokningsbekräftelse : Window
    {
        public Avbokningsbekräftelse(
            int bokningsNummer,
            string kundText,
            DateTime avbokningsdatum,
            List<LogiRad> logiRader,
            List<UtrustningRad> utrustningRader,
            List<KonferensRad> konferensRader,
            List<SkidlektionRad> skidlektionRader)
        {
            InitializeComponent();

            AvbokningsbekräftelseViewModel viewModel =
            new AvbokningsbekräftelseViewModel(
                bokningsNummer,
                kundText,
                avbokningsdatum,
                logiRader,
                utrustningRader,
                konferensRader,
                skidlektionRader);

            viewModel.StängFönster += Close;

            DataContext = viewModel;

        }
    }
}