using System;
using System.Collections.Generic;
using System.Linq;
using BannerlordEnhancedFramework.extendedtypes;
using BannerlordEnhancedFramework.src.utils;
using BannerlordEnhancedFramework.utils;
using BannerlordEnhancedPartyRoles.Behaviors;
using BannerlordEnhancedPartyRoles.src.Behaviors;
using BannerlordEnhancedPartyRoles.src.Services;
using TaleWorlds.CampaignSystem.Inventory;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace BannerlordEnhancedPartyRoles.patches;

public class QuartermasterPatches
{
    public static void DoneLogic_Postfix(InventoryLogic __instance, bool __result)
    {
        DebugUtils.LogAndPrintInfo("DoneLogic_Postfix is working");
		if (InventoryManager.InventoryLogic.IsTrading)
		{

			InventoryLogic inventoryLogic = InventoryManager.InventoryLogic;
			if (inventoryLogic.CurrentSettlementComponent == null || inventoryLogic.CurrentSettlementComponent.IsTown == false)
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
			if (EnhancedQuaterMasterService.AutoTradeItems.GetAllowResources())
			{
				itemCategories.Add(ExtendedItemCategory.ResourcesGoodsItemCategory);
			}
			if (EnhancedQuaterMasterService.AutoTradeItems.GetAllowBanners())
			{
				itemCategories.Add(ExtendedItemCategory.BannerItemCategory);
			}

			List<ItemRosterElement> itemRosterElements = HeroEquipmentCustomization.getItemsByCategories(MobileParty.MainParty.ItemRoster.ToList(), itemCategories);
			
			if(EnhancedQuaterMasterService.AutoTradeItems.GetAllowAnyCulture() == false)
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
			
			EnhancedQuaterMasterService.SellItems(itemRosterElements, itemCategories);
		}
	}

	public static void CloseInventoryPresentation_Prefix(bool fromCancel)
	{
		EnhancedQuaterMasterService.CompanionEquipment.SetIsLastInventoryCancelPressed(fromCancel);
	}
	public static void CloseInventoryPresentation_Postfix(bool fromCancel)
	{
		int currentVersionNo = MobileParty.MainParty.ItemRoster.VersionNo;
		if (EnhancedQuaterMasterService.CompanionEquipment.GetIsLastInventoryCancelPressed() == false && currentVersionNo != EnhancedQuaterMasterService.CompanionEquipment.GetLastItemRosterVersionNo())
		{
			EnhancedQuaterMasterBehavior.GiveBestEquipmentFromItemRoster();
		}
		EnhancedQuaterMasterService.CompanionEquipment.SetLastItemRosterVersionNo(currentVersionNo);
	}
	
	public static void ClanPresentationDone_Postfix()
	{
		int latestVersionNo = EnhancedQuaterMasterService.CompanionEquipment.GetLatestFilterSettingsVersionNo();
		if (latestVersionNo != EnhancedQuaterMasterService.CompanionEquipment.GetPreviousFilterSettingsVersionNo())
		{
			EnhancedQuaterMasterBehavior.GiveBestEquipmentFromItemRoster();
		}
		EnhancedQuaterMasterService.CompanionEquipment.SetPreviousFilterSettingsVersionNo(latestVersionNo);
	}
}