using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using SYSTEMTASK4.Models;

namespace SYSTEMTASK4
{

    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public ObservableCollection<Car> carList { get; set; }
        private Stopwatch _stopwatch;
        private DispatcherTimer _timer;
        private TimeSpan _timerValue;

        public TimeSpan Timer
        {
            get { return _timerValue; }
            set
            {
                _timerValue = value;
                OnPropertyChanged();
            }
        }
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        CancellationTokenSource token = new CancellationTokenSource();
        public MainWindow()
        {
            InitializeComponent();
            _stopwatch = new Stopwatch();
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;

            carList = new ObservableCollection<Car> { };
            carListView.ItemsSource = carList;
            DataContext = this;

        }






        public event PropertyChangedEventHandler? PropertyChanged;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                token = new CancellationTokenSource();
                _stopwatch.Start();
                _timer.Start();
                carList = new ObservableCollection<Car> { };
                carListView.ItemsSource = carList;

                if (Single2.IsChecked == true)
                {

                    ThreadPool.QueueUserWorkItem((state) =>
                    {
                        Thread.Sleep(2000);
                        CancellationToken cancellation = (CancellationToken)state;
                        if (cancellation.IsCancellationRequested)
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                MessageBox.Show("Cancelled");


                            });
                            Thread.Sleep(500);
                            return;
                        }
                        else
                        {
                            Thread.Sleep(2000);
                            var jsonFiles = new List<string>
                    {
                        "C:\\Users\\Mirtalib\\source\\repos\\SYSTEMTASK4\\SYSTEMTASK4\\Jsons\\j1.json",
                        "C:\\Users\\Mirtalib\\source\\repos\\SYSTEMTASK4\\SYSTEMTASK4\\Jsons\\j2.json",
                        "C:\\Users\\Mirtalib\\source\\repos\\SYSTEMTASK4\\SYSTEMTASK4\\Jsons\\j3.json",
                        "C:\\Users\\Mirtalib\\source\\repos\\SYSTEMTASK4\\SYSTEMTASK4\\Jsons\\j4.json",
                        "C:\\Users\\Mirtalib\\source\\repos\\SYSTEMTASK4\\SYSTEMTASK4\\Jsons\\j5.json"
                    };
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                carList.Clear();


                            });
                            foreach (var path in jsonFiles)
                            {
                                string text = File.ReadAllText(path);
                                var datas = JsonSerializer.Deserialize<List<Car>>(text);
                                Thread.Sleep(900);


                                Application.Current.Dispatcher.Invoke(() =>
                                {

                                    if (datas != null)
                                    {
                                        foreach (var data in datas)
                                        {
                                            carList.Add(data);
                                        }
                                    }


                                });
                            }

                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                MessageBox.Show($"Process ended");
                            });
                        }

                    }, token.Token);
                }
                else if (Multi.IsChecked == true)
                {
                   
                       
                        if (token.IsCancellationRequested)
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                MessageBox.Show("Cancelled");
                            });
                            Thread.Sleep(500);
                            return;
                        }
                        else
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                MessageBox.Show("Multi Started");
                                carList.Clear();
                            });

                            var jsonFiles = new List<string>
            {
                "C:\\Users\\Mirtalib\\source\\repos\\SYSTEMTASK4\\SYSTEMTASK4\\Jsons\\j1.json",
                "C:\\Users\\Mirtalib\\source\\repos\\SYSTEMTASK4\\SYSTEMTASK4\\Jsons\\j2.json",
                "C:\\Users\\Mirtalib\\source\\repos\\SYSTEMTASK4\\SYSTEMTASK4\\Jsons\\j3.json",
                "C:\\Users\\Mirtalib\\source\\repos\\SYSTEMTASK4\\SYSTEMTASK4\\Jsons\\j4.json",
                "C:\\Users\\Mirtalib\\source\\repos\\SYSTEMTASK4\\SYSTEMTASK4\\Jsons\\j5.json"
            };


                        foreach (var path in jsonFiles)
                        {

                            new Thread(() =>
                           {
                               try
                               {
                                   Thread.Sleep(1000);
                                   var text = File.ReadAllText(path);
                                   var datas = JsonSerializer.Deserialize<List<Car>>(text);
                                   Thread.Sleep(1000);

                                   if (token.IsCancellationRequested)
                                   {
                                        
                                       return;
                                   }
                                   if (datas != null)
                                   Thread.Sleep(1000);
                                   {
                                       Application.Current.Dispatcher.Invoke(() =>
                                       {
                                           foreach (var car in datas)
                                           {
                                               carList.Add(car);
                                           }
                                       });

                                   }
                                  
                               }

                               catch (Exception ex)
                               {
                                   Application.Current.Dispatcher.Invoke(() =>
                                   {
                                       MessageBox.Show($"Error processing file {path}: {ex.Message}");
                                   });
                               }
                           }).Start();
                        }

                        //

                        
                    }
                  
                }

            }


            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show(ex.Message);
                });
            }
        }




        private void Timer_Tick(object sender, EventArgs e)
        {
            Timer = _stopwatch.Elapsed;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            token.Cancel();
            _stopwatch.Stop();
            _timer.Stop();
            Application.Current.Dispatcher.Invoke(() =>
            {
                MessageBox.Show($"Cancel requested");
            });
        }
    }
}