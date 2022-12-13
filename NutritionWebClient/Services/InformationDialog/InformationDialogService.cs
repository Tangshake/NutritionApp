using System;
using System.Timers;

namespace NutritionWebClient.Services.InformationDialog
{
    public class InformationDialogService : IDisposable
    {
        public event Action<string, DialogType> OnShow;
        public event Action OnHide;

        private Timer CountdownTimer;

        public void ShowInformationDialog(string message, DialogType type)
        {
            Console.WriteLine($"[ShowInformationDialog] Type: {type} {message}");

            if(OnShow is null)
                Console.WriteLine("[ShowInformationDialog] No subscribers");

            OnShow?.Invoke(message, type);
            StartCountdownTimer();
        }

        private void StartCountdownTimer()
        {
            InitCountdownTimer();
            CountdownTimer.Stop();
            CountdownTimer.Start();
        }

        private void InitCountdownTimer()
        {
            if(CountdownTimer is null)
            {
                CountdownTimer = new Timer(4000);
                CountdownTimer.Elapsed +=  new ElapsedEventHandler(HideCountdownTimer);
                CountdownTimer.AutoReset = false;
                CountdownTimer.Enabled = true;
            }
        }

        private void HideCountdownTimer(object sender, ElapsedEventArgs e)
        {
            CountdownTimer.Stop();
            Console.WriteLine("[HideCountdownTimer] Timer elapsed");
            if(OnHide is null)
                Console.WriteLine("[HideCountdownTimer] No subscribers");

            OnHide?.Invoke();
        }

        public void Dispose()
        {
            CountdownTimer?.Dispose();
        }
    }
}