using TaleWorlds.CampaignSystem.Party;

namespace BannerlordEnhancedFramework.utils;

public static class PlayerUtils
{
    public static MobileParty PlayerParty()
    {
        return MobileParty.MainParty;
    }
    
    public static bool IsPlayerHostileToParty(MobileParty mobileParty)
    {
        return PlayerParty().MapFaction.IsAtWarWith(mobileParty.MapFaction);
    }
}
