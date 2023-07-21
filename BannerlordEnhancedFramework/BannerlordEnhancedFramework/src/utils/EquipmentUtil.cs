using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.Library;

namespace BannerlordEnhancedFramework.src.utils
{
	public static class EquipmentUtil
	{
		public static bool IsItemArmour(ItemObject item)
		{
			return (item.HasArmorComponent) ? true : false;
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
			return (item.HasHorseComponent) ? true : false;
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

		// IsEmpty is there if item is there. For example if you remove all body armor from character it won't have Item but still have an EquipmentElement.
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
		public static bool CanEquipHorseHarness(Equipment equipment, ItemObject item)
		{
			EquipmentElement equipmentElement = equipment[EquipmentIndex.Horse];
			if (HasHorseForHorseHarness(equipmentElement) && equipmentElement.Item.HorseComponent.Monster.FamilyType == item.ArmorComponent.FamilyType)
			{
				return true;
			}
			return false;
		}

		public static bool HasHorseForHorseHarness(EquipmentElement equipmentElement)
		{
			bool hasHorseEquipped = IsItemEquipped(equipmentElement);
			return hasHorseEquipped ? true : false;
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

		public static List<ItemRosterElement> RemoveLockedItems(List<ItemRosterElement> itemRosterElementList)
		{
			return itemRosterElementList.Where((currentItemRosterElement) => {
				return IsEquipmentElementLocked(currentItemRosterElement.EquipmentElement) == false;
			}).ToList();
		}

		/* CalculateArmourEffectiveness is added because there is I believe a bug that causes items that is same but different variantions to get same effectiveness. 
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

		public static EquipmentIndex GetItemTypeWithItemObject(ItemObject item)
		{
			if (item == null)
			{
				return EquipmentIndex.None;
			}
			ItemObject.ItemTypeEnum type = item.Type;
			switch (type)
			{
				case ItemObject.ItemTypeEnum.Horse:
					return EquipmentIndex.ArmorItemEndSlot;
				case ItemObject.ItemTypeEnum.OneHandedWeapon:
				case ItemObject.ItemTypeEnum.TwoHandedWeapon:
				case ItemObject.ItemTypeEnum.Polearm:
				case ItemObject.ItemTypeEnum.Bow:
				case ItemObject.ItemTypeEnum.Crossbow:
				case ItemObject.ItemTypeEnum.Thrown:
				case ItemObject.ItemTypeEnum.Goods:
					break;
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
				default:
					switch (type)
					{
						case ItemObject.ItemTypeEnum.Cape:
							return EquipmentIndex.Cape;
						case ItemObject.ItemTypeEnum.HorseHarness:
							return EquipmentIndex.HorseHarness;
						case ItemObject.ItemTypeEnum.Banner:
							return EquipmentIndex.ExtraWeaponSlot;
					}
					break;
			}
			if (item.WeaponComponent != null)
			{
				return EquipmentIndex.WeaponItemBeginSlot;
			}
			return EquipmentIndex.None;
		}

	}
}
