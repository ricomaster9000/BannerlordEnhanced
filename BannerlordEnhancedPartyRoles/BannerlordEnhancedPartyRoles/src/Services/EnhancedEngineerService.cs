using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BannerlordEnhancedFramework.utils;
using TaleWorlds.CampaignSystem;

namespace BannerlordEnhancedPartyRoles.src.Services
{
	internal class EnhancedEngineerService
	{
		public static bool IsPlayerTalkingToPlayerClanEngineer()
		{
			return Campaign.Current != null &&
			GameUtils.PlayerParty() != null &&
			GameUtils.PlayerParty().EffectiveQuartermaster != null &&
			Campaign.Current.ConversationManager.OneToOneConversationCharacter == GameUtils.PlayerParty().EffectiveEngineer.CharacterObject;
		}
	}
}
