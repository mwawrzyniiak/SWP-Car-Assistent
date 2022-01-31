using Microsoft.Speech.Recognition;
using Microsoft.Speech.Synthesis;
using SWPCarAssistent.Core.Common.Entities;
using SWPCarAssistent.Infrastructure.Clients;
using SWPCarAssistent.Infrastructure.Configurations;
using SWPCarAssistent.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace SWPCarAssistent
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly SimpleAppConfigurations simpleAppConfigurations;
        private readonly OpenWeatherApiHttpClient openWeatherApiHttpClient;
        private static SpeechSynthesizer ss;
        private static SpeechRecognitionEngine sre;

        private Grammar carAssistentGrammar;
        private Grammar phoneInteractionGrammar;
        private Grammar radioGrammar;
        private MediaPlayer mediaPlayer = new MediaPlayer();
        private CarRepository carRepository;

        private bool start = false;
        private bool polonez = false;
        public static StartupParams startupParamshelper = new StartupParams();


        public MainWindow()
        {
            InitializeComponent();

            ResizeMode = ResizeMode.NoResize;

            simpleAppConfigurations = new SimpleAppConfigurations();
            openWeatherApiHttpClient = new OpenWeatherApiHttpClient(simpleAppConfigurations.API_KEY);
            carRepository = new CarRepository();

            listBoxHelper.Visibility = Visibility.Collapsed;
            el1Icon.Visibility = Visibility.Collapsed;
            el2Icon.Visibility = Visibility.Collapsed;
            el3Icon.Visibility = Visibility.Collapsed;


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

            phoneInteractionGrammar = new Grammar("Grammars\\PhoneInteractionGrammar.xml");
            phoneInteractionGrammar.Enabled = true;

            radioGrammar = new Grammar("Grammars\\RadioGrammar.xml");
            radioGrammar.Enabled = true;

            sre.LoadGrammar(carAssistentGrammar);
            sre.RecognizeAsync(RecognizeMode.Multiple);

            ss.SpeakAsync("Cześć! Jestem asystentem samochodu Polonez. Powiedz \"Hej Polonez\", aby rozpocząć.");
        }

        private void Sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            float confidence = e.Result.Confidence;
            listBoxHelper.Visibility = Visibility.Collapsed;

            if (confidence <= 0.6)
            {
                ss.SpeakAsync("Nie rozumiem, możesz powtórzyć?");
            }
            else
            {
                if (polonez == false && e.Result.Semantics["polonez"].Value.ToString() == "on")
                {
                    polonez = true;
                    ss.SpeakAsync("Cześć! Co działamy?");
                    textBlock1.Text = "Witaj w programie inteligentnego Poloneza!\n\n";
                    textBlock1.Text += "Jeżeli chcesz:\n";
                    textBlock1.Text += " • Odpalić samochód powiedz: \"odpal silnik\"\n";
                    textBlock1.Text += " • Zgasić samochód powiedz: \"zgaś silnik\"\n";
                    textBlock1.Text += " • Włączyć muzykę powiedz: \"puść muzykę\"\n";
                    textBlock1.Text += " • Zadzwonić do kogoś powiedz: \"kontakty\"\n";
                    textBlock1.Text += " • Załadować domyślne ustawienia samochodu powiedz: \n \"załaduj domyślne ustawienia\"\n";
                    textBlock1.Text += " • Zapisać aktualne ustawienia powiedz: \n \"zapisz konfiguracje\"\n";

                    return;
                }
                if (polonez == true)
                {
                    if (e.Result.Semantics["start"].Value.ToString() == "offEngine" || e.Result.Semantics["start"].Value.ToString() == "onEngine")
                    {
                        if (e.Result.Semantics["start"].Value.ToString() == "offEngine" && start == true)
                        {
                            start = false;
                            el1Icon.Visibility = Visibility.Collapsed;
                            el2Icon.Visibility = Visibility.Collapsed;
                            el3Icon.Visibility = Visibility.Collapsed;
                            ss.SpeakAsync("Silnik został wyłączony");
                            return;
                        }
                        if (e.Result.Semantics["start"].Value.ToString() == "onEngine" && start == false)
                        {
                            start = true;
                            el1Icon.Visibility = Visibility.Visible;
                            el2Icon.Visibility = Visibility.Visible;
                            el3Icon.Visibility = Visibility.Visible;

                            ss.SpeakAsync("Silnik został włączony");
                            return;
                        }
                    }

                    if (e.Result.Semantics["weather"].Value.ToString() != "null" && e.Result.Semantics["weather"].ToString() != null)
                    {
                        WeatherDialogue(e);
                    }
                    else if (e.Result.Semantics["telephoneContacts"].Value.ToString() != "null" && e.Result.Semantics["telephoneContacts"].ToString() != null)
                    {
                        sre.UnloadAllGrammars();
                        sre.SpeechRecognized -= Sre_SpeechRecognized;
                        sre.SpeechRecognized += Sre_SpeechRecognizedTelephoneNumbers;
                        sre.LoadGrammar(phoneInteractionGrammar);
                        ss.SpeakAsync("Co chcesz zrobić z listą kontaktów? Wyświetlić czy do kogoś zadzwonić?");
                        textBlock1.Text = "                               Witaj w kontaktach\n\n";
                        textBlock1.Text += "Aby wyświetlić kontakty powiedz: \"Wyświetl kontakty\"\n";
                        textBlock1.Text += "Jeżeli chcesz zadzwonić do kogoś powiedz np.: \"Zadzwoń do brata\"\n";
                        return;
                    }
                    else if (e.Result.Semantics["turnOnRadio"].Value.ToString() != "null" && e.Result.Semantics["turnOnRadio"].ToString() != null)
                    {
                        if (startupParamshelper.Radio == false)
                        {
                            startupParamshelper.Radio = true;
                            ss.SpeakAsync("Włączam radio");
                        }
                        sre.UnloadAllGrammars();
                        sre.SpeechRecognized -= Sre_SpeechRecognized;
                        sre.SpeechRecognized += Sre_SpeechRecognizedRadio;
                        sre.LoadGrammar(radioGrammar);
                        ss.SpeakAsync("Chcesz wyświetlić listę stacji czy puścić wybraną?");
                        textBlock1.Text = "                                 Witaj w radiu\n\n";
                        textBlock1.Text += "Aby wyświetlić stacje powiedz: \"Wyświetl stacje\"\n";
                        textBlock1.Text += "Jeżeli chcesz włączyć radio powiedz np.: \"Puść Eskę\"\n";
                        return;
                    }
                    else
                    {
                        if (start == false)
                        {
                            if (e.Result.Semantics["config"].Value.ToString() != null && e.Result.Semantics["config"].Value.ToString() != "null")
                            {
                                if (e.Result.Semantics["config"].Value.ToString() == "getConfig")
                                    Repozytorium(carRepository);
                                else if (e.Result.Semantics["config"].Value.ToString() == "saveConfig")
                                {
                                    carRepository.ChangeStartupParams(startupParamshelper);
                                    textBlock1.Text = "Zapisano ustawienia samochodu";
                                    ss.SpeakAsync("Zapisano nowe domyślne ustawienia." + "\n" + "Na ekranie widzisz teraz aktualne i zapisane ustawienia");
                                }
                            }
                            else
                            {
                                Dopytywanie(e);
                            }
                        }
                        if (start == true)
                        {
                            Wlaczwylacz(e);
                        }
                    }
                    textBlock1.Text = "";

                    foreach (var helper in startupParamshelper.GetType().GetProperties())
                    {
                        string textValue = "";
                        bool propertyValue = false;
                        try
                        {
                            propertyValue = (bool)(helper.GetValue(startupParamshelper));
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("TODO: wytrych");
                        }

                        switch (helper.Name)
                        {
                            case "Lights":
                                textValue = "Światło: ";
                                if (propertyValue)
                                    textValue += "Włączone";
                                else
                                    textValue += "Wyłączone";
                                break;
                            case "Wipers":
                                textValue = "Wycieraczki: ";
                                if (propertyValue)
                                    textValue += "Włączone";
                                else
                                    textValue += "Wyłączone";
                                break;
                            case "CarWindows":
                                textValue = "Okna: ";
                                if (propertyValue)
                                    textValue += "Otwarte";
                                else
                                    textValue += "Zamknięte";
                                break;
                            case "Radio":
                                textValue = "Radio: ";
                                if (propertyValue)
                                    textValue += "Włączone";
                                else
                                    textValue += "Wyłączone";
                                break;
                            case "AirConditioning":
                                textValue = "Nawiew: ";
                                if (propertyValue)
                                    textValue += "Włączone";
                                else
                                    textValue += "Wyłączone";
                                break;
                            case "Heating":
                                textValue = "Ogrzewanie: ";
                                if (propertyValue)
                                    textValue += "Włączone";
                                else
                                    textValue += "Wyłączone";
                                break;
                            default:
                                break;
                        }
                        if (helper.PropertyType != typeof(int))
                        {
                            textBlock1.Text += textValue + "\n";
                        }
                        else
                        {
                            textBlock1.Text += "Oto stan Twojego samochodu:" + "\n";
                        }
                    }
                }
            }
        }

        private void WeatherDialogue(SpeechRecognizedEventArgs e)
        {
            var city = e.Result.Semantics["weather"].Value.ToString();
            var task = Task.Run(async () => await openWeatherApiHttpClient.GetQueryAsync(city));
            var weatherRoot = task.Result;

            int tempCel = (int)(weatherRoot.WeatherRoot.main.temp - 273.15);
            int feelsTempCel = (int)(weatherRoot.WeatherRoot.main.feels_like - 273.15);

            this.tempLbl.Content = tempCel + " °C";
            this.cityLbl.Content = city;

            string endOfTempCel = " stopni Celsjusza";
            string endOfFeelsTempCel = " stopni Celsjusza";
            
            if(Math.Abs(tempCel) == 1)
                endOfTempCel = " stopień Celsjusza";
            else if(Math.Abs(tempCel) >= 2 && Math.Abs(tempCel) < 5)
                endOfTempCel = " stopnie Celsjusza";

            if (Math.Abs(feelsTempCel) == 1)
                endOfFeelsTempCel = " stopień Celsjusza";
            else if (Math.Abs(feelsTempCel) >= 2 && Math.Abs(feelsTempCel) < 5)
                endOfFeelsTempCel = " stopnie Celsjusza";


            ss.SpeakAsync("Pogoda w mieście " + city + " jest następująca");

            if (tempCel > 0)
                ss.SpeakAsync("Temperatura wynosi " + tempCel.ToString() + endOfTempCel);
            else if (tempCel == 0)
                ss.SpeakAsync("Temperatura wynosi zero stopni Celsjusza");
            else
                ss.SpeakAsync("Temperatura wynosi minus " + tempCel.ToString() + endOfTempCel);

            if (feelsTempCel > 0)
                ss.SpeakAsync("Temperatura odczuwalna to " + feelsTempCel.ToString() + endOfFeelsTempCel);
            else if (feelsTempCel == 0)
                ss.SpeakAsync("Temperatura odczuwalna to zero stopni Celsjusza");
            else
                ss.SpeakAsync("Temperatura odczuwalna to minus " + feelsTempCel.ToString() + endOfFeelsTempCel);

            int weatherRootWindSpeed = (int)weatherRoot.WeatherRoot.wind.speed;
            string endOfString = " metrów na sekundę";

            if (weatherRootWindSpeed == 0)
                weatherRootWindSpeed++;
            if (weatherRootWindSpeed == 1)
                endOfString = " metr na sekundę";
            else if (weatherRootWindSpeed > 1 && weatherRootWindSpeed <= 4)
                endOfString = " metry na sekundę";

            ss.SpeakAsync("Dodatkowo aktualny wiatr wieje z prędkością " + weatherRootWindSpeed.ToString() + endOfString);
        }

        private void Repozytorium(CarRepository carRepository)
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

        private void Sre_SpeechRecognizedTelephoneNumbers(object sender, SpeechRecognizedEventArgs e)
        {
            float confidence = e.Result.Confidence;
            if (confidence <= 0.6)
            {
                ss.SpeakAsync("Nie rozumiem, możesz powtórzyć?");
            }
            else
            {
                if (e.Result.Semantics["showNumbers"].Value.ToString() == "showNumbers")
                {
                    List<string> itemSource = new List<string>();
                    var contacts = carRepository.GetAllContacts();
                    listBoxHelper.Visibility = Visibility.Visible;

                    foreach (var contact in contacts)
                        itemSource.Add(contact.FullName + " " + contact.PhoneNumber);

                    listBoxHelper.ItemsSource = itemSource;
                    ss.SpeakAsync("Wyświetlam listę dostępnych kontaktów");
                }
                else if (e.Result.Semantics["callTo"].Value.ToString() != "null" && e.Result.Semantics["callTo"].ToString() != null)
                {
                    ss.SpeakAsync("Dzwonię do " + e.Result.Semantics["callTo"].Value.ToString());

                    sre.UnloadAllGrammars();
                    sre.SpeechRecognized -= Sre_SpeechRecognizedTelephoneNumbers;
                    sre.SpeechRecognized += Sre_SpeechRecognized;
                    sre.LoadGrammar(carAssistentGrammar);
                    listBoxHelper.Visibility = Visibility.Collapsed;
                }
            }
        }
        private void Sre_SpeechRecognizedRadio(object sender, SpeechRecognizedEventArgs e)
        {
            float confidence = e.Result.Confidence;
            if (confidence <= 0.6)
            {
                ss.SpeakAsync("Nie rozumiem, możesz powtórzyć?");
            }
            else
            {
                if (e.Result.Semantics["Radios"].Value.ToString() == "Radios")
                {
                    List<string> itemSource = new List<string>();
                    var radios = carRepository.GetAllRadios();
                    listBoxHelper.Visibility = Visibility.Visible;

                    foreach (var radio in radios)
                        itemSource.Add(radio.RadioName + " " + radio.Frequency);

                    listBoxHelper.ItemsSource = itemSource;

                    ss.SpeakAsync("Wyświetlam listę stacji radiowych");
                }
                else if (e.Result.Semantics["station"].Value.ToString() != "null" && e.Result.Semantics["station"].ToString() != null)
                {
                    listBoxHelper.Visibility = Visibility.Collapsed;

                    if (e.Result.Semantics["station"].Value.ToString() == "Eska")
                    {
                        radioLbl.Content = "Radio Eska";
                        string path = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, @"Voice\", "Radio2.mp3");
                        Uri uri = new Uri(path);
                        mediaPlayer.Volume = 0.05;
                        mediaPlayer.Open(uri);
                        mediaPlayer.Play();
                    }
                    if (e.Result.Semantics["station"].Value.ToString() == "RadioZet")
                    {
                        radioLbl.Content = "Radio Zet";

                        string path = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, @"Voice\", "Radio1.mp3");
                        Uri uri = new Uri(path);
                        mediaPlayer.Volume = 0.05;
                        mediaPlayer.Open(uri);
                        mediaPlayer.Play();
                    }
                    if (e.Result.Semantics["station"].Value.ToString() == "RadioMaryja")
                    {
                        radioLbl.Content = "Radio Maryja";

                        string path = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, @"Voice\", "Radio4.mp3");
                        Uri uri = new Uri(path);
                        mediaPlayer.Volume = 0.05;
                        mediaPlayer.Open(uri);
                        mediaPlayer.Play();
                    }
                    if (e.Result.Semantics["station"].Value.ToString() == "VoxFm")
                    {
                        radioLbl.Content = "Radio VoxFm";

                        string path = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, @"Voice\", "Radio3.mp3");
                        Uri uri = new Uri(path);
                        mediaPlayer.Volume = 0.05;
                        mediaPlayer.Open(uri);
                        mediaPlayer.Play();
                    }
                    if (e.Result.Semantics["station"].Value.ToString() == "PolskieRadio")
                    {
                        radioLbl.Content = "Polskie Radio";

                        string path = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, @"Voice\", "Radio5.mp3");
                        Uri uri = new Uri(path);
                        mediaPlayer.Volume = 0.05;
                        mediaPlayer.Open(uri);
                        mediaPlayer.Play();
                    }
                    if (e.Result.Semantics["station"].Value.ToString() == "RmfFm")
                    {
                        radioLbl.Content = "Polskie RmfFm";

                        string path = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, @"Voice\", "Radio6.mp3");
                        Uri uri = new Uri(path);
                        mediaPlayer.Volume = 0.05;
                        mediaPlayer.Open(uri);
                        mediaPlayer.Play();
                    }

                    ss.SpeakAsync("Aktualnie leci:  " + e.Result.Semantics["station"].Value.ToString());
                    sre.UnloadAllGrammars();
                    sre.SpeechRecognized -= Sre_SpeechRecognizedRadio;
                    sre.SpeechRecognized += Sre_SpeechRecognized;
                    sre.LoadGrammar(carAssistentGrammar);
                }
            }
        }

        private void Wlaczwylacz(SpeechRecognizedEventArgs e)
        {
            string lights = e.Result.Semantics["lights"].Value.ToString();
            if (lights == "onLights")
            {
                if (startupParamshelper.Lights != false)
                {
                    ss.SpeakAsync("Światła są już włączone");

                }
                else
                {
                    ss.SpeakAsync("Włączono światła");
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
            string wipers = e.Result.Semantics["wipers"].Value.ToString();
            if (wipers == "onWipers")
            {
                if (startupParamshelper.Wipers != false)
                {
                    ss.SpeakAsync("Wycieraczki są już włączone");

                }
                else
                {
                    ss.SpeakAsync("Włączono wycieraczki");
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
            string carWindows = e.Result.Semantics["carWindows"].Value.ToString();
            if (carWindows == "openCarWindows")
            {
                if (startupParamshelper.CarWindows != false)
                {
                    ss.SpeakAsync("Szyby są już otwarte");

                }
                else
                {
                    ss.SpeakAsync("Otworzono szyby");
                    startupParamshelper.CarWindows = true;
                }
            }
            else
            {
                if (carWindows == "closeCarWindows")
                {
                    ss.SpeakAsync("Zamknięto szyby");
                    startupParamshelper.CarWindows = false;
                }
            }
            string radio = e.Result.Semantics["radio"].Value.ToString();
            if (radio == "onRadio")
            {
                if (startupParamshelper.Radio != false)
                {
                    ss.SpeakAsync("Radio jest już włączone");

                }
                else
                {
                    ss.SpeakAsync("Włączono radio");
                    startupParamshelper.Radio = true;
                }
            }
            else
            {
                if (radio == "offRadio")
                {
                    ss.SpeakAsync("Wyłączono radio");
                    radioLbl.Content = "Radio wyłączone";

                    mediaPlayer.Stop();
                    startupParamshelper.Radio = false;
                }
            }

            string airConditioning = e.Result.Semantics["airConditioning"].Value.ToString();
            if (airConditioning == "onAirConditioning")
            {
                if (startupParamshelper.AirConditioning != false)
                {
                    startupParamshelper.AirConditioning = true;
                    ss.SpeakAsync("Nawiew jest już włączony");

                }
                else
                {
                    ss.SpeakAsync("Włączono nawiew");
                    startupParamshelper.AirConditioning = true;
                }
            }
            else
            {
                if (airConditioning == "offAirConditioning")
                {
                    ss.SpeakAsync("Wyłączono nawiew");
                    startupParamshelper.AirConditioning = false;
                }
            }
            string heating = e.Result.Semantics["heating"].Value.ToString();
            if (heating == "onHeating")
            {
                if (startupParamshelper.Heating != false)
                {
                    ss.SpeakAsync("Ogrzewanie jest już włączone");

                }
                else
                {
                    ss.SpeakAsync("Włączono ogrzewanie");
                    startupParamshelper.Heating = true;
                }
            }
            else
            {
                if (heating == "offHeating")
                {
                    ss.SpeakAsync("Wyłączono ogrzewanie");
                    startupParamshelper.Heating = false;
                }
            }
        }
        private void Dopytywanie(SpeechRecognizedEventArgs e)
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
                        radioLbl.Content = "Radio wyłączone";
                        mediaPlayer.Stop();
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
            int a = 0;
            if (startupParams != null)
            {
                ss.SpeakAsync("Chcesz jechać jak zawsze, w związku z tym załadowano domyślne ustawienia:");
                if (startupParams.Lights == true && startupParamshelper.Lights == false)
                {
                    ss.SpeakAsync("Włączono światła");
                    a++;
                }
                if (startupParams.Wipers == true && startupParamshelper.Wipers == false)
                {
                    ss.SpeakAsync("Włączono Wycieraczki");
                    a++;
                }
                if (startupParams.CarWindows == true && startupParamshelper.CarWindows == false)
                {
                    ss.SpeakAsync("Otwarto szyby");
                    a++;
                }
                if (startupParams.Radio == true && startupParamshelper.Radio == false)
                {
                    ss.SpeakAsync("Włączono Radio");
                    a++;
                }
                if (startupParams.AirConditioning == true && startupParamshelper.AirConditioning == false)
                {
                    ss.SpeakAsync("Włączono nawiew");
                    a++;
                }
                if (startupParams.Heating == true && startupParamshelper.Heating == false)
                {
                    ss.SpeakAsync("Włączono ogrzewanie");
                    a++;
                }
                if (a == 0)
                {
                    ss.SpeakAsync("Samochód już jest ustawiony");
                }
                return startupParams;
            }
            return null;
        }
    }
}
