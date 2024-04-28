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
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Inventory;
using static TaleWorlds.MountAndBlade.ViewModelCollection.Scoreboard.ScoreboardBaseVM;
using TaleWorlds.CampaignSystem.Extensions;

namespace BannerlordEnhancedPartyRoles.Behaviors;
class EnhancedQuaterMasterBehavior : CampaignBehaviorBase
{

	// Don't define low level logic reference high level logic here. (Single resposibility)
	[CommandLineFunctionality.CommandLineArgumentFunction("quatermaster_give_best_items_to_companions_new_version", "debug")]
	public static string DebugGiveBestItemsToCompanions(List<string> strings)
	{
		EnhancedQuaterMasterService.GiveBestEquipmentFromItemRoster();
		return "Done";
	}

	public override void RegisterEvents()
	{
		CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(AddDialogs));
		CampaignEvents.SettlementEntered.AddNonSerializedListener(this, new Action<MobileParty, Settlement, Hero>(this.OnSettlementEntered));
	}

	public override void SyncData(IDataStore dataStore)
	{
	}

	public void OnSettlementEntered(MobileParty partyEnteredSettlement, Settlement settlement, Hero leader)
	{
		// Our logic attach with game logic (First line)
		if(partyEnteredSettlement != null && partyEnteredSettlement == MobileParty.MainParty)
		{
			// Should not have low level implementation of mod logic
			SettlementComponent settlementComponent = settlement.SettlementComponent;
			if (settlementComponent == null || settlementComponent.IsTown == false)
			{
				return;
			}
			AutoTraderService.SellItemsWhenMainPartyEntersSettlement(settlement);
		}
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
				).WithCondition(EnhancedQuaterMasterService.IsPlayerTalkingToPlayerClanQuaterMaster))
			.WithConversationPart(
				new SimpleConversationPart(
					"enhanced_quatermaster_conv_menu_configure",
					"Configurations",
					ConversationSentenceType.DialogueTreeBranchStart
				))
			.WithConversationPart(
				new SimpleConversationPart(
					"enhanced_quatermaster_conv_menu_configure_equipment_settings",
					"Equipment Filter Settings" + Environment.NewLine
					+ Environment.NewLine + "Automatically allocates equipment to your companions, streamlining the management of gear throughout your party. " 
					+ Environment.NewLine + "Upon acquiring new items, the system intelligently assigns them to companions, taking into account their combat roles."
					+ Environment.NewLine + " Additionally, you can customize how gear is distributed based on the faction of origin.",
					ConversationSentenceType.DialogueTreeBranchStart
				))
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_equipment_settings_allow_locked_items",
						"Allow Locked Items",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => CompanionEquipmentService.GetAllowLockedItems() == true)
					.WithConsequence(CompanionEquipmentService.ToggleQuaterMasterAllowLockedItems),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_equipment_settings_add_culture_type_any",
						"Allow Any Equipment",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => CompanionEquipmentService.GetAllowAnyCulture() == true)
					.WithConsequence(CompanionEquipmentService.ToggleQuaterMasterAllow_AnyCulture),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_equipment_settings_add_culture_type_battania",
						"Allow Battania Equipment",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => CompanionEquipmentService.GetAllowBattaniaCulture() == true)
					.WithConsequence(CompanionEquipmentService.ToggleQuaterMasterAllow_BattaniaCulture),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_equipment_settings_add_culture_type_sturgia",
						"Allow Sturgia Equipment",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => CompanionEquipmentService.GetAllowSturgiaCulture() == true)
					.WithConsequence(CompanionEquipmentService.ToggleQuaterMasterAllow_SturgiaCulture),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_equipment_settings_add_culture_type_aserai",
						"Allow Aserai Equipment",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => CompanionEquipmentService.GetAllowAseraiCulture() == true)
					.WithConsequence(CompanionEquipmentService.ToggleQuaterMasterAllow_AseraiCulture),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_equipment_settings_add_culture_type_khuzait",
						"Allow Khuzait Equipment",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => CompanionEquipmentService.GetAllowKhuzaitCulture() == true)
					.WithConsequence(CompanionEquipmentService.ToggleQuaterMasterAllow_KhuzaitCulture),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_equipment_settings_add_culture_type_vlandia",
						"Allow Vlandia Equipment",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => CompanionEquipmentService.GetAllowVlandiaCulture() == true)
					.WithConsequence(CompanionEquipmentService.ToggleQuaterMasterAllow_VlandiaCulture),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_equipment_settings_add_culture_type_empire",
						"Allow Empire Equipment",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => CompanionEquipmentService.GetAllowEmpireCulture() == true)
					.WithConsequence(CompanionEquipmentService.ToggleQuaterMasterAllow_EmpireCulture),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_equipment_type",
						"Allow Battle Equipment",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => CompanionEquipmentService.GetAllowBattleEquipment() == true)
					.WithConsequence(CompanionEquipmentService.ToggleQuaterMasterAllow_BattleEquipment),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_equipment_type_civilian_equipment",
						"Allow Civilian Equipment",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => CompanionEquipmentService.GetAllowCivilianEquipment() == true)
					.WithConsequence(CompanionEquipmentService.ToggleQuaterMasterAllow_CivilianEquipment),
				AppliedDialogueLineRelation.LinkToCurrentBranch)

			// Auto Trade Items Branch
			// Culture
			.WithConversationPart(
				new SimpleConversationPart(
					"enhanced_quatermaster_conv_menu_configure_auto_trade_items_settings",
					"Auto Trade Items Filter Settings" + Environment.NewLine
					+ Environment.NewLine + "Automates the selling process by automatically offloading selected items from your inventory each time you visit a town. "
					+ Environment.NewLine + "Designed to simplify the management of your goods, "
					+ Environment.NewLine + "this ensures you can sell bulk items effortlessly without the need to manually handle each transaction.",
					ConversationSentenceType.DialogueTreeBranchStart
				), AppliedDialogueLineRelation.LinkToParentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_equipment_settings_add_culture_type_any",
						"Allow Any Equipment",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => AutoTraderService.GetAllowAnyCulture() == true)
					.WithConsequence(AutoTraderService.ToggleQuaterMasterAllow_AnyCulture),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_equipment_settings_add_culture_type_battania",
						"Allow Battania Equipment",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => AutoTraderService.GetAllowBattaniaCulture() == true)
					.WithConsequence(AutoTraderService.ToggleQuaterMasterAllow_BattaniaCulture),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_equipment_settings_add_culture_type_sturgia",
						"Allow Sturgia Equipment",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => AutoTraderService.GetAllowSturgiaCulture() == true)
					.WithConsequence(AutoTraderService.ToggleQuaterMasterAllow_SturgiaCulture),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_equipment_settings_add_culture_type_aserai",
						"Allow Aserai Equipment",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => AutoTraderService.GetAllowAseraiCulture() == true)
					.WithConsequence(AutoTraderService.ToggleQuaterMasterAllow_AseraiCulture),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_equipment_settings_add_culture_type_khuzait",
						"Allow Khuzait Equipment",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => AutoTraderService.GetAllowKhuzaitCulture() == true)
					.WithConsequence(AutoTraderService.ToggleQuaterMasterAllow_KhuzaitCulture),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_equipment_settings_add_culture_type_vlandia",
						"Allow Vlandia Equipment",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => AutoTraderService.GetAllowVlandiaCulture() == true)
					.WithConsequence(AutoTraderService.ToggleQuaterMasterAllow_VlandiaCulture),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_equipment_settings_add_culture_type_empire",
						"Allow Empire Equipment",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => AutoTraderService.GetAllowEmpireCulture() == true)
					.WithConsequence(AutoTraderService.ToggleQuaterMasterAllow_EmpireCulture),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_auto_trade_items_settings_is_lighest_items_first",
						"Light Items First",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => AutoTraderService.GetIsLightestItemsFirst() == true)
					.WithConsequence(AutoTraderService.ToggleQuaterMasterIsLightestItemsFirst),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_auto_trade_items_settings_allow_locked_items",
						"Allow Locked Items",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => AutoTraderService.GetExcludeLockedItemsWhenSelling() == false)
					.WithConsequence(AutoTraderService.ToggleQuaterMasterAllowLockedItems),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			// Categories
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_auto_trade_items_settings_allow_armour_items",
						"Allow Body Armour Items",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => AutoTraderService.GetAllowBodyArmour() == true)
					.WithConsequence(AutoTraderService.ToggleQuaterMasterAllow_ArmourCategory),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_auto_trade_items_settings_allow_weapons_items",
						"Allow Weapons Items",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => AutoTraderService.GetAllowWeapons() == true)
					.WithConsequence(AutoTraderService.ToggleQuaterMasterAllow_WeaponsCategory),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_auto_trade_items_settings_allow_saddles_items",
						"Allow Saddles Items",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => AutoTraderService.GetAllowSaddles() == true)
					.WithConsequence(AutoTraderService.ToggleQuaterMasterAllow_SaddlesCategory),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_auto_trade_items_settings_allow_horses_items",
						"Allow Horses",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => AutoTraderService.GetAllowHorses() == true)
					.WithConsequence(AutoTraderService.ToggleQuaterMasterAllow_HorsesCategory),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_auto_trade_items_settings_allow_camels_items",
						"Allow Camels",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => AutoTraderService.GetAllowCamels() == true)
					.WithConsequence(AutoTraderService.ToggleQuaterMasterAllow_CamelsCategory),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_auto_trade_items_settings_allow_resources_items",
						"Allow Miscellaneous Items",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => AutoTraderService.GetAllowMiscellaneous() == true)
					.WithConsequence(AutoTraderService.ToggleQuaterMasterAllow_MiscellaneousCategory),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_auto_trade_items_settings_allow_banners_items",
						"Allow Banners Items",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => AutoTraderService.GetAllowBanners() == true)
					.WithConsequence(AutoTraderService.ToggleQuaterMasterAllow_BannersCategory),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.Build(starter);
	}
}

