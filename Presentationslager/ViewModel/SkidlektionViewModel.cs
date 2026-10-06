using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Presentationslager.ViewModel
{
    public class SkidlektionViewModel : INotifyPropertyChanged
    {
        public SkidlektionViewModel()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}