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


            _viewModel.StängFönster +=
                StängFönster;


            DataContext = _viewModel;
        }


        private void VisaMeddelande(
            string meddelande)
        {
            MessageBox.Show(meddelande);
        }


        private void VisaBokningsbekräftelse(
                 string kundText,
                 DateTime bokningsdatum,
                 List<LogiRad> logiRader,
                 List<UtrustningRad> utrustningRader)
        {
            Bokningsbekräftelse bekräftelseFönster =
                new Bokningsbekräftelse(
                    kundText,
                    bokningsdatum,
                    logiRader,
                    utrustningRader);

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