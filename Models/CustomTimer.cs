using System;

namespace TimerApp.Models
{
    public class CustomTimer
    {
        public string Name { get; set; }
        public string Message { get; set; }
        public bool IsCountdown { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan CurrentTime { get; set; }
        public bool IsRunning { get; set; }

        public CustomTimer(string name, string message, bool isCountdown, TimeSpan startTime)
        {
            Name = name;
            Message = message;
            IsCountdown = isCountdown;
            StartTime = startTime;
            CurrentTime = startTime;
            IsRunning = false;
        }
    }
}
