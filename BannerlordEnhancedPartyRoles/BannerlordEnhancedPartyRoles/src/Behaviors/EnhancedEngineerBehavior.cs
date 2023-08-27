using System;
using TaleWorlds.CampaignSystem;
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
					).WithCondition(() => true).WithConsequence(EnhancedEngineerService.MassExecution), AppliedDialogueLineRelation.LinkToPreviousStart)
				.Build(starter);
		}
	}
}
