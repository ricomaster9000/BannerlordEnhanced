using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.Core;
using TaleWorlds.LinQuick;

namespace BannerlordEnhancedFramework.utils;

public static class GameStateUtils
{
    public static MapState WorldMapGameState()
    {
        MapState result = null;
        if (Game.Current != null)
        {
            foreach (GameState gameState in Game.Current.GameStateManager.GameStates)
            {
                if (gameState.GetType() == typeof(MapState))
                {
                    result = (MapState)gameState;
                    break;
                }
            }
        }
        return result;
    }
    
    public static MapState ConversationGameState()
    {
        MapState result = null;
        if (Game.Current != null)
        {
            foreach (GameState gameState in Game.Current.GameStateManager.GameStates)
            {
                if (gameState.GetType() == typeof(GameState))
                {
                    result = (MapState)gameState;
                    break;
                }
            }
        }
        return result;
    }
    
    public static void GoToWorldMap()
    {
        //MapState mapState = WorldMapGameState();
        //Game.Current.GameStateManager.ActiveState
        //mapState.Level = 1;
        //Game.Current.GameStateManager.CleanStates(0);
        //mapState.ExitMenuMode();
        //Game.Current.GameStateManager
        //Game.Current.GameStateManager.PopState(1);
        //mapState.Level = 0;
        //Game.Current.GameStateManager.PopState(0);
        //mapState.OnMapConversationOver();
        //Game.Current.GameStateManager.CreateState<MapState>(mapState);
    }
}