using System;
using System.Collections.Generic;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Inventory;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Library;
using TaleWorlds.Core;
using BannerlordEnhancedFramework.utils;

namespace BannerlordEnhancedPartyRoles.Behaviors
{
    class QuaterMasterDialog : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(AddDialogs));
        }

        public override void SyncData(IDataStore dataStore)
        {
        }

        // How dialogs work my understanding so far. is the outputToken arguments you can go from there when adding it as inputToken in next dialogs
        // you can also add to show it in this example player must have quatermaster role to see that dialog
        private void AddDialogs(CampaignGameStarter starter)
        {
            starter.AddPlayerLine("QauerMaster_Ontmoeting", "hero_main_options", "quatermaster_continue_conversation", "Give Companions best items from my inventory", isQuaterMaster, null);
            starter.AddDialogLine("QauerMaster_Ontmoeting", "quatermaster_continue_conversation", "end", "Alright!", null, giveBestItems);
        }

        public bool isQuaterMaster()
        {
            if (Campaign.Current.ConversationManager.OneToOneConversationCharacter == Campaign.Current.MainParty.EffectiveQuartermaster.CharacterObject)
            {
                return true;
            }
            return false;
        }

        public void giveBestItems()
        {
            //Campaign.Current.InventoryManager
            MobileParty mainParty = MobileParty.MainParty;
            Hero leaderHero = mainParty.LeaderHero;

            MBList<TroopRosterElement> troopsRoster = mainParty.Party.MemberRoster.GetTroopRoster();

            foreach (TroopRosterElement troop in troopsRoster)
            {
                if (troop.Character.IsHero && troop.Character.HeroObject != leaderHero)
                {

                    InventoryLogic inventoryLogic = InventoryManager.InventoryLogic;
                    /*
                    IEnumerable<ItemRosterElement> allItemRosterElement = inventoryLogic.GetElementsInRoster(InventoryLogic.InventorySide.PlayerInventory);
 
                    foreach (var itemRosterElement in allItemRosterElement)
                    {
                        DebugUtils.LogAndPrintInfo(itemRosterElement.ToString());
                    }
                    */
                    Hero companion = troop.Character.HeroObject;
                    Equipment battleEquipment = companion.BattleEquipment;

                    DebugUtils.LogAndPrintInfo("Found Hero " + troop.Character.ToString());
                    DebugUtils.LogAndPrintInfo("ArmArmorSum = " + battleEquipment.GetArmArmorSum());
                    DebugUtils.LogAndPrintInfo("HeadArmorSum = " + battleEquipment.GetHeadArmorSum());
                    DebugUtils.LogAndPrintInfo("HorseArmorSum = " + battleEquipment.GetHorseArmorSum());
                    DebugUtils.LogAndPrintInfo("HumanBodyArmorSum = " + battleEquipment.GetHumanBodyArmorSum());
                    DebugUtils.LogAndPrintInfo("LegArmorSum = " + battleEquipment.GetLegArmorSum());
                    DebugUtils.LogAndPrintInfo("TotalWeightOfArmor = " + battleEquipment.GetTotalWeightOfArmor(true));

                    battleEquipment[10] = leaderHero.BattleEquipment[10];
                    break;
                }
            }
            InformationManager.DisplayMessage(new InformationMessage("Quatermaster is giving best items to companions.", Color.White));
        }

    }
}







// Will delete later

/*
 *         starter.AddDialogLine("QauerMaster_Ontmoeting", "start", "continue_conversation", "Yes me lord?", isQuaterMaster, null);
            starter.AddPlayerLine(
                "QauerMaster_Vraag",
                "continue_conversation",
                "respond_give_best_items",
                "Give Companions best items from my inventory",
                null,
                null
             );
            starter.AddDialogLine("QauerMaster_Ontmoeting", "respond_give_best_items", "end", "Alright!", null, giveBestItems);
 */

/*
public bool showInformationMessage(out TextObject explanation)
{
    InformationManager.DisplayMessage(new InformationMessage("Giving best items to companions.", Color.White));
    explanation = TextObject.Empty;
    return true;
}
*/


/*
public ConversationSentence AddPlayerLine(
    string id,
    string inputToken,
    string outputToken,
    string text,
    ConversationSentence.OnConditionDelegate conditionDelegate,  // Condition that will know which Character to show Dialog
    ConversationSentence.OnConsequenceDelegate consequenceDelegate, // Function to call after click dialog
    int priority = 100,
    ConversationSentence.OnClickableConditionDelegate clickableConditionDelegate = null,
    ConversationSentence.OnPersuasionOptionDelegate persuasionOptionDelegate = null
    )
{
    return this.AddDialogLine(new ConversationSentence(id,
        new TextObject(text, null),
        inputToken, outputToken,
        conditionDelegate,
        clickableConditionDelegate,
        consequenceDelegate,
        1U,
        priority,
        0,
        0,
        null,
        false,
        null,
        null,
        persuasionOptionDelegate
        )
     );
}
*/