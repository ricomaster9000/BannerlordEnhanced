using BannerlordEnhancedPartyRoles.Storage;

namespace BannerlordEnhancedPartyRoles.Services;

public static class EnhancedScoutService
{
    public static void SetIsActive(bool active)
    {
        EnhancedScoutData.Active = active;
    }
    
    public static void SetScoutAlertsNearbyEnemies(bool scoutAlertsNearbyEnemies)
    {
        EnhancedScoutData.ScoutAlertsNearbyEnemies = scoutAlertsNearbyEnemies;
    }
}