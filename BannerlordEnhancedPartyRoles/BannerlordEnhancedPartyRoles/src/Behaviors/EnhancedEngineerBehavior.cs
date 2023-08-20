using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;
using BannerlordEnhancedPartyRoles.src.Services;
using BannerlordEnhancedFramework.dialogues;
using BannerlordEnhancedFramework;

namespace BannerlordEnhancedPartyRoles.src.Behaviors
{
	internal class EnhancedEngineerBehavior : CampaignBehaviorBase
	{

		public override void RegisterEvents()
		{
			CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(AddDialogs));
		}

		public override void SyncData(IDataStore dataStore)
		{
		}

		private void AddDialogs(CampaignGameStarter starter)
		{
			new DialogueBuilder()
				.WithConversationPart(
					new SimpleConversationPart(
						"enhanced_engineer_conv_start",
						"Enhanced Engineer Menu",
						ConversationSentenceType.DialogueTreeRootStart,
						CoreInputToken.Entry.HeroMainOptions
					).WithCondition(EnhancedEngineerService.IsPlayerTalkingToPlayerClanEngineer))
				.WithConversationPart(
					new SimpleConversationPart(
						"enhanced_quatermaster_mass_execute_prisoners",
						"Send Lord Prisoners For Mass Exececution",
						ConversationSentenceType.DialogueTreeBranchPart,
						CoreInputToken.Entry.HeroMainOptions
					).WithCondition(() => true).WithConsequence(MassExecution), AppliedDialogueLineRelation.LinkToPreviousStart)
				.Build(starter);
		}

		// TODO Perhaps move into util
		public static List<TroopRosterElement> GetHeros(MBList<TroopRosterElement> troopsRosterElement)
		{
			List<TroopRosterElement> heros = new List<TroopRosterElement>();

			foreach (TroopRosterElement troop in troopsRosterElement)
			{
				Hero hero = troop.Character.HeroObject;

				if (troop.Character.IsHero)
				{
					heros.Add(troop);
				}
			}

			return heros;
		}

		public static void MassExecution()
		{
			MobileParty mainParty = MobileParty.MainParty;

			List<TroopRosterElement> lords = GetHeros(mainParty.PrisonRoster.GetTroopRoster());

			InformationManager.DisplayMessage(new InformationMessage(lords.Count + " lords will be executed shortly", BannerlordEnhancedFramework.Colors.Yellow));

			foreach (TroopRosterElement prisonerHero in lords)
			{
				Hero hero = prisonerHero.Character.HeroObject;
				// InformationManager.DisplayMessage(new InformationMessage("Can Be excecuted "+ hero.ToString() + " " + (hero.CanDie(KillCharacterAction.KillCharacterActionDetail.Executed), BannerlordEnhancedFramework.Colors.Yellow)));
				if (hero.CanDie(KillCharacterAction.KillCharacterActionDetail.Executed))
				{
					hero.AddDeathMark(mainParty.LeaderHero, KillCharacterAction.KillCharacterActionDetail.Executed);
				}
			}
		}
	}
}
