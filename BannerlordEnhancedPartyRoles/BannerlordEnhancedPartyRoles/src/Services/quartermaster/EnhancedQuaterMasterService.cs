using System;
using System.Collections.Generic;
using System.Linq;
using BannerlordEnhancedFramework.extendedtypes;
using BannerlordEnhancedFramework.src.utils;
using BannerlordEnhancedFramework.utils;
using BannerlordEnhancedPartyRoles.src.Storage;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Inventory;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace BannerlordEnhancedPartyRoles.src.Services;

internal class EnhancedQuaterMasterService
{
	public static bool IsPlayerTalkingToPlayerClanQuaterMaster()
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

		CultureCode chosenCulture = CompanionEquipmentService.GetChosenCulture();

		HeroEquipmentCustomization heroEquipmentCustomization = new HeroEquipmentCustomizationByClass();

		if (chosenCulture == CultureCode.Invalid)
		{
			return;
		}
		else if(chosenCulture != CultureCode.AnyOtherCulture)
		{
			heroEquipmentCustomization = new HeroEquipmentCustomizationByClassAndCulture(chosenCulture); 
		}

		foreach (TroopRosterElement troopCompanion in allCompanionsTroopRosterElement)
		{
			fighters.Add(new FighterClass(troopCompanion.Character.HeroObject, heroEquipmentCustomization));
		}

		Dictionary<string, int> categories = new Dictionary<string, int>();

		foreach (FighterClass fighterClass in fighters)
		{
			List<ItemRosterElement> items = canRemoveLockedItems ? EquipmentUtil.RemoveLockedItems(itemRoster.ToList()) : itemRoster.ToList();

			if (CompanionEquipmentService.GetAllowBattleEquipment())
			{
				PartyUtils.updateItemRoster(itemRoster, fighterClass.removeRelavantBattleEquipment(items), new List<ItemRosterElement>());

				items = canRemoveLockedItems ? EquipmentUtil.RemoveLockedItems(itemRoster.ToList()) : itemRoster.ToList();
				var changes = fighterClass.assignBattleEquipment(items);
				categories = ExtendedItemCategory.AddItemCategoryNamesFromItemList(changes.removals, fighterClass.MainItemCategories, categories);
				PartyUtils.updateItemRoster(itemRoster, changes.additions, changes.removals);
			}
			if (CompanionEquipmentService.GetAllowCivilianEquipment())
			{
				items = canRemoveLockedItems ? EquipmentUtil.RemoveLockedItems(itemRoster.ToList()) : itemRoster.ToList();
				PartyUtils.updateItemRoster(itemRoster, fighterClass.removeRelavantCivilianEquipment(items), new List<ItemRosterElement>());
				var changes = fighterClass.assignCivilianEquipment(items);

				categories = ExtendedItemCategory.AddItemCategoryNamesFromItemList(changes.removals, fighterClass.MainItemCategories, categories);
				PartyUtils.updateItemRoster(itemRoster, changes.additions, changes.removals);
			}
		}

		if (categories.Count > 0)
		{
			List<string> categoriesNames = new List<string>();
			foreach(KeyValuePair<string, int> item in categories)
			{
				categoriesNames.Add(item.Key);
			}
			InformationManager.DisplayMessage(new InformationMessage("Quatermaster updated companions " + BuildQuaterMasterNotification(categoriesNames), BannerlordEnhancedFramework.Colors.Yellow));
		}
	}
	
	public static string BuildQuaterMasterNotification(List<string> list)
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

