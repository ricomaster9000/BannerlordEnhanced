using TaleWorlds.CampaignSystem.Party;

namespace BannerlordEnhancedPartyRoles.Storage;

public static class EnhancedScoutData
{
    public static bool Active = true;

    // Alert if enemies nearby data
    public static bool ScoutAlertsNearbyEnemies = true;
    public static bool ScoutAlertsNearbyEnemiesFrozen = false;
    public static bool ScoutAlertsNearbyEnemiesTempDisabled = false;
    public static readonly int ScoutAlertsNearbyEnemiesAutoDisabledDurationInMillis = 2500;
    public static readonly int ScoutAlertsNearbyEnemiesTempDisabledDurationInMin = 5;
    
    // TEMP DATA
    public static MobileParty PrevPossibleHostilePartyTargetingPlayer;
}