using Entitetslager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Presentationslager
{
    public partial class Bokningsbekräftelse : Window
    {
        public Bokningsbekräftelse(
            string kundText,
            DateTime bokningsdatum,
            List<LogiRad> logiRader)
        {
            InitializeComponent();

            txtKund.Text = kundText;
            txtBokningsdatum.Text = bokningsdatum.ToString("yyyy-MM-dd");

            lstLogiRader.ItemsSource = logiRader;

            decimal total = logiRader.Sum(x => x.LogiBelopp);

            txtTotal.Text = $"{total:N0} kr";
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}