using System;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Library;

namespace BannerlordEnhancedFramework.utils;

public static class PartyUtils
{
    public static bool WillOrCouldPartyBeAttackedByParty(MobileParty partyWhoMightAttack, MobileParty party, bool excludeIfNotSeen = true) {
        return (!excludeIfNotSeen || IsTargetPartySpottedByParty(partyWhoMightAttack, party)) &&
                IsPartyWeakerThanParty(party, partyWhoMightAttack) &&
                (
                    CouldPartyPossiblyInterceptParty(partyWhoMightAttack, party) ||
                    WillPartyBeAttackedByParty(partyWhoMightAttack, party)
                );
    }

    private static bool IsTargetPartySpottedByParty(MobileParty targetParty, MobileParty party)
    {
        return party.SeeingRange > CalculateDistanceBetweenParties(targetParty, party);
    }

    public static bool WillPartyBeAttackedByParty(MobileParty partyWhoMightAttack, MobileParty targetParty) {
        return IsPartyHostileToParty(partyWhoMightAttack, targetParty) &&
               WillPartyInterceptParty(partyWhoMightAttack, targetParty);
    }

    public static bool CouldPartyBeAttackedByParty(MobileParty partyWhoCouldAttack, MobileParty targetParty) {
        return IsPartyHostileToParty(partyWhoCouldAttack, targetParty) &&
               IsPartyWeakerThanParty(targetParty, partyWhoCouldAttack) &&
               CouldPartyPossiblyInterceptParty(partyWhoCouldAttack, targetParty);
    }

    public static bool WillPartyInterceptParty(MobileParty partyIntercepting, MobileParty targetParty) {
        return IsPartyTargetedByParty(partyIntercepting, targetParty) &&
               !IsPartyConsiderablySlowerThanParty(partyIntercepting, targetParty) &&
               CalculateDistanceBetweenParties(partyIntercepting, targetParty) < 10f;
    }

    public static bool CouldPartyPossiblyInterceptParty(MobileParty partyIntercepting, MobileParty targetParty, bool onlyIfHostile = true) {
        return (!onlyIfHostile || IsPartyHostileToParty(targetParty, partyIntercepting)) && CalculateDistanceBetweenParties(partyIntercepting, targetParty) < 1.5f;
    }

    public static bool IsPartyWeakerThanParty(MobileParty party, MobileParty weakerThanThisParty, float howMuchWeaker = 0.00f) {
        return party.Party.TotalStrength + howMuchWeaker < weakerThanThisParty.Party.TotalStrength;
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
    
    public static bool IsPartyConsiderablySlowerThanParty(MobileParty party, MobileParty targetParty) {
        return IsPartySlowerThanParty(party,targetParty,2.5f);
    }

    public static bool IsPartyReasonablyFasterThanParty(MobileParty party, MobileParty targetParty) {
        return IsPartyFasterThanParty(party,targetParty,0.5f);
    }

    public static bool IsPartyFasterThanParty(MobileParty party, MobileParty targetParty, float howMuchFaster = 0.00f) {
        return party.Speed + howMuchFaster > targetParty.Speed;
    }
    
    public static bool IsPartySlowerThanParty(MobileParty party, MobileParty targetParty, float howMuchSlower = 0.00f) {
        return party.Speed + howMuchSlower < targetParty.Speed;
    }

    public static bool IsPartyTargetedByParty(MobileParty partyTargeting, MobileParty targetParty) {
        return partyTargeting.IsMainParty ? targetParty == partyTargeting.TargetParty : targetParty == partyTargeting.Ai.MoveTargetParty;
    }

    public static bool IsPartyFacingSameDirectionOfPartyDirection(MobileParty party, MobileParty party2, float directionOffToleranceFactor = 0.10f) {
        // Compute the bearing
        double bearingDegreesParty1 = (Math.Atan2(party.Bearing.y, party.Bearing.x)) * (180.0 / Math.PI);
        if (bearingDegreesParty1 < 0) {
            bearingDegreesParty1 += 360;
        }

        double bearingDegreesParty2 = (Math.Atan2(party2.Bearing.y, party2.Bearing.x)) * (180.0 / Math.PI);
        if (bearingDegreesParty2 < 0) {
            bearingDegreesParty2 += 360;
        }

        double allowedBearingDifference = 360 * directionOffToleranceFactor;

        return bearingDegreesParty1 > bearingDegreesParty2 ?
            (bearingDegreesParty1 - bearingDegreesParty2) < allowedBearingDifference :
            (bearingDegreesParty2 - bearingDegreesParty1) < allowedBearingDifference;
    }
}