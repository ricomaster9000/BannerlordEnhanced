using System;
using System.Collections.Generic;
using System.Linq;
using BannerlordEnhancedFramework.extendedtypes.asynchronous;
using BannerlordEnhancedFramework.utils;
using BannerlordEnhancedPartyRoles.Storage;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;

namespace BannerlordEnhancedPartyRoles.Services;

public static class EnhancedScoutService
{
    public static void SetIsActive(bool active)
    {
        EnhancedScoutData.Active = active;
    }

    public static void SetScoutAlertsNearbyEnemiesFrozen(bool scoutAlertsNearbyEnemiesFrozen)
    {
        EnhancedScoutData.ScoutAlertsNearbyEnemiesFrozen = scoutAlertsNearbyEnemiesFrozen;
    }

    public static bool IsScoutAlertsNearbyEnemiesFrozen()
    {
        return EnhancedScoutData.ScoutAlertsNearbyEnemiesFrozen;
    }

    public static bool IsPlayerTalkingToPlayerClanScout()
    {
        return Campaign.Current != null &&
               PlayerUtils.PlayerParty() != null &&
               PlayerUtils.PlayerParty().EffectiveScout != null &&
               Campaign.Current.ConversationManager.OneToOneConversationCharacter == PlayerUtils.PlayerParty().EffectiveScout.CharacterObject;
    }

    public static void SetScoutAlertsNearbyEnemies(bool scoutAlertsNearbyEnemies)
    {
        EnhancedScoutData.ScoutAlertsNearbyEnemies = scoutAlertsNearbyEnemies;
    }

    public static bool GetScoutAlertsNearbyEnemies()
    {
        return EnhancedScoutData.ScoutAlertsNearbyEnemies;
    }

    public static void AlertPlayerToNearbyHostileParties()
    {
        if (IsScoutAlertsNearbyEnemiesFrozen() || !PlayerUtils.IsPlayerActiveInWorldMap())
        {
            return;
        }
        List<String> hostilePartiesInfo = FindAnyNearbyHostilePartiesPotentiallyTargetingPlayer().Select(party => party.Name + " with " + party.MemberRoster.TotalHealthyCount + " soldiers").ToList();
        if (hostilePartiesInfo.Count > 0)
        {
            PlayerUtils.PauseGame();
            SetScoutAlertsNearbyEnemiesFrozen(true);
            WindowUtils.PopupSimpleInquiry(
                "Enemies Approaching",
                string.Join( ","+Environment.NewLine, hostilePartiesInfo),
                () => {
                    new OneTimeJob(
                        EnhancedScoutData.ScoutAlertsNearbyEnemiesAutoDisabledDurationInMillis,
                        () =>
                        {
                            SetScoutAlertsNearbyEnemiesFrozen(false);
                        }
                    ).StartJobImmediately();
                }
            );
        }
    }

    public static List<MobileParty> FindAnyNearbyHostilePartiesPotentiallyTargetingPlayer()
    {
        List<MobileParty> hostileParties = new List<MobileParty>();
        List<MobileParty> partiesToCheck = MobileParty.AllLordParties
            .Concat(MobileParty.AllBanditParties)
            .Concat(MobileParty.AllMilitiaParties)
            .ToList();

        foreach (MobileParty party in partiesToCheck)
        {
            if (PartyInInterceptPathToPlayerParty(party)) {
                hostileParties.Add(party);
                break;
            }
        }
        return hostileParties;
    }

    public static bool PartyInInterceptPathToPlayerParty(MobileParty party)
    {
        bool partyInVicinityOfPlayer = party.IsVisible && party.IsSpotted();
        //DebugUtils.LogAndPrintInfo("partyInVicinityOfPlayer"+partyInVicinityOfPlayer);
        if (!partyInVicinityOfPlayer || PlayerUtils.PlayerParty().TargetParty == party)
        {
            return false;
        }

        bool partyWillInterceptOrLikelyToInterceptPlayer = party.IsMoving &&
                                                           PlayerUtils.IsPlayerHostileToParty(party) &&
                                                           (
                                                               (party.Ai.MoveTargetParty == PlayerUtils.PlayerParty() && party.Position2D.Distance(PlayerUtils.PlayerParty().Position2D) < 10f) || 
                                                               (PlayerUtils.IsPlayerWeakerThanParty(party) && party.Position2D.Distance(PlayerUtils.PlayerParty().Position2D) < 1.5f)
                                                           );
        //DebugUtils.LogAndPrintInfo("partyWillInterceptOrLikelyToInterceptPlayer"+partyWillInterceptOrLikelyToInterceptPlayer);
        return partyWillInterceptOrLikelyToInterceptPlayer;
    }
}