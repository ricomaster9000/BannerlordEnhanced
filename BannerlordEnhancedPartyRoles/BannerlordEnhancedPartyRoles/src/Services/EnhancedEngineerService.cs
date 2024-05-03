using System.Collections.Generic;
using BannerlordEnhancedFramework.utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Library;

namespace BannerlordEnhancedPartyRoles.src.Services
{
	internal class EnhancedEngineerService
	{
		public static void MassExecution()
		{
			MobileParty mainParty = MobileParty.MainParty;

			List<TroopRosterElement> lords = PartyUtils.GetHeros(mainParty.PrisonRoster.GetTroopRoster());

			InformationManager.DisplayMessage(new InformationMessage(lords.Count + " lords will be executed shortly", BannerlordEnhancedFramework.Colors.Yellow));

			foreach (TroopRosterElement prisonerHero in lords)
			{
				Hero hero = prisonerHero.Character.HeroObject;
				if (hero.CanDie(KillCharacterAction.KillCharacterActionDetail.Executed))
				{
					hero.AddDeathMark(mainParty.LeaderHero, KillCharacterAction.KillCharacterActionDetail.Executed);
				}
			}
		}
	}
}
