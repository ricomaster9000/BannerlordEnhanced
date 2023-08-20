using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem.Party;

namespace BannerlordEnhancedPartyRoles.src.Storage;

static class EnhancedQuaterMasterData
{
	public static bool AllowLockedEquipment= false;
	public static bool AllowAnyCulture = false;
	public static bool AllowBattaniaCulture = true;
	public static bool AllowSturgiaCulture = false;
	public static bool AllowAseraiCulture = false;
	public static bool AllowKhuzaitCulture = false;
	public static bool AllowVlandiaCulture = false;
	public static bool AllowEmpireCulture = false;
	public static int  PreviousFilterSettingsVersionNo = 0;
	public static int  LatestFilterSettingsVersionNo = 0;

	public static bool AllowBattleEquipment = true;
	public static bool AllowCivilianEquipment = true;

	public static int LastItemRosterVersionNo = MobileParty.MainParty.ItemRoster.VersionNo;
	public static bool IsLastInventoryCancelPressed = false;

}
