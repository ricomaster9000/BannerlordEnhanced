using System;
using System.Collections.Generic;
using System.Linq;
using BannerlordEnhancedFramework.utils;
using BannerlordEnhancedPartyRoles.src.Services;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Library.NewsManager;
using TaleWorlds.LinQuick;
using TaleWorlds.Localization;

namespace BannerlordEnhancedPartyRoles.Services
{
	public class EnhancedQuaterMasterService
	{

		public class EquipmentListData
		{	
			public List<ItemRosterElement> sortedItemRosterElement = new List<ItemRosterElement>();
			public List<ItemRosterElement> swappedItemRosterElement = new List<ItemRosterElement>();
			public List<ItemRosterElement> removeItemRosterElement = new List<ItemRosterElement>();
			public EquipmentListData(List<ItemRosterElement> sortedItemRosterElement, List<ItemRosterElement> swappedItemRosterElement, List<ItemRosterElement> removeItemRosterElement)
			{
				this.sortedItemRosterElement = sortedItemRosterElement;
				this.swappedItemRosterElement = swappedItemRosterElement;
				this.removeItemRosterElement = removeItemRosterElement;
			}
			public EquipmentListData() { }
		}

		public static Tuple<List<ItemRosterElement>, List<ItemRosterElement>> UpdateCompanionsArmour(List<ItemRosterElement> itemRosterElement, List<Hero> allCompanions)
		{
			// TaleWorlds.Core.Extensions.Shuffle(allCompanions); 

			List<ItemRosterElement> swappedItemRosterElement = new List<ItemRosterElement>();
			List<ItemRosterElement> removeItemRosterElement = new List<ItemRosterElement>();

			EquipmentListData equipmentListData;

			itemRosterElement = EnhancedQuaterMasterService.OrderByArmor(itemRosterElement);
			itemRosterElement = EnhancedQuaterMasterService.OrderItemRosterByMostEffective(itemRosterElement);
			itemRosterElement = RemoveLockedItems(itemRosterElement);

			// itemRosterElement = RemoveItemsWithFlags(itemRosterElement, ItemFlags.Civilian);

			foreach (Hero companion in allCompanions)
			{
				if (itemRosterElement.Count == 0) break;
				equipmentListData = new EquipmentListData();
				equipmentListData.sortedItemRosterElement = itemRosterElement;
				equipmentListData = EnhancedQuaterMasterService.GiveArmourBasedOnEffectiveness(equipmentListData, companion);
				itemRosterElement = equipmentListData.sortedItemRosterElement;
				swappedItemRosterElement.AddRange(equipmentListData.swappedItemRosterElement);
				removeItemRosterElement.AddRange(equipmentListData.removeItemRosterElement);
			}

			if (swappedItemRosterElement.Count > 0)
			{
				var tuple = UpdateCompanionsArmour(swappedItemRosterElement, allCompanions);
				removeItemRosterElement.AddRange(tuple.Item1);
				swappedItemRosterElement.AddRange(tuple.Item2);
			}

			return new Tuple<List<ItemRosterElement>, List<ItemRosterElement>> (removeItemRosterElement, swappedItemRosterElement);
		}

		public static EquipmentListData GiveArmourBasedOnEffectiveness(EquipmentListData equipmentListData, Hero companion)
		{
			List<ItemRosterElement> removeItemRosterElement = equipmentListData.removeItemRosterElement;
			List<ItemRosterElement> swappedItemRosterElement = equipmentListData.swappedItemRosterElement;
			List<ItemRosterElement> sortedItemRosterElement = equipmentListData.sortedItemRosterElement;

			Equipment battleEquipment = companion.BattleEquipment;

			foreach(ItemRosterElement itemRosterElement in sortedItemRosterElement)
			{
				EquipmentElement newEquipmentElement = itemRosterElement.EquipmentElement;
				ItemObject newItem = newEquipmentElement.Item;

				if (!IsAllowedToEquip(companion.CharacterObject, newEquipmentElement) || (!companion.IsFemale && IsFemaleClothing(newItem)))
				{
					continue;
				}

				EquipmentIndex equipmentIndex = GetItemTypeWithItemObject(newItem);

				if(equipmentIndex == EquipmentIndex.HorseHarness && CanEquipHorseHarness(battleEquipment, newItem) == false)
				{
					continue;
				}

				EquipmentElement currentEquipmentElement = battleEquipment[equipmentIndex];

				DebugUtils.LogAndPrintInfo("Item Name: " + itemRosterElement.EquipmentElement.Item.Name 
					+ " - Effectiveness: " + itemRosterElement.EquipmentElement.Item.Effectiveness 
					+ " Body Armor: " + itemRosterElement.EquipmentElement.GetModifiedBodyArmor()
					+ " Head Armour " + itemRosterElement.EquipmentElement.GetModifiedHeadArmor()
					+ " Leg Armour " + itemRosterElement.EquipmentElement.GetModifiedLegArmor()
					+ " Arm Armour " + itemRosterElement.EquipmentElement.GetModifiedArmArmor());

				bool isOpenSlot = WeaponsManager.IsItemEquipped(currentEquipmentElement) == false; // IsEmpty is there if item is there. For example if you remove all body armor from character it won't have Item but still have an EquipmentElement.
				bool canGive = isOpenSlot;

				/* CalculateArmourEffectiveness is added because there is I believe a bug that causes items that is same but different variantions to get same effectiveness. 
				   Although we order items based on effectiveness we then atleast check here. */
				if (isOpenSlot == false && itemRosterElement.Amount > 0)
				{
					bool isHorseHarness = newItem.Type == ItemObject.ItemTypeEnum.HorseHarness;
					float effectiveness = newItem.HasArmorComponent && !isHorseHarness ? CalculateArmourEffectiveness(newItem) : CalculateHorseOrSaddleEffectiveness(newItem);
					if (effectiveness  > currentEquipmentElement.Item.Effectiveness)
					{
						canGive = true;
						AddEquipmentElement(swappedItemRosterElement, currentEquipmentElement);
					}
				}
				if (canGive)
				{
					battleEquipment[equipmentIndex] = newEquipmentElement;
					AddEquipmentElement(removeItemRosterElement, newEquipmentElement);
				}
			}

			equipmentListData.sortedItemRosterElement = RemoveItemRosterElement(sortedItemRosterElement, removeItemRosterElement);

			return equipmentListData;
		}

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

		public static float CalculateHorseOrSaddleEffectiveness(ItemObject item)
		{
			ArmorComponent armorComponent = item.ArmorComponent;
			HorseComponent horseComponent = item.HorseComponent;
			float result;
			if (item.Type == ItemObject.ItemTypeEnum.HorseHarness)
			{
				result = (float)armorComponent.BodyArmor * 1.67f;
			}else
			{
				result = ((float)(horseComponent.ChargeDamage * horseComponent.Speed + horseComponent.Maneuver * horseComponent.Speed) + (float)horseComponent.BodyLength * item.Weight * 0.025f) * (float)(horseComponent.HitPoints + horseComponent.HitPointBonus) * 0.0001f;
			}
			return result;
		}
		public static List<ItemRosterElement> AddEquipmentElement(List<ItemRosterElement> targetItemRosterElement, EquipmentElement equipmentElement)
		{
			Predicate<ItemRosterElement> getItemRosterElement = (itemRE) => {
				return itemRE.EquipmentElement.IsEqualTo(equipmentElement);
			};

			int newAmount = 1;

			ItemRosterElement foundItemRosterElement = targetItemRosterElement.Find(getItemRosterElement);

			if (foundItemRosterElement.Amount > 0) {
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

		public static List<Hero> GetCompanionsHeros(MBList<TroopRosterElement> troopsRosterElement, Hero playerHero)
		{
			List<Hero> companions = new List<Hero>();

			foreach (TroopRosterElement troop in troopsRosterElement)
			{
				Hero hero = troop.Character.HeroObject;

				if (troop.Character.IsHero && hero != playerHero) companions.Add(hero);
			}

			return companions;
		}

		public static List<TroopRosterElement> GetCompanionsTroopRosterElement(MBList<TroopRosterElement> troopsRosterElement, Hero playerHero)
		{
			List<TroopRosterElement> companions = new List<TroopRosterElement>();

			foreach (TroopRosterElement troop in troopsRosterElement)
			{
				Hero hero = troop.Character.HeroObject;

				if (troop.Character.IsHero && hero != playerHero)
				{
					companions.Add(troop);
				}
			}

			return companions;
		}

		public static List<ItemRosterElement> OrderItemRosterByMostEffective(List<ItemRosterElement> itemRosterElementList)
		{
			return itemRosterElementList.OrderBy(itemRosterElement => itemRosterElement.EquipmentElement.Item.Effectiveness, new CompareEffectiveness()).ToList();
		}

		public static List<ItemRosterElement> OrderByArmor(List<ItemRosterElement> itemRosterElementList)
		{
			return itemRosterElementList.Where(IsItemArmour).ToList();
		}
		public static List<ItemRosterElement> OrderByWeapons(List<ItemRosterElement> itemRosterElementList)
		{
			return itemRosterElementList.Where(IsItemWeapon).ToList();
		}
		public static List<ItemRosterElement> OrderByBanners(List<ItemRosterElement> itemRosterElementList)
		{
			return itemRosterElementList.Where(IsItemBanner).ToList();
		}
		public static List<ItemRosterElement> OrderByCondition(List<ItemRosterElement> itemRosterElementList, Func<ItemRosterElement, bool> condition)
		{
			return itemRosterElementList.Where(condition).ToList();
		}

		public static List<ItemRosterElement> OrderBannersByLevel(List<ItemRosterElement> itemRosterElementList)
		{
			return itemRosterElementList.OrderByDescending(itemRosterElement => itemRosterElement.EquipmentElement.Item.BannerComponent.BannerLevel).ToList();
		}

		public static List<TroopRosterElement> OrderByHeros(List<TroopRosterElement> troopRosterElementist)
		{
			return troopRosterElementist.Where((troopRosterElement) => {
				return troopRosterElement.Character.IsHero;
			}).ToList();
		}
		public static List<TroopRosterElement> OrderByCompanions(List<TroopRosterElement> troopRosterElementMBList)
		{
			return troopRosterElementMBList.Where((troopRosterElement) => {
				return troopRosterElement.Character.IsHero && troopRosterElement.Character.HeroObject.IsPlayerCompanion;
			}).ToList();
		}


		public static List<ItemRosterElement> RemoveLockedItems(List<ItemRosterElement> itemRosterElementList)
		{
			return itemRosterElementList.Where((currentItemRosterElement) => {
				return IsItemLocked(currentItemRosterElement.EquipmentElement) == false;
			}).ToList();
		}

		public static List<ItemRosterElement> RemoveItemsWithFlags(List<ItemRosterElement> itemRosterElementList, ItemFlags itemFlags)
		{
			return itemRosterElementList.Where((currentItemRosterElement) => {
				return currentItemRosterElement.EquipmentElement.Item.ItemFlags.HasAnyFlag(itemFlags) == false;
			}).ToList();
		}

		// Maybe there is beter way to do this but so far I don't find IsLocked on ItemObject from SPItemVM
		// TODO to improve performance maybe let's make iViewDataTracker a static field.
		public static bool IsItemLocked(EquipmentElement equipmentElement)
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

		public static bool IsFemaleClothing(ItemObject item)
		{
			TextObject name = item.Name;
			if (name.Contains("Dress") || item.StringId.Contains("female") || name.Contains("Female") || name.Contains("Ladies"))
			{
				return true;
			}
			return false;
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

		public static bool IsItemArmour(ItemRosterElement itemRosterElement)
		{
			ItemObject item = itemRosterElement.EquipmentElement.Item;
			return (item.HasArmorComponent) ? true : false;
		}

		public static bool IsItemWeapon(ItemRosterElement itemRosterElement)
		{
			ItemObject item = itemRosterElement.EquipmentElement.Item;
			return (item.HasWeaponComponent && item.IsBannerItem == false) ? true : false;
		}

		public static bool IsItemBanner(ItemRosterElement itemRosterElement)
		{
			ItemObject item = itemRosterElement.EquipmentElement.Item;
			return (item.IsBannerItem) ? true : false;
		}

		public static bool IsItemHorse(ItemRosterElement itemRosterElement)
		{
			ItemObject item = itemRosterElement.EquipmentElement.Item;
			return (item.HasHorseComponent) ? true : false;
		}

		public static bool IsAllowedToEquip(CharacterObject character, EquipmentElement equipmentElement)
		{
			ItemObject item = equipmentElement.Item;
			bool hasEnoughSkills = CharacterHelper.CanUseItemBasedOnSkill(character, equipmentElement);
			bool isUsable = !item.HasHorseComponent || item.HorseComponent.IsRideable;
			isUsable = isUsable && equipmentElement.IsQuestItem == false;
			return hasEnoughSkills && isUsable;
		}

		public static bool CanEquipHorseHarness(Equipment equipment, ItemObject item)
		{
			EquipmentElement equipmentElement = equipment[EquipmentIndex.Horse];
			bool hasHorseEquipped = WeaponsManager.IsItemEquipped(equipmentElement);
			if (hasHorseEquipped && equipmentElement.Item.HorseComponent.Monster.FamilyType == item.ArmorComponent.FamilyType)
			{
				return true;
			}
			return false;
		}

		// TODO add as util in framework
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
		public static bool IsQuaterMaster()
		{
			if (Campaign.Current.ConversationManager.OneToOneConversationCharacter == Campaign.Current.MainParty.EffectiveQuartermaster.CharacterObject)
			{
				return true;
			}
			return false;
		}
	}

	public class CompareEffectiveness : IComparer<float>
	{
		// Sorts by highest value first
		public int Compare(float x, float y)
		{
			return y.CompareTo(x);
		}
	}
}







/*foreach(string id in lockIds) {
	DebugUtils.LogAndPrintInfo("Lock string ID: " + id);
}*/










// private readonly List<ItemRosterElement> playersItemRosterElement;
// private readonly List<ItemRosterElement> swappedItemRosterElement;

/* 		private class MyTuple<T1, T2>
		{
			public (T1, T2) Tuple { get; private set; }

			public MyTuple(T1 t1, T2 t2)
			{
				Tuple = (t1, t2);
			}
		}
*/

/*
 * 		public static List<ItemRosterElement> OrderItemRosterByMostEffective(List<ItemRosterElement> itemRosterElementList)
		{
			return (List<ItemRosterElement>)itemRosterElementList.OrderBy(itemRosterElement => itemRosterElement.EquipmentElement.Item.Effectiveness, new CompareEffectiveness());
		}

		public static List<ItemRosterElement> OrderByArmorHorseAndSaddle(List<ItemRosterElement> itemRosterElementList)
		{
			List<ItemRosterElement> sortedItemRosterElement = itemRosterElementList.Where(SortByArmorWeaponsHorseAndSaddle).ToList();
			return sortedItemRosterElement;
		}
*/

// Notes
// SPInventoryVM.IsItemEquipmentPossible -> use this to see if character is strong enough to carry item
// I found the code to see get EquipmentIndex from ItemObject go to ItemVM.GetItemTypeWithItemObject
// ItemMenuVM.SetItem()
// Equiptment.GetEquiptmentFromSlot(EquiptmentIndex equiptmentIndex) -- gets EquiptmentElement of item from equiptmentIndex 

/*
 * 
 * 	public static IOrderedEnumerable<ItemRosterElement> SortItemRosterByMostEffective(ItemRoster itemRoster)
		{
			return itemRoster.OrderBy(itemRosterElement => itemRosterElement.EquipmentElement.Item.Effectiveness, new CompareEffectiveness());
		}
 * 
 */

/*				if (Object.ReferenceEquals(currentEquipmentElement, null) || currentEquipmentElement.IsEmpty)
				{
					hasGiven = true;
					battleEquipment[equipmentIndex] = newEquipmentElement;
					if (decrement(itemRosterElement)) toRemove.Add(itemRosterElement);
				}
				else if (newItem.Effectiveness > currentEquipmentElement.Item.Effectiveness)
				{
					battleEquipment[equipmentIndex] = newEquipmentElement;
					if (decrement(itemRosterElement)) toRemove.Add(itemRosterElement);
					// TODO make it since ItemRosterElement can contain more than one of same item should only remove item based on amount.
					companionsItemRosterElement.Add(new ItemRosterElement(currentEquipmentElement, 1));
				}*/


/*
 * 
 * 		public static List<EquipmentElement> RemoveItems(List<EquipmentElement> removeEquipmentElement, List<ItemRosterElement> sortedItemRosterElement)
		{
			IEnumerator<ItemRosterElement> iEnumerator = sortedItemRosterElement.GetEnumerator();
			while (iEnumerator.MoveNext())
			{
				ItemRosterElement itemRosterElement = iEnumerator.Current;
				foreach (EquipmentElement equipmentElement in removeEquipmentElement)
				{
					if (itemRosterElement.EquipmentElement.IsEqualTo(equipmentElement) == false)
					{
						continue;
					}
					else if(itemRosterElement.Amount > 0)
					{
						itemRosterElement.Amount -= 1;
					}
					if (itemRosterElement.Amount == 0)
					{
						itemRosterElement.Clear();
					}
				}
			}
		}


			while (iEnumerator.MoveNext())
			{
				ItemRosterElement itemRosterElement = iEnumerator.Current;
				EquipmentElement newEquipmentElement = itemRosterElement.EquipmentElement;

				if (!IsAllowedToEquip(companion.CharacterObject, newEquipmentElement))
				{
					continue;
				}
				
				ItemObject newItem = newEquipmentElement.Item;
				EquipmentIndex equipmentIndex = GetItemTypeWithItemObject(newItem);
				EquipmentElement currentEquipmentElement = battleEquipment[equipmentIndex];

				DebugUtils.LogAndPrintInfo("Item Name: " + itemRosterElement.EquipmentElement.Item.Name + " - Effectiveness: " + itemRosterElement.EquipmentElement.Item.Effectiveness + " Modified Body Armor: " + itemRosterElement.EquipmentElement.GetModifiedBodyArmor());

				bool isOpenSlot = Object.ReferenceEquals(currentEquipmentElement, null) || currentEquipmentElement.IsEmpty; // IsEmpty is there if item is there. For example if you remove all body armor from character it won't have Item but still have an EquipmentElement.
				bool canGive = isOpenSlot;
				if (isOpenSlot == false && itemRosterElement.Amount > 0 && newItem.Effectiveness > currentEquipmentElement.Item.Effectiveness)
				{
					canGive = true;
					swappedItemRosterElement.Add(new ItemRosterElement(currentEquipmentElement, 1));
				}
				if (canGive)
				{
					InformationManager.DisplayMessage(new InformationMessage(companion.Name + " received: " + newItem.Name, Color.White));
					battleEquipment[equipmentIndex] = newEquipmentElement;
					itemRosterElement.Amount -= 1;

					removeEquipmentElement.Add(currentEquipmentElement);
				}

*/

/*
 * 


			List<ItemRosterElement> removeItemRosterElement = new List<ItemRosterElement>();

			sortedItemRosterElement.ForEach(itemRosterElement =>
			{
				foreach (EquipmentElement equipmentElement in removeEquipmentElement)
				{
					if (itemRosterElement.EquipmentElement.IsEqualTo(equipmentElement) == false)
					{
						continue;
					}
					if (itemRosterElement.Amount > 0)
					{
						itemRosterElement.Amount -= 1;
					}
					if (itemRosterElement.Amount == 0)
					{
						removeItemRosterElement.Add(itemRosterElement);
					}
				}
			});

			foreach (ItemRosterElement itemRosterElement in removeItemRosterElement)
			{
				sortedItemRosterElement.Remove(itemRosterElement);
			}
*/

/*
 	public class EquipmentListData
		{	
			public List<ItemRosterElement> sortedItemRosterElement = new List<ItemRosterElement>();
			public List<ItemRosterElement> swappedItemRosterElement = new List<ItemRosterElement>();
			public List<ItemRosterElement> removeItemRosterElement = new List<ItemRosterElement>();
			public EquipmentListData(List<ItemRosterElement> sortedItemRosterElement, List<ItemRosterElement> swappedItemRosterElement, List<ItemRosterElement> removeItemRosterElement)
			{
				this.sortedItemRosterElement = sortedItemRosterElement;
				this.swappedItemRosterElement = swappedItemRosterElement;
				this.removeItemRosterElement = removeItemRosterElement;
			}
			public EquipmentListData() { }
		}
		public static List<ItemRosterElement> GiveBestArmor(List<ItemRosterElement> itemRosterElement, List<Hero> allCompanions)
		{
			TaleWorlds.Core.Extensions.Shuffle(allCompanions);

			List<ItemRosterElement> swappedItemRosterElement = new List<ItemRosterElement>();
			List<ItemRosterElement> removeItemRosterElement = new List<ItemRosterElement>();

			EquipmentListData equipmentListData = new EquipmentListData();

			foreach (Hero companion in allCompanions)
			{
				IOrderedEnumerable<ItemRosterElement> orderedItemRosterElement = EnhancedQuaterMasterService.OrderItemRosterByMostEffective(itemRosterElement);
				equipmentListData.sortedItemRosterElement = EnhancedQuaterMasterService.OrderByArmorHorseAndSaddle(orderedItemRosterElement);
				equipmentListData = EnhancedQuaterMasterService.GiveArmourBasedOnEffectiveness(equipmentListData, companion);
				itemRosterElement = equipmentListData.sortedItemRosterElement;
				swappedItemRosterElement.AddRange(equipmentListData.swappedItemRosterElement);
				removeItemRosterElement.AddRange(equipmentListData.removeItemRosterElement);
			}

			if (swappedItemRosterElement.Count > 0)
			{
				removeItemRosterElement.AddRange(GiveBestArmor(swappedItemRosterElement, allCompanions));
			}

			return removeItemRosterElement;
		}

		public static EquipmentListData GiveArmourBasedOnEffectiveness(EquipmentListData equipmentListData, Hero companion)
		{
			List<ItemRosterElement> removeItemRosterElement = equipmentListData.removeItemRosterElement;
			List<ItemRosterElement> swappedItemRosterElement = equipmentListData.swappedItemRosterElement;
			List<ItemRosterElement> sortedItemRosterElement = equipmentListData.sortedItemRosterElement;

			Equipment battleEquipment = companion.BattleEquipment;

			InformationManager.DisplayMessage(new InformationMessage("Quatermaster updated companions inventory.", Colors.Yellow));

			IEnumerator<ItemRosterElement> iEnumerator = sortedItemRosterElement.GetEnumerator();

			while (iEnumerator.MoveNext())
			{
				ItemRosterElement itemRosterElement = iEnumerator.Current;
				EquipmentElement newEquipmentElement = itemRosterElement.EquipmentElement;

				if (!IsAllowedToEquip(companion.CharacterObject, newEquipmentElement))
				{
					continue;
				}
				
				ItemObject newItem = newEquipmentElement.Item;
				EquipmentIndex equipmentIndex = GetItemTypeWithItemObject(newItem);
				EquipmentElement currentEquipmentElement = battleEquipment[equipmentIndex];

				DebugUtils.LogAndPrintInfo("Item Name: " + itemRosterElement.EquipmentElement.Item.Name + " - Effectiveness: " + itemRosterElement.EquipmentElement.Item.Effectiveness + " Modified Body Armor: " + itemRosterElement.EquipmentElement.GetModifiedBodyArmor());

				bool isOpenSlot = Object.ReferenceEquals(currentEquipmentElement, null) || currentEquipmentElement.IsEmpty; // IsEmpty is there if item is there. For example if you remove all body armor from character it won't have Item but still have an EquipmentElement.
				bool canGive = isOpenSlot;
				if (isOpenSlot == false && itemRosterElement.Amount > 0 && newItem.Effectiveness > currentEquipmentElement.Item.Effectiveness)
				{
					canGive = true;
					swappedItemRosterElement.Add(new ItemRosterElement(currentEquipmentElement, 1));
				}
				if (canGive)
				{
					InformationManager.DisplayMessage(new InformationMessage(companion.Name + " received: " + newItem.Name, Color.White));
					battleEquipment[equipmentIndex] = newEquipmentElement;

					Predicate<ItemRosterElement> getItemRosterElement = itemRE =>
					{
						return itemRE.EquipmentElement.IsEqualTo(newEquipmentElement);
					};
					// Does not remove Amount value because it is value type it gets copied after getting Find.
					ItemRosterElement foundItemRosterElement = removeItemRosterElement.Find(getItemRosterElement);
					if (foundItemRosterElement.Amount > 0)
					{
						foundItemRosterElement.Amount += 1;
					}
					else
					{
						removeItemRosterElement.Add(new ItemRosterElement(newEquipmentElement, 1));
					}
				}
			}

			//List<ItemRosterElement> cloneSortedItemRosterElement = new List<ItemRosterElement>();
			//sortedItemRosterElement.CopyTo(cloneSortedItemRosterElement);

			//List<ItemRosterElement> copiedSortedItemRosterElement = new List<ItemRosterElement>(sortedItemRosterElement);

			for(int i = 0; i < sortedItemRosterElement.Count; i++)
			{
				ItemRosterElement itemRosterElement = sortedItemRosterElement[i];

				foreach (ItemRosterElement toRemove in removeItemRosterElement)
				{
					if (itemRosterElement.EquipmentElement.IsEqualTo(toRemove.EquipmentElement))
					{
						itemRosterElement.Amount -= toRemove.Amount;
						if (itemRosterElement.Amount == 0)
						{
							sortedItemRosterElement[i].Clear();
						}
						break;
					}
				}
			}
			sortedItemRosterElement.ForEach(itemRosterElement =>
			{
				foreach (ItemRosterElement toRemove in removeItemRosterElement)
				{
					if (itemRosterElement.EquipmentElement.IsEqualTo(toRemove.EquipmentElement))
					{
						itemRosterElement.Amount -= toRemove.Amount;
						if (itemRosterElement.Amount == 0)
						{
							itemRosterElement.Clear();
						}
						break;
					}
				}
			});

return equipmentListData;
		}


		public class MockItemRosterElement
		{
			EquipmentElement equipmentElement;
			int Amount;
			public MockItemRosterElement(EquipmentElement equipmentElement, int amount)
			{
				this.equipmentElement = equipmentElement;
				Amount = amount;
			}
		}

 */ 