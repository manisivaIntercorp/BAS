using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Threading.Tasks;
using DataAccessLayer.Model;
using DataAccessLayer.Uow.Interface;
using ICSharpCode.Decompiler.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;

namespace WebApi.Services
{
    public class TranslationService
    {
        //private readonly IStringLocalizer<TranslationService> _globalLocalizer;
        private readonly IStringLocalizer _globalLocalizer;
        private readonly IStringLocalizerFactory _localizerFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TranslationService(
            IStringLocalizer<TranslationService> globalLocalizer,
            IStringLocalizerFactory localizerFactory,
            IHttpContextAccessor httpContextAccessor)
        {
            _globalLocalizer = globalLocalizer;
            _localizerFactory = localizerFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Get translation from resource files
        /// </summary>
        public string GetTranslation(string key, string scope = "Global")
        {
            string langCode = _httpContextAccessor.HttpContext?.Session.GetString("Culture") ?? "en";
            CultureInfo.CurrentUICulture = new CultureInfo(langCode);

            Console.WriteLine($"🔍 Searching Key: {key} in {scope} for {langCode}");

            if (scope.Equals("Global", StringComparison.OrdinalIgnoreCase))
            {
                //var  _globalLocalizer1 = _localizerFactory.Create("TranslationService", Assembly.GetExecutingAssembly().GetName().Name);
                var _globalLocalizer1 = _localizerFactory.Create("Services.TranslationService", "WebApi");

                //foreach (var key1 in _globalLocalizer1.GetAllStrings())
                //{
                //    Console.WriteLine($"Key: {key1.Name}, Value: {key1.Value}");
                //}
                var translation = _globalLocalizer1[key];
                Console.WriteLine($"✅ Found Translation: {translation}");
                return translation ?? $"[{key}]"; // Fallback if missing
            }
            else
            {
                var resourcePath = scope.StartsWith("Services.") ? scope : $"Services.{scope}";
                var localizer = _localizerFactory.Create(resourcePath, Assembly.GetExecutingAssembly().GetName().Name);
                // var localizer = _localizerFactory.Create(scope, Assembly.GetExecutingAssembly().GetName().Name);
                // Get all available strings in the localizer
                //foreach (var key1 in localizer.GetAllStrings(includeParentCultures: true))  // Use 'localizer' instead of '_localizerFactory'
                //{
                //    Console.WriteLine($"Key: {key1.Name}, Value: {key1.Value}");
                //}
                var translation = localizer[key];
                Console.WriteLine($"✅ Found Translation (Scoped): {translation}");
                return translation ?? $"[{key}]";
            }
        }

        /// <summary>
        /// Set the application's language
        /// </summary>
        public void SetLanguage(string languageCode)
        {
            var cultureInfo = new CultureInfo(languageCode);
            CultureInfo.CurrentCulture = cultureInfo;
            CultureInfo.CurrentUICulture = cultureInfo;

            _httpContextAccessor.HttpContext?.Session.SetString("Culture", languageCode);
            Console.WriteLine($"🌍 Language set to {languageCode}");
        }

        /// <summary>
        /// Generate resource files dynamically from database
        /// </summary>
        public async Task GenerateResourceFiles(IUowTranslation _translationRepository)
        {
            var translations = await _translationRepository.GetAllTranslations();
            var groupedTranslations = translations.GroupBy(t => new { t.LanguageCode, t.Scope });

            string resourcesPath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Services");

            if (!Directory.Exists(resourcesPath))
            {
                Directory.CreateDirectory(resourcesPath);
            }

            foreach (var group in groupedTranslations)
            {
                string langCode = group.Key.LanguageCode;
                string scope = string.IsNullOrEmpty(group.Key.Scope) ? "TranslationService" : group.Key.Scope;

                // Check if this is the neutral/default language
                bool isNeutralLanguage = string.IsNullOrEmpty(langCode) || langCode.Equals("en", StringComparison.OrdinalIgnoreCase) || langCode.Equals("en-US", StringComparison.OrdinalIgnoreCase);

                string fileName = isNeutralLanguage ? $"{scope}.resx" : $"{scope}.{langCode}.resx";
                string filePath = Path.Combine(resourcesPath, fileName);

                using (var writer = new ResXResourceWriter(filePath))
                {
                    foreach (var translation in group)
                    {
                        string formattedKey = translation.Key.Split('&')[0].Replace("name=", "").Trim();

                        // Store normal translation value, fallback to neutral value if missing
                        string finalValue = string.IsNullOrEmpty(translation.Value) ? translation.NeutralValue ?? "" : translation.Value;
                        writer.AddResource(formattedKey, finalValue);

                        // Store the neutral value separately for reference
                        if (!string.IsNullOrEmpty(translation.NeutralValue))
                        {
                            writer.AddResource($"{formattedKey}_Neutral", translation.NeutralValue);
                        }
                    }
                }

                Console.WriteLine($"✅ Created Resource File: {filePath}");
            }
        }

    }
}
