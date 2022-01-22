using Microsoft.Speech.Recognition;
using Microsoft.Speech.Synthesis;
using SWPCarAssistent.Infrastructure.Repositories;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using Microsoft.Win32;
using System.IO;

namespace SWPCarAssistent
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static SpeechSynthesizer ss;
        private static SpeechRecognitionEngine sre;
        private Grammar carAssistentGrammar;
        private MediaPlayer mediaPlayer = new MediaPlayer();

        public MainWindow()
        {
            InitializeComponent();
            string path = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, @"Voice\", "rafonix2.mp3");
            Uri uri = new Uri(path);
            mediaPlayer.Open(uri);
            mediaPlayer.Play();
            ConfigureSpeecher();
        }

        private void ConfigureSpeecher()
        {
            ss = new SpeechSynthesizer();
            ss.SetOutputToDefaultAudioDevice();

            var cultureInfo = new CultureInfo("pl-PL");
            sre = new SpeechRecognitionEngine(cultureInfo);
            sre.SetInputToDefaultAudioDevice();
            sre.SpeechRecognized += Sre_SpeechRecognized;

            carAssistentGrammar = new Grammar("Grammars\\CarAssistantGrammar.xml");
            carAssistentGrammar.Enabled = true; 
            sre.LoadGrammar(carAssistentGrammar);
            sre.RecognizeAsync(RecognizeMode.Multiple);

            //ss.SpeakAsync("Cześć! Jestem asystentem samochodu Polonez. Powiedz \"Hej Polonez\", aby rozpocząć.");
        }

        private void Sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            CarRepository carRepository = new CarRepository();
            float confidence = e.Result.Confidence;
            mediaPlayer.Open(new Uri("pack://application:,,,/Voice/rafonix2.mp3"));
            mediaPlayer.Play();

            if (confidence <= 0.6)
            {
                ss.SpeakAsync("Nie rozumiem, możesz powtórzyć?");

            }
            else
            {
                string lights = e.Result.Semantics["lights"].Value.ToString();
                string wipers = e.Result.Semantics["wipers"].Value.ToString();
                string carWindows = e.Result.Semantics["carWindows"].Value.ToString();
                string radio = e.Result.Semantics["radio"].Value.ToString();


                if(e.Result.Semantics["config"].Value.ToString() != null)
                {
                    var startupParams = carRepository.GetStartupParams();
                    if (startupParams != null)
                    {
                        ss.SpeakAsync("Chcesz jechać jak zawsze, w związku z tym:");
                        if(startupParams.CarWindows == true)
                        {
                            ss.SpeakAsync("Włączono szyby");
                        }
                        if (startupParams.AirConditioning == true)
                        {
                            ss.SpeakAsync("Włączono klimatyzację");
                        }
                        if (startupParams.Heating == true)
                        {
                            ss.SpeakAsync("Włączono ogrzewanie");
                        }
                        if (startupParams.Radio == true)
                        {
                            ss.SpeakAsync("Włączono Radio");
                        }
                        if (startupParams.Wipers == true)
                        {
                            ss.SpeakAsync("Włączono Wycieraczki");
                        }
                        if (startupParams.Lights == true)
                        {
                            ss.SpeakAsync("Włączono światła");
                        }
                    }
                }
            }
        }
    }
}
