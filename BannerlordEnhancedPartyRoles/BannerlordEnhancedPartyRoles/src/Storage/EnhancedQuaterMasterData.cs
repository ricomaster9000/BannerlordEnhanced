using TaleWorlds.CampaignSystem.Party;

namespace BannerlordEnhancedPartyRoles.src.Storage;

static class EnhancedQuaterMasterData
{
	public static class CompanionEquiptment
	{
		public static bool AllowLockedEquipment = false;
		public static bool AllowAnyCulture = false;
		public static bool AllowBattaniaCulture = true;
		public static bool AllowSturgiaCulture = false;
		public static bool AllowAseraiCulture = false;
		public static bool AllowKhuzaitCulture = false;
		public static bool AllowVlandiaCulture = false;
		public static bool AllowEmpireCulture = false;
		public static int PreviousFilterSettingsVersionNo = 0;
		public static int LatestFilterSettingsVersionNo = 0;

		public static bool AllowBattleEquipment = true;
		public static bool AllowCivilianEquipment = true;

		public static int LastItemRosterVersionNo = MobileParty.MainParty.ItemRoster.VersionNo;
		public static bool IsLastInventoryCancelPressed = false;
	}

	public static class AutoTradeItems
	{
		public static bool AllowAnyCulture = true;
		public static bool AllowBattaniaCulture = false;
		public static bool AllowSturgiaCulture = false;
		public static bool AllowAseraiCulture = false;
		public static bool AllowKhuzaitCulture = false;
		public static bool AllowVlandiaCulture = false;
		public static bool AllowEmpireCulture = false;

		public static bool IsLighestItemsFirst = true;

		public static bool AllowLockedItems = true;
		public static bool AllowArmour = true;
		public static bool AllowWeapons = true;
		public static bool AllowSaddles = true;
		public static bool AllowMiscellaneous = true;
		public static bool AllowHorses = true;
		public static bool AllowCamels = true;
		public static bool AllowBanners = true;
	}

}
