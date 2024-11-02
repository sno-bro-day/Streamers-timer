using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using TimerApp.Models;
using System.Windows.Media;

namespace TimerApp.Views
{
    public partial class TimerControl : UserControl
    {
        private readonly CustomTimer timer;
        private readonly DispatcherTimer dispatcherTimer;
        public event EventHandler<CustomTimer>? TimerDeleted;

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
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            string timeString = $"{timer.CurrentTime.Days:00}:{timer.CurrentTime.Hours:00}:{timer.CurrentTime.Minutes:00}:{timer.CurrentTime.Seconds:00}";
            string displayMessage = timer.IsCountdown ? 
                timer.Message.Replace("{timer}", timeString) :
                timer.Message.Replace("{countup}", timeString);
            
            MessageDisplay.Text = displayMessage;
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

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            StopTimer();
            timer.CurrentTime = timer.StartTime;
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
private Window? popoutWindow;
private bool isUIHidden = false;

private void PopoutButton_Click(object sender, RoutedEventArgs e)
{
    if (popoutWindow == null)
    {
        var miniTimer = new TimerControl(timer) { Width = 150, Height = 100 };
        miniTimer.HideUI();
        
        popoutWindow = new Window
        {
            Title = timer.Name,
            Content = miniTimer,
            Width = 150,
            Height = 100,
            WindowStyle = WindowStyle.ToolWindow,
            Topmost = true
        };
        
        popoutWindow.Closed += (s, args) => popoutWindow = null;
        popoutWindow.Show();
    }
    else
    {
        popoutWindow.Activate();
    }
}

private void HideUIButton_Click(object sender, RoutedEventArgs e)
{
    if (isUIHidden)
        ShowUI();
    else
        HideUI();
}

public void HideUI()
{
    isUIHidden = true;
    foreach (var element in FindVisualChildren<FrameworkElement>(this))
    {
        if (element != MessageDisplay && element != TimeDisplay)
            element.Visibility = Visibility.Collapsed;
    }
}

public void ShowUI()
{
    isUIHidden = false;
    foreach (var element in FindVisualChildren<FrameworkElement>(this))
    {
        element.Visibility = Visibility.Visible;
    }
}

private static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
{
    if (depObj == null) yield break;
    for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
    {
        var child = VisualTreeHelper.GetChild(depObj, i);
        if (child is T t) yield return t;
        foreach (T childOfChild in FindVisualChildren<T>(child))
            yield return childOfChild;
    }
}
    }
}
