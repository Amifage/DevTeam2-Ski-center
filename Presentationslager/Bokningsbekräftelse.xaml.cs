using Entitetslager;
using Presentationslager.ViewModel;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Presentationslager
{
    public partial class Bokningsbekräftelse : Window
    {
        public Bokningsbekräftelse(
                 int bokningsNummer,
                 string kundText,
                 DateTime bokningsDatum,
                 List<LogiRad> logiRader,
                 List<UtrustningRad> utrustningRader,
                 List<KonferensRad> konferensRader,
                 List<SkidlektionRad> skidlektionRader)
        {
            InitializeComponent();

            BokningsbekräftelseViewModel viewModel =
            new BokningsbekräftelseViewModel(
                bokningsNummer,
                kundText,
                bokningsDatum,
                logiRader,
                utrustningRader,
                konferensRader,
                skidlektionRader);

            viewModel.StängFönster += Close;

            DataContext = viewModel;
        }
    }
}