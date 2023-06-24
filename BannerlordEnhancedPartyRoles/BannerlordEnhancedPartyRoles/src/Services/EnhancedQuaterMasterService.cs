using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BannerlordEnhancedFramework.utils;
using TaleWorlds.CampaignSystem;
using Helpers;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.ViewModelCollection.Inventory;
using TaleWorlds.Core;
using TaleWorlds.LinQuick;
using TaleWorlds.Localization;
using TaleWorlds.Library.NewsManager;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.CampaignSystem.ViewModelCollection.CharacterDeveloper;
using System.Windows.Forms;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using static BannerlordEnhancedPartyRoles.Services.EnhancedQuaterMasterService;
using System.Collections;

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
		public static List<ItemRosterElement> GiveBestArmor(List<ItemRosterElement> itemRosterElement, List<Hero> allCompanions)
		{
			TaleWorlds.Core.Extensions.Shuffle(allCompanions);

			List<ItemRosterElement> swappedItemRosterElement = new List<ItemRosterElement>();
			List<ItemRosterElement> removeItemRosterElement = new List<ItemRosterElement>();

			foreach (Hero companion in allCompanions)
			{
				IOrderedEnumerable<ItemRosterElement> orderedItemRosterElement = EnhancedQuaterMasterService.OrderItemRosterByMostEffective(itemRosterElement);
				List<ItemRosterElement> sortedItemRosterElement = EnhancedQuaterMasterService.KeepArmorHorseAndSaddle(orderedItemRosterElement);
				EquipmentListData equipmentListData = EnhancedQuaterMasterService.GiveItemBasedOnEffectiveness(sortedItemRosterElement, companion);
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

		public static EquipmentListData GiveItemBasedOnEffectiveness(List<ItemRosterElement> sortedItemRosterElement, Hero companion)
		{
			List<ItemRosterElement> removeItemRosterElement = new List<ItemRosterElement>();
			List<ItemRosterElement> swappedItemRosterElement = new List<ItemRosterElement>(); 

			Equipment battleEquipment = companion.BattleEquipment;
			IEnumerator<ItemRosterElement> iEnumerator = sortedItemRosterElement.GetEnumerator();

			InformationManager.DisplayMessage(new InformationMessage("Quatermaster updated companions inventory.", Colors.Yellow));

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
					// itemRosterElement.Amount -= 1; // maybe not needed

					Predicate<ItemRosterElement> getItemRosterElement = itemRE =>
					{
						return itemRE.EquipmentElement.IsEqualTo(newEquipmentElement);
					};
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
			sortedItemRosterElement.ForEach(itemRosterElement =>
			{
				foreach (ItemRosterElement toRemove in removeItemRosterElement)
				{
					if (itemRosterElement.IsEqualTo(toRemove))
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

			return new EquipmentListData(sortedItemRosterElement, swappedItemRosterElement, removeItemRosterElement);
		}

		public static List<Hero> GetCompanions(MBList<TroopRosterElement> troopsRosterElement, Hero playerHero)
		{
			List<Hero> companions = new List<Hero>();

			foreach (TroopRosterElement troop in troopsRosterElement)
			{
				Hero hero = troop.Character.HeroObject;

				if (troop.Character.IsHero && hero != playerHero) companions.Add(hero);
			}

			return companions;
		}

		public static IOrderedEnumerable<ItemRosterElement> OrderItemRosterByMostEffective(List<ItemRosterElement> itemRosterElementList)
		{
			return itemRosterElementList.OrderBy(itemRosterElement => itemRosterElement.EquipmentElement.Item.Effectiveness, new CompareEffectiveness());
		}

		public static List<ItemRosterElement> KeepArmorHorseAndSaddle(IOrderedEnumerable<ItemRosterElement> itemRosterElementList)
		{
			List<ItemRosterElement> sortedItemRosterElement = itemRosterElementList.Where(IsItemArmorHorseOrSaddle).ToList();
			return sortedItemRosterElement;
		}

		public static bool IsItemArmorHorseOrSaddle(ItemRosterElement itemRosterElement) {
			ItemObject item = itemRosterElement.EquipmentElement.Item;
			return (item.HasArmorComponent || item.HasHorseComponent || item.HasSaddleComponent) ? true : false;
		}

		public static bool IsItemWeapon(ItemRosterElement itemRosterElement)
		{
			ItemObject item = itemRosterElement.EquipmentElement.Item;
			return (item.HasWeaponComponent) ? true : false;
		}

		public static bool IsAllowedToEquip(CharacterObject character, EquipmentElement equipmentElement)
		{
			ItemObject item = equipmentElement.Item;
			bool hasEnoughSkills = CharacterHelper.CanUseItemBasedOnSkill(character, equipmentElement);
			bool isUsable = !item.HasHorseComponent || item.HorseComponent.IsRideable;

			return hasEnoughSkills && isUsable;
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

		public static List<ItemRosterElement> KeepArmorHorseAndSaddle(List<ItemRosterElement> itemRosterElementList)
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