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
             string kundText,
             DateTime bokningsdatum,
             List<LogiRad> logiRader,
             List<UtrustningRad> utrustningRader,
             List<KonferensRad> konferensRader,
             List<SkidlektionRad> skidlektionRader)
        {
            InitializeComponent();

            DataContext = new BokningsbekräftelseViewModel(
                kundText,
                bokningsdatum,
                logiRader,
                utrustningRader,
                konferensRader,
                skidlektionRader);
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}