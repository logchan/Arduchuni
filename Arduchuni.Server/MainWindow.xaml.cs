using System.Collections.Generic;
using System.IO.Ports;
using System.Windows;

namespace Arduchuni.Server {
    public partial class MainWindow : Window {
        private const int NumSensors = 6;

        private readonly List<Sensor> _sensors = new List<Sensor>(NumSensors);
        private readonly List<string> _portNames = new List<string>();
        private SerialPort _serial = null;

        public MainWindow() {
            InitializeComponent();

            for (var i = 0; i < NumSensors; ++i) {
                var sensor = new Sensor();
                _sensors.Add(sensor);

                if (FindName($"Sensor{i}") is SensorView view) {
                    view.Sensor = sensor;
                }
            }
        }

        private void ComRefreshBtn_Click(object sender, RoutedEventArgs e) {
            
        }

        private void ComSetBtn_Click(object sender, RoutedEventArgs e) {

        }
    }
}
