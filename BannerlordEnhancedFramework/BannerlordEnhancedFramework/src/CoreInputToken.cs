using System;

namespace BannerlordEnhancedFramework
{

    public static class CoreInputToken
    {
        // A-Z

        public static class Entry
        {
            // base game dialogues
            // every hero should have this dialogue option, base game dialogue
            public static readonly InputToken HeroMainOptions = new InputToken("hero_main_options");

            public static readonly InputToken MerchantWeaponsmith = new InputToken("weaponsmith_talk_player");

            public static readonly InputToken MerchantBarber = new InputToken("barber_question1");

            public static readonly InputToken MerchantShopworker = new InputToken("shopworker_npc_player");

            public static readonly InputToken MerchantBlacksmith = new InputToken("blacksmith_player");

            public static readonly InputToken TavernKeeper = new InputToken("tavernkeeper_talk");

            public static readonly InputToken TavernMaid = new InputToken("tavernmaid_talk");

            public static readonly InputToken TavernBard = new InputToken("talk_bard_player");

            public static readonly InputToken TavernGamehost = new InputToken("taverngamehost_talk");

            public static readonly InputToken TavernRansombroker = new InputToken("ransom_broker_talk");

            public static readonly InputToken TownOrVillageGeneral = new InputToken("town_or_village_player");

            public static readonly InputToken TownAlley = new InputToken("alley_talk_start");

            public static readonly InputToken TownArenamaster = new InputToken("arena_master_talk");
        }

        public static class End
        {

            // generic, can apply anywhere, name pretty self explantory
            public static readonly InputToken CloseWindow = new InputToken("close_window");

            public static readonly InputToken Exit = new InputToken("end");

            public static readonly InputToken HeroReturnToStart = new InputToken("lord_pretalk");

            public static readonly InputToken MerchantBackToStart = new InputToken("merchant_response_3");

            public static readonly InputToken MerchantShopworkerBackToStart = new InputToken("start_2");

            public static readonly InputToken MerchantBlacksmithBackToStart =
                new InputToken("player_blacksmith_after_craft");

            public static readonly InputToken TavernKeeperReturnToStart = new InputToken("tavernkeeper_pretalk");

            public static readonly InputToken TownOrVillageBackToStart = new InputToken("town_or_village_pretalk");
        }
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