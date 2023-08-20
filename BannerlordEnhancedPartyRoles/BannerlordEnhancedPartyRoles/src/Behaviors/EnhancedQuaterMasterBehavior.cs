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
using TaleWorlds.CampaignSystem.Actions;

namespace BannerlordEnhancedPartyRoles.Behaviors;
class EnhancedQuaterMasterBehavior : CampaignBehaviorBase
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
					"Equipment Filter Settings",
					ConversationSentenceType.DialogueTreeBranchStart
				), AppliedDialogueLineRelation.LinkToPreviousStart)
			/*.WithConversationParts(
				new SimpleConversationPart(
					"enhanced_quatermaster_conv_menu_configure_equipment_type",
					"Equipment Type Settings",
					ConversationSentenceType.DialogueTreeBranchStart
				),
				new SimpleConversationPart(
					"enhanced_quatermaster_conv_menu_configure_equipment_settings",
					"Equipment Filter Settings",
					ConversationSentenceType.DialogueTreeBranchStart
				), AppliedDialogueLineRelation.LinkToPreviousStart)*/
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_equipment_settings_allow_locked_items",
						"Allow Locked Items",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => EnhancedQuaterMasterService.GetAllowLockedItems() == true)
					.WithConsequence(EnhancedQuaterMasterService.ToggleQuaterMasterAllowLockedItems),
				AppliedDialogueLineRelation.LinkToPreviousStart)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_equipment_settings_add_culture_type_any",
						"Allow Any Equipment",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => EnhancedQuaterMasterService.GetAllowAnyCulture() == true)
					.WithConsequence(EnhancedQuaterMasterService.ToggleQuaterMasterAllow_AnyCulture),
				AppliedDialogueLineRelation.LinkToPreviousStart)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_equipment_settings_add_culture_type_battania",
						"Allow Battania Equipment",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => EnhancedQuaterMasterService.GetAllowBattaniaCulture() == true)
					.WithConsequence(EnhancedQuaterMasterService.ToggleQuaterMasterAllow_BattaniaCulture),
				AppliedDialogueLineRelation.LinkToPreviousStart)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_equipment_settings_add_culture_type_sturgia",
						"Allow Sturgia Equipment",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => EnhancedQuaterMasterService.GetAllowSturgiaCulture() == true)
					.WithConsequence(EnhancedQuaterMasterService.ToggleQuaterMasterAllow_SturgiaCulture),
				AppliedDialogueLineRelation.LinkToPreviousStart)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_equipment_settings_add_culture_type_aserai",
						"Allow Aserai Equipment",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => EnhancedQuaterMasterService.GetAllowAseraiCulture() == true)
					.WithConsequence(EnhancedQuaterMasterService.ToggleQuaterMasterAllow_AseraiCulture),
				AppliedDialogueLineRelation.LinkToPreviousStart)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_equipment_settings_add_culture_type_khuzait",
						"Allow Khuzait Equipment",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => EnhancedQuaterMasterService.GetAllowKhuzaitCulture() == true)
					.WithConsequence(EnhancedQuaterMasterService.ToggleQuaterMasterAllow_KhuzaitCulture),
				AppliedDialogueLineRelation.LinkToPreviousStart)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_equipment_settings_add_culture_type_vlandia",
						"Allow Vlandia Equipment",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => EnhancedQuaterMasterService.GetAllowVlandiaCulture() == true)
					.WithConsequence(EnhancedQuaterMasterService.ToggleQuaterMasterAllow_VlandiaCulture),
				AppliedDialogueLineRelation.LinkToPreviousStart)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_equipment_settings_add_culture_type_empire",
						"Allow Empire Equipment",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => EnhancedQuaterMasterService.GetAllowEmpireCulture() == true)
					.WithConsequence(EnhancedQuaterMasterService.ToggleQuaterMasterAllow_EmpireCulture),
				AppliedDialogueLineRelation.LinkToPreviousStart)

			/*.WithConversationPart(
				new SimpleConversationPart(
					"enhanced_quatermaster_conv_menu_configure_equipment_type",
					"Equipment Type Settings",
					ConversationSentenceType.DialogueTreeBranchStart
				), AppliedDialogueLineRelation.LinkToPreviousStart)*/
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_equipment_type",
						"Allow Battle Equipment",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => EnhancedQuaterMasterService.GetAllowBattleEquipment() == true)
					.WithConsequence(EnhancedQuaterMasterService.ToggleQuaterMasterAllow_BattleEquipment),
				AppliedDialogueLineRelation.LinkToPreviousStart)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_equipment_type_civilian_equipment",
						"Allow Civilian Equipment",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => EnhancedQuaterMasterService.GetAllowCivilianEquipment() == true)
					.WithConsequence(EnhancedQuaterMasterService.ToggleQuaterMasterAllow_CivilianEquipment),
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

		// TODO move in util
		List<TroopRosterElement> allCompanionsTroopRosterElement = EnhancedQuaterMasterService_Unused_Version.GetCompanionsTroopRosterElement(mainParty.Party.MemberRoster.GetTroopRoster(), mainParty.LeaderHero);
		List<FighterClass> fighters = new List<FighterClass>();

		bool canRemoveLockedItems = EnhancedQuaterMasterService.GetAllowLockedItems() == false;

		CultureCode chosenCulture = EnhancedQuaterMasterService.GetChosenCulture();

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

		List<string> categoriesChanged = new List<string>();

		foreach (FighterClass fighterClass in fighters)
		{
			List<ItemRosterElement> items = canRemoveLockedItems ? EquipmentUtil.RemoveLockedItems(itemRoster.ToList()) : itemRoster.ToList();

			if (EnhancedQuaterMasterService.GetAllowBattleEquipment())
			{
				updateItemRoster(itemRoster, fighterClass.removeRelavantBattleEquipment(items), new List<ItemRosterElement>());

				items = canRemoveLockedItems ? EquipmentUtil.RemoveLockedItems(itemRoster.ToList()) : itemRoster.ToList();
				var changes = fighterClass.assignBattleEquipment(items);
				BannerlordEnhancedFramework.extendedtypes.ItemCategory.AddItemCategoryNamesFromItemList(changes.removals, fighterClass.MainItemCategories, categoriesChanged);
				updateItemRoster(itemRoster, changes.additions, changes.removals);
			}
			if (EnhancedQuaterMasterService.GetAllowCivilianEquipment())
			{
				items = canRemoveLockedItems ? EquipmentUtil.RemoveLockedItems(itemRoster.ToList()) : itemRoster.ToList();
				updateItemRoster(itemRoster, fighterClass.removeRelavantCivilianEquipment(items), new List<ItemRosterElement>());
				var changes = fighterClass.assignCivilianEquipment(items);

				BannerlordEnhancedFramework.extendedtypes.ItemCategory.AddItemCategoryNamesFromItemList(changes.removals, fighterClass.MainItemCategories, categoriesChanged);
				updateItemRoster(itemRoster, changes.additions, changes.removals);
			}
		}

		if (categoriesChanged.Count > 0)
		{
			InformationManager.DisplayMessage(new InformationMessage("Quatermaster updated companions " + BuildQuaterMasterNotification(categoriesChanged), BannerlordEnhancedFramework.Colors.Yellow));
		}
	}
}


// List<CavalryRiderClass> cavalryRiders = new List<CavalryRiderClass>();
// cavalryRiders.Add(new CavalryRiderClass(troopCompanion.Character.HeroObject, heroEquipmentCustomization));

// Assigning horses first because saddles needs horse before it can be equipped
/*
foreach (CavalryRiderClass cavalryRiderClass in cavalryRiders)
{
	List<ItemRosterElement> items = canRemoveLockedItems ? EquipmentUtil.RemoveLockedItems(itemRoster.ToList()) : itemRoster.ToList();

	if (EnhancedQuaterMasterService.GetAllowBattleEquipment())
	{
		updateItemRoster(itemRoster, cavalryRiderClass.removeRelavantBattleEquipment(items), new List<ItemRosterElement>());

		items = canRemoveLockedItems ? EquipmentUtil.RemoveLockedItems(itemRoster.ToList()) : itemRoster.ToList();
		var changes = cavalryRiderClass.assignBattleEquipment(items);
		BannerlordEnhancedFramework.extendedtypes.ItemCategory.AddItemCategoryNamesFromItemList(changes.removals, cavalryRiderClass.MainItemCategories, categoriesChanged);
		updateItemRoster(itemRoster, changes.additions, changes.removals);
	}

	if (EnhancedQuaterMasterService.GetAllowCivilianEquipment())
	{
		items = canRemoveLockedItems ? EquipmentUtil.RemoveLockedItems(itemRoster.ToList()) : itemRoster.ToList();
		updateItemRoster(itemRoster, cavalryRiderClass.removeRelavantCivilianEquipment(items), new List<ItemRosterElement>());
		var changes = cavalryRiderClass.assignCivilianEquipment(items);

		BannerlordEnhancedFramework.extendedtypes.ItemCategory.AddItemCategoryNamesFromItemList(changes.removals, cavalryRiderClass.MainItemCategories, categoriesChanged);
		updateItemRoster(itemRoster, changes.additions, changes.removals);
	}
}
*/
