using System;
using System.Collections.Generic;
using System.Linq;
using BannerlordEnhancedFramework.extendedtypes;
using BannerlordEnhancedFramework.extendedtypes.itemcategories;
using BannerlordEnhancedFramework.src.utils;
using BannerlordEnhancedFramework.utils;
using BannerlordEnhancedPartyRoles.src.Storage;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace BannerlordEnhancedPartyRoles.src.Services
{
	public static class AutoTraderService
	{
		public static List<ExtendedItemCategory> GetItemCategoriesWhenSellingByCultureCode(ExtendedCultureCode cultureCode)
		{
			List<ExtendedItemCategory> itemCategories = new List<ExtendedItemCategory>();

			if (!GetAllowCulture(cultureCode))
			{
				return itemCategories;
			}

			if (GetAllowBodyArmour())
			{
				itemCategories.Add(ExtendedItemCategories.BodyArmourItemCategory);
			}
			if (GetAllowWeapons())
			{
				itemCategories.Add(ExtendedItemCategories.WeaponItemCategory);
			}
			if (GetAllowSaddles())
			{
				itemCategories.Add(ExtendedItemCategories.SaddleItemCategory);
			}
			if (GetAllowHorses())
			{
				itemCategories.Add(ExtendedItemCategories.HorseItemCategory);
			}
			if (GetAllowCamels())
			{
				itemCategories.Add(ExtendedItemCategories.CamelItemCategory);
			}
			if (GetAllowBanners())
			{
				itemCategories.Add(ExtendedItemCategories.BannerItemCategory);
			}
			if (GetAllowMiscellaneous())
			{
				itemCategories.Add(ExtendedItemCategories.MiscellaneousItemCategory);
			}
			if (GetIncludeLockedItemsWhenSelling())
			{
				itemCategories.Add(ExtendedItemCategories.LockedItemCategory);
			}
			return itemCategories;
		}

		// Culture
		public static void ToggleQuarterMasterAllowLockedItems()
		{
			if (GetIncludeLockedItemsWhenSelling())
			{
				SetAllowLockedItems(false);
			} else {
				SetAllowLockedItems(true);
			}
		}
		public static void SetAllowLockedItems(bool flag)
		{
			EnhancedQuarterMasterData.AutoTraderData.AllowLockedItems = flag;
		}

		public static bool GetIncludeLockedItemsWhenSelling()
		{
			return EnhancedQuarterMasterData.AutoTraderData.AllowLockedItems;
		}
		public static bool GetAllowBodyArmour(ExtendedCultureCode cultureCode)
		{
			return EnhancedQuarterMasterData.AutoTraderData.CultureToItemCategoryFilters[cultureCode]["AllowBodyArmour"];
		}
		
		public static bool GetAllowBodyArmour()
		{
			return EnhancedQuarterMasterData.AutoTraderData.AllowBodyArmor;
		}

		public static void ToggleAllowBodyArmour()
		{
			bool AllowBodyArmor = !EnhancedQuarterMasterData.AutoTraderData.AllowBodyArmor;
			EnhancedQuarterMasterData.AutoTraderData.AllowBodyArmor = AllowBodyArmor;
		}

		public static bool GetAllowWeapons()
		{
			return EnhancedQuarterMasterData.AutoTraderData.AllowWeapons;
		}
		public static void ToggleAllowWeapons()
		{
			bool AllowWeapons = !EnhancedQuarterMasterData.AutoTraderData.AllowWeapons;
			EnhancedQuarterMasterData.AutoTraderData.AllowWeapons = AllowWeapons;
		}
		public static bool GetAllowSaddles()
		{
			return EnhancedQuarterMasterData.AutoTraderData.AllowSaddles;
		}
		public static void ToggleAllowSaddles()
		{
			bool AllowSaddles = !EnhancedQuarterMasterData.AutoTraderData.AllowSaddles;
			EnhancedQuarterMasterData.AutoTraderData.AllowSaddles = AllowSaddles;
		}
		public static bool GetAllowMiscellaneous()
		{
			return EnhancedQuarterMasterData.AutoTraderData.AllowMiscellaneous;
		}
		public static void ToggleAllowMiscellaneous()
		{
			bool AllowMiscellaneous = !EnhancedQuarterMasterData.AutoTraderData.AllowMiscellaneous;
			EnhancedQuarterMasterData.AutoTraderData.AllowMiscellaneous = AllowMiscellaneous;
		}
		public static bool GetAllowHorses()
		{
			return EnhancedQuarterMasterData.AutoTraderData.AllowHorses;
		}
		public static void ToggleAllowHorses()
		{
			bool ToggleAllowHorses = !EnhancedQuarterMasterData.AutoTraderData.AllowHorses;
			EnhancedQuarterMasterData.AutoTraderData.AllowHorses = ToggleAllowHorses;
		}
		public static bool GetAllowCamels()
		{
			return EnhancedQuarterMasterData.AutoTraderData.AllowCamels;
		}
		public static void ToggleAllowCamels()
		{
			bool AllowCamels = !EnhancedQuarterMasterData.AutoTraderData.AllowCamels;
			EnhancedQuarterMasterData.AutoTraderData.AllowCamels = AllowCamels;
		}
		public static bool GetAllowBanners()
		{
			return EnhancedQuarterMasterData.AutoTraderData.AllowBanners;
		}
		
		public static void ToggleAllowBanners()
		{
			bool AllowBanners = !EnhancedQuarterMasterData.AutoTraderData.AllowBanners;
			EnhancedQuarterMasterData.AutoTraderData.AllowBanners = AllowBanners;
		}
		
		
		public static void SetAllowBodyArmour(ExtendedCultureCode cultureCode, bool flag)
		{
			EnhancedQuarterMasterData.AutoTraderData.CultureToItemCategoryFilters[cultureCode]["AllowBodyArmour"] = flag;
		}
		public static void SetAllowWeapons(ExtendedCultureCode cultureCode, bool flag)
		{
			EnhancedQuarterMasterData.AutoTraderData.CultureToItemCategoryFilters[cultureCode]["AllowWeapons"] = flag;
		}
		public static void SetAllowSaddles(ExtendedCultureCode cultureCode, bool flag)
		{
			EnhancedQuarterMasterData.AutoTraderData.CultureToItemCategoryFilters[cultureCode]["AllowSaddles"] = flag;
		}
		public static void SetAllowHorses(ExtendedCultureCode cultureCode, bool flag)
		{
			EnhancedQuarterMasterData.AutoTraderData.CultureToItemCategoryFilters[cultureCode]["AllowHorses"] = flag;
		}
		public static void SetAllowCamels(ExtendedCultureCode cultureCode, bool flag)
		{
			EnhancedQuarterMasterData.AutoTraderData.CultureToItemCategoryFilters[cultureCode]["AllowCamels"] = flag;
		}
		public static void SetAllowMiscellaneous(ExtendedCultureCode cultureCode, bool flag)
		{
			EnhancedQuarterMasterData.AutoTraderData.CultureToItemCategoryFilters[cultureCode]["AllowMiscellaneous"] = flag;
		}
		public static void SetAllowBanners(ExtendedCultureCode cultureCode, bool flag)
		{
			EnhancedQuarterMasterData.AutoTraderData.CultureToItemCategoryFilters[cultureCode]["AllowBanners"] = flag;
		}

		// Weight 
		public static void ToggleQuarterMasterIsLightestItemsFirst()
		{
			SetIsLightestItemsFirst(!GetIsLightestItemsFirst());
		}
		
		public static EquipmentUtil.OrderBy GetItemOrderByWhenSelling()
		{
			return GetIsLightestItemsFirst() ? EquipmentUtil.OrderBy.LIGHTEST_TO_HEAVIEST : EquipmentUtil.OrderBy.HEAVIEST_TO_LIGHTEST;
		}
		public static bool GetIsLightestItemsFirst()
		{
			return EnhancedQuarterMasterData.AutoTraderData.IsLightestItemsFirst;
		}
		public static void SetIsLightestItemsFirst(bool flag)
		{
			EnhancedQuarterMasterData.AutoTraderData.IsLightestItemsFirst = flag;
		}

		public static void SellItemsWhenMainPartyEntersSettlement(Settlement settlement)
		{
			List<ItemRosterElement> itemRosterElementsToSell = new List<ItemRosterElement>();
			// iterate through every faction and filter the items out for that faction, add it to the main filtered out list
			Dictionary<string, int> categoriesSold = new Dictionary<string, int>();
			foreach (ExtendedCultureCode cultureCode in ExtendedCultureCode.values()) {
				
				List<ExtendedItemCategory> itemCategories = GetItemCategoriesWhenSellingByCultureCode(cultureCode);
				
				itemRosterElementsToSell.AddRange(
					EquipmentUtil.FilterItemRosterByItemCategories(
						MobileParty.MainParty.ItemRoster.ToList(),
						itemCategories,
						GetItemOrderByWhenSelling())
				);

				categoriesSold = categoriesSold.Concat(
						ExtendedItemCategory.GetItemCategoryToTotalWorthForCategoryFromItems(itemRosterElementsToSell, itemCategories)
					)
					.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
			}
			if(itemRosterElementsToSell.Count > 0)
			{
				List<ItemRosterElement> itemsSold = PartyUtils.SellItemsToSettlement(MobileParty.MainParty, settlement, itemRosterElementsToSell);
				if (itemsSold.Count > 0)
				{
					WindowUtils.DisplayMessageListNameAndTotal(categoriesSold, "Quartermaster sold items from your inventory");
				}
			}		
		}

		public static bool GetAllowAnyCulture()
		{
			return EnhancedQuarterMasterData.AutoTraderData.AllowAnyCulture;
		}

		public static void ToggleAllowAnyCulture()
		{
			if (!EnhancedQuarterMasterData.AutoTraderData.AllowAnyCulture)
			{
				foreach (ExtendedCultureCode extendedCultureCode in ExtendedCultureCode.values())
				{
					EnhancedQuarterMasterData.AutoTraderData.CultureToItemCategoryFilters[extendedCultureCode]["LockedAll"] = false;
				}

				EnhancedQuarterMasterData.AutoTraderData.AllowAnyCulture = true;
			}
			else
			{
				foreach (ExtendedCultureCode extendedCultureCode in ExtendedCultureCode.values())
				{
					EnhancedQuarterMasterData.AutoTraderData.CultureToItemCategoryFilters[extendedCultureCode]["LockedAll"] = true;
				}

				EnhancedQuarterMasterData.AutoTraderData.AllowAnyCulture = false;
			}
		}

		public static bool GetAllowCulture(ExtendedCultureCode cultureCode)
		{
			return EnhancedQuarterMasterData.AutoTraderData.CultureToItemCategoryFilters[cultureCode]["AllowAll"];
		}
		
		public static void SetAllowCulture(ExtendedCultureCode cultureCode, bool flag)
		{
			EnhancedQuarterMasterData.AutoTraderData.CultureToItemCategoryFilters[cultureCode]["AllowAll"] = flag;
		}
		
		public static void ToggleAllowCulture(ExtendedCultureCode cultureCode)
		{
			SetAllowCulture(cultureCode,!GetAllowCulture(cultureCode));
		}
	}
}
