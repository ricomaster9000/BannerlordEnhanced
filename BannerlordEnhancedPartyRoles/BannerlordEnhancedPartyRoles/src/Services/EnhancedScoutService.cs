using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using BannerlordEnhancedFramework.extendedtypes.asynchronous;
using BannerlordEnhancedFramework.utils;
using BannerlordEnhancedPartyRoles.Storage;
using SandBox.View.Map;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.MapEvents;
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
        try {
            if (IsScoutAlertsNearbyEnemiesFrozen() || !PlayerUtils.IsPlayerActiveInWorldMap() || PlayerUtils.IsPlayerImprisoned())
            {
                return;
            }

            MobileParty hostileParty = FindFirstNearbyHostilePartyPotentiallyTargetingPlayer();
            if (hostileParty != null)
            {
                // we assume player is aware of being targeted and is trying to get away if they are relatively facing the same direction as chasing party
                if (EnhancedScoutData.PrevPossibleHostilePartyTargetingPlayer != null &&
                    EnhancedScoutData.PrevPossibleHostilePartyTargetingPlayer == hostileParty &&
                    PartyUtils.IsPartyFacingSameDirectionOfPartyDirection(PlayerUtils.PlayerParty(), hostileParty, 0.25f) &&
                    PlayerUtils.PlayerParty().Speed > 0 /*player is not standing still */)
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
        } catch (Exception ignored) { return; }
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
            if (PartyUtils.WillOrCouldPartyBeAttackedByParty(party, PlayerUtils.PlayerParty())) {
                hostileParty = party;
                break;
            }
        }
        return hostileParty;
	}

	public static void ShowSiegePopupIfSettlementIsInScoutDetectedRange(SiegeEvent siegeEvent)
	{
		GameUtils.PauseGame();
		Settlement settlement = siegeEvent.BesiegedSettlement;
		List<PartyBase> attackers = siegeEvent.GetInvolvedPartiesForEventType(MapEvent.BattleTypes.Siege);

		float distanceToSettlement = PartyUtils.GetDistanceToSettlement(MobileParty.MainParty, settlement);
		float scoutSkillValue = MobileParty.MainParty.EffectiveScout.GetSkillValue(DefaultSkills.Scouting);

		const float baseDistance = 40;
		const double distanceMultplier = 2.5;
		double detectSiegeDistance = baseDistance + scoutSkillValue * distanceMultplier;

		if (distanceToSettlement > detectSiegeDistance)
		{
			return;
		}

		string title = "Settlement is being besieged";
		string attackersDetails = "";
		int totalSoldiers = 0;

		for (int i = 0; i < attackers.Count; i++)
		{
			PartyBase attacker = attackers[i];
			MobileParty? mobileParty = attacker.MobileParty;
			if (mobileParty != null && PlayerUtils.IsPlayerHostileToParty(mobileParty))
			{
				int soldiers = mobileParty.MemberRoster.TotalHealthyCount;
				attackersDetails = attackersDetails.Add(mobileParty.Name + " with " + soldiers + " soldiers ", true);
				totalSoldiers += soldiers;
			}
		}
		string subTitle = "Scout found " + settlement.Name + " being besieged by " + totalSoldiers.ToString() + " Soldiers";
		WindowUtils.PopupSimpleInquiry(
			title,
			subTitle
			+ Environment.NewLine
			+ attackersDetails,
			"Show on map",
			"Ok",
			() => MapScreen.Instance.FastMoveCameraToPosition(settlement.Position2D),
			() => { }
		);
	}
	
	public static void ShowSiegeAlertPopupIfConditionsAreMet(SiegeEvent siegeEvent)
	{
		Hero owner = siegeEvent.BesiegedSettlement.Owner;
		MobileParty mainParty = MobileParty.MainParty;
		Hero kingdomLeader = mainParty.Owner;
		if (owner == kingdomLeader || owner.Clan.Kingdom == kingdomLeader.Clan.Kingdom)
		{
			GameUtils.PauseGame();
			Settlement settlement = siegeEvent.BesiegedSettlement;
			List<PartyBase> attackers = siegeEvent.GetInvolvedPartiesForEventType(MapEvent.BattleTypes.Siege);

			float distanceToSettlement = PartyUtils.GetDistanceToSettlement(MobileParty.MainParty, settlement);
			float scoutSkillValue = MobileParty.MainParty.EffectiveScout.GetSkillValue(DefaultSkills.Scouting);

			const float baseDistance = 40;
			const double distanceMultplier = 2.5;
			double detectSiegeDistance = baseDistance + scoutSkillValue * distanceMultplier;

			if (distanceToSettlement > detectSiegeDistance)
			{
				return;
			}

			string title = "Settlement is being besieged";
			string attackersDetails = "";
			int totalSoldiers = 0;

			for (int i = 0; i < attackers.Count; i++)
			{
				PartyBase attacker = attackers[i];
				MobileParty? mobileParty = attacker.MobileParty;
				if (mobileParty != null && PlayerUtils.IsPlayerHostileToParty(mobileParty))
				{
					int soldiers = mobileParty.MemberRoster.TotalHealthyCount;
					attackersDetails = attackersDetails.Add(mobileParty.Name + " with " + soldiers + " soldiers ", true);
					totalSoldiers += soldiers;
				}
			}
			string subTitle = "Scout found " + settlement.Name + " being besieged by " + totalSoldiers.ToString() + " Soldiers";
			WindowUtils.PopupSimpleInquiry(
				title,
				subTitle
				+ Environment.NewLine
				+ attackersDetails,
				"Show on map",
				"Ok",
				() => MapScreen.Instance.FastMoveCameraToPosition(settlement.Position2D),
				() => { }
			);
		}
	}

}