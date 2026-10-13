using Presentationslager.Command;
using Servicelager;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Presentationslager.ViewModel
{
    public class LogiViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private readonly LogiController logiController;
        public ICommand SokCommand { get; }
        public ICommand RensaCommand { get; }
        public ICommand UppdateraStatusCommand { get; }
        public ICommand TillbakaCommand { get; set; }

        public

        public LogiViewModel() 
        {
            logiController = new LogiController();
            SokCommand = new RelayCommand(UtforSokning);
            RensaCommand = new RelayCommand(RensaFalt);
            UppdateraStatusCommand = new RelayCommand(UppdateraStatus, CanUppdateraStatus);
            TillbakaCommand = new RelayCommand(TillbakaTillMeny);
            LaddaData();
        }

        private void TillbakaTillMeny(object obj)
        {
            Meny menyFonster = new Meny();
            menyFonster.Show();

            // Stäng det nuvarande fönstret
            if (obj is Window nuvarandeFonster)
            {
                nuvarandeFonster.Close();
            }
        }
    }
}
