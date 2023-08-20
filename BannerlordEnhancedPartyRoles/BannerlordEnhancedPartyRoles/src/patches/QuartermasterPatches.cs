using BannerlordEnhancedFramework.utils;
using BannerlordEnhancedPartyRoles.Behaviors;
using BannerlordEnhancedPartyRoles.src.Services;
using TaleWorlds.CampaignSystem.Inventory;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;

namespace BannerlordEnhancedPartyRoles.patches;

public class QuartermasterPatches
{
    /*public static void DoneLogic_Postfix(InventoryLogic __instance, bool __result)
    {
        DebugUtils.LogAndPrintInfo("DoneLogic_Postfix is working");
		EnhancedQuaterMasterBehavior.GiveBestEquipmentFromItemRoster();
	}*/

	public static void CloseInventoryPresentation_Prefix(bool fromCancel)
	{
		EnhancedQuaterMasterService.SetIsLastInventoryCancelPressed(fromCancel);
	}
	public static void CloseInventoryPresentation_Postfix(bool fromCancel)
	{
		int currentVersionNo = MobileParty.MainParty.ItemRoster.VersionNo;
		if (EnhancedQuaterMasterService.GetIsLastInventoryCancelPressed() == false && currentVersionNo != EnhancedQuaterMasterService.GetLastItemRosterVersionNo())
		{
			EnhancedQuaterMasterBehavior.GiveBestEquipmentFromItemRoster();
		}
		EnhancedQuaterMasterService.SetLastItemRosterVersionNo(currentVersionNo);
	}
	
	public static void ClanPresentationDone_Postfix()
	{
		int latestVersionNo = EnhancedQuaterMasterService.GetLatestFilterSettingsVersionNo();
		if (latestVersionNo != EnhancedQuaterMasterService.GetPreviousFilterSettingsVersionNo())
		{
			EnhancedQuaterMasterBehavior.GiveBestEquipmentFromItemRoster();
		}
		EnhancedQuaterMasterService.SetPreviousFilterSettingsVersionNo(latestVersionNo);
	}
}