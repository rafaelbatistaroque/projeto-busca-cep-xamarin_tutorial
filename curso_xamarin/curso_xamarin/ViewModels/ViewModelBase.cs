using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace curso_xamarin.ViewModels
{
    class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string nomePropriedade = "")
        {
            if (PropertyChanged is null) return;
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nomePropriedade));
        }

        private bool _ocupado = false;
        public bool Ocupado
        {
            get => _ocupado;
            set
            {
                _ocupado = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Desocupado));
            }
        }
        public bool Desocupado { get => !_ocupado; }
    }
}
