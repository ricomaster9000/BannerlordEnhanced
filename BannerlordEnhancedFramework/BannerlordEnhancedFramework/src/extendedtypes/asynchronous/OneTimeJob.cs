using System;
using System.Timers;

namespace BannerlordEnhancedFramework.extendedtypes.asynchronous;

public class OneTimeJob : ExtendedTimer 
{
    public OneTimeJob(int delayBeforeStart, Action jobFunction) : base(delayBeforeStart, jobFunction)
    {
    }

    public OneTimeJob StartJobImmediately()
    {
        base.StartTimer();
        return this;
    }

    protected override void BaseTimerJob(object source, ElapsedEventArgs e)
    {
        base._timerJobDoCallback();
        base.StopTimer();
        base.Dispose();
    }
}