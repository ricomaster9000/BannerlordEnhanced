using TaleWorlds.CampaignSystem.Party;

namespace BannerlordEnhancedFramework.utils;

public static class PlayerUtils
{
    public static bool IsPlayerHostileToParty(MobileParty mobileParty)
    {
        return MobileParty.MainParty.MapFaction.IsAtWarWith(mobileParty.MapFaction);
    }
}