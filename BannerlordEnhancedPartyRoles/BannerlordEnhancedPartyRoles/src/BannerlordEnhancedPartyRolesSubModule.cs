using BannerlordEnhancedFramework.utils;
using BannerlordEnhancedPartyRoles.patches;
using TaleWorlds.CampaignSystem.Inventory;
using TaleWorlds.MountAndBlade;

namespace BannerlordEnhancedPartyRoles
{
    public class BannerlordEnhancedPartyRolesSubModule : MBSubModuleBase 
    {
        protected override void OnSubModuleLoad()
        {
            HarmonyPatcher harmonyPatcher = new HarmonyPatcher("BannerlordEnhancedPartyRoles");

            // TODO - remove or find a purpose or ignore
            // ConfigurationUtils.LoadValues();
            if (DebugUtils.IsDebugModeActive())
            {
                DebugUtils.PrintMethodNames(typeof(InventoryLogic));
            }
            DebugUtils.PrintMethodNames(typeof(InventoryLogic));

            DebugUtils.LogAndPrintInfo("add harmony patches - BEGIN");
            harmonyPatcher.PatchMethodPostfix(
                typeof(InventoryLogic).GetMethod("DoneLogic"),
                typeof(QuartermasterPatches).GetMethod("DoneLogic_Postfix")
            );
            DebugUtils.LogAndPrintInfo("add harmony patches - END");

            DebugUtils.LogAndPrintInfo("add dialogues - BEGIN");

            // Kenneth...

            DebugUtils.LogAndPrintInfo("add dialogues - END");
        }
    }
}