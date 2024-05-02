using System;
using BannerlordEnhancedFramework;
using BannerlordEnhancedFramework.dialogues;
using BannerlordEnhancedFramework.extendedtypes;
using TaleWorlds.CampaignSystem;
using BannerlordEnhancedPartyRoles.Services;
namespace BannerlordEnhancedPartyRoles.Behaviors
{
    class EnhancedScoutBehavior : CampaignBehaviorBase
    {
        private ExtendedTimer _enemyAlertCloseByTimer;

        public override void RegisterEvents()
        {
            // add Dialogs
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(AddDialogs));

            // add enemy close by alert timer
            _enemyAlertCloseByTimer = new ExtendedTimer(250, () =>
                {
                    if (Campaign.Current == null)
                    {
                        _enemyAlertCloseByTimer.StopTimer();
                    }
					// DebugUtils.LogAndPrintInfo("_enemyAlertCloseByTimer running");
					if (EnhancedScoutService.GetScoutAlertsNearbyEnemies())
                    {
                        EnhancedScoutService.AlertPlayerToNearbyHostileParties();
                    }
                });
            _enemyAlertCloseByTimer.StartTimer();
        }

        public override void SyncData(IDataStore dataStore)
        {   
        }
		
        private void AddDialogs(CampaignGameStarter starter)
        {
            new DialogueTreeBuilder()
                .WithConversationPart(
                    new SimpleConversationPart(
                        "enhanced_scout_conv_start",
                        "Enhanced Scout Menu",
                        ConversationSentenceType.DialogueTreeRootStart,
                        CoreInputToken.Entry.HeroMainOptions
                    ).WithCondition(EnhancedScoutService.IsPlayerTalkingToPlayerClanScout))
                    .WithConversationPart(
                        new SimpleConversationPart(
                            "enhanced_scout_conv_menu_configure",
                            "Configurations",
                            ConversationSentenceType.DialogueTreeBranchStart
                        ))
                        .WithConversationPart(
                            new SimpleConversationPart(
                                "enhanced_scout_conv_menu_configure_enemy_alerts",
                                "Enemy Alert Settings",
                                ConversationSentenceType.DialogueTreeBranchStart
                            ))
                            .WithTrueFalseConversationToggle(
                                new SimpleConversationPart(
                                        "enhanced_scout_conv_menu_configure_enemy_alerts_toggle_pause_game",
                                        "Pause Game On Enemy Alert",
                                        ConversationSentenceType.DialogueTreeBranchPart
                                    ).WithCondition(() => EnhancedScoutService.GetScoutAlertsNearbyEnemies() == true)
                                    .WithConsequence(EnhancedScoutService.ToggleScoutAlertsNearbyEnemies),
                                AppliedDialogueLineRelation.LinkToCurrentBranch)
		.Build(starter);
        }
    }
}
