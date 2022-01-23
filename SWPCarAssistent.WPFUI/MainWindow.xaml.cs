using Microsoft.Speech.Recognition;
using Microsoft.Speech.Synthesis;
using SWPCarAssistent.Infrastructure.Repositories;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using Microsoft.Win32;
using System.IO;
using SWPCarAssistent.Core.Common.Entities;

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
        private bool start = false;
        public static StartupParams startupParamshelper = new StartupParams();

        public MainWindow()
        {
            InitializeComponent();
            //string path = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, @"Voice\", "rafonix we nie kozacz.mp3");
            //Uri uri = new Uri(path);
            // mediaPlayer.Open(uri);
            //mediaPlayer.Play();
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
            if (confidence <= 0.6)
            {
                ss.SpeakAsync("Nie rozumiem, możesz powtórzyć?");

            }
            else
            {
                if (e.Result.Semantics["weather"].Value.ToString() != "null" && e.Result.Semantics["weather"].ToString() != null)
                {
                    ss.SpeakAsync(e.Result.Semantics["weather"].Value.ToString());
                }
                else
                {
                    if (e.Result.Semantics["config"].Value.ToString() != null && e.Result.Semantics["config"].Value.ToString() != "null")
                    {
                        var startupParam = GetStartupParamsFromRepository(carRepository);
                        textBlock1.Text = "";
                        foreach (var param in startupParamshelper.GetType().GetProperties())
                        {
                            if (param.GetValue(startupParamshelper).ToString() == "False")
                            {
                                if (param.Name.ToString() == "CarWindows")
                                {
                                    if (startupParamshelper.CarWindows == false && startupParam.CarWindows == true)
                                    {
                                        textBlock1.Text += "Otwarto szyby\n";
                                        startupParamshelper.CarWindows = true;
                                    }
                                    else
                                    {
                                        textBlock1.Text += "Szyby są już otwarte\n";
                                    }
                                }
                                if (param.Name.ToString() == "AirConditioning")
                                {
                                    if (startupParamshelper.AirConditioning == false && startupParam.AirConditioning == true)
                                    {
                                        textBlock1.Text += "Włączono nawiew\n";
                                        startupParamshelper.AirConditioning = true;
                                    }
                                    else
                                    {
                                        textBlock1.Text += "Nawiew jest już włączony\n";
                                    }
                                }
                                if (param.Name.ToString() == "Heating")
                                {
                                    if (startupParamshelper.Heating == false && startupParam.Heating == true)
                                    {
                                        textBlock1.Text += "Włączono ogrzewanie\n";
                                        startupParamshelper.Heating = true;
                                    }
                                    else
                                    {
                                        textBlock1.Text += "Ogrzewanie jest już włączone\n";
                                    }
                                }
                                if (param.Name.ToString() == "Radio")
                                {
                                    if (startupParamshelper.Radio == false && startupParam.Radio == true)
                                    {
                                        textBlock1.Text += "Włączono radio\n";
                                        startupParamshelper.Radio = true;
                                    }
                                    else
                                    {
                                        textBlock1.Text += "Radio jest już włączone\n";
                                    }
                                }
                                if (param.Name.ToString() == "Wipers")
                                {
                                    if (startupParamshelper.Wipers == false && startupParam.Wipers == true)
                                    {
                                        textBlock1.Text += "Włączono wycieraczki\n";
                                        startupParamshelper.Wipers = true;
                                    }
                                    else
                                    {
                                        textBlock1.Text += "Wycieraczki są już włączone\n";
                                    }
                                }
                                if (param.Name.ToString() == "Lights")
                                {
                                    if (startupParamshelper.Lights == false && startupParam.Lights == true)
                                    {
                                        textBlock1.Text += "Włączono światła\n";
                                        startupParamshelper.Lights = true;
                                    }
                                    else
                                    {
                                        textBlock1.Text += "Światła są już włączone\n";
                                    }
                                }

                            }
                        }
                    }
                    else
                    {
                        Dopytywanie(e);
                    }
                }
            }
        }

        private static void Dopytywanie(SpeechRecognizedEventArgs e)
        {
            string lights = e.Result.Semantics["lights"].Value.ToString();
            if (lights == "null" && startupParamshelper.Lights == false)
            {
                ss.SpeakAsync("Czy włączyć światła?");
            }
            else
            {
                if (lights == "onLights")
                {
                    if (startupParamshelper.Lights != false)
                    {
                        ss.SpeakAsync("Światła są już włączone");

                    }
                    else
                    {
                        startupParamshelper.Lights = true;
                    }
                }
                else
                {
                    if (lights == "offLights" && startupParamshelper.Lights == true)
                    {
                        ss.SpeakAsync("Wyłączono światła");
                        startupParamshelper.Lights = false;
                    }
                }
            }
            string wipers = e.Result.Semantics["wipers"].Value.ToString();
            if (wipers == "null" && startupParamshelper.Wipers == false)
            {
                ss.SpeakAsync("Czy włączyć wycieraczki?");
            }
            else
            {
                if (wipers == "onWipers")
                {
                    if (startupParamshelper.Wipers != false)
                    {
                        ss.SpeakAsync("Wycieraczki są już włączone");

                    }
                    else
                    {
                        startupParamshelper.Wipers = true;
                    }
                }
                else
                {
                    if (wipers == "offWipers" && startupParamshelper.Wipers == true)
                    {
                        ss.SpeakAsync("Wyłączono wycieraczki");
                        startupParamshelper.Wipers = false;
                    }
                }
            }
            string carWindows = e.Result.Semantics["carWindows"].Value.ToString();
            if (carWindows == "null" && startupParamshelper.CarWindows == false)
            {
                ss.SpeakAsync("Czy uchylić szyby?");
            }
            else
            {
                if (carWindows == "openCarWindows")
                {
                    if (startupParamshelper.CarWindows != false)
                    {
                        ss.SpeakAsync("Szyby są już otworzone");

                    }
                    else
                    {
                        startupParamshelper.CarWindows = true;
                    }
                }
                else
                {
                    if (carWindows == "closeCarWindows")
                    {
                        startupParamshelper.CarWindows = false;
                    }
                }
            }
            string radio = e.Result.Semantics["radio"].Value.ToString();
            if (radio == "null" && startupParamshelper.Radio == false)
            {
                ss.SpeakAsync("Czy włączyć radio?");
            }
            else
            {
                if (radio == "onRadio")
                {
                    if (startupParamshelper.Radio != false)
                    {
                        ss.SpeakAsync("Radio jest już włączone");

                    }
                    else
                    {
                        startupParamshelper.Radio = true;
                    }
                }
                else
                {
                    if (radio == "offRadio")
                    {
                        startupParamshelper.Radio = false;
                    }
                }
            }
            string airConditioning = e.Result.Semantics["airConditioning"].Value.ToString();
            if (airConditioning == "null" && startupParamshelper.AirConditioning == false)
            {
                ss.SpeakAsync("Czy włączyć nawiew?");
            }
            else
            {
                if (airConditioning == "onAirConditioning")
                {
                    if (startupParamshelper.AirConditioning != false)
                    {
                        startupParamshelper.AirConditioning = true;
                        ss.SpeakAsync("Nawiew jest już włączone");

                    }
                    else
                    {
                        startupParamshelper.AirConditioning = true;
                    }
                }
                else
                {
                    if (airConditioning == "offAirConditioning")
                    {
                        startupParamshelper.AirConditioning = false;
                    }
                }
            }
            string heating = e.Result.Semantics["heating"].Value.ToString();
            if (heating == "null" && startupParamshelper.Heating == false)
            {
                ss.SpeakAsync("Czy włączyć ogrzewanie?");
            }
            else
            {
                if (heating == "onHeating")
                {
                    if (startupParamshelper.Heating != false)
                    {
                        ss.SpeakAsync("Ogrzewanie jest już włączone");

                    }
                    else
                    {
                        startupParamshelper.Heating = true;
                    }
                }
                else
                {
                    if (heating == "offHeating")
                    {
                        startupParamshelper.Heating = false;
                    }
                }
            }
        }

        private StartupParams GetStartupParamsFromRepository(CarRepository carRepository)
        {
            var startupParams = carRepository.GetStartupParams();
            if (startupParams != null)
            {
                ss.SpeakAsync("Chcesz jechać jak zawsze, w związku z tym:");
                if (startupParams.Lights == true)
                {
                    ss.SpeakAsync("Włączono światła");
                }
                if (startupParams.Wipers == true)
                {
                    ss.SpeakAsync("Włączono Wycieraczki");
                }
                if (startupParams.CarWindows == true)
                {
                    ss.SpeakAsync("Otwarto szyby");
                }
                if (startupParams.Radio == true)
                {
                    ss.SpeakAsync("Włączono Radio");
                }
                if (startupParams.AirConditioning == true)
                {
                    ss.SpeakAsync("Włączono nawiew");
                }
                if (startupParams.Heating == true)
                {
                    ss.SpeakAsync("Włączono ogrzewanie");
                }
                return startupParams;
            }
            return null;
        }
    }
}
