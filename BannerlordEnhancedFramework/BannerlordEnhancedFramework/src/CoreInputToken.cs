using System;

namespace BannerlordEnhancedFramework;

public class CoreInputToken
{
    // A-Z

    public class Entry
    {
        // base game dialogues
        // every hero should have this dialogue option, base game dialogue
        public static InputToken HeroMainOptions = new InputToken("hero_main_options");

        public static InputToken MerchantWeaponsmith = new InputToken("weaponsmith_talk_player");

        public static InputToken MerchantBarber = new InputToken("barber_question1");

        public static InputToken MerchantShopworker = new InputToken("shopworker_npc_player");

        public static InputToken MerchantBlacksmith = new InputToken("blacksmith_player");

        public static InputToken TavernKeeper = new InputToken("tavernkeeper_talk");

        public static InputToken TavernMaid = new InputToken("tavernmaid_talk");

        public static InputToken TavernBard = new InputToken("talk_bard_player");

        public static InputToken TavernGamehost = new InputToken("taverngamehost_talk");

        public static InputToken TavernRansombroker = new InputToken("ransom_broker_talk");

        public static InputToken TownOrVillageGeneral = new InputToken("town_or_village_player");

        public static InputToken TownAlley = new InputToken("alley_talk_start");

        public static InputToken TownArenamaster = new InputToken("arena_master_talk");
    }

    public class End
    {
        
        // generic, can apply anywhere, name pretty self explantory
        public static InputToken CloseWindow = new InputToken("close_window");

        public static InputToken Exit = new InputToken("end");
        
        public static InputToken HeroReturnToStart = new InputToken("lord_pretalk");

        public static InputToken MerchantBackToStart = new InputToken("merchant_response_3");

        public static InputToken MerchantShopworkerBackToStart = new InputToken("start_2");

        public static InputToken MerchantBlacksmithBackToStart = new InputToken("player_blacksmith_after_craft");

        public static InputToken TavernKeeperReturnToStart = new InputToken("tavernkeeper_pretalk");

        public static InputToken TownOrVillageBackToStart = new InputToken("town_or_village_pretalk");
    }

    public class InputToken
    {
        private readonly String _tokenName;

        internal InputToken(String tokenName)
        {
            this._tokenName = tokenName;
        }

        public String TokenName()
        {
            return _tokenName;
        }
    }
}