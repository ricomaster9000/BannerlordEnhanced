namespace BannerlordEnhancedPartyRoles.Storage;

public static class EnhancedScoutData
{
    public static bool Active = true;

    // Alert if enemies nearby data
    public static bool ScoutAlertsNearbyEnemies = true;
    public static int ScoutAlertsNearbyEnemiesRange = 25; // 1 is equivalent to the distance achieved for 1 movement speed in one second
    public static bool ScoutAlertsNearbyEnemiesFrozen = false;
    public static bool ScoutAlertsNearbyEnemiesTempDisabled = false;
    public static int ScoutAlertsNearbyEnemiesAutoDisabledDurationInMillis = 2500;
    public static int ScoutAlertsNearbyEnemiesTempDisabledDurationInMin = 5;
}