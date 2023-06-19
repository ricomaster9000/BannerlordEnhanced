using System;
using System.Timers;
using Timer = System.Timers.Timer;

namespace BannerlordEnhancedFramework.extendedtypes;

public class ExtendedTimer : Timer
{
    private Action _timerJobDoCallback;
    
    public bool Active = false;
    private bool _timerJobActive;

    public ExtendedTimer(int interval, Action timerJobDoCallback) // pass in method with no arguments and no return value(void)
    {
        this._timerJobDoCallback = timerJobDoCallback;
        this.Interval = interval;

        // Hook up the Elapsed event for the timer.
        this.Elapsed += BaseTimerJob;
    }

    private void BaseTimerJob(object source, ElapsedEventArgs e)
    {
        if (!_timerJobActive)
        {
            _timerJobActive = true;
            this._timerJobDoCallback();
            _timerJobActive = false;
        }
    }

}