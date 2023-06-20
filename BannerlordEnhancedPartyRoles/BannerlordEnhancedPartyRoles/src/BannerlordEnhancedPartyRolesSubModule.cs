using BannerlordEnhancedFramework.utils;
using BannerlordEnhancedPartyRoles.patches;
using BannerlordEnhancedPartyRoles.Behaviors;

using TaleWorlds.CampaignSystem.Inventory;
using TaleWorlds.MountAndBlade;
using TaleWorlds.Library;
using TaleWorlds.Core;
using TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu.Overlay;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem;

namespace BannerlordEnhancedPartyRoles;

public class BannerlordEnhancedPartyRolesSubModule : MBSubModuleBase 
{
    protected override void OnSubModuleLoad()
    {
        base.OnSubModuleLoad();
        DebugUtils.LogAndPrintInfo("OnSubModuleLoad ->");
        HarmonyPatcher harmonyPatcher = new HarmonyPatcher("BannerlordEnhancedPartyRoles");

        DebugUtils.LogAndPrintInfo("add harmony patches - BEGIN");
        harmonyPatcher.PatchMethodPostfix(
            typeof(InventoryLogic).GetMethod("DoneLogic"),
            typeof(QuartermasterPatches).GetMethod("DoneLogic_Postfix")
        );
        DebugUtils.LogAndPrintInfo("add harmony patches - END");
    }

    public override void OnGameLoaded(Game game, object initializerObject)
    {
        base.OnGameLoaded(game, initializerObject);

        DebugUtils.LogAndPrintInfo("OnGameLoaded-------------------------------->!");
    }

    protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
    {
        base.OnGameStart(game, gameStarterObject);
        if (!(game.GameType is Campaign)) return;
        if (gameStarterObject != null)  (gameStarterObject as CampaignGameStarter).AddBehavior(new QuaterMasterDialog());
    }
}
