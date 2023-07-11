using System.Collections.Generic;
using TaleWorlds.CampaignSystem;

namespace BannerlordEnhancedFramework.models;

public class ExtendedBehavior : CampaignBehaviorBase
{
    private readonly List<BehaviorEventListener> _events = new();

    public ExtendedBehavior(List<BehaviorEventListener> events)
    {
        _events = events;
    }

    public override void RegisterEvents()
    {
        foreach (var behaviorEventListener in _events)
            behaviorEventListener.LinkedImbEvent()
                .AddNonSerializedListener(this, behaviorEventListener.ActionTriggeredOnEvent());
        //CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(AddDialogs));
    }

    public override void SyncData(IDataStore dataStore)
    {
    }

    private void AddDialogs(CampaignGameStarter starter)
    {
        
    }
}