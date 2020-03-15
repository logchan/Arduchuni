using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Threading;
using System.Windows;

namespace Arduchuni.Server {
    public partial class MainWindow : Window {
        private const int NumSensors = 6;
        private const int ReadEvery = 16;
        private const string ConfigFile = "Arduchuni.Server.json";

        private readonly Config _config = new Config();
        public Config Config => _config;

        private readonly List<Sensor> _sensors = new List<Sensor>(NumSensors);

        private volatile bool _serialStop = true;
        private Thread _serialThread;

        public MainWindow() {
            if (File.Exists(ConfigFile)) {
                _config = Helpers.Deserialize<Config>(ConfigFile);
            }
            else {
                for (var i = 0; i < NumSensors; ++i) {
                    _config.Thresholds.Add(960);
                }
            }

            InitializeComponent();
            Title += " version " + Helpers.GetVersion();

            for (var i = 0; i < NumSensors; ++i) {
                var sensor = new Sensor();
                _sensors.Add(sensor);

                var view = FindName($"Sensor{i}") as SensorView;
                Debug.Assert(view != null);
                view.Sensor = sensor;
            }

            RefreshCom();
        }

        private void ComRefreshBtn_Click(object sender, RoutedEventArgs e) {
            RefreshCom();
        }

        private void RefreshCom() {
            ComCombo.Items.Clear();

            var names = SerialPort.GetPortNames();
            var selected = 0;
            for (var i = 0; i < names.Length; ++i) {
                var name = names[i];
                ComCombo.Items.Add(name);

                if (name == _config.ComName) {
                    selected = i;
                }
            }

            if (ComCombo.Items.Count > selected) {
                ComCombo.SelectedIndex = selected;
            }
        }

        private void ComSetBtn_Click(object sender, RoutedEventArgs e) {
            if (_serialThread != null) {
                _serialStop = true;
                _serialThread.Join();
            }

            if (!(ComCombo.SelectedItem is string name)) {
                return;
            }

            _serialStop = false;
            _config.ComName = name;
            _serialThread = new Thread(SerialThreadWork);
            _serialThread.Start();
        }

        private void SerialThreadWork() {
            var serial = new SerialPort(_config.ComName) {
                BaudRate = 19200, 
                ReadTimeout = 1000
            };
            var buffer = new byte[serial.ReadBufferSize];
            var cmd = new byte[] { 0x42 };

            try {
                serial.Open();
                
                while (!_serialStop) {
                    serial.Write(cmd, 0, 1);
                    Thread.Sleep(ReadEvery);

                    var count = serial.BaseStream.Read(buffer, 0, buffer.Length);
                    if (count != 16) {
                        Debug.WriteLine(count);
                        continue;
                    }

                    for (var i = 0; i < 6; ++i) {
                        var value = buffer[2 * i] + buffer[2 * i + 1] * 256;
                        var threshold = _config.Thresholds[i];

                        _sensors[i].Value = value;
                        _sensors[i].Difference = value - threshold;
                        _sensors[i].Active = value > threshold;
                    }

                    ChuniIO.Send(_sensors);
                }
            }
            catch (Exception ex) {
                // TODO log
                Dispatcher?.Invoke(() => { MessageBox.Show(ex.Message); });
            }
            finally {
                serial.Close();
            }
        }

        private void ThisWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            _serialStop = true;
            _serialThread?.Join();

            Helpers.Serialize(_config, ConfigFile);
        }
    }
}
