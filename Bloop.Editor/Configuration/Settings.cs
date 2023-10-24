using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace Bloop.Editor.Configuration
{
    public class Settings
    {
        private static readonly string _defaultAppName = "New Bloop Document";
        private static readonly int _defaultFontSize = 24;

        private static Settings? _instance;

        private IConfigurationRoot _settings;

        private string _title;
        private int _fontSize;

        private Settings()
        {
            _title = _defaultAppName;
            _fontSize = _defaultFontSize;

            _settings = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("Configuration\\editor-settings.json")
                .Build();
        }

        private static Settings Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Settings();

                return _instance;
            }
        }

        public static int FontSize => Instance._fontSize;

        public static void LoadSettings()
        {
            ConsoleManager.Initialize();
            SetFullscreen();
            LoadTitle();
            LoadFont();
            LoadFont();
        }

        private static void LoadTitle()
        {
            string? title = Instance._settings["AppName"];
            if (title == null)
                return;

            Console.Title = title;
        }

        private static void SetFullscreen()
        {
            IntPtr hWnd = Process.GetCurrentProcess().MainWindowHandle;
            ShowWindow(hWnd, 3);
        }

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private static void LoadFont()
        {
            if (!int.TryParse(Instance._settings["FontSize"], out var fontSize))
                return;

            SetFontSize(fontSize);
        }

        public static void IncrementFontSize()
        {
            SetFontSize(Instance._fontSize + 2);
            Save();
        }

        public static void DecrementFontSize()
        {
            SetFontSize(Instance._fontSize - 4);
            SetFontSize(Instance._fontSize + 2);
            Save();
        }

        private static void SetFontSize(int fontSize)
        {
            if (fontSize == Instance._fontSize)
                return;

            Instance._fontSize = fontSize;

            SetupConsoleFontSize(Instance._fontSize);

            var maxWidth = Console.LargestWindowWidth;
            var maxHeigth = Console.LargestWindowHeight;

            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Console.SetWindowSize(maxWidth, maxHeigth);
                Console.SetBufferSize(maxWidth, maxHeigth);
            }
        }

        [DllImport("ConsoleManager.dll")]
        public static extern int SetupConsoleFontSize(int size);

        private static void Save()
        {
            var jsonObject = new
            {
                AppName = Instance._title,
                FontSize = Instance._fontSize.ToString()
            };

            var json = JsonSerializer.Serialize(jsonObject, new JsonSerializerOptions
            {
                WriteIndented = true // Make the JSON output formatted for readability
            });

            // Save the updated configuration back to the file
            var configurationPath = AppDomain.CurrentDomain.BaseDirectory + "Configuration\\editor-settings.json";

            // Write the updated configuration to the JSON file
            System.IO.File.WriteAllText(configurationPath, json);
        }
    }
}
