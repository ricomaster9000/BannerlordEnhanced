using System;
using System.Collections.Generic;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Inventory;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Library;
using TaleWorlds.Core;
using BannerlordEnhancedFramework.utils;
using BannerlordEnhancedPartyRoles.Services;
using System.Linq;
using System.Runtime.CompilerServices;
using BannerlordEnhancedPartyRoles.src.Services;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;
using BannerlordEnhancedFramework.models;

namespace BannerlordEnhancedPartyRoles.Behaviors
{

    class QuaterMasterDialog : CampaignBehaviorBase
    {
		[CommandLineFunctionality.CommandLineArgumentFunction("quatermaster_give_best_items_to_companions", "debug")]
		public static string DebugGiveBestItemsToCompanions(List<string> strings)
		{
			GiveBestEquipmentFromItemRoster();
			return "Done";
		}

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
            starter.AddPlayerLine("QauerMaster_Ontmoeting", "hero_main_options", "quatermaster_continue_conversation", "Give Companions best items from my inventory", EnhancedQuaterMasterServicePreviousVersion.IsQuaterMaster, null);
            starter.AddDialogLine("QauerMaster_Ontmoeting", "quatermaster_continue_conversation", "end", "Alright!", null, GiveBestEquipmentFromItemRoster);
        }

		public static string BuildQuaterMasterNotification(List<string> list)
		{
			string text = "";
			int i = 0;
			int size = list.Count;
			foreach (var word in list)
			{
				i += 1;
				if (i == size)
				{
					string addsPlural = word[word.Length - 1] != 'r' ? word + "s" : word;
					text += addsPlural;
				}
				else if (i == size - 1)
				{ 
					text += word + " and ";
				}
				else
				{
					text += word + ", ";
				}
			}
			return text;
		}

		public static void GiveBestEquipmentFromItemRoster()
		{
			MobileParty mainParty = MobileParty.MainParty;

			ItemRoster itemRoster = mainParty.ItemRoster;

			List<TroopRosterElement> allCompanionsTroopRosterElement = EnhancedQuaterMasterServicePreviousVersion.GetCompanionsTroopRosterElement(mainParty.Party.MemberRoster.GetTroopRoster(), mainParty.LeaderHero);
			bool hasGivenWeapon = WeaponsManager.UpdateCompanionWeapons(itemRoster, allCompanionsTroopRosterElement);
			bool hasGivenBanner = WeaponsManager.UpdateCompanionBanners(itemRoster, allCompanionsTroopRosterElement);

			List<Hero> allCompanionsHeros = EnhancedQuaterMasterServicePreviousVersion.GetCompanionsHeros(mainParty.Party.MemberRoster.GetTroopRoster(), mainParty.LeaderHero);

			var tuple = EnhancedQuaterMasterServicePreviousVersion.UpdateCompanionsArmour(itemRoster.ToList(), allCompanionsHeros);

			List<ItemRosterElement> removeItemRosterElement = tuple.Item1;
			List<ItemRosterElement> swappedItemRosterElement = tuple.Item2;

			bool hasGivenArmour = WeaponsManager.CountItems(removeItemRosterElement) > 0;

			foreach (ItemRosterElement itemRosterElement in swappedItemRosterElement)
			{
				itemRoster.Add(itemRosterElement);
			}
			foreach (ItemRosterElement itemRosterElement in removeItemRosterElement)
			{
				itemRoster.Remove(itemRosterElement);
			}

			if(hasGivenArmour || hasGivenWeapon || hasGivenBanner)
			{
				List<string> words = new List<string>();
				if (hasGivenArmour)
				{
					words.Add("Armour");
				}
				if (hasGivenWeapon) 
				{
					words.Add("Weapon");
				}
				if (hasGivenBanner)
				{
					words.Add("Banner");
				}
				InformationManager.DisplayMessage(new InformationMessage("Quatermaster updated companions " + BuildQuaterMasterNotification(words), Colors.Yellow));
			}
		}

	}
}

// TODO
/*
 * Find item type if apparel or weapon or mount & saddle
 * 
 * 
 */


//Campaign.Current.InventoryManager




// TODO Will delete later

/*
 * 
 * 
 * 
    DebugUtils.LogAndPrintInfo("Found Hero " + troop.Character.ToString());
    DebugUtils.LogAndPrintInfo("ArmArmorSum = " + battleEquipment.GetArmArmorSum());
    DebugUtils.LogAndPrintInfo("HeadArmorSum = " + battleEquipment.GetHeadArmorSum());
    DebugUtils.LogAndPrintInfo("HorseArmorSum = " + battleEquipment.GetHorseArmorSum());
    DebugUtils.LogAndPrintInfo("HumanBodyArmorSum = " + battleEquipment.GetHumanBodyArmorSum());
    DebugUtils.LogAndPrintInfo("LegArmorSum = " + battleEquipment.GetLegArmorSum());
    DebugUtils.LogAndPrintInfo("TotalWeightOfArmor = " + battleEquipment.GetTotalWeightOfArmor(true));

	IEnumerator<ItemRosterElement> enumerator = itemRoster.GetEnumerator();
	while (enumerator.MoveNext())
	{
		//DebugUtils.LogAndPrintInfo(enumerator.Current.ToString());
	}

	foreach(ItemRosterElement itemRosterElement in itemRoster)
	{
		//DebugUtils.LogAndPrintInfo("Item Name: "+ itemRosterElement.EquipmentElement.Item.Name + " - Effectiveness: "+ itemRosterElement.EquipmentElement.Item.Effectiveness + " Modified Body Armor: "+ itemRosterElement.EquipmentElement.GetModifiedBodyArmor());
		// DebugUtils.LogAndPrintInfo(itemRosterElement.ToString());
	}

	// Look for _data it contains array of ItemRosterElement
	InventoryLogic inventoryLogic = InventoryManager.InventoryLogic;
   
	IEnumerable<ItemRosterElement> allItemRosterElement = inventoryLogic.GetElementsInRoster(InventoryLogic.InventorySide.PlayerInventory);

	foreach (var itemRosterElement in allItemRosterElement)
	{
		DebugUtils.LogAndPrintInfo(itemRosterElement.ToString());
	}
	*/


/*
foreach (ItemRosterElement itemRosterElement in sortedItemRosterElement)
{
	DebugUtils.LogAndPrintInfo("Item Name: " + itemRosterElement.EquipmentElement.Item.Name + " - Effectiveness: " + itemRosterElement.EquipmentElement.Item.Effectiveness + " Modified Body Armor: " + itemRosterElement.EquipmentElement.GetModifiedBodyArmor());
}

Equipment battleEquipment = companion.BattleEquipment;
battleEquipment[10] = leaderHero.BattleEquipment[10];
*/