using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Library;

namespace BannerlordEnhancedFramework.utils;

public static class GameUtils
{
    public static MobileParty PlayerParty()
    {
        return MobileParty.MainParty;
    }

    public static bool IsPlayerHostileToParty(MobileParty mobileParty)
    {
        return PlayerParty().MapFaction.IsAtWarWith(mobileParty.MapFaction);
    }
    
    public static bool IsPlayerWeakerThanParty(MobileParty mobileParty)
    {
        return PlayerParty().Party.TotalStrength < mobileParty.Party.TotalStrength;
    }

    public static void PauseGame()
    {
        Campaign.Current.SetTimeSpeed(0);
    }
	public static bool IsPlayerConversing()
    {
        return Campaign.Current.ConversationManager.OneToOneConversationCharacter != null;
    }

    public static bool IsPlayerActiveInWorldMap()
    {
        return (!IsPlayerConversing() &&
                !Campaign.Current.TimeControlMode.Equals(CampaignTimeControlMode.Stop) &&
                !Campaign.Current.TimeControlMode.Equals(CampaignTimeControlMode.FastForwardStop)
                );
    }

    public static bool IsPlayerImprisoned()
    {
        return (Hero.MainHero.IsPrisoner);
    }
}
