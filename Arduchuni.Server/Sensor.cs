namespace Arduchuni.Server {
    public sealed class Sensor : Helpers.PropertyChangedInvoker {
        static Sensor() {
            Helpers.PatchNotifyPropertyChanged<Sensor>();
        }

        public int Value { get; set; }
        public bool Activated { get; set; }
    }
}
