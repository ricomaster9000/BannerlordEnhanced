using System;
using TaleWorlds.CampaignSystem;

namespace BannerlordEnhancedFramework.models;

public abstract class BehaviorEventListener
{
    private readonly Action _actionTriggeredOnEvent;
    private readonly IMbEvent _linkedImbEvent;

    public BehaviorEventListener(IMbEvent imbEvent, Action actionTriggeredOnEventTriggeredOnEvent)
    {
        _linkedImbEvent = imbEvent;
        _actionTriggeredOnEvent = actionTriggeredOnEventTriggeredOnEvent;
    }

    public IMbEvent LinkedImbEvent()
    {
        return _linkedImbEvent;
    }

    public Action ActionTriggeredOnEvent()
    {
        return _actionTriggeredOnEvent;
    }
}