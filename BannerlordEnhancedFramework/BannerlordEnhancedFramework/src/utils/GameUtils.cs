using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Library;

namespace BannerlordEnhancedFramework.utils;

public static class GameUtils
{

    public static void PauseGame()
    {
        Campaign.Current.SetTimeSpeed(0);
    }
}
