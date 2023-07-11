using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Library;

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

    public static bool IsPlayerTargetingOppositeDirectionOfPartyDirection(MobileParty party) {
        // x and y works between 0.00 and 1.00
        // TODO - not very accurate, improve accuracy

        float xMinAllowed = party.Bearing.x - 0.50f;
        float xMaxAllowed = party.Bearing.x + 0.50f;

        float yMinAllowed = party.Bearing.y - 0.50f;
        float yMaxAllowed = party.Bearing.y + 0.50f;

        return PlayerParty().TargetPosition != party.Position2D &&
               PlayerParty().Bearing.x > xMinAllowed &&
               PlayerParty().Bearing.x < xMaxAllowed &&
               PlayerParty().Bearing.y > yMinAllowed &&
               PlayerParty().Bearing.y < yMaxAllowed;
    }
    
    public static bool IsPlayerActiveInWorldMap()
    {
        return (!IsPlayerConversing() &&
                !Campaign.Current.TimeControlMode.Equals(CampaignTimeControlMode.Stop) &&
                !Campaign.Current.TimeControlMode.Equals(CampaignTimeControlMode.FastForwardStop)
                );
    }
}
