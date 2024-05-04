using System;
using System.Collections.Generic;
using System.Linq;
using BannerlordEnhancedFramework.extendedtypes;
using BannerlordEnhancedFramework.src.utils;
using BannerlordEnhancedFramework.utils;
using BannerlordEnhancedPartyRoles.Behaviors;
using BannerlordEnhancedPartyRoles.src.Behaviors;
using BannerlordEnhancedPartyRoles.src.Services;
using TaleWorlds.CampaignSystem.Inventory;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement;
using TaleWorlds.Core;

namespace BannerlordEnhancedPartyRoles.patches;

public class QuartermasterPatches
{
	/*static void DoneLogic_Postfix(InventoryLogic __instance, bool __result)
    {
	    if (!__instance.IsTrading && !__instance.IsPreviewingItem && !__instance.IsDiscardDonating)
	    {
		    DebugUtils.LogAndPrintInfo("DoneLogic_Postfix is working");
		    EnhancedQuaterMasterService.GiveBestEquipmentFromItemRoster();
	    }
    }*/

	public static void CloseInventoryPresentation_Prefix(InventoryManager __instance, bool fromCancel)
	{
		AutoEquipService.SetIsLastInventoryCancelPressed(fromCancel);
	}
	public static void CloseInventoryPresentation_Postfix(InventoryManager __instance, bool fromCancel)
	{
		int currentVersionNo = MobileParty.MainParty.ItemRoster.VersionNo;
		if (AutoEquipService.GetIsLastInventoryCancelPressed() == false &&
		    currentVersionNo != AutoEquipService.GetLastItemRosterVersionNo() &&
		    __instance.CurrentMode != InventoryMode.Trade)
		{
			AutoEquipService.GiveBestEquipmentFromItemRoster();
		}
		AutoEquipService.SetLastItemRosterVersionNo(currentVersionNo);
	}
	
	public static void ClanPresentationDone_Postfix(ClanManagementVM __instance)
	{
		int latestVersionNo = AutoEquipService.GetLatestFilterSettingsVersionNo();
		if (latestVersionNo != AutoEquipService.GetPreviousFilterSettingsVersionNo())
		{
			AutoEquipService.GiveBestEquipmentFromItemRoster();
		}
		AutoEquipService.SetPreviousFilterSettingsVersionNo(latestVersionNo);
	}
}