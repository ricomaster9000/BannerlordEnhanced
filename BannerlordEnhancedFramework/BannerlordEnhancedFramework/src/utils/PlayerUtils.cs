using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;

namespace BannerlordEnhancedFramework.utils;

public class PlayerUtils
{
        public static bool IsPlayerTalkingToPlayerClanQuarterMaster()
        {
            return Campaign.Current != null &&
                   PlayerParty() != null &&
                   PlayerParty().EffectiveQuartermaster != null &&
                   Campaign.Current.ConversationManager.OneToOneConversationCharacter == PlayerParty().EffectiveQuartermaster.CharacterObject;
        }
        
        public static bool IsPlayerTalkingToPlayerClanEngineer()
        {
            return Campaign.Current != null &&
                   PlayerParty() != null &&
                   PlayerParty().EffectiveQuartermaster != null && 
                   Campaign.Current.ConversationManager.OneToOneConversationCharacter == PlayerParty().EffectiveEngineer.CharacterObject;
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
}