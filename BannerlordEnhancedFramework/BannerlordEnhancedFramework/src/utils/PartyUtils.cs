using System;
using System.Collections.Generic;
using BannerlordEnhancedFramework.extendedtypes;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement;
using TaleWorlds.Core;
using TaleWorlds.Library;
using static TaleWorlds.MountAndBlade.ViewModelCollection.Scoreboard.ScoreboardBaseVM;

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

	public static List<TroopRosterElement> GetHerosExcludePlayerHero(MBList<TroopRosterElement> troopsRosterElement, Hero playerHero)
	{
		List<TroopRosterElement> heros = new List<TroopRosterElement>();
		foreach (TroopRosterElement troop in troopsRosterElement)
		{
			if (troop.Character.IsHero && troop.Character.HeroObject != playerHero)
			{
				heros.Add(troop);
			}
		}
		return heros;
	}

	public static List<TroopRosterElement> GetHeros(MBList<TroopRosterElement> troopsRosterElement)
	{
		List<TroopRosterElement> heros = new List<TroopRosterElement>();
		foreach (TroopRosterElement troop in troopsRosterElement)
		{
			if (troop.Character.IsHero)
			{
				heros.Add(troop);
			}
		}
		return heros;
	}

	public static float GetDistanceToSettlement(MobileParty mobileParty, Settlement settlement)
	{
		return settlement.Position2D.Distance(mobileParty.Position2D);
	}
	public static List<ItemRosterElement> SellItemsToSettlement(MobileParty sellerParty, Settlement settlement, List<ItemRosterElement> items)
	{
		List<ValueTuple<ItemRosterElement, int>> itemsToSellTuple = new List<ValueTuple<ItemRosterElement, int>>();
		List<ValueTuple<ItemRosterElement, int>> itemsToBuyTuple = new List<ValueTuple<ItemRosterElement, int>>();
		List<ItemRosterElement> itemsSold = new List<ItemRosterElement>();

		SettlementComponent settlementComponent = settlement.SettlementComponent;
		TownMarketData marketData = new TownMarketData(settlement.Town);

		int settlementGold = settlementComponent.Gold;
		foreach (ItemRosterElement itemRosterElement in items)
		{
			ItemObject item = itemRosterElement.EquipmentElement.Item;

			if (item == null || itemRosterElement.EquipmentElement.IsQuestItem == true)
			{
				continue;
			}
			int price = marketData.GetPrice(itemRosterElement.EquipmentElement, sellerParty, true, null);
			int totalSold = 0;
			for (int _ = 0; _ < itemRosterElement.Amount; _++)
			{
				if (settlementGold - price < 0)
				{
					break;
				};
				settlementGold -= price;
				totalSold += 1;
			}
			ItemRosterElement soldItemRosterElement = new ItemRosterElement(itemRosterElement.EquipmentElement.Item, totalSold, itemRosterElement.EquipmentElement.ItemModifier);
			itemsToSellTuple.Add(new ValueTuple<ItemRosterElement, int>(soldItemRosterElement, price));
			itemsSold.Add(soldItemRosterElement);

		}

		int income = settlementComponent.Gold - settlementGold;
		if (income == 0)
		{
			return itemsSold;
		}

		bool isTrading = true;
		CampaignEventDispatcher.Instance.OnPlayerInventoryExchange(itemsToBuyTuple, itemsToSellTuple, isTrading);

		settlementComponent.ChangeGold(settlementComponent.Gold - income);

		GiveGoldAction.ApplyBetweenCharacters(null, sellerParty.Party.LeaderHero, income, false);
		if (sellerParty.Party.LeaderHero.CompanionOf != null)
		{
			sellerParty.AddTaxGold((int)((float)income * 0.1f));
		}

		updateItemRoster(sellerParty.ItemRoster, new List<ItemRosterElement>(), itemsSold);
		updateItemRoster(Settlement.CurrentSettlement.ItemRoster, itemsSold, new List<ItemRosterElement>());

		return itemsSold;
	}

	public static bool IsMobilePartyValid(MobileParty party)
	{
		if (party.IsGarrison || party.IsMilitia)
		{
			return false;
		}
		if (party.IsMainParty && (!party.IsMainParty || Campaign.Current.IsMainHeroDisguised))
		{
			return false;
		}
		if (party.Army != null)
		{
			Army army = party.Army;
			return army != null && army.LeaderParty.IsMainParty && !Campaign.Current.IsMainHeroDisguised;
		}
		return true;
	}

	public static void updateItemRoster(ItemRoster itemRoster, List<ItemRosterElement> additions, List<ItemRosterElement> removals)
	{
		foreach (ItemRosterElement itemRosterElement in additions)
		{
			itemRoster.Add(itemRosterElement);
		}
		foreach (ItemRosterElement itemRosterElement in removals)
		{
			itemRoster.Remove(itemRosterElement);
		}
	}
}