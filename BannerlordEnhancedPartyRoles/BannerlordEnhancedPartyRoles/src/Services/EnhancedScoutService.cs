using System;
using System.Collections.Generic;
using System.Linq;
using BannerlordEnhancedFramework.extendedtypes.asynchronous;
using BannerlordEnhancedFramework.utils;
using BannerlordEnhancedPartyRoles.Storage;
using SandBox.View.Map;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Siege;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace BannerlordEnhancedPartyRoles.Services;

public static class EnhancedScoutService
{

    public static void ToggleScoutAlertsNearbyEnemies()
    {
        if (GetScoutAlertsNearbyEnemies()) {
            SetScoutAlertsNearbyEnemies(false);
        } else {
            SetScoutAlertsNearbyEnemies(true);
        }
    }
    
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
               GameUtils.PlayerParty() != null &&
               GameUtils.PlayerParty().EffectiveScout != null &&
               Campaign.Current.ConversationManager.OneToOneConversationCharacter == GameUtils.PlayerParty().EffectiveScout.CharacterObject;
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
        if (IsScoutAlertsNearbyEnemiesFrozen() || !GameUtils.IsPlayerActiveInWorldMap() || GameUtils.IsPlayerImprisoned())
        {
            return;
        }

        MobileParty hostileParty = FindFirstNearbyHostilePartyPotentiallyTargetingPlayer();
        //List<String> hostilePartiesInfo = FindFirstNearbyHostilePartyPotentiallyTargetingPlayer().Select(party => party.Name + " with " + party.MemberRoster.TotalHealthyCount + " soldiers").ToList();
        if (hostileParty != null)
        {
            // we assume player is aware of being targeted and is trying to get away
            if (EnhancedScoutData.PrevPossibleHostilePartyTargetingPlayer != null &&
                EnhancedScoutData.PrevPossibleHostilePartyTargetingPlayer == hostileParty &&
                PartyUtils.IsPartyFacingSameDirectionOfPartyDirection(GameUtils.PlayerParty(),hostileParty, 0.25f))
            {
                return;
            }
            EnhancedScoutData.PrevPossibleHostilePartyTargetingPlayer = hostileParty;

            GameUtils.PauseGame();
            SetScoutAlertsNearbyEnemiesFrozen(true);

            WindowUtils.PopupSimpleInquiry(
                "Enemies Approaching",
                hostileParty.Name + " with " + hostileParty.MemberRoster.TotalHealthyCount + " soldiers",
                () => new OneTimeJob(
                        EnhancedScoutData.ScoutAlertsNearbyEnemiesAutoDisabledDurationInMillis, 
                        () => SetScoutAlertsNearbyEnemiesFrozen(false)).StartJobImmediately()
            );
        }
    }

    public static MobileParty FindFirstNearbyHostilePartyPotentiallyTargetingPlayer()
    {
        MobileParty hostileParty = null;
        List<MobileParty> partiesToCheck = MobileParty.AllLordParties
            .Concat(MobileParty.AllBanditParties)
            .Concat(MobileParty.AllMilitiaParties)
            .ToList();

        foreach (MobileParty party in partiesToCheck)
        {
            if (PartyUtils.WillOrCouldPartyBeAttackedByParty(party, GameUtils.PlayerParty())) {
                hostileParty = party;
                break;
            }
        }
        return hostileParty;
	}

	public static void showSiegePopupIfSettlementIsInScoutDetectedRange(SiegeEvent siegeEvent)
	{
		GameUtils.PauseGame();
		Settlement settlement = siegeEvent.BesiegedSettlement;
		MobileParty hostileParty = siegeEvent.BesiegerCamp.BesiegerParty;

		float distanceToSettlement = PartyUtils.GetDistanceToSettlement(MobileParty.MainParty, settlement);
		float scoutSkillValue = MobileParty.MainParty.EffectiveScout.GetSkillValue(DefaultSkills.Scouting);

		const float baseDistance = 40;
		const double distanceMultplier = 2.5;
		double detectSiegeDistance = baseDistance + scoutSkillValue * distanceMultplier;

		InformationManager.DisplayMessage(new InformationMessage("Distance to Settlement: " + distanceToSettlement, BannerlordEnhancedFramework.Colors.Yellow));
		InformationManager.DisplayMessage(new InformationMessage("Scout skill value:: " + scoutSkillValue, BannerlordEnhancedFramework.Colors.Yellow));
		InformationManager.DisplayMessage(new InformationMessage("detectSiegeDistance: " + detectSiegeDistance, BannerlordEnhancedFramework.Colors.Yellow));

		if (distanceToSettlement < detectSiegeDistance)
		{
			WindowUtils.PopupSimpleInquiry(
			"Settlement is being besieged",
				"Scout found " + hostileParty.Name + " with " + hostileParty.MemberRoster.TotalHealthyCount + " soldiers " + "besieging " + settlement.Name + ".", // + "\nTheir siege startegy is " + settlement.SiegeStrategy.Name + " " + settlement.SiegeStrategy.Description,
				"Show on map",
				"Ok",
				() => MapScreen.Instance.FastMoveCameraToPosition(settlement.Position2D),
				() => {}
			); ;
		}
	}

}