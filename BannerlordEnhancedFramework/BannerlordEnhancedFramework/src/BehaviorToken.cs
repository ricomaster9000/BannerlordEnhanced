using System;

namespace BannerlordEnhancedFramework;

public class BehaviorTokens
{
    // A-Z

    public class Entry
    {
        // base game dialogues
        // every hero should have this dialogue option, base game dialogue
        public static BehaviorToken HeroMainOptions = new BehaviorToken("hero_main_options");

        public static BehaviorToken MerchantWeaponsmith = new BehaviorToken("weaponsmith_talk_player");

        public static BehaviorToken MerchantBarber = new BehaviorToken("barber_question1");

        public static BehaviorToken MerchantShopworker = new BehaviorToken("shopworker_npc_player");

        public static BehaviorToken MerchantBlacksmith = new BehaviorToken("blacksmith_player");

        public static BehaviorToken TavernKeeper = new BehaviorToken("tavernkeeper_talk");

        public static BehaviorToken TavernMaid = new BehaviorToken("tavernmaid_talk");

        public static BehaviorToken TavernBard = new BehaviorToken("talk_bard_player");

        public static BehaviorToken TavernGamehost = new BehaviorToken("taverngamehost_talk");

        public static BehaviorToken TavernRansombroker = new BehaviorToken("ransom_broker_talk");

        public static BehaviorToken TownOrVillageGeneral = new BehaviorToken("town_or_village_player");

        public static BehaviorToken TownAlley = new BehaviorToken("alley_talk_start");

        public static BehaviorToken TownArenamaster = new BehaviorToken("arena_master_talk");
    }

    public class End
    {
        
        // generic, can apply anywhere, name pretty self explantory
        public static BehaviorToken CloseWindow = new BehaviorToken("close_window");
        
        // TODO - not sure what this one does
        public static BehaviorToken Start = new BehaviorToken("start");
        
        public static BehaviorToken HeroReturnToStart = new BehaviorToken("lord_pretalk");

        public static BehaviorToken MerchantBackToStart = new BehaviorToken("merchant_response_3");

        public static BehaviorToken MerchantShopworkerBackToStart = new BehaviorToken("start_2");

        public static BehaviorToken MerchantBlacksmithBackToStart = new BehaviorToken("player_blacksmith_after_craft");

        public static BehaviorToken TavernKeeperReturnToStart = new BehaviorToken("tavernkeeper_pretalk");

        public static BehaviorToken TownOrVillageBackToStart = new BehaviorToken("town_or_village_pretalk");
    }

    public class BehaviorToken
    {
        private readonly String _tokenName;

        public BehaviorToken(String tokenName)
        {
            this._tokenName = tokenName;
        }

        private String TokenName()
        {
            return _tokenName;
        }
    }
}