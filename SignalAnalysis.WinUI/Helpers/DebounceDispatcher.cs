using Microsoft.UI.Dispatching;

namespace SignalAnalysis.Helpers;

public class DebounceDispatcher
{
    readonly DispatcherQueue _dispatcher;
    readonly TimeSpan _interval;
    DispatcherQueueTimer? _timer;

    public DebounceDispatcher(DispatcherQueue dispatcher, TimeSpan interval)
    {
        _dispatcher = dispatcher;
        _interval = interval;
    }

    public void Debounce(Action action)
    {
        _timer?.Stop();
        _timer = _dispatcher.CreateTimer();
        _timer.Interval = _interval;
        _timer.Tick += (s, e) =>
        {
            _timer?.Stop();
            action();
        };
        _timer.Start();
    }
}