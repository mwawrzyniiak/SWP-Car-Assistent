using System;
using System.Collections.Generic;
using System.Globalization;
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
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;
using Microsoft.Speech.Synthesis;
using SWPCarAssistent.Infrastructure.Repositories;

namespace SWPCarAssistent
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static SpeechSynthesizer ss;
        static SpeechRecognitionEngine sre;



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

            ss.SpeakAsync("Witaj ponownie, od czego zaczynamy?");
        }

        private void Sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
