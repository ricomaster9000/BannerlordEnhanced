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
		CampaignEvents.SettlementEntered.AddNonSerializedListener(this, new Action<MobileParty, Settlement, Hero>(this.OnSettlementEntered)); 
	}

	public override void SyncData(IDataStore dataStore)
	{
	}

	public void OnSettlementEntered(MobileParty partyEnteredSettlement, Settlement settlement, Hero leader)
	{
		if(partyEnteredSettlement != null && partyEnteredSettlement == MobileParty.MainParty)
		{
			SettlementComponent settlementComponent = settlement.SettlementComponent;
			if (settlementComponent == null || settlementComponent.IsTown == false)
			{
				return;
			}
			List<ExtendedItemCategory> itemCategories = new List<ExtendedItemCategory>();
			if (EnhancedQuaterMasterService.AutoTradeItems.GetAllowArmour())
			{
				itemCategories.Add(ExtendedItemCategory.ArmorItemCategory);
			}
			if (EnhancedQuaterMasterService.AutoTradeItems.GetAllowWeapons())
			{
				itemCategories.Add(ExtendedItemCategory.WeaponItemCategory);
			}
			if (EnhancedQuaterMasterService.AutoTradeItems.GetAllowSaddles())
			{
				itemCategories.Add(ExtendedItemCategory.SaddleItemCategory);
			}
			if (EnhancedQuaterMasterService.AutoTradeItems.GetAllowHorses())
			{
				itemCategories.Add(ExtendedItemCategory.HorseItemCategory);
			}
			if (EnhancedQuaterMasterService.AutoTradeItems.GetAllowCamels())
			{
				itemCategories.Add(ExtendedItemCategory.CamelItemCategory);
			}
			if (EnhancedQuaterMasterService.AutoTradeItems.GetAllowMiscellaneous())
			{
				itemCategories.Add(ExtendedItemCategory.MiscellaneousItemCategory);
			}
			if (EnhancedQuaterMasterService.AutoTradeItems.GetAllowBanners())
			{
				itemCategories.Add(ExtendedItemCategory.BannerItemCategory);
			}

			List<ItemRosterElement> itemRosterElements = HeroEquipmentCustomization.getItemsByCategories(MobileParty.MainParty.ItemRoster.ToList(), itemCategories);

			if (EnhancedQuaterMasterService.AutoTradeItems.GetAllowAnyCulture() == false)
			{
				itemRosterElements = HeroEquipmentCustomization.getItemsByCulture(itemRosterElements, EnhancedQuaterMasterService.AutoTradeItems.GetChosenCulture());
			}

			itemRosterElements = ExtendedItemCategory.OrderItemRosterByWeight(itemRosterElements);
			if (EnhancedQuaterMasterService.AutoTradeItems.GetAllowLockedItems() == false)
			{
				itemRosterElements = EquipmentUtil.RemoveLockedItems(itemRosterElements);
			}

			int orderByWeight = Convert.ToInt32(EnhancedQuaterMasterService.AutoTradeItems.GetIsLightestItemsFirst());
			itemRosterElements = ExtendedItemCategory.OrderItemRosterByWeight(itemRosterElements, (ExtendedItemCategory.OrderByWeight)orderByWeight);

			EnhancedQuaterMasterService.SellItems(settlement, itemRosterElements, itemCategories);
		}
	}

	// Taken from SettlementNameplatePartyMarkersVM class TODO move in util
	private bool IsMobilePartyValid(MobileParty party)
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
					).WithCondition(() => EnhancedQuaterMasterService.CompanionEquipment.GetAllowLockedItems() == true)
					.WithConsequence(EnhancedQuaterMasterService.CompanionEquipment.ToggleQuaterMasterAllowLockedItems),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_equipment_settings_add_culture_type_any",
						"Allow Any Equipment",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => EnhancedQuaterMasterService.CompanionEquipment.GetAllowAnyCulture() == true)
					.WithConsequence(EnhancedQuaterMasterService.CompanionEquipment.ToggleQuaterMasterAllow_AnyCulture),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_equipment_settings_add_culture_type_battania",
						"Allow Battania Equipment",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => EnhancedQuaterMasterService.CompanionEquipment.GetAllowBattaniaCulture() == true)
					.WithConsequence(EnhancedQuaterMasterService.CompanionEquipment.ToggleQuaterMasterAllow_BattaniaCulture),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_equipment_settings_add_culture_type_sturgia",
						"Allow Sturgia Equipment",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => EnhancedQuaterMasterService.CompanionEquipment.GetAllowSturgiaCulture() == true)
					.WithConsequence(EnhancedQuaterMasterService.CompanionEquipment.ToggleQuaterMasterAllow_SturgiaCulture),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_equipment_settings_add_culture_type_aserai",
						"Allow Aserai Equipment",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => EnhancedQuaterMasterService.CompanionEquipment.GetAllowAseraiCulture() == true)
					.WithConsequence(EnhancedQuaterMasterService.CompanionEquipment.ToggleQuaterMasterAllow_AseraiCulture),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_equipment_settings_add_culture_type_khuzait",
						"Allow Khuzait Equipment",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => EnhancedQuaterMasterService.CompanionEquipment.GetAllowKhuzaitCulture() == true)
					.WithConsequence(EnhancedQuaterMasterService.CompanionEquipment.ToggleQuaterMasterAllow_KhuzaitCulture),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_equipment_settings_add_culture_type_vlandia",
						"Allow Vlandia Equipment",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => EnhancedQuaterMasterService.CompanionEquipment.GetAllowVlandiaCulture() == true)
					.WithConsequence(EnhancedQuaterMasterService.CompanionEquipment.ToggleQuaterMasterAllow_VlandiaCulture),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_equipment_settings_add_culture_type_empire",
						"Allow Empire Equipment",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => EnhancedQuaterMasterService.CompanionEquipment.GetAllowEmpireCulture() == true)
					.WithConsequence(EnhancedQuaterMasterService.CompanionEquipment.ToggleQuaterMasterAllow_EmpireCulture),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_equipment_type",
						"Allow Battle Equipment",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => EnhancedQuaterMasterService.CompanionEquipment.GetAllowBattleEquipment() == true)
					.WithConsequence(EnhancedQuaterMasterService.CompanionEquipment.ToggleQuaterMasterAllow_BattleEquipment),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_equipment_type_civilian_equipment",
						"Allow Civilian Equipment",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => EnhancedQuaterMasterService.CompanionEquipment.GetAllowCivilianEquipment() == true)
					.WithConsequence(EnhancedQuaterMasterService.CompanionEquipment.ToggleQuaterMasterAllow_CivilianEquipment),
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
					).WithCondition(() => EnhancedQuaterMasterService.AutoTradeItems.GetAllowAnyCulture() == true)
					.WithConsequence(EnhancedQuaterMasterService.AutoTradeItems.ToggleQuaterMasterAllow_AnyCulture),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_equipment_settings_add_culture_type_battania",
						"Allow Battania Equipment",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => EnhancedQuaterMasterService.AutoTradeItems.GetAllowBattaniaCulture() == true)
					.WithConsequence(EnhancedQuaterMasterService.AutoTradeItems.ToggleQuaterMasterAllow_BattaniaCulture),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_equipment_settings_add_culture_type_sturgia",
						"Allow Sturgia Equipment",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => EnhancedQuaterMasterService.AutoTradeItems.GetAllowSturgiaCulture() == true)
					.WithConsequence(EnhancedQuaterMasterService.AutoTradeItems.ToggleQuaterMasterAllow_SturgiaCulture),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_equipment_settings_add_culture_type_aserai",
						"Allow Aserai Equipment",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => EnhancedQuaterMasterService.AutoTradeItems.GetAllowAseraiCulture() == true)
					.WithConsequence(EnhancedQuaterMasterService.AutoTradeItems.ToggleQuaterMasterAllow_AseraiCulture),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_equipment_settings_add_culture_type_khuzait",
						"Allow Khuzait Equipment",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => EnhancedQuaterMasterService.AutoTradeItems.GetAllowKhuzaitCulture() == true)
					.WithConsequence(EnhancedQuaterMasterService.AutoTradeItems.ToggleQuaterMasterAllow_KhuzaitCulture),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_equipment_settings_add_culture_type_vlandia",
						"Allow Vlandia Equipment",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => EnhancedQuaterMasterService.AutoTradeItems.GetAllowVlandiaCulture() == true)
					.WithConsequence(EnhancedQuaterMasterService.AutoTradeItems.ToggleQuaterMasterAllow_VlandiaCulture),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_equipment_settings_add_culture_type_empire",
						"Allow Empire Equipment",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => EnhancedQuaterMasterService.AutoTradeItems.GetAllowEmpireCulture() == true)
					.WithConsequence(EnhancedQuaterMasterService.AutoTradeItems.ToggleQuaterMasterAllow_EmpireCulture),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_auto_trade_items_settings_is_lighest_items_first",
						"Light Items First",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => EnhancedQuaterMasterService.AutoTradeItems.GetIsLightestItemsFirst() == true)
					.WithConsequence(EnhancedQuaterMasterService.AutoTradeItems.ToggleQuaterMasterIsLightestItemsFirst),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_auto_trade_items_settings_allow_locked_items",
						"Allow Locked Items",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => EnhancedQuaterMasterService.AutoTradeItems.GetAllowLockedItems() == true)
					.WithConsequence(EnhancedQuaterMasterService.AutoTradeItems.ToggleQuaterMasterAllowLockedItems),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			// Categories
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_auto_trade_items_settings_allow_armour_items",
						"Allow Armour Items",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => EnhancedQuaterMasterService.AutoTradeItems.GetAllowArmour() == true)
					.WithConsequence(EnhancedQuaterMasterService.AutoTradeItems.ToggleQuaterMasterAllow_ArmourCategory),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_auto_trade_items_settings_allow_weapons_items",
						"Allow Weapons Items",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => EnhancedQuaterMasterService.AutoTradeItems.GetAllowWeapons() == true)
					.WithConsequence(EnhancedQuaterMasterService.AutoTradeItems.ToggleQuaterMasterAllow_WeaponsCategory),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_auto_trade_items_settings_allow_saddles_items",
						"Allow Saddles Items",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => EnhancedQuaterMasterService.AutoTradeItems.GetAllowSaddles() == true)
					.WithConsequence(EnhancedQuaterMasterService.AutoTradeItems.ToggleQuaterMasterAllow_SaddlesCategory),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_auto_trade_items_settings_allow_horses_items",
						"Allow Horses",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => EnhancedQuaterMasterService.AutoTradeItems.GetAllowHorses() == true)
					.WithConsequence(EnhancedQuaterMasterService.AutoTradeItems.ToggleQuaterMasterAllow_HorsesCategory),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_auto_trade_items_settings_allow_camels_items",
						"Allow Camel",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => EnhancedQuaterMasterService.AutoTradeItems.GetAllowCamels() == true)
					.WithConsequence(EnhancedQuaterMasterService.AutoTradeItems.ToggleQuaterMasterAllow_CamelsCategory),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_auto_trade_items_settings_allow_resources_items",
						"Allow Miscellaneous Items",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => EnhancedQuaterMasterService.AutoTradeItems.GetAllowMiscellaneous() == true)
					.WithConsequence(EnhancedQuaterMasterService.AutoTradeItems.ToggleQuaterMasterAllow_MiscellaneousCategory),
				AppliedDialogueLineRelation.LinkToCurrentBranch)
			.WithTrueFalseConversationToggle(
				new SimpleConversationPart(
						"enhanced_quatermaster_conv_menu_configure_auto_trade_items_settings_allow_banners_items",
						"Allow Banners Items",
						ConversationSentenceType.DialogueTreeBranchPart
					).WithCondition(() => EnhancedQuaterMasterService.AutoTradeItems.GetAllowBanners() == true)
					.WithConsequence(EnhancedQuaterMasterService.AutoTradeItems.ToggleQuaterMasterAllow_BannersCategory),
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

		bool canRemoveLockedItems = EnhancedQuaterMasterService.CompanionEquipment.GetAllowLockedItems() == false;

		CultureCode chosenCulture = EnhancedQuaterMasterService.CompanionEquipment.GetChosenCulture();

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

			if (EnhancedQuaterMasterService.CompanionEquipment.GetAllowBattleEquipment())
			{
				EnhancedQuaterMasterService.updateItemRoster(itemRoster, fighterClass.removeRelavantBattleEquipment(items), new List<ItemRosterElement>());

				items = canRemoveLockedItems ? EquipmentUtil.RemoveLockedItems(itemRoster.ToList()) : itemRoster.ToList();
				var changes = fighterClass.assignBattleEquipment(items);
				categories = ExtendedItemCategory.AddItemCategoryNamesFromItemList(changes.removals, fighterClass.MainItemCategories, categories);
				EnhancedQuaterMasterService.updateItemRoster(itemRoster, changes.additions, changes.removals);
			}
			if (EnhancedQuaterMasterService.CompanionEquipment.GetAllowCivilianEquipment())
			{
				items = canRemoveLockedItems ? EquipmentUtil.RemoveLockedItems(itemRoster.ToList()) : itemRoster.ToList();
				EnhancedQuaterMasterService.updateItemRoster(itemRoster, fighterClass.removeRelavantCivilianEquipment(items), new List<ItemRosterElement>());
				var changes = fighterClass.assignCivilianEquipment(items);

				categories = ExtendedItemCategory.AddItemCategoryNamesFromItemList(changes.removals, fighterClass.MainItemCategories, categories);
				EnhancedQuaterMasterService.updateItemRoster(itemRoster, changes.additions, changes.removals);
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

