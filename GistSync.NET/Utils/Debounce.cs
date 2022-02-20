using Timer = System.Windows.Forms.Timer;

namespace GistSync.NET.Utils
{
    public class Debounce
    {
        private readonly static IDictionary<int, Timer> DebounceTimers = new Dictionary<int, Timer>();

        public static void Do(Action action, TimeSpan delayTimeSpan, object scope)
        {
            var scopeKey = scope.GetHashCode();

            if (DebounceTimers.TryGetValue(scopeKey, out var timer))
                timer.Dispose();

            var newTimer = new Timer {
                Enabled = true,
                Interval = (int) delayTimeSpan.TotalMilliseconds,
            };

            newTimer.Tick += (_, _) =>
            {
                try
                {
                    action();
                }
                finally
                {
                    DebounceTimers[scopeKey].Dispose();
                    DebounceTimers.Remove(scopeKey);
                }
            };

            DebounceTimers[scopeKey] = newTimer;
        }
    }
}
