using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Linq;
using System.Text.Json;
using TimerApp.Models;
using TimerApp.Views;

namespace TimerApp
{
    public partial class MainWindow : Window
    {
        private readonly List<CustomTimer> timers = new List<CustomTimer>();
        private const int MaxVisibleTimers = 6;
        private readonly string appDataPath;

        public MainWindow()
        {
            InitializeComponent();
            appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "StreamersBestFriend"
            );
            Directory.CreateDirectory(appDataPath);
            LoadTimers();
        }

        private void AddTimer_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new TimerSettingsWindow();
            if (settingsWindow.ShowDialog() == true)
            {
                var timer = new CustomTimer(
                    "Timer " + (timers.Count + 1),
                    settingsWindow.TimerMessage,
                    settingsWindow.IsCountdown,
                    settingsWindow.StartTime
                );
                timers.Add(timer);
                SaveTimers();
                UpdateTimerDisplay();
            }
        }

        private void StartAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (var timer in timers)
            {
                timer.IsRunning = true;
            }
            UpdateTimerDisplay();
        }

        private void UpdateTimerDisplay()
        {
            TimerPanel.Children.Clear();
            
            for (int i = 0; i < Math.Min(timers.Count, MaxVisibleTimers); i++)
            {
                var timerControl = new TimerControl(timers[i]);
                timerControl.TimerDeleted += TimerControl_TimerDeleted;
                TimerPanel.Children.Add(timerControl);
            }

            if (timers.Count > MaxVisibleTimers)
            {
                AddMoreButton();
            }
        }

        private void TimerControl_TimerDeleted(object? sender, CustomTimer timer)
        {
            timers.Remove(timer);
            SaveTimers();
            UpdateTimerDisplay();
        }

        private void SaveTimers()
        {
            var timerDataPath = Path.Combine(appDataPath, "timers.json");
            var timerData = timers.Select(t => new
            {
                t.Name,
                t.Message,
                t.IsCountdown,
                StartTime = t.StartTime.ToString(),
                CurrentTime = t.CurrentTime.ToString(),
                t.IsRunning
            }).ToList();
            
            File.WriteAllText(timerDataPath, JsonSerializer.Serialize(timerData));
        }

        private void LoadTimers()
        {
            var timerDataPath = Path.Combine(appDataPath, "timers.json");
            if (File.Exists(timerDataPath))
            {
                var timerData = JsonSerializer.Deserialize<List<JsonElement>>(File.ReadAllText(timerDataPath));
                foreach (var data in timerData!)
                {
                    var timer = new CustomTimer(
                        data.GetProperty("Name").GetString()!,
                        data.GetProperty("Message").GetString()!,
                        data.GetProperty("IsCountdown").GetBoolean(),
                        TimeSpan.Parse(data.GetProperty("StartTime").GetString()!)
                    )
                    {
                        CurrentTime = TimeSpan.Parse(data.GetProperty("CurrentTime").GetString()!),
                        IsRunning = data.GetProperty("IsRunning").GetBoolean()
                    };
                    timers.Add(timer);
                }
                UpdateTimerDisplay();
            }
        }

        private void AddMoreButton()
        {
            var moreButton = new Button
            {
                Content = "Show All",
                Width = 100,
                Height = 100,
                Background = (System.Windows.Media.Brush)new System.Windows.Media.BrushConverter().ConvertFrom("#FF2D2D2D"),
                Foreground = System.Windows.Media.Brushes.White,
                Margin = new Thickness(5)
            };
            moreButton.Click += MoreButton_Click;
            TimerPanel.Children.Add(moreButton);
        }

        private void MoreButton_Click(object sender, RoutedEventArgs e)
        {
            var allTimersWindow = new AllTimersWindow(timers);
            allTimersWindow.Show();
        }
    }
}
