using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using BannerlordEnhancedFramework.utils;
using BannerlordEnhancedPartyRoles.src.Storage;
using BannerlordEnhancedPartyRoles.Storage;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace BannerlordEnhancedPartyRoles.src.Services;

internal class EnhancedQuaterMasterService
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

	public static bool IsPlayerTalkingToPlayerClanQauterMaster()
	{
		return Campaign.Current != null &&
				GameUtils.PlayerParty() != null &&
				GameUtils.PlayerParty().EffectiveQuartermaster != null &&
				Campaign.Current.ConversationManager.OneToOneConversationCharacter == GameUtils.PlayerParty().EffectiveQuartermaster.CharacterObject;
	}

	public static void SetAllowAnyCulture(bool flag)
	{
		EnhancedQuaterMasterData.LatestFilterSettingsVersionNo += 1;
		EnhancedQuaterMasterData.AllowAnyCulture = flag;
	}
	public static void SetAllowLockedItems(bool flag)
	{
		EnhancedQuaterMasterData.LatestFilterSettingsVersionNo += 1;
		EnhancedQuaterMasterData.AllowLockedEquipment = flag;
	}
	public static void SetAllowBattaniaCulture(bool flag)
	{
		EnhancedQuaterMasterData.LatestFilterSettingsVersionNo += 1;
		EnhancedQuaterMasterData.AllowBattaniaCulture = flag;
	}
	public static void SetAllowSturgiaCulture(bool flag)
	{
		EnhancedQuaterMasterData.LatestFilterSettingsVersionNo += 1;
		EnhancedQuaterMasterData.AllowSturgiaCulture = flag;
	}
	public static void SetAllowAseraiCulture(bool flag)
	{
		EnhancedQuaterMasterData.LatestFilterSettingsVersionNo += 1;
		EnhancedQuaterMasterData.AllowAseraiCulture = flag;
	}
	public static void SetAllowKhuzaitCulture(bool flag)
	{
		EnhancedQuaterMasterData.LatestFilterSettingsVersionNo += 1;
		EnhancedQuaterMasterData.AllowKhuzaitCulture = flag;
	}
	public static void SetAllowVlandiaCulture(bool flag)
	{
		EnhancedQuaterMasterData.LatestFilterSettingsVersionNo += 1;
		EnhancedQuaterMasterData.AllowVlandiaCulture = flag;
	}
	public static void SetAllowEmpireCulture(bool flag)
	{
		EnhancedQuaterMasterData.LatestFilterSettingsVersionNo += 1;
		EnhancedQuaterMasterData.AllowEmpireCulture = flag;
	}

	public static void SetAllowBattleEquipment(bool flag)
	{
		EnhancedQuaterMasterData.AllowBattleEquipment = flag;
	}
	public static void SetAllowCivilianEquipment(bool flag)
	{
		EnhancedQuaterMasterData.AllowCivilianEquipment = flag;
	}

	public static void SetLastItemRosterVersionNo(int version)
	{
		EnhancedQuaterMasterData.LastItemRosterVersionNo = version;
	}
	public static void SetIsLastInventoryCancelPressed(bool fromCancel)
	{ 
		EnhancedQuaterMasterData.IsLastInventoryCancelPressed = fromCancel;
	}

	public static void SetPreviousFilterSettingsVersionNo(int versionNo)
	{
		EnhancedQuaterMasterData.PreviousFilterSettingsVersionNo = versionNo;
	}


	public static bool GetAllowAnyCulture()
	{
		return EnhancedQuaterMasterData.AllowAnyCulture ;
	}
	public static bool GetAllowLockedItems()
	{
		return EnhancedQuaterMasterData.AllowLockedEquipment;
	}
	public static bool GetAllowBattaniaCulture()
	{
		return EnhancedQuaterMasterData.AllowBattaniaCulture;
	}
	public static bool GetAllowSturgiaCulture()
	{
		return EnhancedQuaterMasterData.AllowSturgiaCulture;
	}
	public static bool GetAllowAseraiCulture()
	{
		return EnhancedQuaterMasterData.AllowAseraiCulture;
	}
	public static bool GetAllowKhuzaitCulture()
	{
		return EnhancedQuaterMasterData.AllowKhuzaitCulture;
	}
	public static bool GetAllowVlandiaCulture()
	{
		return EnhancedQuaterMasterData.AllowVlandiaCulture;
	}
	public static bool GetAllowEmpireCulture()
	{
		return EnhancedQuaterMasterData.AllowEmpireCulture;
	}

	public static bool GetAllowBattleEquipment()
	{
		return EnhancedQuaterMasterData.AllowBattleEquipment;
	}
	public static bool GetAllowCivilianEquipment()
	{
		return EnhancedQuaterMasterData.AllowCivilianEquipment;
	}
	public static int GetLastItemRosterVersionNo()
	{
		return EnhancedQuaterMasterData.LastItemRosterVersionNo;
	}

	public static bool GetIsLastInventoryCancelPressed()
	{
		return EnhancedQuaterMasterData.IsLastInventoryCancelPressed;
	}

	public static int GetLatestFilterSettingsVersionNo()
	{
		return EnhancedQuaterMasterData.LatestFilterSettingsVersionNo;
	}
	public static int GetPreviousFilterSettingsVersionNo()
	{
		return EnhancedQuaterMasterData.PreviousFilterSettingsVersionNo;
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
		else if(GetAllowSturgiaCulture())
		{
			return CultureCode.Sturgia;
		}
		else if(GetAllowAseraiCulture())
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

