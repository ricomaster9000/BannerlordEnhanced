using System;
using System.Collections.Generic;
using System.Linq;
using BannerlordEnhancedFramework.utils;
using BannerlordEnhancedPartyRoles.Services;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.Diamond.Ranked;
using static BannerlordEnhancedPartyRoles.Services.EnhancedQuaterMasterService;

namespace BannerlordEnhancedPartyRoles.src.Services
{
	public class WeaponsManager
	{
		public static readonly SkillObject[] CombatSkills = {
			DefaultSkills.OneHanded,
			DefaultSkills.TwoHanded,
			DefaultSkills.Polearm,
			DefaultSkills.Throwing,
			DefaultSkills.Riding,
			DefaultSkills.Crossbow,
			DefaultSkills.Bow,

		};

		public static List<TroopRosterElement> OrderBySkillValue(List<TroopRosterElement> troopRosterElementList, SkillObject skillObject)
		{
			return troopRosterElementList.OrderByDescending(troopRosterElement => troopRosterElement.Character.GetSkillValue(skillObject)).ToList();
		}

		public static SkillObject GetHighestCombatSkill(CharacterObject character)
		{
			return GetSkillByRank(character, 1);
		}

		public static SkillObject GetSkillByRank(CharacterObject character, int rank)
		{
			List<Tuple<SkillObject, int>> skills = new List<Tuple<SkillObject, int>>();
			foreach(SkillObject skillObject in CombatSkills)
			{
				int skillValue = character.GetSkillValue(skillObject);
				skills.Add(new Tuple<SkillObject, int> (skillObject, skillValue));
			}
			IEnumerable<Tuple<SkillObject, int>> orderedSkills = skills.OrderByDescending(tuple => tuple.Item2);
			return orderedSkills.ElementAt(rank-1).Item1;
		}

		// Ordered by descending first element in list is the main weapon class
		public static List<WeaponClass> GetItemWeaponClasses(ItemObject item)
		{
			List<WeaponClass> weapons = new List<WeaponClass>();
			foreach (WeaponComponentData weaponComponentData in item.Weapons)
			{
				weapons.Add(weaponComponentData.WeaponClass);
			}
			return weapons;
		}

		public static bool IsSameWeaponClass(ItemObject item1, ItemObject item2)
		{
			return item1.WeaponComponent.PrimaryWeapon.WeaponClass == item2.WeaponComponent.PrimaryWeapon.WeaponClass;
		}

		public static EquipmentIndex GetWeaponEquipmentIndexWhere(Equipment equipment, Predicate<ItemObject> predicate)
		{
			EquipmentIndex equipmentIndex = EquipmentIndex.None;
			for (EquipmentIndex i = EquipmentIndex.WeaponItemBeginSlot; i < EquipmentIndex.ExtraWeaponSlot; i++)
			{
				EquipmentElement equipmentElement = equipment[(int)i];
				ItemObject item = equipmentElement.Item;
				if (predicate(item))
				{
					equipmentIndex = i;
					break;
				}
			}
			return equipmentIndex;
		}

		public static EquipmentIndex LowestEquippedWeaponBySkillRank(Equipment equipment, CharacterObject character)
		{
			EquipmentIndex equipmentIndex = EquipmentIndex.None;
			int skillValue = 9000;

			for (EquipmentIndex i = EquipmentIndex.WeaponItemBeginSlot; i < EquipmentIndex.ExtraWeaponSlot; i++)
			{
				EquipmentElement equipmentElement = equipment[(int)i];
				if (Object.ReferenceEquals(equipmentElement, null) || equipmentElement.IsEmpty)
				{
					continue;
				}
				ItemObject item = equipmentElement.Item;

				int itemSkillValue = character.GetSkillValue(item.RelevantSkill);
				if (itemSkillValue < skillValue)
				{
					skillValue = itemSkillValue;
					equipmentIndex = i;
				}
			}
			return equipmentIndex;
		}

		public static bool IsSkillGreater(SkillObject skill1, SkillObject skill2, CharacterObject character)
		{
			return character.GetSkillValue(skill1) > character.GetSkillValue(skill2) ? true : false;
		}

		public static bool HasItemBySkillObject(ItemObject weaponItem, Equipment equipment)
		{
			EquipmentIndex equipmentIndex = GetWeaponEquipmentIndexWhere(equipment, (item) =>
			{
				return item != null && item.RelevantSkill == weaponItem.RelevantSkill;
			});
			return equipmentIndex != EquipmentIndex.None;
		}

		public static EquipmentIndex FindOpenWeaponSlot(Equipment equipment)
		{
			return GetWeaponEquipmentIndexWhere(equipment, (item) =>
			{
				return item == null;
			});
		}

		public static int CountItems(List<ItemRosterElement> itemRosterElementList)
		{
			int total = 0;
			foreach (ItemRosterElement itemRosterElement in itemRosterElementList)
			{
				total += itemRosterElement.Amount;
			}
			return total;
		}
		public static bool UpdateCompanionBanners(ItemRoster itemRoster, List<TroopRosterElement> troopRosterElementList)
		{
			List<ItemRosterElement> itemRosterElementList = itemRoster.ToList();
			itemRosterElementList = EnhancedQuaterMasterService.OrderByBanners(itemRosterElementList);
			itemRosterElementList = OrderBannersByLevel(itemRosterElementList);
			itemRosterElementList = RemoveLockedItems(itemRosterElementList);
			bool hasGivenBanner = GiveBannersByHighestLevel(itemRoster, itemRosterElementList, troopRosterElementList);
			return hasGivenBanner;
		}

		public static bool UpdateCompanionWeapons(ItemRoster itemRoster, List<TroopRosterElement> troopRosterElementList)
		{
			List<ItemRosterElement> itemRosterElementList = itemRoster.ToList();
			itemRosterElementList = EnhancedQuaterMasterService.OrderByCondition(itemRosterElementList, (ItemRosterElement) =>
			{
				return IsItemWeapon(ItemRosterElement) || IsItemHorse(ItemRosterElement);
			});
			itemRosterElementList = EnhancedQuaterMasterService.OrderItemRosterByMostEffective(itemRosterElementList);
			itemRosterElementList = RemoveLockedItems(itemRosterElementList);
			bool hasGivenWeapon = GiveWeaponsBySkillAndEffectiveness(itemRoster, itemRosterElementList, troopRosterElementList);
			return hasGivenWeapon;
		}

		public static bool IsItemEquipped(EquipmentElement equipmentElement)
		{
			return !(Object.ReferenceEquals(equipmentElement, null) || equipmentElement.IsEmpty); // IsEmpty is there if item is there. For example if you remove all body armor from character it won't have Item but still have an EquipmentElement.
		}

		public static bool IsCombinationWeapon(ItemObject item)
		{
			if (item.RelevantSkill == DefaultSkills.OneHanded)
			{
				if (item.WeaponComponent.PrimaryWeapon.IsShield)
				{
					return true;
				}else if (item.WeaponComponent.PrimaryWeapon.IsMeleeWeapon)
				{
					return true;
				}
			}
			else if (item.RelevantSkill == DefaultSkills.Bow || item.RelevantSkill == DefaultSkills.Crossbow)
			{
				if (item.WeaponComponent.PrimaryWeapon.IsAmmo || item.WeaponComponent.PrimaryWeapon.IsRangedWeapon)
				{
					return true;
				}
			}
			return false;
		}

		public static bool IsACombination(ItemObject weaponItem1, ItemObject weaponItem2)
		{
			if(Object.ReferenceEquals(weaponItem1.WeaponComponent, null) || Object.ReferenceEquals(weaponItem2.WeaponComponent, null))
			{
				return false;
			}
			SkillObject skill1 = weaponItem1.RelevantSkill;
			SkillObject skill2 = weaponItem2.RelevantSkill;
			WeaponComponentData primaryWeapon1 = weaponItem1.WeaponComponent.PrimaryWeapon;
			WeaponComponentData primaryWeapon2 = weaponItem2.WeaponComponent.PrimaryWeapon;

			if (skill1 == DefaultSkills.OneHanded && skill2 == DefaultSkills.OneHanded)
			{
				if(primaryWeapon1.IsShield && primaryWeapon2.IsMeleeWeapon)
				{
					return true;
				}else if (primaryWeapon1.IsMeleeWeapon && primaryWeapon2.IsShield)
				{
					return true;
				}
			}else if ((skill1 == DefaultSkills.Bow || skill2 == DefaultSkills.Bow) || (skill1 == DefaultSkills.Crossbow || skill2 == DefaultSkills.Crossbow))
			{
				if (primaryWeapon1.IsAmmo && primaryWeapon2.IsRangedWeapon)
				{
					return true;
				}
				else if (primaryWeapon1.IsRangedWeapon && primaryWeapon2.IsAmmo)
				{
					return true;
				}
			}
			return false;
		}

		public static bool GiveBannersByHighestLevel(ItemRoster itemRoster, List<ItemRosterElement> toGiveItemRosterElement, List<TroopRosterElement> troopRosterElementList)
		{
			List<ItemRosterElement> removeItemRosterElement = new List<ItemRosterElement>();
			List<ItemRosterElement> swappedItemRosterElement = new List<ItemRosterElement>();

			bool hasGivenBanner = false;

			foreach (TroopRosterElement troopRosterElement in troopRosterElementList)
			{
				foreach (ItemRosterElement itemRosterElement in toGiveItemRosterElement)
				{
					EquipmentElement newEquipmentElement = itemRosterElement.EquipmentElement;
					ItemObject newItem = newEquipmentElement.Item;

					DebugUtils.LogAndPrintInfo("Banner Name: " + newItem.ToString() + " Banner Level: "+ newItem.BannerComponent.BannerLevel);

					if (!IsAllowedToEquip(troopRosterElement.Character, newEquipmentElement) || itemRosterElement.Amount <= 0)
					{
						continue;
					}
					Equipment battleEquipment = troopRosterElement.Character.FirstBattleEquipment;
					EquipmentElement currentEquipmentElement = battleEquipment[EquipmentIndex.ExtraWeaponSlot];
					bool hasBanner = IsItemEquipped(currentEquipmentElement);
					bool canGive = hasBanner == false;

					if (hasBanner && newItem.BannerComponent.BannerLevel > currentEquipmentElement.Item.BannerComponent.BannerLevel)
					{
						AddEquipmentElement(swappedItemRosterElement, currentEquipmentElement);
						canGive = true;
					} 
					if (canGive)
					{
						hasGivenBanner = true;
						battleEquipment[EquipmentIndex.ExtraWeaponSlot] = newEquipmentElement;
						itemRoster.Remove(new ItemRosterElement(newEquipmentElement, 1));
						removeItemRosterElement.Add(new ItemRosterElement(newEquipmentElement, 1));
					}
				}

				if (removeItemRosterElement.Count > 0)
				{
					toGiveItemRosterElement = RemoveItemRosterElement(toGiveItemRosterElement, removeItemRosterElement);
					removeItemRosterElement = new List<ItemRosterElement>();
				}
			}

			if (swappedItemRosterElement.Count > 0)
			{
				foreach (ItemRosterElement itemRosterElement in swappedItemRosterElement)
				{
					itemRoster.Add(itemRosterElement);
				}
				GiveBannersByHighestLevel(itemRoster, swappedItemRosterElement, troopRosterElementList);
			}

			return hasGivenBanner;
		}

		public static bool GiveWeaponsBySkillAndEffectiveness(ItemRoster itemRoster, List<ItemRosterElement> toGiveItemRosterElement, List<TroopRosterElement> troopRosterElementList)
		{
			List<ItemRosterElement> removeItemRosterElement = new List<ItemRosterElement>();
			List<ItemRosterElement> swappedItemRosterElement = new List<ItemRosterElement>();

			bool hasGivenWeapon = false;

			for (int rank = 1; rank - 1 < CombatSkills.Length; rank++)
			{
				foreach (TroopRosterElement troopRosterElement in troopRosterElementList)
				{
					foreach (ItemRosterElement itemRosterElement in toGiveItemRosterElement)
					{
						EquipmentElement newEquipmentElement = itemRosterElement.EquipmentElement;
						if (!IsAllowedToEquip(troopRosterElement.Character, newEquipmentElement))
						{
							continue;
						}
	
						Equipment battleEquipment = troopRosterElement.Character.FirstBattleEquipment;

						ItemObject newItem = newEquipmentElement.Item;

						SkillObject testSkill = GetSkillByRank(troopRosterElement.Character, rank); int testValue = troopRosterElement.Character.GetSkillValue(testSkill);
						DebugUtils.LogAndPrintInfo("Item Name: " + newItem.Name + " Relavant Skill: " + newItem.RelevantSkill + " - Effectiveness: " + newItem.Effectiveness + " Skill Rank " + rank + " : " + testSkill.Name.ToString() + " Value: " + testValue);
						 
						if (GetSkillByRank(troopRosterElement.Character, rank) != newItem.RelevantSkill)
						{
							continue;
						}

						bool isHorseItem = newItem.RelevantSkill == DefaultSkills.Riding;

						// TODO Horse is not build into functions like FindOpenWeaponSlot or GetWeaponEquipmentIndexWhere
						EquipmentIndex equipmentIndex = isHorseItem == true ? EquipmentIndex.Horse : GetWeaponEquipmentIndexWhere(battleEquipment, (item) =>
						{
							return item != null && item.RelevantSkill == newItem.RelevantSkill && !IsACombination(newItem, item);
						});
						EquipmentElement currentEquipmentElement = equipmentIndex != EquipmentIndex.None ? battleEquipment[equipmentIndex] : new EquipmentElement();

						bool hasItemBySkill = IsItemEquipped(currentEquipmentElement);
						bool canGive = false;

						if (hasItemBySkill && itemRosterElement.Amount > 0 && newItem.Effectiveness > currentEquipmentElement.Item.Effectiveness)
						{
							AddEquipmentElement(swappedItemRosterElement, currentEquipmentElement);
							canGive = true;
						}
						else if (hasItemBySkill == false && !isHorseItem)
						{
							equipmentIndex = FindOpenWeaponSlot(battleEquipment);
							canGive = equipmentIndex != EquipmentIndex.None;
							if (canGive == false)
							{
								equipmentIndex = LowestEquippedWeaponBySkillRank(battleEquipment, troopRosterElement.Character);
								currentEquipmentElement = battleEquipment[equipmentIndex]; // TODO add EquipmentIndex.None check.

								if (IsSkillGreater(newItem.RelevantSkill, currentEquipmentElement.Item.RelevantSkill, troopRosterElement.Character))
								{
									canGive = true;
									AddEquipmentElement(swappedItemRosterElement, currentEquipmentElement);
								};
							}
						}else if(hasItemBySkill == false && isHorseItem == true)
						{
							canGive = true;
						}

						if (canGive)
						{
							hasGivenWeapon = true;
							battleEquipment[equipmentIndex] = newEquipmentElement;
							itemRoster.Remove(new ItemRosterElement(newEquipmentElement, 1));
							removeItemRosterElement.Add(new ItemRosterElement(newEquipmentElement, 1));
						}
					}

					if(removeItemRosterElement.Count > 0)
					{
						toGiveItemRosterElement = RemoveItemRosterElement(toGiveItemRosterElement, removeItemRosterElement);
						removeItemRosterElement = new List<ItemRosterElement>();
					}
				}

				if (toGiveItemRosterElement.Count == 0)
				{
					break;
				}
			}

			if (swappedItemRosterElement.Count > 0)
			{
				foreach (ItemRosterElement itemRosterElement in swappedItemRosterElement)
				{
					itemRoster.Add(itemRosterElement);
				}
				GiveWeaponsBySkillAndEffectiveness(itemRoster, swappedItemRosterElement, troopRosterElementList);
			}

			return hasGivenWeapon;
		}
	}
}

// TODO 
/*
 * 
 * Make sure horse horse is given with weapons and should be first then saddle should be next Maybe split up IsAllowedToEquip for more efficiency.
 * 
 */



/* 
 * Done how will weapons work
 * 
 * Loop with the item loop through all troops or heros in TroopRosterElement
 * Then see if weapon is the troop strongest skill start by index 1 The index is the position of skills 1 would mean his strongest skill. 
		If there is already an item compare items to see most effective else if there is open slot just added to open slot (shields should not be replaced they use one handed)
			If no open slot is found find weakest skill point weapon and replace it only if the weapon effectiveness is higher 
 * Loop through all position until weapon is given in all troops/companions
 * 
 */


/*
public static SkillObject GetSkillByRank1(CharacterObject character, int rank)
{
	SkillObject skill = new SkillObject("");
	int value;
	int[] orderedSkillValues = new int[] { };

	for (int i = 0; i < rank; i++)
	{
		value = 0;
		foreach (SkillObject currentSkillObject in CombatSkills)
		{
			int currentValue = character.GetSkillValue(currentSkillObject);
			if (currentValue >= value)
			{
				if (orderedSkillValues.Length != 0)
				{
					int lastValue = orderedSkillValues[orderedSkillValues.Length];
					if (currentValue >= lastValue)
					{
						continue;
					}
				}

				value = currentValue;
				skill = currentSkillObject;
			}
		}
		if(value != 0)
		{
			orderedSkillValues.Append(value);
		}
	}
	return skill;
}
*/

/*public static void CompareItemToTroop(List<ItemRosterElement> toGiveItemRosterElement, List<TroopRosterElement> troopRosterElementList, Action<TroopRosterElement, ItemRosterElement> action)
{
	foreach (TroopRosterElement troopRosterElement in troopRosterElementList)
	{
		foreach (ItemRosterElement itemRosterElement in toGiveItemRosterElement)
		{
			EquipmentElement newEquipmentElement = itemRosterElement.EquipmentElement;
			if (!IsAllowedToEquip(troopRosterElement.Character, newEquipmentElement))
			{
				continue;
			}
			action(troopRosterElement, itemRosterElement);
		}
	}
}*/

/*
public struct SkillsDictionary
{
	public Dictionary<SkillObject, List<TroopRosterElement>> Value;
	public SkillsDictionary()
	{
		Value = new Dictionary<SkillObject, List<TroopRosterElement>>();
	}
}
*/

/*public class FightingSkills
{
	public static SkillObject OneHanded = Defautl
}*/

/*
public static SkillsDictionary CreateDictionaryBySkillName(List<TroopRosterElement> troopRosterElement)
{
	//Game.Current.ObjectManager.GetObject<SkillObject>(text)
	SkillsDictionary skilsDictionary = new SkillsDictionary();
	//SkillObject relavantSkill = item.Type == ItemObject.ItemTypeEnum.Shield;

	foreach (TroopRosterElement troop in troopRosterElement)
	{
		DebugUtils.LogAndPrintInfo("Skill name ------------> " + DefaultSkills.OneHanded.StringId);
		skilsDictionary.Value.Add(DefaultSkills.OneHanded, new List<TroopRosterElement>()
		{

		});
	}
	return skilsDictionary;
}
*/

/*
public static SkillObject GetHighestCombatSkill(CharacterObject character)
{
	SkillObject selectedSkill = CombatSkills[1];
	int highestValue = character.GetSkillValue(selectedSkill);
	for (int i = 1; i < CombatSkills.Length; i++)
	{
		SkillObject skill = CombatSkills[i];
		int nextSkillValue = character.GetSkillValue(skill);
		if (nextSkillValue > highestValue)
		{
			selectedSkill = skill;
			highestValue = nextSkillValue;
		}
	}
	return selectedSkill;
}
*/


/*foreach (ItemRosterElement itemRosterElement in toGiveItemRosterElement)
{
	leftOverItemRosterElement.Add(itemRosterElement);
};

CompareItemToTroop(toGiveItemRosterElement, troopRosterElementList, (troopRosterElement, itemRosterElement) =>
{
	Equipment battleEquipment = troopRosterElement.Character.HeroObject.BattleEquipment;
	EquipmentElement newEquipmentElement = itemRosterElement.EquipmentElement;

	if (HasItemBySkillObject(newEquipmentElement.Item, battleEquipment))
	{
		EquipmentIndex equipmentIndex = FindOpenWeaponSlot(battleEquipment);
		if(equipmentIndex != EquipmentIndex.None)
		{
			battleEquipment[equipmentIndex] = newEquipmentElement;
			itemRoster.Remove(new ItemRosterElement(newEquipmentElement, 1));
		}
	}
});*/


/*public static void GiveWeaponsBySkillAndEffectiveness(ItemRoster itemRoster, List<ItemRosterElement> toGiveItemRosterElement, List<TroopRosterElement> troopRoster)
{

	SkillsDictionary skilsDictionary = new SkillsDictionary();
	foreach (ItemRosterElement itemRosterElement in toGiveItemRosterElement)
	{
		ItemObject item = itemRosterElement.EquipmentElement.Item;
		List<TroopRosterElement> troopRosterElementList;

		bool hasElement = skilsDictionary.Value.TryGetValue(item.RelevantSkill, out troopRosterElementList);
		if (hasElement)
		{

		} else 
		{
			List<TroopRosterElement> orderedItemRosterElement = OrderBySkillValue(new List<TroopRosterElement>(), item.RelevantSkill);
			skilsDictionary.Value.Add(item.RelevantSkill, orderedItemRosterElement);
		}

		foreach(TroopRosterElement troopRosterElement in troopRosterElementList)
		{
			// TODO Compare effectiveness
			Hero heroObject = troopRosterElement.Character.HeroObject;
			Equipment battleEquipment = heroObject.BattleEquipment;
		}
	}
}*/

/*public static bool HasItemByWeaponClass(ItemObject weaponItem, Equipment equipment)
{
	// TO get more information on all weapon classes find Item.Weapon These is MBList<WeaponComponentData> in that you can find more than one WeaponClas
	List<WeaponClass> weapons = GetItemWeaponClasses(weaponItem);
	foreach(WeaponClass weaponClass in weapons)
	{
		for (EquipmentIndex i = EquipmentIndex.WeaponItemBeginSlot; i < EquipmentIndex.ExtraWeaponSlot; i++)
		{
			ItemObject item = equipment[(int)i].Item;
			if (item != null && item.PrimaryWeapon.WeaponClass == weaponItem.PrimaryWeapon.WeaponClass)
			{
				return true;
			}
		}
	}
	return false;
}*/

/*if(newItem.RelevantSkill == DefaultSkills.OneHanded && newItem.Type == ItemObject.ItemTypeEnum.Shield)
{

}
if (troopRosterElement.Character.GetSkillValue(relevantSkill)) && newItem.Effectiveness > currentItem.Effectiveness)
{

}*/

/*public static Dictionary<SkillObject, int> GetAndOrderSkillsByDescending(CharacterObject character)
{
	Dictionary<SkillObject, int> skillsOrdered = new Dictionary<SkillObject, int>();

	foreach (SkillObject skillObject in CombatSkills)
	{
		skillsOrdered.Add(skillObject, character.GetSkillValue(skillObject));
	}

	//skillsOrdered.OrderByDescending(x  => x.Value);
	DebugUtils.LogAndPrintInfo("Skill name ------------> " + DefaultSkills.OneHanded.StringId);
	foreach (SkillObject skillObject in skillsOrdered.Keys.ToList())
	{
		DebugUtils.LogAndPrintInfo(skillsOrdered[skillObject]);
	}
}*/

/*
public static List<ItemRosterElement> GiveWeaponsBySkillAndEffectivenessTest(ItemRoster itemRoster, List<ItemRosterElement> toGiveItemRosterElement, List<TroopRosterElement> troopRosterElementList)
		{
			List<ItemRosterElement> swappedItemRosterElement = new List<ItemRosterElement>();
			List<ItemRosterElement> leftOverItemRosterElement = new List<ItemRosterElement>();

			int index = 1;

			CompareItemToTroop(toGiveItemRosterElement, troopRosterElementList, (troopRosterElement, itemRosterElement) =>
			{
				EquipmentElement newEquipmentElement = itemRosterElement.EquipmentElement;

				Hero troopHero = troopRosterElement.Character.HeroObject;
				Equipment battleEquipment = troopHero.BattleEquipment;

				ItemObject newItem = newEquipmentElement.Item;
				EquipmentIndex equipmentIndex = GetItemTypeWithItemObject(newItem);
				EquipmentElement currentEquipmentElement = battleEquipment[equipmentIndex];
	
				SkillObject testSkill = GetHighestCombatSkill(troopHero.CharacterObject);
				int testValue = troopRosterElement.Character.GetSkillValue(testSkill);
				DebugUtils.LogAndPrintInfo("Item Name: " + newItem.Name + " - Effectiveness: " + newItem.Effectiveness + " Highest Skill: " + testSkill.Name.ToString() + " Value: "+testValue);

				bool isOpenSlot = Object.ReferenceEquals(currentEquipmentElement, null) || currentEquipmentElement.IsEmpty; // IsEmpty is there if item is there. For example if you remove all body armor from character it won't have Item but still have an EquipmentElement.
				bool canGive = isOpenSlot;

				if (isOpenSlot == false && itemRosterElement.Amount > 0 && GetHighestCombatSkill(troopHero.CharacterObject) == newItem.RelevantSkill && newItem.Effectiveness > currentEquipmentElement.Item.Effectiveness)
				{
					battleEquipment[equipmentIndex] = newEquipmentElement;
					itemRoster.Remove(new ItemRosterElement(newEquipmentElement, 1));
					AddEquipmentElement(swappedItemRosterElement, currentEquipmentElement);
				}
				if (canGive)
				{
					itemRoster.Remove(new ItemRosterElement(newEquipmentElement, 1));
					battleEquipment[equipmentIndex] = newEquipmentElement;
				}
				// TODO check shields they also have onehanded SkillObject
			});

			foreach (ItemRosterElement itemRosterElement in toGiveItemRosterElement)
			{
				leftOverItemRosterElement.Add(itemRosterElement);
			};

			CompareItemToTroop(toGiveItemRosterElement, troopRosterElementList, (troopRosterElement, itemRosterElement) =>
			{
				Equipment battleEquipment = troopRosterElement.Character.HeroObject.BattleEquipment;
				EquipmentElement newEquipmentElement = itemRosterElement.EquipmentElement;

				if (HasItemBySkillObject(newEquipmentElement.Item, battleEquipment))
				{
					EquipmentIndex equipmentIndex = FindOpenWeaponSlot(battleEquipment);
					if(equipmentIndex != EquipmentIndex.None)
					{
						battleEquipment[equipmentIndex] = newEquipmentElement;
						itemRoster.Remove(new ItemRosterElement(newEquipmentElement, 1));
					}
				}
			});

			if (swappedItemRosterElement.Count > 0)
			{
				swappedItemRosterElement.AddRange(GiveWeaponsBySkillAndEffectivenessTest(itemRoster, swappedItemRosterElement, troopRosterElementList));
			}

			foreach (ItemRosterElement itemRosterElement in swappedItemRosterElement)
			{
				itemRoster.Add(itemRosterElement);
			}

			return swappedItemRosterElement;
		}
	}
 */

