using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
