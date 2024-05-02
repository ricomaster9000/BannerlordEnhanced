﻿using System.Collections.Generic;
using System.Linq;
using BannerlordEnhancedFramework.extendedtypes;
using BannerlordEnhancedFramework.extendedtypes.itemcategories;
using BannerlordEnhancedFramework.src.utils;
using BannerlordEnhancedFramework.utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace BannerlordEnhancedPartyRoles.src.Services;

internal class EnhancedQuarterMasterService
{
	public static bool IsPlayerTalkingToPlayerClanQuarterMaster()
	{
		return Campaign.Current != null &&
				GameUtils.PlayerParty() != null &&
				GameUtils.PlayerParty().EffectiveQuartermaster != null &&
				Campaign.Current.ConversationManager.OneToOneConversationCharacter == GameUtils.PlayerParty().EffectiveQuartermaster.CharacterObject;
	}

	public static void DisplayMessageListCategoryNameAndTotal(Dictionary<string, int> categoriesDetails, string startLineMessage)
	{
		foreach (KeyValuePair<string, int> item in categoriesDetails)
		{
			startLineMessage += "\n" + item.Key + " " + item.Value;
		}
		InformationManager.DisplayMessage(new InformationMessage(startLineMessage, BannerlordEnhancedFramework.Colors.Yellow));
	}
	
	public static void GiveBestEquipmentFromItemRoster()
	{
		MobileParty mainParty = MobileParty.MainParty;
		ItemRoster itemRoster = mainParty.ItemRoster;
		List<TroopRosterElement> allCompanionsTroopRosterElement = PartyUtils.GetHerosExcludePlayerHero(mainParty.Party.MemberRoster.GetTroopRoster(), mainParty.LeaderHero);
		List<FighterClass> fighters = new List<FighterClass>();
		bool canRemoveLockedItems = CompanionEquipmentService.GetAllowLockedItems() == false;
		Dictionary<string, int> categories = new Dictionary<string, int>();
		
		List<ExtendedCultureCode> chosenCultures = CompanionEquipmentService.GetChosenCultures();
		foreach (var chosenCulture in chosenCultures)
		{
			HeroEquipmentCustomization heroEquipmentCustomization = new HeroEquipmentCustomizationByClass();

			if (chosenCulture == ExtendedCultureCode.get(CultureCode.Invalid))
			{
				return;
			}
			
			if (chosenCulture != ExtendedCultureCode.get(CultureCode.AnyOtherCulture))
			{
				heroEquipmentCustomization = new HeroEquipmentCustomizationByClassAndCulture(chosenCulture.nativeCultureCode());
			}

			foreach (TroopRosterElement troopCompanion in allCompanionsTroopRosterElement)
			{
				fighters.Add(new FighterClass(troopCompanion.Character.HeroObject, heroEquipmentCustomization));
			}

			foreach (FighterClass fighterClass in fighters)
			{
				List<ItemRosterElement> items = canRemoveLockedItems
					? EquipmentUtil.RemoveLockedItems(itemRoster.ToList())
					: itemRoster.ToList();

				if (CompanionEquipmentService.GetAllowBattleEquipment())
				{
					PartyUtils.updateItemRoster(itemRoster, fighterClass.removeRelavantBattleEquipment(items),
						new List<ItemRosterElement>());

					items = canRemoveLockedItems
						? EquipmentUtil.RemoveLockedItems(itemRoster.ToList())
						: itemRoster.ToList();
					var changes = fighterClass.assignBattleEquipment(items);
					categories = ExtendedItemCategory.GetAllItemCategoryNamesByItemsAndCategories(changes.removals,
						fighterClass.MainItemCategories, categories);
					PartyUtils.updateItemRoster(itemRoster, changes.additions, changes.removals);
				}

				if (CompanionEquipmentService.GetAllowCivilianEquipment())
				{
					items = canRemoveLockedItems
						? EquipmentUtil.RemoveLockedItems(itemRoster.ToList())
						: itemRoster.ToList();
					PartyUtils.updateItemRoster(itemRoster, fighterClass.removeRelavantCivilianEquipment(items),
						new List<ItemRosterElement>());
					var changes = fighterClass.assignCivilianEquipment(items);

					categories = ExtendedItemCategory.GetAllItemCategoryNamesByItemsAndCategories(changes.removals,
						fighterClass.MainItemCategories, categories);
					PartyUtils.updateItemRoster(itemRoster, changes.additions, changes.removals);
				}
			}
		}

		if (categories.Count > 0)
		{
			List<string> categoriesNames = new List<string>();
			foreach(KeyValuePair<string, int> item in categories)
			{
				categoriesNames.Add(item.Key);
			}
			InformationManager.DisplayMessage(new InformationMessage("Quartermaster updated companions " + BuildQuarterMasterNotification(categoriesNames), BannerlordEnhancedFramework.Colors.Yellow));
		}
		
	}
	
	public static string BuildQuarterMasterNotification(List<string> list)
	{
		string text = "";
		int i = 0;
		int size = list.Count;
		foreach (var word in list)
		{
			i += 1;
			if (i == size)
			{
				string addsPlural = word[word.Length - 1] != 'r' ? word + "s" : word;
				text += addsPlural;
			}
			else if (i == size - 1)
			{
				text += word + " and ";
			}
			else
			{
				text += word + ", ";
			}
		}
		return text;
	}
}

