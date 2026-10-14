using Entitetslager;
using Presentationslager.ViewModel;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Presentationslager
{
    public partial class Bokning : Window
    {
        private readonly BokningViewModel _viewModel;


        public Bokning()
        {
            InitializeComponent();

            _viewModel =
                new BokningViewModel();


            _viewModel.VisaMeddelande +=
                VisaMeddelande;


            _viewModel.BokningSparad += VisaBokningsbekräftelse;

          


            DataContext = _viewModel;
        }


        private void VisaMeddelande(
            string meddelande)
        {
            MessageBox.Show(meddelande);
        }


        private void VisaBokningsbekräftelse(
                 int bokningsNummer, 
                 string kundText,
                 DateTime bokningsdatum,
                 bool harAvbeställningsskydd,
                 List<LogiRad> logiRader,
                 List<UtrustningRad> utrustningRader,
                 List<KonferensRad> konferensRader,
                 List<SkidlektionRad> skidlektionRader)
        {
            Bokningsbekräftelse bekräftelseFönster =
                new Bokningsbekräftelse(
                    bokningsNummer, 
                    kundText,
                    bokningsdatum,
                    harAvbeställningsskydd,
                    logiRader,
                    utrustningRader,
                    konferensRader,
                    skidlektionRader);

            bekräftelseFönster.Owner = this;

            bekräftelseFönster.ShowDialog();

            _viewModel.ÅterställFormulär();
        }


        private void StängFönster()
        {
            Close();
        }
    }
}