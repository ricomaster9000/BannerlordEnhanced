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
                    //DebugUtils.LogAndPrintInfo("_enemyAlertCloseByTimer running");
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
            new DialogueBuilder()
                .WithDialogueLine(
                    new SimpleConversationPart(
                        "enhanced_scout_conv_start",
                        "Enhanced Scout Menu",
                        ConversationSentenceType.DialogueTreeRootStart,
                        CoreInputToken.Entry.HeroMainOptions
                    ).WithCondition(EnhancedScoutService.IsPlayerTalkingToPlayerClanScout))
                .WithDialogueLine(
                    new SimpleConversationPart(
                        "enhanced_scout_conv_menu_configure",
                        "Configurations",
                        ConversationSentenceType.DialogueTreeBranchStart
                    ), AppliedDialogueLineRelation.LinkToPreviousStart)
                .WithDialogueLine(
                    new SimpleConversationPart(
                        "enhanced_scout_conv_menu_configure_enemy_alerts",
                        "Enemy Alert Settings",
                        ConversationSentenceType.DialogueTreeBranchStart
                    ), AppliedDialogueLineRelation.LinkToPreviousStart)
                // Pause Game On Enemy Alert - BEGIN
                .WithDialogueLine(
                    new SimpleConversationPart(
                            "enhanced_scout_conv_menu_configure_enemy_alerts_toggle_pause_game_true",
                            "Pause Game On Enemy Alert - True",
                            ConversationSentenceType.DialogueTreeBranchPart
                        ).WithCondition(() => EnhancedScoutService.GetScoutAlertsNearbyEnemies() == true)
                        .WithConsequence(() => EnhancedScoutService.SetScoutAlertsNearbyEnemies(false)),
                    AppliedDialogueLineRelation.LinkToPreviousStart)
                .WithDialogueLine(
                    new SimpleConversationPart(
                            "enhanced_scout_conv_menu_configure_enemy_alerts_toggle_pause_game_false",
                            "Pause Game On Enemy Alert - False",
                            ConversationSentenceType.DialogueTreeBranchPart
                        ).WithCondition(() => EnhancedScoutService.GetScoutAlertsNearbyEnemies() == false)
                        .WithConsequence(() => EnhancedScoutService.SetScoutAlertsNearbyEnemies(true)),
                    AppliedDialogueLineRelation.LinkToPreviousStart)
                // Pause Game On Enemy Alert - END
                .Build(starter);
        }
    }
}