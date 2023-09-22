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
using TaleWorlds.Core;

namespace BannerlordEnhancedPartyRoles.patches;

public class QuartermasterPatches
{
    /*public static void DoneLogic_Postfix(InventoryLogic __instance, bool __result)
    {
        DebugUtils.LogAndPrintInfo("DoneLogic_Postfix is working");
	}*/

	public static void CloseInventoryPresentation_Prefix(bool fromCancel)
	{
		CompanionEquipmentService.SetIsLastInventoryCancelPressed(fromCancel);
	}
	public static void CloseInventoryPresentation_Postfix(bool fromCancel)
	{
		int currentVersionNo = MobileParty.MainParty.ItemRoster.VersionNo;
		if (CompanionEquipmentService.GetIsLastInventoryCancelPressed() == false && currentVersionNo != CompanionEquipmentService.GetLastItemRosterVersionNo())
		{
			EnhancedQuaterMasterBehavior.GiveBestEquipmentFromItemRoster();
		}
		CompanionEquipmentService.SetLastItemRosterVersionNo(currentVersionNo);
	}
	
	public static void ClanPresentationDone_Postfix()
	{
		int latestVersionNo = CompanionEquipmentService.GetLatestFilterSettingsVersionNo();
		if (latestVersionNo != CompanionEquipmentService.GetPreviousFilterSettingsVersionNo())
		{
			EnhancedQuaterMasterBehavior.GiveBestEquipmentFromItemRoster();
		}
		CompanionEquipmentService.SetPreviousFilterSettingsVersionNo(latestVersionNo);
	}
}