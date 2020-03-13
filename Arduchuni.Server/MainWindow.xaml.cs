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
        private const int ReadEvery = 20;
        private const string ConfigFile = "Arduchuni.Server.json";

        private Config _config = new Config();
        public Config Config => _config;

        private readonly List<Sensor> _sensors = new List<Sensor>(NumSensors);
        private readonly List<SensorView> _sensorViews = new List<SensorView>();

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
                _sensorViews.Add(view);
            }

            RefreshCom();
        }

        private void ComRefreshBtn_Click(object sender, RoutedEventArgs e) {
            RefreshCom();
        }

        private void RefreshCom() {
            ComCombo.Items.Clear();
            foreach (var name in SerialPort.GetPortNames()) {
                ComCombo.Items.Add(name);
            }

            if (_config.ComName != null) {
                for (var i = 0; i < ComCombo.Items.Count; ++i) {
                    if (ComCombo.Items[i] as string == _config.ComName) {
                        ComCombo.SelectedIndex = i;
                        break;
                    }
                }
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
                BaudRate = 9600, 
                ReadTimeout = Int32.MaxValue
            };

            try {
                serial.Open();
                serial.DiscardInBuffer();

                var cmd = new byte[] { 0x42 };
                var buffer = new byte[2 * 6];
                while (!_serialStop) {
                    Thread.Sleep(ReadEvery);
                    
                    serial.Write(cmd, 0, 1);
                    var length = serial.Read(buffer, 0, buffer.Length);
                    if (length == 0) {
                        continue;
                    }

                    if (length == 12) {
                        for (var i = 0; i < 6; ++i) {
                            var value = buffer[2 * i] + buffer[2 * i + 1] * 256;
                            _sensors[i].Value = value;
                            _sensors[i].Active = value > _config.Thresholds[i];
                        }

                        ChuniIO.Send(_sensors);
                    }
                }
            }
            catch (Exception ex) {
                // TODO log
                Dispatcher.Invoke(() => { MessageBox.Show(ex.Message); });
                throw;
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
