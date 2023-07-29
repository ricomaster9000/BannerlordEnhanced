using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BannerlordEnhancedFramework.utils;
using BannerlordEnhancedPartyRoles.src.Storage;
using BannerlordEnhancedPartyRoles.Storage;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;

namespace BannerlordEnhancedPartyRoles.src.Services;

internal class EnhancedQuaterMasterService
{
	public static void ToggleQuaterMasterAllowLockedItems()
	{
		InformationManager.DisplayMessage(new InformationMessage(!GetAllowLockedItems() == true ? "true" : "false", Colors.Yellow));
		SetAllowLockedItems(!GetAllowLockedItems()); 
	}

	public static bool IsPlayerTalkingToPlayerClanQauterMaster()
	{
		return Campaign.Current != null &&
				GameUtils.PlayerParty() != null &&
				GameUtils.PlayerParty().EffectiveQuartermaster != null &&
				Campaign.Current.ConversationManager.OneToOneConversationCharacter == GameUtils.PlayerParty().EffectiveQuartermaster.CharacterObject;
	}

	public static void SetAllowLockedItems(bool scoutAlertsNearbyEnemiesFrozen)
	{
		EnhancedScoutData.ScoutAlertsNearbyEnemiesFrozen = scoutAlertsNearbyEnemiesFrozen;
	}

	public static bool GetAllowLockedItems()
	{
		return EnhancedQuaterMasterData.AllowLockedItems;
	}

}

