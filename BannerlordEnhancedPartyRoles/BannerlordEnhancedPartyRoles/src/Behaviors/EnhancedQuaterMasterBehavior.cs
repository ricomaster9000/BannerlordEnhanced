using System;
using System.Collections.Generic;
using System.Linq;
using BannerlordEnhancedFramework;
using BannerlordEnhancedFramework.dialogues;
using BannerlordEnhancedFramework.extendedtypes;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using BannerlordEnhancedFramework.src.utils;
using BannerlordEnhancedPartyRoles.src.Services;
using BannerlordEnhancedFramework.utils;

namespace BannerlordEnhancedPartyRoles.Behaviors;
class EnhancedQuaterMasterBehavior : CampaignBehaviorBase
{
	[CommandLineFunctionality.CommandLineArgumentFunction("quatermaster_give_best_items_to_companions_new_version", "debug")]
	public static string DebugGiveBestItemsToCompanions(List<string> strings)
	{
		GiveBestEquipmentFromItemRoster();
		return "Done";
	}

	public override void RegisterEvents()
	{
		CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(AddDialogs));
	}

	public override void SyncData(IDataStore dataStore)
	{
	}

	private void AddDialogs(CampaignGameStarter starter)
	{
		new DialogueTreeBuilder()
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
							ConversationSentenceType.DialogueTreeBranchStart),
						AppliedDialogueLineRelation.LinkToParentBranch)
							.WithConversationPart(
								new SimpleConversationPart(
									"enhanced_quatermaster_conv_menu_configure_equipment_settings",
									"Equipment Filter Settings",
									ConversationSentenceType.DialogueTreeBranchStart
								), AppliedDialogueLineRelation.LinkToParentBranch)
							.WithTrueFalseConversationToggle(
								new SimpleConversationPart(
										"enhanced_quatermaster_conv_menu_configure_equipment_settings_allow_locked_items",
										"Allow Locked Items",
										ConversationSentenceType.DialogueTreeBranchPart
									).WithCondition(() => EnhancedQuaterMasterService.GetAllowLockedItems() == true)
									.WithConsequence(EnhancedQuaterMasterService.ToggleQuaterMasterAllowLockedItems),
								AppliedDialogueLineRelation.LinkToCurrentBranch)
							.WithTrueFalseConversationToggle(
								new SimpleConversationPart(
										"enhanced_quatermaster_conv_menu_configure_equipment_settings_add_culture_type_any",
										"Allow Any Equipment",
										ConversationSentenceType.DialogueTreeBranchPart
									).WithCondition(() => EnhancedQuaterMasterService.GetAllowAnyCulture() == true)
									.WithConsequence(EnhancedQuaterMasterService.ToggleQuaterMasterAllow_AnyCulture),
								AppliedDialogueLineRelation.LinkToCurrentBranch)
							.WithTrueFalseConversationToggle(
								new SimpleConversationPart(
										"enhanced_quatermaster_conv_menu_configure_equipment_settings_add_culture_type_battania",
										"Allow Battania Equipment",
										ConversationSentenceType.DialogueTreeBranchPart
									).WithCondition(() => EnhancedQuaterMasterService.GetAllowBattaniaCulture() == true)
									.WithConsequence(EnhancedQuaterMasterService.ToggleQuaterMasterAllow_BattaniaCulture),
								AppliedDialogueLineRelation.LinkToCurrentBranch)
							.WithTrueFalseConversationToggle(
								new SimpleConversationPart(
										"enhanced_quatermaster_conv_menu_configure_equipment_settings_add_culture_type_sturgia",
										"Allow Sturgia Equipment",
										ConversationSentenceType.DialogueTreeBranchPart
									).WithCondition(() => EnhancedQuaterMasterService.GetAllowSturgiaCulture() == true)
									.WithConsequence(EnhancedQuaterMasterService.ToggleQuaterMasterAllow_SturgiaCulture),
								AppliedDialogueLineRelation.LinkToCurrentBranch)
							.WithTrueFalseConversationToggle(
								new SimpleConversationPart(
										"enhanced_quatermaster_conv_menu_configure_equipment_settings_add_culture_type_aserai",
										"Allow Aserai Equipment",
										ConversationSentenceType.DialogueTreeBranchPart
									).WithCondition(() => EnhancedQuaterMasterService.GetAllowAseraiCulture() == true)
									.WithConsequence(EnhancedQuaterMasterService.ToggleQuaterMasterAllow_AseraiCulture),
								AppliedDialogueLineRelation.LinkToCurrentBranch)
							.WithTrueFalseConversationToggle(
								new SimpleConversationPart(
										"enhanced_quatermaster_conv_menu_configure_equipment_settings_add_culture_type_khuzait",
										"Allow Khuzait Equipment",
										ConversationSentenceType.DialogueTreeBranchPart
									).WithCondition(() => EnhancedQuaterMasterService.GetAllowKhuzaitCulture() == true)
									.WithConsequence(EnhancedQuaterMasterService.ToggleQuaterMasterAllow_KhuzaitCulture),
								AppliedDialogueLineRelation.LinkToCurrentBranch)
							.WithTrueFalseConversationToggle(
								new SimpleConversationPart(
										"enhanced_quatermaster_conv_menu_configure_equipment_settings_add_culture_type_vlandia",
										"Allow Vlandia Equipment",
										ConversationSentenceType.DialogueTreeBranchPart
									).WithCondition(() => EnhancedQuaterMasterService.GetAllowVlandiaCulture() == true)
									.WithConsequence(EnhancedQuaterMasterService.ToggleQuaterMasterAllow_VlandiaCulture),
								AppliedDialogueLineRelation.LinkToCurrentBranch)
							.WithTrueFalseConversationToggle(
								new SimpleConversationPart(
										"enhanced_quatermaster_conv_menu_configure_equipment_settings_add_culture_type_empire",
										"Allow Empire Equipment",
										ConversationSentenceType.DialogueTreeBranchPart
									).WithCondition(() => EnhancedQuaterMasterService.GetAllowEmpireCulture() == true)
									.WithConsequence(EnhancedQuaterMasterService.ToggleQuaterMasterAllow_EmpireCulture),
								AppliedDialogueLineRelation.LinkToCurrentBranch)
							.WithTrueFalseConversationToggle(
								new SimpleConversationPart(
										"enhanced_quatermaster_conv_menu_configure_equipment_type",
										"Allow Battle Equipment",
										ConversationSentenceType.DialogueTreeBranchPart
									).WithCondition(() => EnhancedQuaterMasterService.GetAllowBattleEquipment() == true)
									.WithConsequence(EnhancedQuaterMasterService.ToggleQuaterMasterAllow_BattleEquipment),
								AppliedDialogueLineRelation.LinkToCurrentBranch)
							.WithTrueFalseConversationToggle(
								new SimpleConversationPart(
										"enhanced_quatermaster_conv_menu_configure_equipment_type_civilian_equipment",
										"Allow Civilian Equipment",
										ConversationSentenceType.DialogueTreeBranchPart
									).WithCondition(() => EnhancedQuaterMasterService.GetAllowCivilianEquipment() == true)
									.WithConsequence(EnhancedQuaterMasterService.ToggleQuaterMasterAllow_CivilianEquipment),
								AppliedDialogueLineRelation.LinkToCurrentBranch)
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

	public static void GiveBestEquipmentFromItemRoster()
	{
		MobileParty mainParty = MobileParty.MainParty;

		ItemRoster itemRoster = mainParty.ItemRoster;

		List<TroopRosterElement> allCompanionsTroopRosterElement = PartyUtils.GetHerosExcludePlayerHero(mainParty.Party.MemberRoster.GetTroopRoster(), mainParty.LeaderHero);
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
				EnhancedQuaterMasterService.updateItemRoster(itemRoster, fighterClass.removeRelavantBattleEquipment(items), new List<ItemRosterElement>());

				items = canRemoveLockedItems ? EquipmentUtil.RemoveLockedItems(itemRoster.ToList()) : itemRoster.ToList();
				var changes = fighterClass.assignBattleEquipment(items);
				BannerlordEnhancedFramework.extendedtypes.ItemCategory.AddItemCategoryNamesFromItemList(changes.removals, fighterClass.MainItemCategories, categoriesChanged);
				EnhancedQuaterMasterService.updateItemRoster(itemRoster, changes.additions, changes.removals);
			}
			if (EnhancedQuaterMasterService.GetAllowCivilianEquipment())
			{
				items = canRemoveLockedItems ? EquipmentUtil.RemoveLockedItems(itemRoster.ToList()) : itemRoster.ToList();
				EnhancedQuaterMasterService.updateItemRoster(itemRoster, fighterClass.removeRelavantCivilianEquipment(items), new List<ItemRosterElement>());
				var changes = fighterClass.assignCivilianEquipment(items);

				BannerlordEnhancedFramework.extendedtypes.ItemCategory.AddItemCategoryNamesFromItemList(changes.removals, fighterClass.MainItemCategories, categoriesChanged);
				EnhancedQuaterMasterService.updateItemRoster(itemRoster, changes.additions, changes.removals);
			}
		}

		if (categoriesChanged.Count > 0)
		{
			InformationManager.DisplayMessage(new InformationMessage("Quatermaster updated companions " + BuildQuaterMasterNotification(categoriesChanged), BannerlordEnhancedFramework.Colors.Yellow));
		}
	}
}

