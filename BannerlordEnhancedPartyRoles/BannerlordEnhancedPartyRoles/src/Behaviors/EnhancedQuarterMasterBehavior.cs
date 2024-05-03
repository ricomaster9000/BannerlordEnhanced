using System;
using System.Collections.Generic;
using BannerlordEnhancedFramework;
using BannerlordEnhancedFramework.dialogues;
using BannerlordEnhancedFramework.extendedtypes;
using BannerlordEnhancedFramework.utils;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using BannerlordEnhancedPartyRoles.src.Services;
using TaleWorlds.CampaignSystem.Settlements;

namespace BannerlordEnhancedPartyRoles.Behaviors;
class EnhancedQuarterMasterBehavior : CampaignBehaviorBase
{

	// Don't define low level logic reference high level logic here. (Single resposibility)
	[CommandLineFunctionality.CommandLineArgumentFunction("quatermaster_give_best_items_to_companions_new_version", "debug")]
	public static string DebugGiveBestItemsToCompanions(List<string> strings)
	{
		AutoEquipService.GiveBestEquipmentFromItemRoster();
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
				).WithCondition(PlayerUtils.IsPlayerTalkingToPlayerClanQuarterMaster))
			.WithConversationPart(
				new SimpleConversationPart(
					"enhanced_quatermaster_conv_menu_configure",
					"Configurations",
					ConversationSentenceType.DialogueTreeBranchStart
				))
			.WithConversationPart(
				new SimpleConversationPart(
					"enhanced_quatermaster_conv_menu_configure_equipment_settings",
					"Equipment Filter Settings",
					ConversationSentenceType.DialogueTreeBranchStart
				))
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_equipment_settings_allow_locked_items",
						"Allow Locked Items",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => AutoEquipService.GetAllowLockedItems() == true)
					.WithConsequence(AutoEquipService.ToggleQuaterMasterAllowLockedItems),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_equipment_settings_add_culture_type_any",
						"Allow Any Equipment",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => AutoEquipService.GetAllowAnyCulture() == true)
					.WithConsequence(AutoEquipService.ToggleQuaterMasterAllow_AnyCulture),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_equipment_settings_add_culture_type_battania",
						"Allow Battania Equipment",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => AutoEquipService.GetAllowBattaniaCulture() == true)
					.WithConsequence(AutoEquipService.ToggleQuaterMasterAllow_BattaniaCulture),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_equipment_settings_add_culture_type_sturgia",
						"Allow Sturgia Equipment",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => AutoEquipService.GetAllowSturgiaCulture() == true)
					.WithConsequence(AutoEquipService.ToggleQuaterMasterAllow_SturgiaCulture),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_equipment_settings_add_culture_type_aserai",
						"Allow Aserai Equipment",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => AutoEquipService.GetAllowAseraiCulture() == true)
					.WithConsequence(AutoEquipService.ToggleQuaterMasterAllow_AseraiCulture),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_equipment_settings_add_culture_type_khuzait",
						"Allow Khuzait Equipment",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => AutoEquipService.GetAllowKhuzaitCulture() == true)
					.WithConsequence(AutoEquipService.ToggleQuaterMasterAllow_KhuzaitCulture),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_equipment_settings_add_culture_type_vlandia",
						"Allow Vlandia Equipment",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => AutoEquipService.GetAllowVlandiaCulture() == true)
					.WithConsequence(AutoEquipService.ToggleQuaterMasterAllow_VlandiaCulture),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_equipment_settings_add_culture_type_empire",
						"Allow Empire Equipment",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => AutoEquipService.GetAllowEmpireCulture() == true)
					.WithConsequence(AutoEquipService.ToggleQuaterMasterAllow_EmpireCulture),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_equipment_type",
						"Allow Battle Equipment",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => AutoEquipService.GetAllowBattleEquipment() == true)
					.WithConsequence(AutoEquipService.ToggleQuaterMasterAllow_BattleEquipment),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_equipment_type_civilian_equipment",
						"Allow Civilian Equipment",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => AutoEquipService.GetAllowCivilianEquipment() == true)
					.WithConsequence(AutoEquipService.ToggleQuaterMasterAllow_CivilianEquipment),
				AppliedDialogueLineRelation.LinkToCurrentBranch)

			// Auto Trade Items Branch
			// Culture
			.WithConversationPart(
				new SimpleConversationPart(
					"enhanced_quatermaster_conv_menu_configure_auto_trade_items_settings",
					"Auto Trade Items Filter Settings",
					ConversationSentenceType.DialogueTreeBranchStart
				), AppliedDialogueLineRelation.LinkToParentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_equipment_settings_add_culture_type_any",
						"Allow Any Equipment",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => AutoTraderService.GetAllowAnyCulture() == true)
					.WithConsequence(AutoTraderService.ToggleAllowAnyCulture),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_equipment_settings_add_culture_type_battania",
						"Allow Battania Equipment",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => AutoTraderService.GetAllowCulture(ExtendedCultureCode.get(CultureCode.Battania)))
					.WithConsequence(() => AutoTraderService.ToggleAllowCulture(ExtendedCultureCode.get(CultureCode.Battania))),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_equipment_settings_add_culture_type_sturgia",
						"Allow Sturgia Equipment",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => AutoTraderService.GetAllowCulture(ExtendedCultureCode.get(CultureCode.Sturgia)))
					.WithConsequence(() => AutoTraderService.ToggleAllowCulture(ExtendedCultureCode.get(CultureCode.Sturgia))),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_equipment_settings_add_culture_type_aserai",
						"Allow Aserai Equipment",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => AutoTraderService.GetAllowCulture(ExtendedCultureCode.get(CultureCode.Aserai)))
					.WithConsequence(() => AutoTraderService.ToggleAllowCulture(ExtendedCultureCode.get(CultureCode.Aserai))),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_equipment_settings_add_culture_type_khuzait",
						"Allow Khuzait Equipment",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => AutoTraderService.GetAllowCulture(ExtendedCultureCode.get(CultureCode.Khuzait)))
					.WithConsequence(() => AutoTraderService.ToggleAllowCulture(ExtendedCultureCode.get(CultureCode.Khuzait))),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_equipment_settings_add_culture_type_vlandia",
						"Allow Vlandia Equipment",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => AutoTraderService.GetAllowCulture(ExtendedCultureCode.get(CultureCode.Vlandia)))
					.WithConsequence(() => AutoTraderService.ToggleAllowCulture(ExtendedCultureCode.get(CultureCode.Vlandia))),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_equipment_settings_add_culture_type_empire",
						"Allow Empire Equipment",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => AutoTraderService.GetAllowCulture(ExtendedCultureCode.get(CultureCode.Empire)))
					.WithConsequence(() => AutoTraderService.ToggleAllowCulture(ExtendedCultureCode.get(CultureCode.Empire))),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_auto_trade_items_settings_is_lighest_items_first",
						"Light Items First",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => AutoTraderService.GetIsLightestItemsFirst() == true)
					.WithConsequence(AutoTraderService.ToggleQuarterMasterIsLightestItemsFirst),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_auto_trade_items_settings_allow_locked_items",
						"Allow Locked Items",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => AutoTraderService.GetIncludeLockedItemsWhenSelling() == false)
					.WithConsequence(AutoTraderService.ToggleQuarterMasterAllowLockedItems),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			// Categories
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_auto_trade_items_settings_allow_armour_items",
						"Allow Body Armour Items",
						ConversationSentenceType.DialogueTreeBranchPart)
					.WithCondition(() => AutoTraderService.GetAllowBodyArmour() == false)
					.WithConsequence(AutoTraderService.ToggleAllowBodyArmour),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_auto_trade_items_settings_allow_weapons_items",
						"Allow Weapons Items",
						ConversationSentenceType.DialogueTreeBranchPart)
					.WithCondition(() => AutoTraderService.GetAllowWeapons() == false)
					.WithConsequence(AutoTraderService.ToggleAllowWeapons),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_auto_trade_items_settings_allow_saddles_items",
						"Allow Saddles Items",
						ConversationSentenceType.DialogueTreeBranchPart)
					.WithCondition(() => AutoTraderService.GetAllowSaddles() == false)
					.WithConsequence(AutoTraderService.ToggleAllowSaddles),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_auto_trade_items_settings_allow_horses_items",
						"Allow Horses",
						ConversationSentenceType.DialogueTreeBranchPart)
					.WithCondition(() => AutoTraderService.GetAllowHorses() == false)
					.WithConsequence(AutoTraderService.ToggleAllowHorses),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_auto_trade_items_settings_allow_camels_items",
						"Allow Camels",
						ConversationSentenceType.DialogueTreeBranchPart)
					.WithCondition(() => AutoTraderService.GetAllowCamels() == false)
					.WithConsequence(AutoTraderService.ToggleAllowCamels),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_auto_trade_items_settings_allow_resources_items",
						"Allow Miscellaneous Items",
						ConversationSentenceType.DialogueTreeBranchPart)
					.WithCondition(() => AutoTraderService.GetAllowMiscellaneous() == false)
					.WithConsequence(AutoTraderService.ToggleAllowMiscellaneous),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_auto_trade_items_settings_allow_banners_items",
						"Allow Banners Items",
						ConversationSentenceType.DialogueTreeBranchPart)
					.WithCondition(() => AutoTraderService.GetAllowBanners() == false)
					.WithConsequence(AutoTraderService.ToggleAllowBanners),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.Build(starter);
	}
}

