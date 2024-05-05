using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.Library;
using BannerlordEnhancedFramework.extendedtypes;
using BannerlordEnhancedFramework.extendedtypes.itemcategories;
using Helpers;
using TaleWorlds.CampaignSystem.Inventory;

namespace BannerlordEnhancedFramework.src.utils
{
	public static partial class EquipmentUtil
	{
		public static bool IsItemAnimal(ItemObject item)
		{
			return item.IsAnimal;
		}
		public static bool IsItemFood(ItemObject item)
		{
			return (item.IsFood) ? true : false;
		}
		public static bool IsItemArmour(ItemObject item)
		{
			return (item.HasArmorComponent) ? true : false;
		}
		public static bool IsItemSaddle(ItemObject item)
		{
			return InventoryManager.GetInventoryItemTypeOfItem(item) == InventoryItemType.HorseHarness;
		}

		public static bool IsItemWeapon(ItemObject item)
		{
			return (item.HasWeaponComponent && item.IsBannerItem == false) ? true : false;
		}

		public static bool IsItemBanner(ItemObject item)
		{
			return (item.IsBannerItem) ? true : false;
		}

		public static bool IsItemHorse(ItemObject item)
		{
			return (item.HasHorseComponent && item.HorseComponent.Monster.FamilyType == (int)MountFamilyType.Horse) ? true : false;
		}

		public static bool IsItemCamel(ItemObject item)
		{
			return (item.HasHorseComponent && item.HorseComponent.Monster.FamilyType == (int)MountFamilyType.Camel) ? true : false;
		}
		public static bool IsItemMule(ItemObject item)
		{
			return EquipmentUtil.IsItemHorse(item) && item.HorseComponent.Monster.StringId == "mule" ? true : false;
		}

		public static bool IsFemaleClothing(ItemObject item)
		{
			TextObject name = item.Name;
			if (name.Contains("Dress") || item.StringId.Contains("female") || name.Contains("Female") || name.Contains("Ladies"))
			{
				return true;
			}
			return false;
		}

		public static bool IsCombinationWeapon(ItemObject item)
		{
			if (item.RelevantSkill == DefaultSkills.OneHanded)
			{
				if (item.WeaponComponent.PrimaryWeapon.IsShield)
				{
					return true;
				}
				else if (item.WeaponComponent.PrimaryWeapon.IsMeleeWeapon)
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
			if (Object.ReferenceEquals(weaponItem1.WeaponComponent, null) || Object.ReferenceEquals(weaponItem2.WeaponComponent, null))
			{
				return false;
			}
			SkillObject skill1 = weaponItem1.RelevantSkill;
			SkillObject skill2 = weaponItem2.RelevantSkill;
			WeaponComponentData primaryWeapon1 = weaponItem1.WeaponComponent.PrimaryWeapon;
			WeaponComponentData primaryWeapon2 = weaponItem2.WeaponComponent.PrimaryWeapon;

			if (skill1 == DefaultSkills.OneHanded && skill2 == DefaultSkills.OneHanded)
			{
				if (primaryWeapon1.IsShield && primaryWeapon2.IsMeleeWeapon)
				{
					return true;
				}
				else if (primaryWeapon1.IsMeleeWeapon && primaryWeapon2.IsShield)
				{
					return true;
				}
			}
			else if ((skill1 == DefaultSkills.Bow || skill2 == DefaultSkills.Bow) || (skill1 == DefaultSkills.Crossbow || skill2 == DefaultSkills.Crossbow))
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

		// IsEmpty is there if item is there. For example if you remove all body armor from character it won't have Item but still have an EquipmentElement.
		// If equipmentElement is null it means it is also not equipped
		public static bool IsItemEquipped(EquipmentElement equipmentElement)
		{
			return !(Object.ReferenceEquals(equipmentElement, null) || equipmentElement.IsEmpty);
		}

		// Maybe there is beter way to do this but so far I don't find IsLocked on ItemObject from SPItemVM
		// TODO to improve performance maybe let's make iViewDataTracker a static field.
		public static bool IsEquipmentElementLocked(EquipmentElement equipmentElement)
		{
			IViewDataTracker iViewDataTracker = Campaign.Current.GetCampaignBehavior<IViewDataTracker>();
			IEnumerable<string> lockIds = iViewDataTracker.GetInventoryLocks();
			string targetItemId = CampaignUIHelper.GetItemLockStringID(equipmentElement);

			foreach (string id in lockIds)
			{
				if (targetItemId == id)
				{
					return true;
				}
			}
			return false;
		}
		public static bool CompareSkillObjects(CharacterObject character, SkillObject skillObject1, SkillObject skillObject2)
		{
			return character.GetSkillValue(skillObject1) > character.GetSkillValue(skillObject2);
		}
		public static bool CanEquipHorseHarness(Equipment equipment, ItemObject item)
		{
			EquipmentElement equipmentElement = equipment[EquipmentIndex.Horse];
			return (HasHorseForHorseHarness(equipmentElement) && equipmentElement.Item.HorseComponent.Monster.FamilyType == item.ArmorComponent.FamilyType);
		}

		public static bool HasHorseForHorseHarness(EquipmentElement equipmentElement)
		{
			return IsItemEquipped(equipmentElement);
		}

		public static bool CanUseItemByGender(bool isFemale, ItemObject item)
		{
			// TODO refactore to be troopRosterElement then use TroopRosterElement in other functions bool isMale = troopRosterElement.Character.IsFemale;
			if (isFemale && item.ItemFlags.HasAnyFlag(ItemFlags.NotUsableByFemale) == false)
			{
				return true;
			}
			else if (!isFemale && item.ItemFlags.HasAnyFlag(ItemFlags.NotUsableByMale) == false)
			{
				return true;
			}
			return false;
		}

		public static bool CanUseItem(Hero hero, EquipmentElement equipmentElement)
		{
			ItemObject item = equipmentElement.Item;
			bool canUseByGender = EquipmentUtil.CanUseItemByGender(hero.IsFemale, item);
			if (canUseByGender == false) return false;

			bool hasEnoughSkills = CharacterHelper.CanUseItemBasedOnSkill(hero.CharacterObject, equipmentElement);
			if (hasEnoughSkills == false) return false;

			bool isUsable = !item.HasHorseComponent || item.HorseComponent.IsRideable;
			isUsable = isUsable && equipmentElement.IsQuestItem == false;
			if (isUsable == false) return false;

			return true;
		}

		public static List<ItemRosterElement> RemoveLockedItems(List<ItemRosterElement> itemRosterElementList)
		{
			return itemRosterElementList.Where((currentItemRosterElement) => {
				return IsEquipmentElementLocked(currentItemRosterElement.EquipmentElement) == false;
			}).ToList();
		}

		/* CalculateArmourEffectiveness is added because there is I believe a bug that causes items that is same but different variations to get same effectiveness. 
		Although we order items based on effectiveness we then atleast check here. */
		public static float CalculateArmourEffectiveness(ItemObject item)
		{
			ArmorComponent armorComponent = item.ArmorComponent;
			float result;
			if (item.Type == ItemObject.ItemTypeEnum.HorseHarness)
			{
				result = (float)armorComponent.BodyArmor * 1.67f;
			}
			else
			{
				result = ((float)armorComponent.HeadArmor * 34f + (float)armorComponent.BodyArmor * 42f + (float)armorComponent.LegArmor * 12f + (float)armorComponent.ArmArmor * 12f) * 0.03f;
			}
			return result;
		}
		public static float CalculateHorseEffectiveness(ItemObject item)
		{
			HorseComponent horseComponent = item.HorseComponent;
			return ((float)(horseComponent.ChargeDamage * horseComponent.Speed + horseComponent.Maneuver * horseComponent.Speed) + (float)horseComponent.BodyLength * item.Weight * 0.025f) * (float)(horseComponent.HitPoints + horseComponent.HitPointBonus) * 0.0001f;

		}
		public static List<ItemRosterElement> AddEquipmentElement(List<ItemRosterElement> targetItemRosterElement, EquipmentElement equipmentElement)
		{
			Predicate<ItemRosterElement> getItemRosterElement = (itemRosterElement) => {
				return itemRosterElement.EquipmentElement.IsEqualTo(equipmentElement);
			};

			int newAmount = 1;

			ItemRosterElement foundItemRosterElement = targetItemRosterElement.Find(getItemRosterElement);

			if (foundItemRosterElement.Amount > 0)
			{
				targetItemRosterElement.Remove(foundItemRosterElement);
				newAmount = foundItemRosterElement.Amount + 1;
			}

			targetItemRosterElement.Add(new ItemRosterElement(equipmentElement, newAmount));

			return targetItemRosterElement;
		}

		public static List<ItemRosterElement> RemoveItemRosterElement(List<ItemRosterElement> targetItemRosterElement, List<ItemRosterElement> toRemoveItemRosterElement)
		{
			List<ItemRosterElement> newItemRosterElement = new List<ItemRosterElement>();
			foreach (ItemRosterElement itemRosterElement in targetItemRosterElement)
			{
				bool isInRemoveList = false;
				foreach (ItemRosterElement toRemove in toRemoveItemRosterElement)
				{
					if (itemRosterElement.EquipmentElement.IsEqualTo(toRemove.EquipmentElement))
					{
						isInRemoveList = true;
						int newAmount = itemRosterElement.Amount - toRemove.Amount;
						if (newAmount > 0)
						{
							newItemRosterElement.Add(new ItemRosterElement(itemRosterElement.EquipmentElement, newAmount));
						}
						break;
					}

				}
				if (isInRemoveList == false)
				{
					newItemRosterElement.Add(new ItemRosterElement(itemRosterElement.EquipmentElement, itemRosterElement.Amount));
				}
			}
			return newItemRosterElement;
		}

		public static List<ItemRosterElement> FilterByItemFlags(List<ItemRosterElement> items, ItemFlags itemFlags)
		{
			return items.Where((itemRosterElement) =>
			{
				return itemRosterElement.EquipmentElement.Item.ItemFlags.HasAnyFlag(itemFlags);
			}).ToList();
		}
		public static List<ItemRosterElement> ExcludeByItemFlags(List<ItemRosterElement> items, ItemFlags itemFlags)
		{
			return items.Where((itemRosterElement) =>
			{
				return itemRosterElement.EquipmentElement.Item.ItemFlags.HasAnyFlag(itemFlags) == false;
			}).ToList();
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

		public static EquipmentIndex GetOpenWeaponEquipmentIndex(Equipment equipment)
		{
			return GetWeaponEquipmentIndexWhere(equipment, (item) =>
			{
				return item == null;
			});
		}

		public static EquipmentIndex GetLowestWeaponEquipmentIndexBySkillRank(Equipment equipment, CharacterObject character)
		{
			EquipmentIndex equipmentIndex = EquipmentIndex.None;
			int skillValue = Int32.MaxValue;

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

		public static EquipmentIndex GetItemTypeFromItemObject(ItemObject item)
		{
			if (item == null)
			{
				return EquipmentIndex.None;
			}
			switch (item.Type)
			{
				case ItemObject.ItemTypeEnum.Horse:
					return EquipmentIndex.ArmorItemEndSlot;
				case ItemObject.ItemTypeEnum.Arrows:
					return EquipmentIndex.WeaponItemBeginSlot;
				case ItemObject.ItemTypeEnum.Bolts:
					return EquipmentIndex.WeaponItemBeginSlot;
				case ItemObject.ItemTypeEnum.Shield:
					return EquipmentIndex.WeaponItemBeginSlot;
				case ItemObject.ItemTypeEnum.HeadArmor:
					return EquipmentIndex.NumAllWeaponSlots;
				case ItemObject.ItemTypeEnum.BodyArmor:
					return EquipmentIndex.Body;
				case ItemObject.ItemTypeEnum.LegArmor:
					return EquipmentIndex.Leg;
				case ItemObject.ItemTypeEnum.HandArmor:
					return EquipmentIndex.Gloves;
				case ItemObject.ItemTypeEnum.Cape:
					return EquipmentIndex.Cape;
				case ItemObject.ItemTypeEnum.HorseHarness:
					return EquipmentIndex.HorseHarness;
				case ItemObject.ItemTypeEnum.Banner:
					return EquipmentIndex.ExtraWeaponSlot;
				default:
					if (item.WeaponComponent != null)
					{
						return EquipmentIndex.WeaponItemBeginSlot;
					}
					return EquipmentIndex.None;
			}
		}
		
		public static List<ItemRosterElement> FilterItemRosterByItemCategories(
			List<ItemRosterElement> itemRoster,
			List<ExtendedItemCategory> itemCategories,
			OrderBy orderBy = OrderBy.HEAVIEST_TO_LIGHTEST)
		{
			List<ItemRosterElement> itemRosterElements = HeroEquipmentCustomization.getItemsByCategories(itemRoster, itemCategories);
			itemRosterElements = ExtendedItemCategory.OrderItemRoster(itemRosterElements, orderBy);
			return itemRosterElements;
		}
		
		public static List<ItemRosterElement> FilterItemRosterByItemCategoriesAndCulture(
			List<ItemRosterElement> itemRoster,
			List<ExtendedItemCategory> itemCategories,
			ExtendedCultureCode cultureCode,
			OrderBy orderBy = OrderBy.HEAVIEST_TO_LIGHTEST)
		{
			List<ItemRosterElement> itemRosterElements = HeroEquipmentCustomization.getItemsByCategories(itemRoster, itemCategories);
			List<ItemRosterElement> filterByCulture = new List<ItemRosterElement>();
			foreach (ItemRosterElement item in itemRosterElements)
			{
				if (cultureCode.IsItemOfCulture(item))
				{
					filterByCulture.Add(item);
				}
			}
			filterByCulture = ExtendedItemCategory.OrderItemRoster(filterByCulture, orderBy);
			return filterByCulture;
		}

		public static ExtendedCultureCode GetCultureCodeOfItem(ItemRosterElement item)
		{
			ExtendedCultureCode result = null;
			foreach (ExtendedCultureCode cultureCode in ExtendedCultureCode.values())
			{
				if (cultureCode.IsItemOfCulture(item))
				{
					result = cultureCode;
					break;
				}	
			}
			return result;
		}
	}
}
