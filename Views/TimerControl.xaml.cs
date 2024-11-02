using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Data;
using TimerApp.Models;

namespace TimerApp.Views
{
    public partial class TimerControl : UserControl
    {
        private readonly CustomTimer timer;
        private readonly DispatcherTimer dispatcherTimer;
        private Window? popoutWindow;

        public TimerControl(CustomTimer timer)
        {
            InitializeComponent();
            this.timer = timer;
            
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
            dispatcherTimer.Tick += Timer_Tick;
            
            UpdateDisplay();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (timer.IsCountdown)
            {
                if (timer.CurrentTime.TotalSeconds > 0)
                {
                    timer.CurrentTime = timer.CurrentTime.Subtract(TimeSpan.FromSeconds(1));
                    if (timer.CurrentTime.TotalSeconds <= 0)
                    {
                        timer.CurrentTime = TimeSpan.Zero;
                        StopTimer();
                    }
                }
            }
            else
            {
                timer.CurrentTime = timer.CurrentTime.Add(TimeSpan.FromSeconds(1));
            }
            timer.UpdateFile();
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            string timeString = $"{timer.CurrentTime.Days:00}:{timer.CurrentTime.Hours:00}:{timer.CurrentTime.Minutes:00}:{timer.CurrentTime.Seconds:00}";
            TimeDisplay.Text = timeString;
            StartButton.IsEnabled = !timer.IsRunning;
            StopButton.IsEnabled = timer.IsRunning;
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            StartTimer();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            StopTimer();
        }

        private void StartTimer()
        {
            timer.IsRunning = true;
            dispatcherTimer.Start();
            UpdateDisplay();
        }

        private void StopTimer()
        {
            timer.IsRunning = false;
            dispatcherTimer.Stop();
            UpdateDisplay();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new TimerSettingsWindow(timer);
            if (settingsWindow.ShowDialog() == true)
            {
                UpdateDisplay();
            }
        }

        private void PopoutButton_Click(object sender, RoutedEventArgs e)
        {
            if (popoutWindow == null)
            {
                var miniControl = new Grid
                {
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF404040"))
                };

                var timeDisplay = new TextBlock
                {
                    Text = TimeDisplay.Text,
                    Foreground = Brushes.White,
                    FontSize = 24,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };

                var hideButton = new Button
                {
                    Content = "👁️",
                    Width = 20,
                    Height = 20,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness(0, 2, 2, 0),
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF607D8B")),
                    Foreground = Brushes.White
                };

                miniControl.Children.Add(timeDisplay);
                miniControl.Children.Add(hideButton);

                popoutWindow = new Window
                {
                    Title = timer.Name,
                    Content = miniControl,
                    Width = 150,
                    Height = 60,
                    WindowStyle = WindowStyle.ToolWindow,
                    Topmost = true,
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF2D2D2D"))
                };

                var binding = new Binding("Text") { Source = TimeDisplay };
                timeDisplay.SetBinding(TextBlock.TextProperty, binding);

                hideButton.Click += (s, args) => 
                {
                    if (popoutWindow.WindowStyle == WindowStyle.ToolWindow)
                    {
                        popoutWindow.WindowStyle = WindowStyle.None;
                        popoutWindow.ResizeMode = ResizeMode.NoResize;
                        hideButton.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        popoutWindow.WindowStyle = WindowStyle.ToolWindow;
                        popoutWindow.ResizeMode = ResizeMode.CanResize;
                        hideButton.Visibility = Visibility.Visible;
                    }
                };

                popoutWindow.Closed += (s, args) => popoutWindow = null;
                popoutWindow.Show();
            }
            else
            {
                popoutWindow.Activate();
            }
        }
        public event EventHandler<CustomTimer>? TimerDeleted;

// Add this method to handle delete functionality
private void DeleteButton_Click(object sender, RoutedEventArgs e)
{
    if (MessageBox.Show("Are you sure you want to delete this timer?", 
        "Delete Timer", 
        MessageBoxButton.YesNo, 
        MessageBoxImage.Warning) == MessageBoxResult.Yes)
    {
        TimerDeleted?.Invoke(this, timer);
    }
}

    }
}
