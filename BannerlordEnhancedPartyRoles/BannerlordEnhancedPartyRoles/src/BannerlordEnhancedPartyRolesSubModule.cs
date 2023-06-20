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

    public override void OnGameLoaded(Game game, object initializerObject)
    {
        base.OnGameLoaded(game, initializerObject);

        InformationManager.DisplayMessage(new InformationMessage("OnGameLoaded-------------------------------->!", Color.White));
    }

    protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
    {
        base.OnGameStart(game, gameStarterObject);
        if (!(game.GameType is Campaign)) return;

        if (gameStarterObject != null)  (gameStarterObject as CampaignGameStarter).AddBehavior(new QuaterMasterDialog());

        // gameStarterObject.AddModel(new MoneyHandler());

        MobileParty playerParty = Campaign.Current.MainParty;
        Debug.Print(playerParty.ToString(), 0);
    }
}
