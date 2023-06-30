using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Library;

namespace BannerlordEnhancedFramework.utils;

public static class PartyUtils
{
    public static bool WillOrCouldPartyBeAttackedByParty(MobileParty partyWhoMightAttack, MobileParty targetParty) {
        return IsPartyHostileToParty(partyWhoMightAttack, targetParty) &&
               IsPartyWeakerThanParty(targetParty, partyWhoMightAttack) &&
               (
                   CouldPartyPossiblyInterceptParty(partyWhoMightAttack, targetParty) ||
                   WillPartyBeAttackedByParty(partyWhoMightAttack, targetParty)
                );
    }

    public static bool WillPartyBeAttackedByParty(MobileParty partyWhoMightAttack, MobileParty targetParty) {
        return IsPartyHostileToParty(partyWhoMightAttack, targetParty) &&
               IsPartyWeakerThanParty(targetParty, partyWhoMightAttack) &&
               IsPartyFasterThanParty(partyWhoMightAttack, targetParty) &&
               CouldPartyPossiblyInterceptParty(partyWhoMightAttack, targetParty);
    }

    public static bool CouldPartyBeAttackedByParty(MobileParty partyWhoCouldAttack, MobileParty targetParty) {
        return IsPartyHostileToParty(partyWhoCouldAttack, targetParty) &&
               IsPartyWeakerThanParty(targetParty, partyWhoCouldAttack) &&
               CouldPartyPossiblyInterceptParty(partyWhoCouldAttack, targetParty);
    }

    public static bool WillPartyPossiblyInterceptParty(MobileParty partyIntercepting, MobileParty targetParty) {
        return IsPartyTargetedByParty(partyIntercepting, targetParty) &&
               CalculateDistanceBetweenParties(partyIntercepting, targetParty) < 10f;
    }

    public static bool CouldPartyPossiblyInterceptParty(MobileParty partyIntercepting, MobileParty targetParty) {
        return CalculateDistanceBetweenParties(partyIntercepting, targetParty) < 1.5f;
    }

    public static bool IsPartyWeakerThanParty(MobileParty party, MobileParty targetParty, float howMuchWeaker = 0.00f) {
        return party.Party.TotalStrength + howMuchWeaker < targetParty.Party.TotalStrength;
    }

    public static bool IsPartyHostileToParty(MobileParty party, MobileParty targetParty) {
        return party.MapFaction.IsAtWarWith(targetParty.MapFaction);
    }

    public static float CalculateDistanceBetweenParties(MobileParty partyFrom, MobileParty partyTo) {
        return CalculateDistanceBetweenLocations(partyFrom.Position2D, partyTo.Position2D);
    }
    
    public static float CalculateDistanceBetweenLocations(Vec2 locationFrom, Vec2 locationTo) {
        return locationFrom.Distance(locationTo);
    }

    public static bool IsPartyConsiderablyFasterThanParty(MobileParty party, MobileParty targetParty) {
        return IsPartyFasterThanParty(party,targetParty,2.5f);
    }

    public static bool IsPartyReasonablyFasterThanParty(MobileParty party, MobileParty targetParty) {
        return IsPartyFasterThanParty(party,targetParty,0.5f);
    }

    public static bool IsPartyFasterThanParty(MobileParty party, MobileParty targetParty, float howMuchFaster = 0.00f) {
        return party.Speed + howMuchFaster > targetParty.Speed;
    }

    public static bool IsPartyTargetedByParty(MobileParty partyTargeting, MobileParty targetParty) {
        return partyTargeting.Ai.MoveTargetParty != null && partyTargeting.Ai.MoveTargetParty == targetParty;
    }
}