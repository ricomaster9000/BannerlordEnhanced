using System;

namespace BannerlordEnhancedFramework;

public class BehaviorTokens
{
    // A-Z

    // every hero should have this dialogue option, base game dialogue
    public static BehaviorToken HeroMainOptions = new BehaviorToken("hero_main_options");

    // base game dialogue
    public static BehaviorToken MerchantWeaponsmith = new BehaviorToken("merchant_response_3");

    // base game dialogue
    public static BehaviorToken MerchantBarber = new BehaviorToken("barber_question1");

    // base game dialogue
    public static BehaviorToken MerchantShopworker = new BehaviorToken("shopworker_npc_player");

    // base game dialogue
    public static BehaviorToken MerchantBlacksmith = new BehaviorToken("blacksmith_player");
    
    // base game dialogue
    public static BehaviorToken TavernKeeper = new BehaviorToken("tavernkeeper_talk");
        
    // base game dialogue
    public static BehaviorToken TavernMaid = new BehaviorToken("tavernmaid_talk");

    // base game dialogue
    public static BehaviorToken TavernBard = new BehaviorToken("talk_bard_player");

    // base game dialogue
    public static BehaviorToken TavernGamehost = new BehaviorToken("taverngamehost_talk");

    // base game dialogue
    public static BehaviorToken TavernRansombroker = new BehaviorToken("ransom_broker_talk");

    // base game dialogue
    public static BehaviorToken TownOrVillageGeneral = new BehaviorToken("town_or_village_player");

    // base game dialogue
    public static BehaviorToken TownAlley = new BehaviorToken("alley_talk_start");

    // base game dialogue
    public static BehaviorToken TownArenamaster = new BehaviorToken("arena_master_talk");
}

public class BehaviorToken
{
    private readonly String _inputToken;

    public BehaviorToken(String inputToken)
    {
        this._inputToken = inputToken;
    }

    private String InputToken()
    {
        return _inputToken;
    }
}