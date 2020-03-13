using System.ComponentModel;

namespace Arduchuni.Server {
    public sealed class Sensor : INotifyPropertyChanged {

        private int _value;

        public int Value {
            get => _value;
            set {
                _value = value;
                OnPropertyChanged(nameof(Value));
            }
        }

        private bool _active;

        public bool Active {
            get => _active;
            set {
                _active = value;
                OnPropertyChanged(nameof(Active));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
