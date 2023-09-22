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

	public static void DisplayMessageListCategoryNameAndTotal(Dictionary<string, int> categoriesDetails, string startLineMessage)
	{
		foreach (KeyValuePair<string, int> item in categoriesDetails)
		{
			startLineMessage += "\n" + item.Key + " " + item.Value;
		}
		InformationManager.DisplayMessage(new InformationMessage(startLineMessage, BannerlordEnhancedFramework.Colors.Yellow));
	}
}

