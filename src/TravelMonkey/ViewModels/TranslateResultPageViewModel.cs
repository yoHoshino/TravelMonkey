using System.Collections.Generic;
using System.Linq;
using TravelMonkey.Services;
using Xamarin.Forms;
using Xamarin.Essentials;

namespace TravelMonkey.ViewModels
{
    public class TranslateResultPageViewModel : BaseViewModel
    {
        private readonly TranslationService _translationService =
            new TranslationService();

        private string _inputText;
        private Dictionary<string, string> _translations;

        public string InputText
        {
            get => _inputText;
            set
            {
                if (_inputText == value)
                    return;

                Set(ref _inputText, value);

                TranslateText();
            }
        }

        public Dictionary<string, string> Translations
        {
            get => _translations;
            set
            {
                Set(ref _translations, value);
            }
        }

        public Command<string> TranslateTextCommand => new Command<string>((inputText) =>
        {
            InputText = inputText;
        });

        public Command<string> SpeechTextCommand => new Command<string>(async key =>
        {
            var locales = await TextToSpeech.GetLocalesAsync();
            var local = locales.FirstOrDefault(l => l.Language == key);

            var value = _translations[key];

            await TextToSpeech.SpeakAsync(value, new SpeechOptions {Locale = local});
        });

        private async void TranslateText()
        {
            var result = await _translationService.TranslateText(_inputText);

            if (!result.Succeeded)
                MessagingCenter.Send(this, Constants.TranslationFailedMessage);

            Translations = result.Translations;
        }
    }
}