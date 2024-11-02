using System;
using System.Windows;
using TimerApp.Models;

namespace TimerApp.Views
{
    public partial class TimerSettingsWindow : Window
    {
        public string TimerMessage { get; private set; } = string.Empty;
        public bool IsCountdown { get; private set; }
        public TimeSpan StartTime { get; private set; }
        private readonly CustomTimer? existingTimer;

        public TimerSettingsWindow(CustomTimer? timer = null)
        {
            InitializeComponent();
            existingTimer = timer;
            TimerMessage = "Time remaining: {timer}";
            SetInitialValues();
        }

        private void SetInitialValues()
        {
            if (existingTimer != null)
            {
                DaysBox.Text = existingTimer.StartTime.Days.ToString();
                HoursBox.Text = existingTimer.StartTime.Hours.ToString();
                MinutesBox.Text = existingTimer.StartTime.Minutes.ToString();
                SecondsBox.Text = existingTimer.StartTime.Seconds.ToString();
                TimerMessageBox.Text = existingTimer.Message;
            }
            else
            {
                DaysBox.Text = "0";
                HoursBox.Text = "0";
                MinutesBox.Text = "0";
                SecondsBox.Text = "0";
                TimerMessageBox.Text = TimerMessage;
            }
        }

        private TimeSpan CalculateTimeSpan()
        {
            int days = int.TryParse(DaysBox.Text, out int d) ? d : 0;
            int hours = int.TryParse(HoursBox.Text, out int h) ? h : 0;
            int minutes = int.TryParse(MinutesBox.Text, out int m) ? m : 0;
            int seconds = int.TryParse(SecondsBox.Text, out int s) ? s : 0;

            return new TimeSpan(days, hours, minutes, seconds);
        }

        private void CountdownButton_Click(object sender, RoutedEventArgs e)
        {
            TimerMessage = TimerMessageBox.Text;
            IsCountdown = true;
            StartTime = CalculateTimeSpan();
            DialogResult = true;
            Close();
        }

        private void CountupButton_Click(object sender, RoutedEventArgs e)
        {
            TimerMessage = TimerMessageBox.Text;
            IsCountdown = false;
            StartTime = TimeSpan.Zero;
            DialogResult = true;
            Close();
        }
    }
}
