using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BannerlordEnhancedPartyRoles.Services;
using BannerlordEnhancedPartyRoles.src.Services;
using BannerlordEnhancedFramework.extendedtypes;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using BannerlordEnhancedFramework.src.utils;

namespace BannerlordEnhancedPartyRoles.Behaviors;
	class EnhancedQuaterMasterBehaviorNewVersion : CampaignBehaviorBase
{
	[CommandLineFunctionality.CommandLineArgumentFunction("quatermaster_give_best_items_to_companions_new_version", "debug")]
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
		starter.AddPlayerLine("QauerMaster_Ontmoeting", "hero_main_options", "quatermaster_continue_conversation", "Give Companions best items from my inventory", EnhancedQuaterMasterService.IsQuaterMaster, null);
		starter.AddDialogLine("QauerMaster_Ontmoeting", "quatermaster_continue_conversation", "end", "Alright!", null, GiveBestEquipmentFromItemRoster);
	}

	public static void updateItemRoster(ItemRoster itemRoster, List<ItemRosterElement> additions, List<ItemRosterElement> removals)
	{
		foreach (ItemRosterElement itemRosterElement in additions)
		{
			itemRoster.Add(itemRosterElement);
		}
		foreach (ItemRosterElement itemRosterElement in removals)
		{
			itemRoster.Remove(itemRosterElement);
		}

	}
	public static void GiveBestEquipmentFromItemRoster()
	{
		MobileParty mainParty = MobileParty.MainParty;

		ItemRoster itemRoster = mainParty.ItemRoster;

		List<TroopRosterElement> allCompanionsTroopRosterElement = EnhancedQuaterMasterService.GetCompanionsTroopRosterElement(mainParty.Party.MemberRoster.GetTroopRoster(), mainParty.LeaderHero);
		List<FighterClass> fighters = new List<FighterClass> ();

		bool canRemoveLockedItems = false;

		foreach(TroopRosterElement troopCompanion in allCompanionsTroopRosterElement)
		{
			FighterClass fighterClass = new FighterClass(troopCompanion.Character.HeroObject);
			fighters.Add(fighterClass);
		}

		foreach(FighterClass fighterClass in fighters)
		{
			List<ItemRosterElement> items = canRemoveLockedItems ? EquipmentUtil.RemoveLockedItems(itemRoster.ToList()) : itemRoster.ToList();
			var changes = fighterClass.assignBattleEquipment(items);
			updateItemRoster(itemRoster, changes.additions, changes.removals);

			items = canRemoveLockedItems ? EquipmentUtil.RemoveLockedItems(itemRoster.ToList()) : itemRoster.ToList();
			changes = fighterClass.assignCivilianEquipment(items);
			updateItemRoster(itemRoster, changes.additions, changes.removals);
		}
	}

}

