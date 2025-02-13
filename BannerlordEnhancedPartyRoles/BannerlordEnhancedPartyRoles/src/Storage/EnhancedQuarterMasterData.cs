﻿using System;
using System.Collections.Generic;
using BannerlordEnhancedFramework.extendedtypes;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace BannerlordEnhancedPartyRoles.src.Storage;

static class EnhancedQuarterMasterData
{
	static EnhancedQuarterMasterData() {}

	public static class AutoTraderData
	{
		
		static AutoTraderData()
		{
			foreach (var extendedCultureCode in ExtendedCultureCode.values())
			{
				CultureToItemCategoryFilters.Add(extendedCultureCode, new Dictionary<string, bool>()
				{
					{ "LockedAll", false },
					{ "AllowBodyArmour", true },
					{ "AllowWeapons", true },
					{ "AllowSaddles", true },
					{ "AllowMiscellaneous", true },
					{ "AllowHorses", true },
					{ "AllowCamels", true },
					{ "AllowBanners", true }
				});
			}
		}
		
		public static readonly Dictionary<ExtendedCultureCode, Dictionary<String, Boolean>> CultureToItemCategoryFilters = new Dictionary<ExtendedCultureCode, Dictionary<string, bool>>();
		public static bool AllowAnyCulture = true;
		public static bool AllowBodyArmor = true;
		public static bool AllowWeapons = true;
		public static bool AllowSaddles = true;
		public static bool AllowCamels = true;
		public static bool AllowHorses = true;
		public static bool AllowBanners = true;
		public static bool AllowMiscellaneous = false;
		// OrderBy filtering settings
		public static bool IsLightestItemsFirst = true;
		public static bool AllowLockedItems = true;
	}
	
	public static class AutoEquip
	{
		static AutoEquip()
		{
			foreach (ExtendedCultureCode extendedCultureCode in ExtendedCultureCode.values())
			{
				CultureToItemCategoryFilters.Add(extendedCultureCode, new Dictionary<string, bool>()
				{
					{ "LockedAll", false },
					{ "AllowBodyArmour", true },
					{ "AllowWeapons", true },
					{ "AllowSaddles", true },
					{ "AllowMiscellaneous", true },
					{ "AllowHorses", true },
					{ "AllowCamels", true },
					{ "AllowBanners", true }
				});
			}
		}
		
		public static readonly Dictionary<ExtendedCultureCode, Dictionary<String, Boolean>> CultureToItemCategoryFilters = new Dictionary<ExtendedCultureCode, Dictionary<string, bool>>();
		// OrderBy filtering settings
		public static bool IsLighestItemsFirst = true;
		public static bool AllowLockedItems = true;
		
		public static bool AllowLockedEquipment = false;
		public static int PreviousFilterSettingsVersionNo = 0;
		public static int LatestFilterSettingsVersionNo = 0;

		public static bool AllowBattleEquipment = true;
		public static bool AllowCivilianEquipment = true;

		public static int LastItemRosterVersionNo = MobileParty.MainParty.ItemRoster.VersionNo;
		public static bool IsLastInventoryCancelPressed = false;
	}

}
