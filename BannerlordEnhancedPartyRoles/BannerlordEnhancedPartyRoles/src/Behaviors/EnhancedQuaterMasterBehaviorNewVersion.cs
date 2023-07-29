using System;
using System.Collections.Generic;
using System.Linq;
using BannerlordEnhancedFramework;
using BannerlordEnhancedFramework.dialogues;
using BannerlordEnhancedPartyRoles.Services;
using BannerlordEnhancedFramework.extendedtypes;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using BannerlordEnhancedFramework.src.utils;
using TaleWorlds.Engine;
using BannerlordEnhancedPartyRoles.src.Services;
using BannerlordEnhancedPartyRoles.src.Storage;

namespace BannerlordEnhancedPartyRoles.Behaviors;
class EnhancedQuaterMasterBehaviorNewVersion : CampaignBehaviorBase
{
	[CommandLineFunctionality.CommandLineArgumentFunction("quatermaster_give_best_items_to_companions_new_version", "debug")]
	public static string DebugGiveBestItemsToCompanions(List<string> strings)
	{
		GiveBestEquipmentFromItemRoster();
		return "Done";
	}

	// Not needed in debug ` there is option for config.cheat_mode 1 to activate 0 to disable
	[CommandLineFunctionality.CommandLineArgumentFunction("on_config_text_file_changed", "debug")]
	public static string OnConfigTextFileChanged(List<string> strings)
	{
		NativeConfig.OnConfigChanged();
		return "Done";
	}


	public override void RegisterEvents()
	{
		CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(AddDialogs));
	}

	public override void SyncData(IDataStore dataStore)
	{
	}

	// How dialogs work my understanding so far. is the outputToken arguments you can go from there when adding it as inputToken in next dialogs
	// you can also add to show it in this example player must have quatermaster role to see that dialog
	/*
	private void AddDialogs(CampaignGameStarter starter)
	{
		starter.AddPlayerLine("QauerMaster_Ontmoeting", "hero_main_options", "quatermaster_continue_conversation", "Give Companions best items from my inventory", EnhancedQuaterMasterService.IsQuaterMaster, null);
		starter.AddDialogLine("QauerMaster_Ontmoeting", "quatermaster_continue_conversation", "end", "Alright!", null, GiveBestEquipmentFromItemRoster);
	}
	*/

	private void AddDialogs(CampaignGameStarter starter)
	{
		new DialogueBuilder()
			.WithConversationPart(
				new SimpleConversationPart(
					"enhanced_quatermaster_conv_start",
					"Enhanced Quatermaster Menu",
					ConversationSentenceType.DialogueTreeRootStart,
					CoreInputToken.Entry.HeroMainOptions
				).WithCondition(EnhancedQuaterMasterService.IsPlayerTalkingToPlayerClanQauterMaster))
			.WithConversationPart(
				new SimpleConversationPart(
					"enhanced_quatermaster_conv_menu_configure",
					"Configurations",
					ConversationSentenceType.DialogueTreeBranchStart
				), AppliedDialogueLineRelation.LinkToPreviousStart)
			.WithConversationPart(
				new SimpleConversationPart(
					"enhanced_quatermaster_conv_menu_configure_equipment_settings",
					"Equipment Update Settings",
					ConversationSentenceType.DialogueTreeBranchStart
				), AppliedDialogueLineRelation.LinkToPreviousStart)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_enemy_alerts_toggle_pause_game",
						"Allow Locked Items",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => EnhancedQuaterMasterService.GetAllowLockedItems() == true)
					.WithConsequence(EnhancedQuaterMasterService.ToggleQuaterMasterAllowLockedItems),
				AppliedDialogueLineRelation.LinkToPreviousStart)
			.Build(starter);
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

	public static void GiveBestEquipmentFromItemRoster()
	{
		MobileParty mainParty = MobileParty.MainParty;

		ItemRoster itemRoster = mainParty.ItemRoster;

		List<TroopRosterElement> allCompanionsTroopRosterElement = EnhancedQuaterMasterServicePreviousVersion.GetCompanionsTroopRosterElement(mainParty.Party.MemberRoster.GetTroopRoster(), mainParty.LeaderHero);
		List<FighterClass> fighters = new List<FighterClass>();
		List<CavalryRiderClass> cavalryRiders = new List<CavalryRiderClass>();

		bool canRemoveLockedItems = EnhancedQuaterMasterService.GetAllowLockedItems();


		foreach (TroopRosterElement troopCompanion in allCompanionsTroopRosterElement)
		{
			fighters.Add(new FighterClass(troopCompanion.Character.HeroObject, new HeroEquipmentCustomizationByClassAndCulture(CultureCode.Battania)));
			cavalryRiders.Add(new CavalryRiderClass(troopCompanion.Character.HeroObject, new HeroEquipmentCustomizationByClassAndCulture(CultureCode.Battania)));
		}

		List<string> categoriesChanged = new List<string>();

		// Assigning horses first because saddles needs horse before it can be equipped
		foreach (CavalryRiderClass cavalryRiderClass in cavalryRiders)
		{
			List<ItemRosterElement> items = canRemoveLockedItems ? EquipmentUtil.RemoveLockedItems(itemRoster.ToList()) : itemRoster.ToList();
			updateItemRoster(itemRoster, cavalryRiderClass.removeRelavantBattleEquipment(items), new List<ItemRosterElement>());

			items = canRemoveLockedItems ? EquipmentUtil.RemoveLockedItems(itemRoster.ToList()) : itemRoster.ToList();
			var changes = cavalryRiderClass.assignBattleEquipment(items);
            BannerlordEnhancedFramework.extendedtypes.ItemCategory.AddItemCategoryNamesFromItemList(changes.removals, cavalryRiderClass.MainItemCategories, categoriesChanged);
			updateItemRoster(itemRoster, changes.additions, changes.removals);

			items = canRemoveLockedItems ? EquipmentUtil.RemoveLockedItems(itemRoster.ToList()) : itemRoster.ToList();
			updateItemRoster(itemRoster, cavalryRiderClass.removeRelavantCivilianEquipment(items), new List<ItemRosterElement>());
			changes = cavalryRiderClass.assignCivilianEquipment(items);

			BannerlordEnhancedFramework.extendedtypes.ItemCategory.AddItemCategoryNamesFromItemList(changes.removals, cavalryRiderClass.MainItemCategories, categoriesChanged);
			updateItemRoster(itemRoster, changes.additions, changes.removals);
		}

		foreach (FighterClass fighterClass in fighters)
		{
			List<ItemRosterElement> items = canRemoveLockedItems ? EquipmentUtil.RemoveLockedItems(itemRoster.ToList()) : itemRoster.ToList();
			updateItemRoster(itemRoster, fighterClass.removeRelavantBattleEquipment(items), new List<ItemRosterElement>());
			
			items = canRemoveLockedItems ? EquipmentUtil.RemoveLockedItems(itemRoster.ToList()) : itemRoster.ToList();
			var changes = fighterClass.assignBattleEquipment(items);
			BannerlordEnhancedFramework.extendedtypes.ItemCategory.AddItemCategoryNamesFromItemList(changes.removals, fighterClass.MainItemCategories, categoriesChanged);
			updateItemRoster(itemRoster, changes.additions, changes.removals);

			items = canRemoveLockedItems ? EquipmentUtil.RemoveLockedItems(itemRoster.ToList()) : itemRoster.ToList();
			updateItemRoster(itemRoster, fighterClass.removeRelavantCivilianEquipment(items), new List<ItemRosterElement>());
			changes = fighterClass.assignCivilianEquipment(items);
			
			BannerlordEnhancedFramework.extendedtypes.ItemCategory.AddItemCategoryNamesFromItemList(changes.removals, fighterClass.MainItemCategories, categoriesChanged);
			updateItemRoster(itemRoster, changes.additions, changes.removals);
		}

		if (categoriesChanged.Count > 0)
		{
			InformationManager.DisplayMessage(new InformationMessage("Quatermaster updated companions " + BuildQuaterMasterNotification(categoriesChanged), BannerlordEnhancedFramework.Colors.Yellow));
		}
	}

}

