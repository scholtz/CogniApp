using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Emotiv2GoogleFit
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        CogniAppProvider cogniAppProvider = new CogniAppProvider(System.Configuration.ConfigurationManager.AppSettings["cogniapp-address"]);
        GoogleFit googleFit = null;
        Struct.Device device = null;
        public MainWindow()
        {
            InitializeComponent();
            logMessage("Welcome to the CogniApp recorder. This application can help you connect to CogniApp games or store your BCI data for further medical research.");

            google.Text = System.Configuration.ConfigurationManager.AppSettings["defaultGmail"];
            disableGoogle();


            
            cogniAppProvider.OnMessage += delegate (object s, string m) {
                logMessage(m);
            };
            cogniAppProvider.OnError += delegate (object s, string m) {
                logMessage("Error: " + m);
            };

            number.Content = "Your current CogniApp number: " + cogniAppProvider.number;

        }

        Listener listener = null;
        private void enableGoogle()
        {
            consent.IsEnabled = true;
            google.IsEnabled = true;
            btnSetGoogle.IsEnabled = true;


        }
        private void disableGoogle()
        {

            consent.IsEnabled = false;
            google.IsEnabled = false;
            btnSetGoogle.IsEnabled = false;

        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                Task.Run(() => {

                    cogniAppProvider.start();

                });

                listener = new Listener(this.login.Text, this.pass.Password);
                listener.OnMessage += delegate (object s, string m) {
                    logMessage(m);
                };
                listener.OnError += delegate (object s, string m) {
                    logMessage("Error: " + m);
                };
                listener.OnDeviceChange += delegate (object s, Struct.Device dev) {

                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        this.device = dev;
                        enableGoogle();
                    }));
                };
                listener.OnStreamDataReceived += delegate (object s, Dictionary<string, Dictionary<string, object>> streamData) {
                    if (streamData.ContainsKey("met"))
                    {
                        Task.Run(() => googleFit?.NewDataPoint(streamData));
                        Task.Run(() => cogniAppProvider?.NewDataPoint(streamData));
                    }
                };
                listener.Start();
            }
            catch (Exception exc)
            {
                logMessage("Error: " + exc.Message);
            }
        }

        private void logMessage(string text)
        {
            try
            {
                string append = DateTimeOffset.Now.ToString("HH:mm:ss") + ": " + text + "\n";
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    richTextBoxLog.AppendText(append);
                }
                ));

            }
            catch (Exception exc)
            {
                Console.Error.WriteLine(exc.Message);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            googleFit = new GoogleFit(google.Text, device);

        }
    }
}
