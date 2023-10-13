using Microsoft.Extensions.Configuration;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Bloop.Editor.Configuration
{
    public class Configure
    {
        private static readonly string _defaultAppName = "New Bloop Document";
        private static readonly bool _defaultFullscreen = false;

        public static void LoadSettings()
        {
            IConfigurationRoot settings = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("Configuration\\editor-settings.json")
                .Build();

            SetTitle(settings);
            SetFullscreen(settings);
        }

        private static void SetTitle(IConfigurationRoot settings)
        {
            string? appName = settings["AppName"];
            Console.Title = appName ?? _defaultAppName;
        }

        private static void SetFullscreen(IConfigurationRoot settings)
        {
            string? fullscreen = settings["Fullscreen"];

            var fullscreenValue = _defaultFullscreen;
            if (fullscreen != null && bool.TryParse(fullscreen, out var value))
                fullscreenValue = value;

            if (fullscreenValue)
            {
                IntPtr hWnd = Process.GetCurrentProcess().MainWindowHandle;
                ShowWindow(hWnd, 3);
            }
        }

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    }
}
