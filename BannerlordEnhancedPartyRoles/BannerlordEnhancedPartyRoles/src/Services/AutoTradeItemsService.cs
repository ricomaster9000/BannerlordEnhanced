using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using BannerlordEnhancedFramework.extendedtypes;
using BannerlordEnhancedFramework.src.utils;
using BannerlordEnhancedPartyRoles.src.Storage;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace BannerlordEnhancedPartyRoles.src.Services
{
	public static class AutoTradeItemsService
	{
		public static List<ExtendedItemCategory> FilterExtendedItemCategoriesByConfigurations()
		{
			List<ExtendedItemCategory> itemCategories = new List<ExtendedItemCategory>();
			if (GetAllowBodyArmour())
			{
				itemCategories.Add(ExtendedItemCategory.BodyArmourItemCategory);
			}
			if (GetAllowWeapons())
			{
				itemCategories.Add(ExtendedItemCategory.WeaponItemCategory);
			}
			if (GetAllowSaddles())
			{
				itemCategories.Add(ExtendedItemCategory.SaddleItemCategory);
			}
			if (GetAllowHorses())
			{
				itemCategories.Add(ExtendedItemCategory.HorseItemCategory);
			}
			if (GetAllowCamels())
			{
				itemCategories.Add(ExtendedItemCategory.CamelItemCategory);
			}
			if (GetAllowMiscellaneous())
			{
				itemCategories.Add(ExtendedItemCategory.MiscellaneousItemCategory);
			}
			if (GetAllowBanners())
			{
				itemCategories.Add(ExtendedItemCategory.BannerItemCategory);
			}
			return itemCategories;
		}
		public static List<ItemRosterElement> FilterItemRosterElementsByConfigurationsAndItemCategories(List<ExtendedItemCategory> itemCategories)
		{
			List<ItemRosterElement> itemRosterElements = HeroEquipmentCustomization.getItemsByCategories(MobileParty.MainParty.ItemRoster.ToList(), itemCategories);

			if (GetAllowAnyCulture() == false)
			{
				itemRosterElements = HeroEquipmentCustomization.getItemsByCulture(itemRosterElements, GetChosenCulture());
			}

			itemRosterElements = ExtendedItemCategory.OrderItemRosterByWeight(itemRosterElements);
			if (GetAllowLockedItems() == false)
			{
				itemRosterElements = EquipmentUtil.RemoveLockedItems(itemRosterElements);
			}

			int orderByWeight = Convert.ToInt32(GetIsLightestItemsFirst());
			itemRosterElements = ExtendedItemCategory.OrderItemRosterByWeight(itemRosterElements, (ExtendedItemCategory.OrderByWeight)orderByWeight);
			return itemRosterElements;
		}

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
			SetAllowBodyArmour(!GetAllowBodyArmour());
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

		public static void SetAllowBodyArmour(bool flag)
		{
			EnhancedQuaterMasterData.AutoTradeItems.AllowBodyArmour = flag;
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
		public static bool GetAllowBodyArmour()
		{
			return EnhancedQuaterMasterData.AutoTradeItems.AllowBodyArmour;
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
}
