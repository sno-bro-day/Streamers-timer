using System.Collections.Generic;
using System.Windows;
using TimerApp.Models;

namespace TimerApp.Views
{
    public partial class AllTimersWindow : Window
    {
        public AllTimersWindow(List<CustomTimer> timers)
        {
            InitializeComponent();
            foreach (var timer in timers)
            {
                AllTimersPanel.Children.Add(new TimerControl(timer));
            }
        }
    }
}
