using Microsoft.Speech.Recognition;
using Microsoft.Speech.Synthesis;
using SWPCarAssistent.Infrastructure.Repositories;
using System;
using System.Globalization;
using System.Windows;

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

        public MainWindow()
        {
            InitializeComponent();
            CarRepository cr = new CarRepository();

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

            ss.SpeakAsync("Cześć! Jestem asystentem samochodu Polonez. Powiedz \"Hej Polonez\", aby rozpocząć.");
        }

        private void Sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            float confidence = e.Result.Confidence;

            if (confidence <= 0.6)
            {
                ss.SpeakAsync("Nie zrozumiałem, proszę powtórzyć");
            }
            else
            {
                string lights = e.Result.Semantics["lights"].Value.ToString();
                string wipers = e.Result.Semantics["wipers"].Value.ToString();
                string carWindows = e.Result.Semantics["carWindows"].Value.ToString();
                string radio = e.Result.Semantics["radio"].Value.ToString();

                ss.SpeakAsync(lights);
                ss.SpeakAsync(wipers);
                ss.SpeakAsync(carWindows);
                ss.SpeakAsync(radio);
            }
        }
    }
}
