using System;
using System.Collections.Generic;
using BannerlordEnhancedFramework.extendedtypes;
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
	public static void SellItems(Settlement settlement,List<ItemRosterElement> items, List<ExtendedItemCategory>  itemCategories)
	{
		InventoryLogic inventoryLogic = InventoryManager.InventoryLogic;
		List<ValueTuple<ItemRosterElement, int>> itemsToSellTuple = new List<ValueTuple<ItemRosterElement, int>>();
		List<ValueTuple<ItemRosterElement, int>> itemsToBuyTuple = new List<ValueTuple<ItemRosterElement, int>>();
		List<ItemRosterElement> itemsToSell = new List<ItemRosterElement>();

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
			int price = marketData.GetPrice(itemRosterElement.EquipmentElement, MobileParty.MainParty, true, null);
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
			itemsToSell.Add(soldItemRosterElement);

		}
		int income = settlementComponent.Gold - settlementGold;
		if (income == 0)
		{
			return;
		}
		// 
		Dictionary<string, int> categories = new Dictionary<string, int>();
		categories = ExtendedItemCategory.AddItemCategoryNamesFromItemList(itemsToSell, itemCategories, categories);

		bool isTrading = true;
		CampaignEventDispatcher.Instance.OnPlayerInventoryExchange(itemsToBuyTuple, itemsToSellTuple, isTrading);

		settlementComponent.ChangeGold(settlementComponent.Gold - income);
		MobileParty mainParty = MobileParty.MainParty;
		GiveGoldAction.ApplyBetweenCharacters(null, mainParty.Party.LeaderHero, income, false);
		if (mainParty.Party.LeaderHero.CompanionOf != null)
		{
			mainParty.AddTaxGold((int)((float)income * 0.1f));
		}

		updateItemRoster(mainParty.ItemRoster, new List<ItemRosterElement>(), itemsToSell);
		updateItemRoster(Settlement.CurrentSettlement.ItemRoster, itemsToSell, new List<ItemRosterElement>());

		string notification = "Quatermaster sold items from your inventory";
		foreach(KeyValuePair<string, int> item in categories)
		{
			notification += "\n"+item.Key + " " + item.Value;
		}
		InformationManager.DisplayMessage(new InformationMessage(notification, BannerlordEnhancedFramework.Colors.Yellow));

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
	
	public static class AutoTradeItems
	{
		// Culture
		public static void ToggleQuaterMasterAllowLockedItems()
		{
			SetAllowLockedItems(GetAllowLockedItems() == false ? true : false);
		}
		public static void ToggleQuaterMasterAllow_AnyCulture()
		{
			bool currentToggle = GetAllowAnyCulture();
			SetAllowAnyCulture(currentToggle == false ? true : false);
			if (GetAllowAnyCulture())
			{
				SetAllCultureToAllowTrue();
			}
			else
			{
				SetAllCultureToAllowFalse();
			}
		}
		public static void ToggleQuaterMasterAllow_BattaniaCulture()
		{
			bool currentToggle = GetAllowBattaniaCulture();
			SetAllCultureToAllowFalse();
			SetAllowBattaniaCulture(currentToggle == false ? true : false);
		}
		public static void ToggleQuaterMasterAllow_SturgiaCulture()
		{
			bool currentToggle = GetAllowSturgiaCulture();
			SetAllCultureToAllowFalse();
			SetAllowSturgiaCulture(currentToggle == false ? true : false);
		}
		public static void ToggleQuaterMasterAllow_AseraiCulture()
		{
			bool currentToggle = GetAllowAseraiCulture();
			SetAllCultureToAllowFalse();
			SetAllowAseraiCulture(currentToggle == false ? true : false);
		}
		public static void ToggleQuaterMasterAllow_KhuzaitCulture()
		{
			bool currentToggle = GetAllowKhuzaitCulture();
			SetAllCultureToAllowFalse();
			SetAllowKhuzaitCulture(currentToggle == false ? true : false);
		}
		public static void ToggleQuaterMasterAllow_VlandiaCulture()
		{
			bool currentToggle = GetAllowVlandiaCulture();
			SetAllCultureToAllowFalse();
			SetAllowVlandiaCulture(currentToggle == false ? true : false);
		}
		public static void ToggleQuaterMasterAllow_EmpireCulture()
		{
			bool currentToggle = GetAllowEmpireCulture();
			SetAllCultureToAllowFalse();
			SetAllowEmpireCulture(currentToggle == false ? true : false);
		}

		public static void SetAllCultureToAllowFalse()
		{
			SetAllowAnyCulture(false);
			SetAllowBattaniaCulture(false);
			SetAllowSturgiaCulture(false);
			SetAllowAseraiCulture(false);
			SetAllowKhuzaitCulture(false);
			SetAllowVlandiaCulture(false);
			SetAllowEmpireCulture(false);
		}
		public static void SetAllCultureToAllowTrue()
		{
			SetAllowAnyCulture(true);
			SetAllowBattaniaCulture(true);
			SetAllowSturgiaCulture(true);
			SetAllowAseraiCulture(true);
			SetAllowKhuzaitCulture(true);
			SetAllowVlandiaCulture(true);
			SetAllowEmpireCulture(true);
		}
		public static void SetAllowAnyCulture(bool flag)
		{
			EnhancedQuaterMasterData.AutoTradeItems.AllowAnyCulture = flag;
		}
		public static void SetAllowLockedItems(bool flag)
		{
			EnhancedQuaterMasterData.AutoTradeItems.AllowLockedItems = flag;
		}
		public static void SetAllowBattaniaCulture(bool flag)
		{
			EnhancedQuaterMasterData.AutoTradeItems.AllowBattaniaCulture = flag;
		}
		public static void SetAllowSturgiaCulture(bool flag)
		{
			EnhancedQuaterMasterData.AutoTradeItems.AllowSturgiaCulture = flag;
		}
		public static void SetAllowAseraiCulture(bool flag)
		{
			EnhancedQuaterMasterData.AutoTradeItems.AllowAseraiCulture = flag;
		}
		public static void SetAllowKhuzaitCulture(bool flag)
		{
			EnhancedQuaterMasterData.AutoTradeItems.AllowKhuzaitCulture = flag;
		}
		public static void SetAllowVlandiaCulture(bool flag)
		{
			EnhancedQuaterMasterData.AutoTradeItems.AllowVlandiaCulture = flag;
		}
		public static void SetAllowEmpireCulture(bool flag)
		{
			EnhancedQuaterMasterData.AutoTradeItems.AllowEmpireCulture = flag;
		}
		public static bool GetAllowAnyCulture()
		{
			return EnhancedQuaterMasterData.AutoTradeItems.AllowAnyCulture;
		}
		public static bool GetAllowBattaniaCulture()
		{
			return EnhancedQuaterMasterData.AutoTradeItems.AllowBattaniaCulture;
		}
		public static bool GetAllowSturgiaCulture()
		{
			return EnhancedQuaterMasterData.AutoTradeItems.AllowSturgiaCulture;
		}
		public static bool GetAllowAseraiCulture()
		{
			return EnhancedQuaterMasterData.AutoTradeItems.AllowAseraiCulture;
		}
		public static bool GetAllowKhuzaitCulture()
		{
			return EnhancedQuaterMasterData.AutoTradeItems.AllowKhuzaitCulture;
		}
		public static bool GetAllowVlandiaCulture()
		{
			return EnhancedQuaterMasterData.AutoTradeItems.AllowVlandiaCulture;
		}
		public static bool GetAllowEmpireCulture()
		{
			return EnhancedQuaterMasterData.AutoTradeItems.AllowEmpireCulture;
		}

		public static CultureCode GetChosenCulture()
		{
			if (GetAllowAnyCulture())
			{
				return CultureCode.AnyOtherCulture;
			}
			if (GetAllowBattaniaCulture())
			{
				return CultureCode.Battania;
			}
			else if (GetAllowSturgiaCulture())
			{
				return CultureCode.Sturgia;
			}
			else if (GetAllowAseraiCulture())
			{
				return CultureCode.Aserai;
			}
			else if (GetAllowKhuzaitCulture())
			{
				return CultureCode.Khuzait;
			}
			else if (GetAllowVlandiaCulture())
			{
				return CultureCode.Vlandia;
			}
			else if (GetAllowEmpireCulture())
			{
				return CultureCode.Empire;
			}
			return CultureCode.Invalid;
		}


		// Categories
		public static void ToggleQuaterMasterAllow_ArmourCategory()
		{
			SetAllowArmour(!GetAllowArmour());
		}
		public static void ToggleQuaterMasterAllow_WeaponsCategory()
		{
			SetAllowWeapons(!GetAllowWeapons());
		}
		public static void ToggleQuaterMasterAllow_SaddlesCategory()
		{
			SetAllowSaddles(!GetAllowSaddles());
		}
		public static void ToggleQuaterMasterAllow_HorsesCategory()
		{
			SetAllowHorses(!GetAllowHorses());
		}
		public static void ToggleQuaterMasterAllow_CamelsCategory()
		{
			SetAllowCamels(!GetAllowCamels());
		}
		public static void ToggleQuaterMasterAllow_MiscellaneousCategory()
		{
			SetAllowMiscellaneous(!GetAllowMiscellaneous());
		}
		public static void ToggleQuaterMasterAllow_BannersCategory()
		{
			SetAllowBanners(!GetAllowBanners());
		}

		public static void SetAllowArmour(bool flag)
		{
			EnhancedQuaterMasterData.AutoTradeItems.AllowArmour = flag;
		}
		public static void SetAllowWeapons(bool flag)
		{
			EnhancedQuaterMasterData.AutoTradeItems.AllowWeapons = flag;
		}
		public static void SetAllowSaddles(bool flag)
		{
			EnhancedQuaterMasterData.AutoTradeItems.AllowSaddles = flag;
		}
		public static void SetAllowHorses(bool flag)
		{
			EnhancedQuaterMasterData.AutoTradeItems.AllowHorses = flag;
		}
		public static void SetAllowCamels(bool flag)
		{
			EnhancedQuaterMasterData.AutoTradeItems.AllowCamels = flag;
		}
		public static void SetAllowMiscellaneous(bool flag)
		{
			EnhancedQuaterMasterData.AutoTradeItems.AllowMiscellaneous = flag;
		}
		public static void SetAllowBanners(bool flag)
		{
			EnhancedQuaterMasterData.AutoTradeItems.AllowBanners = flag;
		}

		public static bool GetAllowLockedItems()
		{
			return EnhancedQuaterMasterData.AutoTradeItems.AllowLockedItems;
		}
		public static bool GetAllowArmour()
		{
			return EnhancedQuaterMasterData.AutoTradeItems.AllowArmour;
		}
		public static bool GetAllowWeapons()
		{
			return EnhancedQuaterMasterData.AutoTradeItems.AllowWeapons;
		}
		public static bool GetAllowSaddles()
		{
			return EnhancedQuaterMasterData.AutoTradeItems.AllowSaddles;
		}
		public static bool GetAllowMiscellaneous()
		{
			return EnhancedQuaterMasterData.AutoTradeItems.AllowMiscellaneous;
		}
		public static bool GetAllowHorses()
		{
			return EnhancedQuaterMasterData.AutoTradeItems.AllowHorses;
		}
		public static bool GetAllowCamels()
		{
			return EnhancedQuaterMasterData.AutoTradeItems.AllowCamels;
		}
		public static bool GetAllowBanners()
		{
			return EnhancedQuaterMasterData.AutoTradeItems.AllowBanners;
		}

		// Weight 
		public static void ToggleQuaterMasterIsLightestItemsFirst()
		{
			SetIsLightestItemsFirst(!GetIsLightestItemsFirst());
		}
		public static bool GetIsLightestItemsFirst()
		{
			return EnhancedQuaterMasterData.AutoTradeItems.IsLighestItemsFirst;
		}
		public static void SetIsLightestItemsFirst(bool flag)
		{
			EnhancedQuaterMasterData.AutoTradeItems.IsLighestItemsFirst = flag;
		}
	}
	public static class CompanionEquipment
	{
		public static void ToggleQuaterMasterAllowLockedItems()
		{
			SetAllowLockedItems(GetAllowLockedItems() == false ? true : false);
		}
		public static void ToggleQuaterMasterAllow_AnyCulture()
		{
			bool currentToggle = GetAllowAnyCulture();
			SetAllowAnyCulture(currentToggle == false ? true : false);
			if (GetAllowAnyCulture())
			{
				SetAllCultureToAllowTrue();
			}
			else
			{
				SetAllCultureToAllowFalse();
			}
		}
		public static void ToggleQuaterMasterAllow_BattaniaCulture()
		{
			bool currentToggle = GetAllowBattaniaCulture();
			SetAllCultureToAllowFalse();
			SetAllowBattaniaCulture(currentToggle == false ? true : false);
		}
		public static void ToggleQuaterMasterAllow_SturgiaCulture()
		{
			bool currentToggle = GetAllowSturgiaCulture();
			SetAllCultureToAllowFalse();
			SetAllowSturgiaCulture(currentToggle == false ? true : false);
		}
		public static void ToggleQuaterMasterAllow_AseraiCulture()
		{
			bool currentToggle = GetAllowAseraiCulture();
			SetAllCultureToAllowFalse();
			SetAllowAseraiCulture(currentToggle == false ? true : false);
		}
		public static void ToggleQuaterMasterAllow_KhuzaitCulture()
		{
			bool currentToggle = GetAllowKhuzaitCulture();
			SetAllCultureToAllowFalse();
			SetAllowKhuzaitCulture(currentToggle == false ? true : false);
		}
		public static void ToggleQuaterMasterAllow_VlandiaCulture()
		{
			bool currentToggle = GetAllowVlandiaCulture();
			SetAllCultureToAllowFalse();
			SetAllowVlandiaCulture(currentToggle == false ? true : false);
		}
		public static void ToggleQuaterMasterAllow_EmpireCulture()
		{
			bool currentToggle = GetAllowEmpireCulture();
			SetAllCultureToAllowFalse();
			SetAllowEmpireCulture(currentToggle == false ? true : false);
		}

		public static void ToggleQuaterMasterAllow_BattleEquipment()
		{
			SetAllowBattleEquipment(GetAllowBattleEquipment() == false ? true : false);
		}
		public static void ToggleQuaterMasterAllow_CivilianEquipment()
		{
			SetAllowCivilianEquipment(GetAllowCivilianEquipment() == false ? true : false);
		}

		public static void SetAllCultureToAllowFalse()
		{
			SetAllowAnyCulture(false);
			SetAllowBattaniaCulture(false);
			SetAllowSturgiaCulture(false);
			SetAllowAseraiCulture(false);
			SetAllowKhuzaitCulture(false);
			SetAllowVlandiaCulture(false);
			SetAllowEmpireCulture(false);
		}
		public static void SetAllCultureToAllowTrue()
		{
			SetAllowAnyCulture(true);
			SetAllowBattaniaCulture(true);
			SetAllowSturgiaCulture(true);
			SetAllowAseraiCulture(true);
			SetAllowKhuzaitCulture(true);
			SetAllowVlandiaCulture(true);
			SetAllowEmpireCulture(true);
		}

		public static void SetAllowAnyCulture(bool flag)
		{
			EnhancedQuaterMasterData.CompanionEquiptment.LatestFilterSettingsVersionNo += 1;
			EnhancedQuaterMasterData.CompanionEquiptment.AllowAnyCulture = flag;
		}
		public static void SetAllowLockedItems(bool flag)
		{
			EnhancedQuaterMasterData.CompanionEquiptment.LatestFilterSettingsVersionNo += 1;
			EnhancedQuaterMasterData.CompanionEquiptment.AllowLockedEquipment = flag;
		}
		public static void SetAllowBattaniaCulture(bool flag)
		{
			EnhancedQuaterMasterData.CompanionEquiptment.LatestFilterSettingsVersionNo += 1;
			EnhancedQuaterMasterData.CompanionEquiptment.AllowBattaniaCulture = flag;
		}
		public static void SetAllowSturgiaCulture(bool flag)
		{
			EnhancedQuaterMasterData.CompanionEquiptment.LatestFilterSettingsVersionNo += 1;
			EnhancedQuaterMasterData.CompanionEquiptment.AllowSturgiaCulture = flag;
		}
		public static void SetAllowAseraiCulture(bool flag)
		{
			EnhancedQuaterMasterData.CompanionEquiptment.LatestFilterSettingsVersionNo += 1;
			EnhancedQuaterMasterData.CompanionEquiptment.AllowAseraiCulture = flag;
		}
		public static void SetAllowKhuzaitCulture(bool flag)
		{
			EnhancedQuaterMasterData.CompanionEquiptment.LatestFilterSettingsVersionNo += 1;
			EnhancedQuaterMasterData.CompanionEquiptment.AllowKhuzaitCulture = flag;
		}
		public static void SetAllowVlandiaCulture(bool flag)
		{
			EnhancedQuaterMasterData.CompanionEquiptment.LatestFilterSettingsVersionNo += 1;
			EnhancedQuaterMasterData.CompanionEquiptment.AllowVlandiaCulture = flag;
		}
		public static void SetAllowEmpireCulture(bool flag)
		{
			EnhancedQuaterMasterData.CompanionEquiptment.LatestFilterSettingsVersionNo += 1;
			EnhancedQuaterMasterData.CompanionEquiptment.AllowEmpireCulture = flag;
		}

		public static void SetAllowBattleEquipment(bool flag)
		{
			EnhancedQuaterMasterData.CompanionEquiptment.AllowBattleEquipment = flag;
		}
		public static void SetAllowCivilianEquipment(bool flag)
		{
			EnhancedQuaterMasterData.CompanionEquiptment.AllowCivilianEquipment = flag;
		}

		public static void SetLastItemRosterVersionNo(int version)
		{
			EnhancedQuaterMasterData.CompanionEquiptment.LastItemRosterVersionNo = version;
		}
		public static void SetIsLastInventoryCancelPressed(bool fromCancel)
		{
			EnhancedQuaterMasterData.CompanionEquiptment.IsLastInventoryCancelPressed = fromCancel;
		}

		public static void SetPreviousFilterSettingsVersionNo(int versionNo)
		{
			EnhancedQuaterMasterData.CompanionEquiptment.PreviousFilterSettingsVersionNo = versionNo;
		}


		public static bool GetAllowAnyCulture()
		{
			return EnhancedQuaterMasterData.CompanionEquiptment.AllowAnyCulture;
		}
		public static bool GetAllowLockedItems()
		{
			return EnhancedQuaterMasterData.CompanionEquiptment.AllowLockedEquipment;
		}
		public static bool GetAllowBattaniaCulture()
		{
			return EnhancedQuaterMasterData.CompanionEquiptment.AllowBattaniaCulture;
		}
		public static bool GetAllowSturgiaCulture()
		{
			return EnhancedQuaterMasterData.CompanionEquiptment.AllowSturgiaCulture;
		}
		public static bool GetAllowAseraiCulture()
		{
			return EnhancedQuaterMasterData.CompanionEquiptment.AllowAseraiCulture;
		}
		public static bool GetAllowKhuzaitCulture()
		{
			return EnhancedQuaterMasterData.CompanionEquiptment.AllowKhuzaitCulture;
		}
		public static bool GetAllowVlandiaCulture()
		{
			return EnhancedQuaterMasterData.CompanionEquiptment.AllowVlandiaCulture;
		}
		public static bool GetAllowEmpireCulture()
		{
			return EnhancedQuaterMasterData.CompanionEquiptment.AllowEmpireCulture;
		}

		public static bool GetAllowBattleEquipment()
		{
			return EnhancedQuaterMasterData.CompanionEquiptment.AllowBattleEquipment;
		}
		public static bool GetAllowCivilianEquipment()
		{
			return EnhancedQuaterMasterData.CompanionEquiptment.AllowCivilianEquipment;
		}
		public static int GetLastItemRosterVersionNo()
		{
			return EnhancedQuaterMasterData.CompanionEquiptment.LastItemRosterVersionNo;
		}

		public static bool GetIsLastInventoryCancelPressed()
		{
			return EnhancedQuaterMasterData.CompanionEquiptment.IsLastInventoryCancelPressed;
		}

		public static int GetLatestFilterSettingsVersionNo()
		{
			return EnhancedQuaterMasterData.CompanionEquiptment.LatestFilterSettingsVersionNo;
		}
		public static int GetPreviousFilterSettingsVersionNo()
		{
			return EnhancedQuaterMasterData.CompanionEquiptment.PreviousFilterSettingsVersionNo;
		}
		public static CultureCode GetChosenCulture()
		{
			if (GetAllowAnyCulture())
			{
				return CultureCode.AnyOtherCulture;
			}
			if (GetAllowBattaniaCulture())
			{
				return CultureCode.Battania;
			}
			else if (GetAllowSturgiaCulture())
			{
				return CultureCode.Sturgia;
			}
			else if (GetAllowAseraiCulture())
			{
				return CultureCode.Aserai;
			}
			else if (GetAllowKhuzaitCulture())
			{
				return CultureCode.Khuzait;
			}
			else if (GetAllowVlandiaCulture())
			{
				return CultureCode.Vlandia;
			}
			else if (GetAllowEmpireCulture())
			{
				return CultureCode.Empire;
			}
			return CultureCode.Invalid;
		}
	}
}

