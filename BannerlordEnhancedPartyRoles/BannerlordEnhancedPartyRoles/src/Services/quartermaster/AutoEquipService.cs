using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BannerlordEnhancedFramework.extendedtypes;
using BannerlordEnhancedFramework.extendedtypes.itemcategories;
using BannerlordEnhancedFramework.src.utils;
using BannerlordEnhancedFramework.utils;
using BannerlordEnhancedPartyRoles.src.Storage;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace BannerlordEnhancedPartyRoles.src.Services
{

	public static class AutoEquipService
	{ 
		
		
		public static void GiveBestEquipmentFromItemRoster()
		{
			MobileParty mainParty = MobileParty.MainParty;
			ItemRoster itemRoster = mainParty.ItemRoster;
			List<TroopRosterElement> allCompanionsTroopRosterElement = PartyUtils.GetHerosExcludePlayerHero(mainParty.Party.MemberRoster.GetTroopRoster(), mainParty.LeaderHero);
			List<FighterClass> fighters = new List<FighterClass>();
			bool canRemoveLockedItems = AutoEquipService.GetAllowLockedItems() == false;
			Dictionary<string, int> categories = new Dictionary<string, int>();
			
			List<ExtendedCultureCode> chosenCultures = AutoEquipService.GetChosenCultures();
			foreach (var chosenCulture in chosenCultures)
			{
				HeroEquipmentCustomization heroEquipmentCustomization = new HeroEquipmentCustomizationByClass();

				if (chosenCulture == ExtendedCultureCode.get(CultureCode.Invalid))
				{
					continue;
				}
				
				if (chosenCulture != ExtendedCultureCode.get(CultureCode.AnyOtherCulture))
				{
					heroEquipmentCustomization = new HeroEquipmentCustomizationByClassAndCulture(chosenCulture.nativeCultureCode());
				}

				foreach (TroopRosterElement troopCompanion in allCompanionsTroopRosterElement)
				{
					fighters.Add(new FighterClass(troopCompanion.Character.HeroObject, heroEquipmentCustomization));
				}

				foreach (FighterClass fighterClass in fighters)
				{
					List<ItemRosterElement> items = canRemoveLockedItems
						? EquipmentUtil.RemoveLockedItems(itemRoster.ToList())
						: itemRoster.ToList();

					if (AutoEquipService.GetAllowBattleEquipment())
					{
						PartyUtils.updateItemRoster(itemRoster, fighterClass.removeRelavantBattleEquipment(items),
							new List<ItemRosterElement>());

						items = canRemoveLockedItems
							? EquipmentUtil.RemoveLockedItems(itemRoster.ToList())
							: itemRoster.ToList();
						var changes = fighterClass.assignBattleEquipment(items);
						categories = ExtendedItemCategory.GetAllItemCategoryNamesByItemsAndCategories(changes.removals,
							fighterClass.MainItemCategories, categories);
						PartyUtils.updateItemRoster(itemRoster, changes.additions, changes.removals);
					}

					if (AutoEquipService.GetAllowCivilianEquipment())
					{
						items = canRemoveLockedItems
							? EquipmentUtil.RemoveLockedItems(itemRoster.ToList())
							: itemRoster.ToList();
						PartyUtils.updateItemRoster(itemRoster, fighterClass.removeRelavantCivilianEquipment(items),
							new List<ItemRosterElement>());
						var changes = fighterClass.assignCivilianEquipment(items);

						categories = ExtendedItemCategory.GetAllItemCategoryNamesByItemsAndCategories(changes.removals,
							fighterClass.MainItemCategories, categories);
						PartyUtils.updateItemRoster(itemRoster, changes.additions, changes.removals);
					}
				}
			}

			if (categories.Count > 0)
			{
				List<string> categoriesNames = new List<string>();
				foreach(KeyValuePair<string, int> item in categories)
				{
					categoriesNames.Add(item.Key);
				}
				InformationManager.DisplayMessage(new InformationMessage("Quartermaster updated companions " + BuildQuarterMasterNotification(categoriesNames), BannerlordEnhancedFramework.Colors.Yellow));
			}
			
		}
		
		public static string BuildQuarterMasterNotification(List<string> list)
		{
			string text = "";
			int i = 0;
			int size = list.Count;
			foreach (var word in list)
			{
				i += 1;
				if (i == size)
				{
					string addsPlural = word[word.Length - 1] != 'r' ? word + "s" : word;
					text += addsPlural;
				}
				else if (i == size - 1)
				{
					text += word + " and ";
				}
				else
				{
					text += word + ", ";
				}
			}
			return text;
		}
		
		public static void ToggleQuaterMasterAllowLockedItems()
		{
			SetAllowLockedItems(GetAllowLockedItems() == false ? true : false);
		}
		public static void ToggleQuaterMasterAllow_AnyCulture()
		{
			bool currentToggle = GetAllowAnyCulture();
			SetAllowAnyCulture(currentToggle == false ? true : false);
			if (GetAllowAnyCulture())
			{
				SetAllCultureToAllowTrue();
			}
			else
			{
				SetAllCultureToAllowFalse();
			}
		}
		public static void ToggleQuaterMasterAllow_BattaniaCulture()
		{
			bool currentToggle = GetAllowBattaniaCulture();
			SetAllCultureToAllowFalse();
			SetAllowBattaniaCulture(currentToggle == false ? true : false);
		}
		public static void ToggleQuaterMasterAllow_SturgiaCulture()
		{
			bool currentToggle = GetAllowSturgiaCulture();
			SetAllCultureToAllowFalse();
			SetAllowSturgiaCulture(currentToggle == false ? true : false);
		}
		public static void ToggleQuaterMasterAllow_AseraiCulture()
		{
			bool currentToggle = GetAllowAseraiCulture();
			SetAllCultureToAllowFalse();
			SetAllowAseraiCulture(currentToggle == false ? true : false);
		}
		public static void ToggleQuaterMasterAllow_KhuzaitCulture()
		{
			bool currentToggle = GetAllowKhuzaitCulture();
			SetAllCultureToAllowFalse();
			SetAllowKhuzaitCulture(currentToggle == false ? true : false);
		}
		public static void ToggleQuaterMasterAllow_VlandiaCulture()
		{
			bool currentToggle = GetAllowVlandiaCulture();
			SetAllCultureToAllowFalse();
			SetAllowVlandiaCulture(currentToggle == false ? true : false);
		}
		public static void ToggleQuaterMasterAllow_EmpireCulture()
		{
			bool currentToggle = GetAllowEmpireCulture();
			SetAllCultureToAllowFalse();
			SetAllowEmpireCulture(currentToggle == false ? true : false);
		}

		public static void ToggleQuaterMasterAllow_BattleEquipment()
		{
			SetAllowBattleEquipment(GetAllowBattleEquipment() == false ? true : false);
		}
		public static void ToggleQuaterMasterAllow_CivilianEquipment()
		{
			SetAllowCivilianEquipment(GetAllowCivilianEquipment() == false ? true : false);
		}

		public static void SetAllCultureToAllowFalse()
		{
			SetAllowAnyCulture(false);
			SetAllowBattaniaCulture(false);
			SetAllowSturgiaCulture(false);
			SetAllowAseraiCulture(false);
			SetAllowKhuzaitCulture(false);
			SetAllowVlandiaCulture(false);
			SetAllowEmpireCulture(false);
		}
		public static void SetAllCultureToAllowTrue()
		{
			SetAllowAnyCulture(true);
			SetAllowBattaniaCulture(true);
			SetAllowSturgiaCulture(true);
			SetAllowAseraiCulture(true);
			SetAllowKhuzaitCulture(true);
			SetAllowVlandiaCulture(true);
			SetAllowEmpireCulture(true);
		}

		public static void SetAllowAnyCulture(bool flag)
		{
			EnhancedQuarterMasterData.AutoEquip.LatestFilterSettingsVersionNo += 1;
		}
		public static void SetAllowLockedItems(bool flag)
		{
			EnhancedQuarterMasterData.AutoEquip.LatestFilterSettingsVersionNo += 1;
			EnhancedQuarterMasterData.AutoEquip.AllowLockedEquipment = flag;
		}
		public static void SetAllowBattaniaCulture(bool flag)
		{
			EnhancedQuarterMasterData.AutoEquip.LatestFilterSettingsVersionNo += 1;
		}
		public static void SetAllowSturgiaCulture(bool flag)
		{
			EnhancedQuarterMasterData.AutoEquip.LatestFilterSettingsVersionNo += 1;
		}
		public static void SetAllowAseraiCulture(bool flag)
		{
			EnhancedQuarterMasterData.AutoEquip.LatestFilterSettingsVersionNo += 1;
		}
		public static void SetAllowKhuzaitCulture(bool flag)
		{
			EnhancedQuarterMasterData.AutoEquip.LatestFilterSettingsVersionNo += 1;
		}
		public static void SetAllowVlandiaCulture(bool flag)
		{
			EnhancedQuarterMasterData.AutoEquip.LatestFilterSettingsVersionNo += 1;
		}
		public static void SetAllowEmpireCulture(bool flag)
		{
			EnhancedQuarterMasterData.AutoEquip.LatestFilterSettingsVersionNo += 1;
		}

		public static void SetAllowBattleEquipment(bool flag)
		{
			EnhancedQuarterMasterData.AutoEquip.AllowBattleEquipment = flag;
		}
		public static void SetAllowCivilianEquipment(bool flag)
		{
			EnhancedQuarterMasterData.AutoEquip.AllowCivilianEquipment = flag;
		}

		public static void SetLastItemRosterVersionNo(int version)
		{
			EnhancedQuarterMasterData.AutoEquip.LastItemRosterVersionNo = version;
		}
		public static void SetIsLastInventoryCancelPressed(bool fromCancel)
		{
			EnhancedQuarterMasterData.AutoEquip.IsLastInventoryCancelPressed = fromCancel;
		}

		public static void SetPreviousFilterSettingsVersionNo(int versionNo)
		{
			EnhancedQuarterMasterData.AutoEquip.PreviousFilterSettingsVersionNo = versionNo;
		}


		public static bool GetAllowAnyCulture()
		{
			return EnhancedQuarterMasterData.AutoTraderData.AllowAnyCulture;
		}
		public static bool GetAllowLockedItems()
		{
			return EnhancedQuarterMasterData.AutoEquip.AllowLockedEquipment;
		}
		public static bool GetAllowBattaniaCulture()
		{
			return !EnhancedQuarterMasterData.AutoEquip.CultureToItemCategoryFilters[ExtendedCultureCode.byName["Battania"]]["LockedAll"];
		}
		public static bool GetAllowSturgiaCulture()
		{
			return !EnhancedQuarterMasterData.AutoEquip.CultureToItemCategoryFilters[ExtendedCultureCode.byName["Sturgia"]]["LockedAll"];
		}
		public static bool GetAllowAseraiCulture()
		{
			return !EnhancedQuarterMasterData.AutoEquip.CultureToItemCategoryFilters[ExtendedCultureCode.byName["Aserai"]]["LockedAll"];
		}
		public static bool GetAllowKhuzaitCulture()
		{
			return !EnhancedQuarterMasterData.AutoEquip.CultureToItemCategoryFilters[ExtendedCultureCode.byName["Khuzait"]]["LockedAll"];
		}
		public static bool GetAllowVlandiaCulture()
		{
			return !EnhancedQuarterMasterData.AutoEquip.CultureToItemCategoryFilters[ExtendedCultureCode.byName["Vlandia"]]["LockedAll"];
		}
		public static bool GetAllowEmpireCulture()
		{
			return !EnhancedQuarterMasterData.AutoEquip.CultureToItemCategoryFilters[ExtendedCultureCode.byName["Empire"]]["LockedAll"];
		}

		public static bool GetAllowBattleEquipment()
		{
			return EnhancedQuarterMasterData.AutoEquip.AllowBattleEquipment;
		}
		public static bool GetAllowCivilianEquipment()
		{
			return EnhancedQuarterMasterData.AutoEquip.AllowCivilianEquipment;
		}
		public static int GetLastItemRosterVersionNo()
		{
			return EnhancedQuarterMasterData.AutoEquip.LastItemRosterVersionNo;
		}

		public static bool GetIsLastInventoryCancelPressed()
		{
			return EnhancedQuarterMasterData.AutoEquip.IsLastInventoryCancelPressed;
		}

		public static int GetLatestFilterSettingsVersionNo()
		{
			return EnhancedQuarterMasterData.AutoEquip.LatestFilterSettingsVersionNo;
		}
		public static int GetPreviousFilterSettingsVersionNo()
		{
			return EnhancedQuarterMasterData.AutoEquip.PreviousFilterSettingsVersionNo;
		}
		public static List<ExtendedCultureCode> GetChosenCultures()
		{
			List<ExtendedCultureCode> choseCultures = new List<ExtendedCultureCode>();
			foreach (var cultureToItemCategoryFilter in EnhancedQuarterMasterData.AutoEquip.CultureToItemCategoryFilters)
			{
				if (!cultureToItemCategoryFilter.Value["LockedAll"])
				{
					choseCultures.Add(cultureToItemCategoryFilter.Key);
				}
			}

			return choseCultures;
		}
	}
}
