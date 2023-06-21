using System;
using System.Timers;

namespace BannerlordEnhancedFramework.extendedtypes;

public class ExtendedTimer : Timer
{
    private bool _timerJobActive;
    private readonly Action _timerJobDoCallback;

    public bool Active = false;

    public ExtendedTimer(int interval, Action timerJobDoCallback) // pass in method with no arguments and no return value(void)
    {
        _timerJobDoCallback = timerJobDoCallback;
        Interval = interval;

        // Hook up the Elapsed event for the timer.
        Elapsed += BaseTimerJob;
    }

    private void BaseTimerJob(object source, ElapsedEventArgs e)
    {
        if (!_timerJobActive)
        {
            _timerJobActive = true;
            _timerJobDoCallback();
            _timerJobActive = false;
        }
    }
}