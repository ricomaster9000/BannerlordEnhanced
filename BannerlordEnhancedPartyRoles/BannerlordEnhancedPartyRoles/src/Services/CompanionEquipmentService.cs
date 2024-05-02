using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BannerlordEnhancedFramework.extendedtypes;
using BannerlordEnhancedPartyRoles.src.Storage;
using TaleWorlds.Core;

namespace BannerlordEnhancedPartyRoles.src.Services
{

	public static class CompanionEquipmentService
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
			EnhancedQuarterMasterData.CompanionEquiptment.LatestFilterSettingsVersionNo += 1;
		}
		public static void SetAllowLockedItems(bool flag)
		{
			EnhancedQuarterMasterData.CompanionEquiptment.LatestFilterSettingsVersionNo += 1;
			EnhancedQuarterMasterData.CompanionEquiptment.AllowLockedEquipment = flag;
		}
		public static void SetAllowBattaniaCulture(bool flag)
		{
			EnhancedQuarterMasterData.CompanionEquiptment.LatestFilterSettingsVersionNo += 1;
		}
		public static void SetAllowSturgiaCulture(bool flag)
		{
			EnhancedQuarterMasterData.CompanionEquiptment.LatestFilterSettingsVersionNo += 1;
		}
		public static void SetAllowAseraiCulture(bool flag)
		{
			EnhancedQuarterMasterData.CompanionEquiptment.LatestFilterSettingsVersionNo += 1;
		}
		public static void SetAllowKhuzaitCulture(bool flag)
		{
			EnhancedQuarterMasterData.CompanionEquiptment.LatestFilterSettingsVersionNo += 1;
		}
		public static void SetAllowVlandiaCulture(bool flag)
		{
			EnhancedQuarterMasterData.CompanionEquiptment.LatestFilterSettingsVersionNo += 1;
		}
		public static void SetAllowEmpireCulture(bool flag)
		{
			EnhancedQuarterMasterData.CompanionEquiptment.LatestFilterSettingsVersionNo += 1;
		}

		public static void SetAllowBattleEquipment(bool flag)
		{
			EnhancedQuarterMasterData.CompanionEquiptment.AllowBattleEquipment = flag;
		}
		public static void SetAllowCivilianEquipment(bool flag)
		{
			EnhancedQuarterMasterData.CompanionEquiptment.AllowCivilianEquipment = flag;
		}

		public static void SetLastItemRosterVersionNo(int version)
		{
			EnhancedQuarterMasterData.CompanionEquiptment.LastItemRosterVersionNo = version;
		}
		public static void SetIsLastInventoryCancelPressed(bool fromCancel)
		{
			EnhancedQuarterMasterData.CompanionEquiptment.IsLastInventoryCancelPressed = fromCancel;
		}

		public static void SetPreviousFilterSettingsVersionNo(int versionNo)
		{
			EnhancedQuarterMasterData.CompanionEquiptment.PreviousFilterSettingsVersionNo = versionNo;
		}


		public static bool GetAllowAnyCulture()
		{
			return EnhancedQuarterMasterData.AutoTraderData.AllowAnyCulture;
		}
		public static bool GetAllowLockedItems()
		{
			return EnhancedQuarterMasterData.CompanionEquiptment.AllowLockedEquipment;
		}
		public static bool GetAllowBattaniaCulture()
		{
			return !EnhancedQuarterMasterData.AutoEquip.CultureToItemCategoryFilters[ExtendedCultureCode.byName["Battania"]]["LockedAll"];
		}
		public static bool GetAllowSturgiaCulture()
		{
			return !EnhancedQuarterMasterData.AutoEquip.CultureToItemCategoryFilters[ExtendedCultureCode.byName["Sturgia"]]["LockedAll"];
		}
		public static bool GetAllowAseraiCulture()
		{
			return !EnhancedQuarterMasterData.AutoEquip.CultureToItemCategoryFilters[ExtendedCultureCode.byName["Aserai"]]["LockedAll"];
		}
		public static bool GetAllowKhuzaitCulture()
		{
			return !EnhancedQuarterMasterData.AutoEquip.CultureToItemCategoryFilters[ExtendedCultureCode.byName["Khuzait"]]["LockedAll"];
		}
		public static bool GetAllowVlandiaCulture()
		{
			return !EnhancedQuarterMasterData.AutoEquip.CultureToItemCategoryFilters[ExtendedCultureCode.byName["Vlandia"]]["LockedAll"];
		}
		public static bool GetAllowEmpireCulture()
		{
			return !EnhancedQuarterMasterData.AutoEquip.CultureToItemCategoryFilters[ExtendedCultureCode.byName["Empire"]]["LockedAll"];
		}

		public static bool GetAllowBattleEquipment()
		{
			return EnhancedQuarterMasterData.CompanionEquiptment.AllowBattleEquipment;
		}
		public static bool GetAllowCivilianEquipment()
		{
			return EnhancedQuarterMasterData.CompanionEquiptment.AllowCivilianEquipment;
		}
		public static int GetLastItemRosterVersionNo()
		{
			return EnhancedQuarterMasterData.CompanionEquiptment.LastItemRosterVersionNo;
		}

		public static bool GetIsLastInventoryCancelPressed()
		{
			return EnhancedQuarterMasterData.CompanionEquiptment.IsLastInventoryCancelPressed;
		}

		public static int GetLatestFilterSettingsVersionNo()
		{
			return EnhancedQuarterMasterData.CompanionEquiptment.LatestFilterSettingsVersionNo;
		}
		public static int GetPreviousFilterSettingsVersionNo()
		{
			return EnhancedQuarterMasterData.CompanionEquiptment.PreviousFilterSettingsVersionNo;
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
