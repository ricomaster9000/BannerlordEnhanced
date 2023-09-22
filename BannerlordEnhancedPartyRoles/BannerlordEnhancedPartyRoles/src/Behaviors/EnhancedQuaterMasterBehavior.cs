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
		GiveBestEquipmentFromItemRoster();
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
			List<ExtendedItemCategory> itemCategories = AutoTradeItemsService.FilterExtendedItemCategoriesByConfigurations();
			List<ItemRosterElement> itemRosterElements = AutoTradeItemsService.FilterItemRosterElementsByConfigurationsAndItemCategories(itemCategories);
			List<ItemRosterElement> itemsSold = PartyUtils.SellItemsToSettlement(MobileParty.MainParty, settlement, itemRosterElements, itemCategories);
			Dictionary<string, int> categoriesSold = ExtendedItemCategory.AddItemCategoryNamesFromItemList(itemsSold, itemCategories, new Dictionary<string, int>());
			if(itemsSold.Count > 0) {
				EnhancedQuaterMasterService.DisplayMessageListCategoryNameAndTotal(categoriesSold, "Quatermaster sold items from your inventory");
			}
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
					"Equipment Filter Settings",
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
					"Auto Trade Items Filter Settings",
					ConversationSentenceType.DialogueTreeBranchStart
				), AppliedDialogueLineRelation.LinkToParentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_equipment_settings_add_culture_type_any",
						"Allow Any Equipment",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => AutoTradeItemsService.GetAllowAnyCulture() == true)
					.WithConsequence(AutoTradeItemsService.ToggleQuaterMasterAllow_AnyCulture),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_equipment_settings_add_culture_type_battania",
						"Allow Battania Equipment",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => AutoTradeItemsService.GetAllowBattaniaCulture() == true)
					.WithConsequence(AutoTradeItemsService.ToggleQuaterMasterAllow_BattaniaCulture),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_equipment_settings_add_culture_type_sturgia",
						"Allow Sturgia Equipment",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => AutoTradeItemsService.GetAllowSturgiaCulture() == true)
					.WithConsequence(AutoTradeItemsService.ToggleQuaterMasterAllow_SturgiaCulture),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_equipment_settings_add_culture_type_aserai",
						"Allow Aserai Equipment",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => AutoTradeItemsService.GetAllowAseraiCulture() == true)
					.WithConsequence(AutoTradeItemsService.ToggleQuaterMasterAllow_AseraiCulture),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_equipment_settings_add_culture_type_khuzait",
						"Allow Khuzait Equipment",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => AutoTradeItemsService.GetAllowKhuzaitCulture() == true)
					.WithConsequence(AutoTradeItemsService.ToggleQuaterMasterAllow_KhuzaitCulture),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_equipment_settings_add_culture_type_vlandia",
						"Allow Vlandia Equipment",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => AutoTradeItemsService.GetAllowVlandiaCulture() == true)
					.WithConsequence(AutoTradeItemsService.ToggleQuaterMasterAllow_VlandiaCulture),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_equipment_settings_add_culture_type_empire",
						"Allow Empire Equipment",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => AutoTradeItemsService.GetAllowEmpireCulture() == true)
					.WithConsequence(AutoTradeItemsService.ToggleQuaterMasterAllow_EmpireCulture),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_auto_trade_items_settings_is_lighest_items_first",
						"Light Items First",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => AutoTradeItemsService.GetIsLightestItemsFirst() == true)
					.WithConsequence(AutoTradeItemsService.ToggleQuaterMasterIsLightestItemsFirst),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_auto_trade_items_settings_allow_locked_items",
						"Allow Locked Items",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => AutoTradeItemsService.GetAllowLockedItems() == true)
					.WithConsequence(AutoTradeItemsService.ToggleQuaterMasterAllowLockedItems),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			// Categories
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_auto_trade_items_settings_allow_armour_items",
						"Allow Body Armour Items",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => AutoTradeItemsService.GetAllowBodyArmour() == true)
					.WithConsequence(AutoTradeItemsService.ToggleQuaterMasterAllow_ArmourCategory),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_auto_trade_items_settings_allow_weapons_items",
						"Allow Weapons Items",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => AutoTradeItemsService.GetAllowWeapons() == true)
					.WithConsequence(AutoTradeItemsService.ToggleQuaterMasterAllow_WeaponsCategory),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_auto_trade_items_settings_allow_saddles_items",
						"Allow Saddles Items",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => AutoTradeItemsService.GetAllowSaddles() == true)
					.WithConsequence(AutoTradeItemsService.ToggleQuaterMasterAllow_SaddlesCategory),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_auto_trade_items_settings_allow_horses_items",
						"Allow Horses",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => AutoTradeItemsService.GetAllowHorses() == true)
					.WithConsequence(AutoTradeItemsService.ToggleQuaterMasterAllow_HorsesCategory),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_auto_trade_items_settings_allow_camels_items",
						"Allow Camels",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => AutoTradeItemsService.GetAllowCamels() == true)
					.WithConsequence(AutoTradeItemsService.ToggleQuaterMasterAllow_CamelsCategory),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_auto_trade_items_settings_allow_resources_items",
						"Allow Miscellaneous Items",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => AutoTradeItemsService.GetAllowMiscellaneous() == true)
					.WithConsequence(AutoTradeItemsService.ToggleQuaterMasterAllow_MiscellaneousCategory),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_auto_trade_items_settings_allow_banners_items",
						"Allow Banners Items",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => AutoTradeItemsService.GetAllowBanners() == true)
					.WithConsequence(AutoTradeItemsService.ToggleQuaterMasterAllow_BannersCategory),
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
}

