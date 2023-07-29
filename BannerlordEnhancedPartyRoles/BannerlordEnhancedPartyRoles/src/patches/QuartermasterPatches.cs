using BannerlordEnhancedFramework.utils;
using BannerlordEnhancedPartyRoles.Behaviors;
using TaleWorlds.CampaignSystem.Inventory;

namespace BannerlordEnhancedPartyRoles.patches;

public class QuartermasterPatches
{
    public static void DoneLogic_Postfix(InventoryLogic __instance, bool __result)
    {
        DebugUtils.LogAndPrintInfo("DoneLogic_Postfix is working");
		// Kenneth, DO YOUR SHIT
		EnhancedQuaterMasterBehaviorNewVersion.GiveBestEquipmentFromItemRoster();
	}
}