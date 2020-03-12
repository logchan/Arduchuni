using System.Windows;
using System.Windows.Controls;

namespace Arduchuni.Server {
    public partial class SensorView : UserControl {
        
        public Sensor Sensor {
            get => (Sensor)GetValue(SensorProperty);
            set => SetValue(SensorProperty, value);
        }

        public static readonly DependencyProperty SensorProperty =
            DependencyProperty.Register("Sensor", typeof(Sensor), typeof(SensorView), new PropertyMetadata(null));

        public SensorView() {
            InitializeComponent();
        }
    }
}
