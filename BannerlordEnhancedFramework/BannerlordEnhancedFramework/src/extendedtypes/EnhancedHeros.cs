using System;
using System.Collections.Generic;
using System.Linq;
using BannerlordEnhancedFramework.src.extendedtypes;
using BannerlordEnhancedFramework.src.utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using static BannerlordEnhancedFramework.extendedtypes.ExtendedItemCategory;
using static TaleWorlds.Core.Equipment;

namespace BannerlordEnhancedFramework.extendedtypes;

public abstract class HeroEquipmentCustomization
{
	public EquipmentType equipmentType = EquipmentType.Battle;
	public HeroEquipmentCustomization() { }
	public virtual (List<ItemRosterElement> removals, List<ItemRosterElement> additions) customizeAndAssignEquipment(List<ItemRosterElement> items, List<ExtendedItemCategory> itemCategories, Hero hero)
	{
		items = ExtendedItemCategory.OrderItemRosterByEffectiveness(items);

		List<ItemRosterElement> additionItems = new List<ItemRosterElement>();
		List<ItemRosterElement> removalItems = new List<ItemRosterElement>();

		foreach (ItemRosterElement itemRosterElement in items)
		{
			ItemObject newItem = itemRosterElement.EquipmentElement.Item;
			Equipment equipment = this.equipmentType == EquipmentType.Battle ? hero.BattleEquipment : hero.CivilianEquipment;

			if (newItem == null || !EquipmentUtil.CanUseItem(hero, itemRosterElement.EquipmentElement) || itemRosterElement.EquipmentElement.IsEmpty || itemRosterElement.Amount <= 0)
			{
				continue;
			}

			// TODO replace equipment that does match filters example player had culture code to Battania but his current equipt item is Sturgia it should be replaced
			EquipmentIndex equipmentIndex = EquipmentUtil.GetItemTypeFromItemObject(newItem);
			EquipmentElement currentEquipmentElement = equipment[equipmentIndex];

			bool isOpenSlot = EquipmentUtil.IsItemEquipped(currentEquipmentElement) == false;
			bool canReplace = false;

			if (equipmentIndex == EquipmentIndex.HorseHarness && !EquipmentUtil.CanEquipHorseHarness(equipment, newItem))
			{
				continue;
			}

			if (!isOpenSlot)
			{
				if (EquipmentUtil.IsItemHorse(newItem))
				{
					if (EquipmentUtil.CalculateHorseEffectiveness(newItem) > EquipmentUtil.CalculateHorseEffectiveness(currentEquipmentElement.Item))
					{
						canReplace = true;
					}
				}
				else if (EquipmentUtil.IsItemArmour(newItem))
				{
					// TODO Rugged heavy male vest is stronger than Fine Heavy male vest check reason should be battania faction
					if (EquipmentUtil.CalculateArmourEffectiveness(newItem) > EquipmentUtil.CalculateArmourEffectiveness(currentEquipmentElement.Item))
					{
						canReplace = true;
					}
				}
				else if (EquipmentUtil.IsItemBanner(newItem))
				{
					if (newItem.BannerComponent.BannerLevel > currentEquipmentElement.Item.BannerComponent.BannerLevel)
					{
						canReplace = true;
					}
				}
			}

			if (EquipmentUtil.IsItemWeapon(newItem))
			{
				equipmentIndex = EquipmentUtil.GetWeaponEquipmentIndexWhere(equipment, (item) => {
					return item != null && item.RelevantSkill == newItem.RelevantSkill && !EquipmentUtil.IsACombination(newItem, item);
				});
				EquipmentElement emptyEquipmentElement = new EquipmentElement();
				currentEquipmentElement = equipmentIndex != EquipmentIndex.None ? equipment[equipmentIndex] : emptyEquipmentElement;

				bool hasItemBySkill = EquipmentUtil.IsItemEquipped(currentEquipmentElement);

				// TODO check scenario Ilani dispossed has onehanded as main polearm second then dagger(onehanded) third and shield last dagger should be update with thrown
				// Remove double items
				if (hasItemBySkill && newItem.Effectiveness > currentEquipmentElement.Item.Effectiveness)
				{
					canReplace = true;
				}
				else if (!hasItemBySkill)
				{
					equipmentIndex = EquipmentUtil.GetOpenWeaponEquipmentIndex(equipment);
					isOpenSlot = equipmentIndex != EquipmentIndex.None;
					if (!isOpenSlot)
					{
						equipmentIndex = EquipmentUtil.GetLowestWeaponEquipmentIndexBySkillRank(equipment, hero.CharacterObject);
						currentEquipmentElement = equipment[equipmentIndex];

						if (EquipmentUtil.CompareSkillObjects(hero.CharacterObject, newItem.RelevantSkill, currentEquipmentElement.Item.RelevantSkill))
						{
							canReplace = true;
						};
					}
				}
			}

			if (canReplace)
			{
				additionItems = EquipmentUtil.AddEquipmentElement(additionItems, currentEquipmentElement);
			}

			if (isOpenSlot || canReplace)
			{
				equipment[equipmentIndex] = itemRosterElement.EquipmentElement;
				removalItems = EquipmentUtil.AddEquipmentElement(removalItems, itemRosterElement.EquipmentElement);
			}
		}

		return (removalItems, additionItems);
	}

	public virtual List<ItemRosterElement> removeEquipment(List<ExtendedItemCategory> itemCategories, Hero hero, Predicate<EquipmentElement> canRemove = null)
	{
		List<ItemRosterElement> additionItems = new List<ItemRosterElement>();
		Equipment equipment = equipmentType == EquipmentType.Battle ? hero.BattleEquipment : hero.CivilianEquipment;
		for (int i = 0; i < 12; i++)
		{
			EquipmentElement currentEquipmentElement = equipment[i];
			if (EquipmentUtil.IsItemEquipped(currentEquipmentElement))
			{
				if (canRemove == null || canRemove(currentEquipmentElement))
				{
					equipment[i] = new EquipmentElement();
					additionItems = EquipmentUtil.AddEquipmentElement(additionItems, currentEquipmentElement);
				}
			}
		}
		return additionItems;
	}
	public static List<ItemRosterElement> getItemsByCategories(List<ItemRosterElement> itemList, ExtendedItemCategory byItemCategory)
	{
		return getItemsByCategories(itemList, new List<ExtendedItemCategory>() { byItemCategory });
	}

	public static List<ItemRosterElement> getItemsByCategories(List<ItemRosterElement> itemList, List<ExtendedItemCategory> byItemCategories)
	{
		return itemList.FindAll(itemRosterElement =>
		{
			foreach (ExtendedItemCategory itemCategory in byItemCategories)
			{
				if (itemCategory.isType(itemRosterElement))
				{
					return true;
				}
			}
			return false;
		});
	}

	public static List<ItemRosterElement> getItemsByCulture(List<ItemRosterElement> itemList, CultureCode cultureCode)
	{
		return getItemsByCulture(itemList, new List<CultureCode>() { cultureCode });
	}
	public static List<ItemRosterElement> getItemsByCulture(List<ItemRosterElement> itemList, List<CultureCode> cultureCodes)
	{
		return itemList.FindAll(itemRosterElement =>
		{
			foreach (CultureCode cultureCode in cultureCodes)
			{
				ItemObject item = itemRosterElement.EquipmentElement.Item;
				if (item.Culture != null && item.Culture.GetCultureCode() == cultureCode)
				{
					return true;
				}
			}
			return false;
		});
	}
	public enum EquipmentType
	{
		Battle,
		Civilian
	}
}

public class HeroEquipmentCustomizationByClass : HeroEquipmentCustomization
{
	public override (List<ItemRosterElement> removals, List<ItemRosterElement> additions) customizeAndAssignEquipment(List<ItemRosterElement> items, List<ExtendedItemCategory> itemCategories, Hero hero)
	{
		items = getItemsByCategories(items, itemCategories);
		return base.customizeAndAssignEquipment(items, itemCategories, hero);
	}

	// Remove equipment that is item is not by this class
	public override List<ItemRosterElement> removeEquipment(List<ExtendedItemCategory> itemCategories, Hero hero, Predicate<EquipmentElement> canRemove = null)
	{
		Predicate<EquipmentElement> canRemoveEquipment = (equipmentElement) =>
		{
			if (canRemove != null && canRemove(equipmentElement) == true)
			{
				return true;
			}
			bool shouldRemove = true;
			foreach (ExtendedItemCategory itemCategory in itemCategories)
			{
				if (itemCategory.isType(new ItemRosterElement(equipmentElement.Item)))
				{
					shouldRemove = false;
					break;
				}
			}
			return shouldRemove;
		};
		return base.removeEquipment(itemCategories, hero, canRemoveEquipment);
	}
}
public class HeroEquipmentCustomizationByClassAndCulture : HeroEquipmentCustomizationByClass
{
	public CultureCode cultureCode;
	public HeroEquipmentCustomizationByClassAndCulture(CultureCode cultureCode)
	{
		this.cultureCode = cultureCode;
	}
	public override (List<ItemRosterElement> removals, List<ItemRosterElement> additions) customizeAndAssignEquipment(List<ItemRosterElement> items, List<ExtendedItemCategory> itemCategories, Hero hero)
	{
		if (this.cultureCode != CultureCode.AnyOtherCulture)
		{
			items = getItemsByCulture(items, this.cultureCode);
		}
		return base.customizeAndAssignEquipment(items, itemCategories, hero);
	}
	// Remove equipment item that is this class culture
	public override List<ItemRosterElement> removeEquipment(List<ExtendedItemCategory> itemCategories, Hero hero, Predicate<EquipmentElement> canRemove = null)
	{
		Predicate<EquipmentElement> canRemoveEquipment = (equipmentElement) =>
		{
			bool shouldRemove = canRemove != null ? canRemove(equipmentElement) : true;
			if (equipmentElement.Item.Culture != null && equipmentElement.Item.Culture.GetCultureCode() == this.cultureCode) shouldRemove = false;
			return shouldRemove;
		};
		return base.removeEquipment(itemCategories, hero, canRemoveEquipment);
	}
}

public abstract class BaseHeroClass
{
	protected Hero hero;
	public HeroEquipmentCustomization heroEquipmentCustomization;

	protected List<ExtendedItemCategory> mainItemCategories = new List<ExtendedItemCategory>();
	protected CombatSkills combatSkills;

	public List<ExtendedItemCategory> MainItemCategories
	{
		get { return mainItemCategories; }
	}
	public BaseHeroClass(Hero hero, HeroEquipmentCustomization heroEquipmentCustomization)
	{
		this.hero = hero;
		this.heroEquipmentCustomization = heroEquipmentCustomization;
	}
	public abstract bool isClass(Hero hero);
	public List<ItemRosterElement> removeEquipmentIfNoReplacementItemFound(List<ItemRosterElement> items)
	{
		Predicate<EquipmentElement> canRemoveEquipment = (equipmentElement) =>
		{
			if (this.heroEquipmentCustomization.GetType() == typeof(HeroEquipmentCustomizationByClass)) return false;

			bool canRemove = false;
			foreach (ItemRosterElement itemRosterElement in items)
			{
				if (itemRosterElement.EquipmentElement.Item.ItemType == equipmentElement.Item.ItemType) canRemove = true;

				if (canRemove && this.heroEquipmentCustomization is HeroEquipmentCustomizationByClassAndCulture)
				{
					HeroEquipmentCustomizationByClassAndCulture equipmentCustomization = ((HeroEquipmentCustomizationByClassAndCulture)this.heroEquipmentCustomization);
					BasicCultureObject itemCulture = itemRosterElement.EquipmentElement.Item.Culture;
					canRemove = (itemCulture != null && itemCulture.GetCultureCode() == equipmentCustomization.cultureCode) ? true : false;
					if ((int)this.heroEquipmentCustomization.equipmentType == (int)EquipmentType.Civilian)
					{
						canRemove = (canRemove && itemRosterElement.EquipmentElement.Item.ItemFlags.HasAnyFlag(ItemFlags.Civilian) == true) ? true : false;
					}
					
				}
				if (canRemove) break;
			}
			return canRemove;
		};
		return this.heroEquipmentCustomization.removeEquipment(MainItemCategories, hero, canRemoveEquipment);
	}

	public List<ItemRosterElement> removeRelavantBattleEquipment(List<ItemRosterElement> items)
	{
		this.heroEquipmentCustomization.equipmentType = HeroEquipmentCustomization.EquipmentType.Battle;
		return removeEquipmentIfNoReplacementItemFound(items);
	}
	public List<ItemRosterElement> removeRelavantCivilianEquipment(List<ItemRosterElement> items)
	{
		this.heroEquipmentCustomization.equipmentType = HeroEquipmentCustomization.EquipmentType.Civilian;
		return removeEquipmentIfNoReplacementItemFound(items);
	}
	public (List<ItemRosterElement> removals, List<ItemRosterElement> additions) assignCivilianEquipment(List<ItemRosterElement> items)
	{
		this.heroEquipmentCustomization.equipmentType = HeroEquipmentCustomization.EquipmentType.Civilian;
		items = EquipmentUtil.FilterByItemFlags(items, ItemFlags.Civilian);
		return assignEquipment(items);
	}

	public (List<ItemRosterElement> removals, List<ItemRosterElement> additions) assignBattleEquipment(List<ItemRosterElement> items)
	{
		this.heroEquipmentCustomization.equipmentType = HeroEquipmentCustomization.EquipmentType.Battle;
		return assignEquipment(items);
	}
	private (List<ItemRosterElement> removals, List<ItemRosterElement> additions) assignEquipment(List<ItemRosterElement> items)
	{
		return this.heroEquipmentCustomization.customizeAndAssignEquipment(items, this.mainItemCategories, this.hero);
	}

}

public class FighterClass : BaseHeroClass
{
	public FighterClass(Hero hero, HeroEquipmentCustomization heroEquipmentCustomization) : base(hero, heroEquipmentCustomization)
	{
		this.mainItemCategories = new List<ExtendedItemCategory>() {
			ExtendedItemCategory.ArmourItemCategory,
			ExtendedItemCategory.WeaponItemCategory,
			ExtendedItemCategory.SaddleItemCategory,
			ExtendedItemCategory.MountItemCategory,
			ExtendedItemCategory.BannerItemCategory,
		};
		this.combatSkills = new CombatSkills(HeroUtil.GetCombatSkills(hero));
	}
	public override bool isClass(Hero hero)
	{
		return true;
	}

}

public class CavalryRiderClass : BaseHeroClass
{
	public CavalryRiderClass(Hero hero, HeroEquipmentCustomization heroEquipmentCustomization) : base(hero, heroEquipmentCustomization)
	{
		this.mainItemCategories = new List<ExtendedItemCategory>() {
			ExtendedItemCategory.MountItemCategory,
		};

		this.combatSkills = new CombatSkills(HeroUtil.GetCombatSkills(hero));
	}
	public override bool isClass(Hero hero)
	{
		return true;
	}
}

public abstract class ExtendedItemCategory
{
	public static readonly ExtendedItemCategory ArmourItemCategory = new ArmourItemCategory();
	public static readonly ExtendedItemCategory BodyArmourItemCategory = new BodyArmourItemCategory();
	public static readonly ExtendedItemCategory WeaponItemCategory = new WeaponItemCategory();
	public static readonly ExtendedItemCategory SaddleItemCategory = new SaddleItemCategory();
	public static readonly ExtendedItemCategory MountItemCategory = new MountItemCategory();
	public static readonly ExtendedItemCategory RangedWeaponItemCategory = new RangedWeaponItemCategory();
	public static readonly ExtendedItemCategory BannerItemCategory = new BannerItemCategory();
	public static readonly ExtendedItemCategory ShieldItemCategory = new ShieldItemCategory();
	public static readonly ExtendedItemCategory OneHandedItemCategory = new OneHandedItemCategory();
	public static readonly ExtendedItemCategory TwoHandedItemCategory = new TwoHandedItemCategory();
	public static readonly ExtendedItemCategory PolearmItemCategory = new PolearmItemCategory();
	public static readonly ExtendedItemCategory FoodItemCategory = new FoodItemCategory();
	public static readonly ExtendedItemCategory GoodsItemCategory = new GoodsItemCategory();
	public static readonly ExtendedItemCategory NonConsumableGoodsItemCategory = new NonConsumableGoodsItemCategory();
	public static readonly ExtendedItemCategory MuleItemCategory = new MuleItemCategory();
	public static readonly ExtendedItemCategory HorseItemCategory = new HorseItemCategory();
	public static readonly ExtendedItemCategory CamelItemCategory = new CamelItemCategory();
	public static readonly ExtendedItemCategory MiscellaneousItemCategory = new MiscellaneousItemCategory();

	public abstract string Name { get; }
	public abstract bool isType(ItemRosterElement itemRosterElement);

	public enum OrderByEffectiveness
	{
		MOST_EFFECTIVE,
		LEAST_EFFECTIVE,
		ABOVE_AVERAGE_EFFECTIVE
	}
	public enum OrderByWeight
	{
		 MOST_HEAVY,
		 LEAST_HEAVY
	}

	public static List<ItemRosterElement> OrderItemRosterByEffectiveness(List<ItemRosterElement> itemRosterElementList, OrderByEffectiveness orderByEffectiveness = OrderByEffectiveness.MOST_EFFECTIVE)
	{
		switch (orderByEffectiveness)
		{
			case OrderByEffectiveness.MOST_EFFECTIVE:
				return itemRosterElementList.OrderByDescending(itemRosterElement => itemRosterElement.EquipmentElement.Item.Effectiveness).ToList();
			case OrderByEffectiveness.ABOVE_AVERAGE_EFFECTIVE:
			default:
				return itemRosterElementList.OrderBy(itemRosterElement => itemRosterElement.EquipmentElement.Item.Effectiveness).ToList();
		}
	}
	public static List<ItemRosterElement> OrderItemRoster(List<ItemRosterElement> itemRosterElementList, EquipmentUtil.OrderBy orderBy)
	{
		switch (orderBy) {
			case EquipmentUtil.OrderBy.HEAVIEST_TO_LIGHTEST:
				return itemRosterElementList.OrderByDescending(itemRosterElement => itemRosterElement.EquipmentElement.Item.Weight).ToList();
			case EquipmentUtil.OrderBy.LIGHTEST_TO_HEAVIEST:
				return itemRosterElementList.OrderBy(itemRosterElement => itemRosterElement.EquipmentElement.Item.Weight).ToList();
			default:
				return itemRosterElementList;
		}
	}
	public static Dictionary<string, int> AddItemCategoryNamesFromItemList(List<ItemRosterElement> itemList, List<ExtendedItemCategory> itemCategories, Dictionary<string, int> categories)
	{
		foreach (ItemRosterElement itemRosterElement in itemList)
		{
			foreach (ExtendedItemCategory itemCategory in itemCategories)
			{
				if (itemCategory.isType(itemRosterElement))
				{
					if (categories.ContainsKey(itemCategory.Name)) 
					{
						int amount = categories[itemCategory.Name];
						categories[itemCategory.Name] = amount + itemRosterElement.Amount;
					} else 
					{
						categories.Add(itemCategory.Name, itemRosterElement.Amount);
					}
					break;
				}
			}
		};
		return categories;
	}
}

public class MountItemCategory : ExtendedItemCategory
{
	public override string Name {
		get { return "Mount";  }
	}

	public override bool isType(ItemRosterElement itemRosterElement)
	{
		ItemObject item = itemRosterElement.EquipmentElement.Item;
		return EquipmentUtil.IsItemHorse(item) || EquipmentUtil.IsItemCamel(item);
	}
}

public class ArmourItemCategory : ExtendedItemCategory
{
	public override string Name
	{
		get { return "Armour"; }
	}
	public override bool isType(ItemRosterElement itemRosterElement)
	{
		return EquipmentUtil.IsItemArmour(itemRosterElement.EquipmentElement.Item);
	}
}

public class WeaponItemCategory : ExtendedItemCategory
{
	public override string Name
	{
		get { return "Weapon"; }
	}
	public override bool isType(ItemRosterElement itemRosterElement)
	{
		return EquipmentUtil.IsItemWeapon(itemRosterElement.EquipmentElement.Item);
	}
}

public class SaddleItemCategory : ExtendedItemCategory
{
	public override string Name
	{
		get { return "Saddle"; }
	}
	public override bool isType(ItemRosterElement itemRosterElement)
	{
		return itemRosterElement.EquipmentElement.Item.Type == ItemObject.ItemTypeEnum.HorseHarness;
	}
}
public class BannerItemCategory : ExtendedItemCategory
{
	public override string Name
	{
		get { return "Banner"; }
	}
	public override bool isType(ItemRosterElement itemRosterElement)
	{
		return EquipmentUtil.IsItemBanner(itemRosterElement.EquipmentElement.Item);
	}
}

public class ShieldItemCategory : WeaponItemCategory
{
	public override string Name
	{
		get { return "Shield"; }
	}
	public override bool isType(ItemRosterElement itemRosterElement)
	{
		ItemObject item = itemRosterElement.EquipmentElement.Item;
		if (base.isType(itemRosterElement))
		{
			switch (item.PrimaryWeapon.WeaponClass)
			{
				case WeaponClass.SmallShield:
				case WeaponClass.LargeShield:
					return true;
			}
		}
		return false;
	}
}

public class LightArmourItemCategory : ArmourItemCategory
{
	public override string Name
	{
		get { return "Light Armour"; }
	}
	public override bool isType(ItemRosterElement itemRosterElement)
	{
		return base.isType(itemRosterElement) && itemRosterElement.EquipmentElement.Item.Weight < 10; // TODO change number
	}
}

public class BodyArmourItemCategory : ArmourItemCategory
{
	public override string Name
	{
		get { return "Body Armour"; }
	}
	public override bool isType(ItemRosterElement itemRosterElement)
	{
		return base.isType(itemRosterElement) && !EquipmentUtil.IsItemSaddle(itemRosterElement.EquipmentElement.Item);
	}
}

public class OneHandedItemCategory : WeaponItemCategory
{
	public override string Name
	{
		get { return "OneHanded"; }
	}
	public override bool isType(ItemRosterElement itemRosterElement)
	{
		ItemObject item = itemRosterElement.EquipmentElement.Item;
		if (base.isType(itemRosterElement))
		{
			foreach(WeaponComponentData weaponData in item.WeaponComponent.Weapons)
			{
				if (!weaponData.IsMeleeWeapon)
				{
					continue;
				}
					switch (weaponData.WeaponClass)
				{
					case WeaponClass.OneHandedSword:
					case WeaponClass.OneHandedPolearm:
					case WeaponClass.OneHandedAxe:
						return true;
				}
			}
		}
		return false;
	}
}

public class TwoHandedItemCategory : WeaponItemCategory
{
	public override string Name
	{
		get { return "TwoHanded"; }
	}
	public override bool isType(ItemRosterElement itemRosterElement)
	{
		ItemObject item = itemRosterElement.EquipmentElement.Item;
		if (base.isType(itemRosterElement))
		{
			foreach (WeaponComponentData weaponData in item.WeaponComponent.Weapons)
			{
				if (!weaponData.IsMeleeWeapon)
				{
					continue;
				}
				switch (weaponData.WeaponClass)
				{
					case WeaponClass.TwoHandedSword:
					case WeaponClass.TwoHandedPolearm:
					case WeaponClass.TwoHandedAxe:
					case WeaponClass.TwoHandedMace:
						return true;
				}
			}
		}
		return false;
	}
}

public class PolearmItemCategory : WeaponItemCategory
{
	public override string Name
	{
		get { return "Polearm"; }
	}
	public override bool isType(ItemRosterElement itemRosterElement)
	{
		ItemObject item = itemRosterElement.EquipmentElement.Item;
		if (base.isType(itemRosterElement))
		{
			foreach (WeaponComponentData weaponData in item.WeaponComponent.Weapons)
			{
				if (weaponData.IsMeleeWeapon && weaponData.IsPolearm)
				{
					return true;
				}
			}
		}
		return false;
	}
}

public class RangedWeaponItemCategory : WeaponItemCategory
{
	public override string Name
	{
		get { return "Ranged Weapon"; }
	}
	public override bool isType(ItemRosterElement itemRosterElement)
	{
		ItemObject item = itemRosterElement.EquipmentElement.Item;
		if (base.isType(itemRosterElement))
		{
			foreach (WeaponComponentData weaponData in item.WeaponComponent.Weapons)
			{
				if (weaponData.IsRangedWeapon || weaponData.IsAmmo)
				{
					return true;
				}
			}
		}
		return false;
	}
}

public class OneHandedPolearmItemCategory : WeaponItemCategory
{
	public override string Name
	{
		get { return "OneHanded Polearm"; }
	}
	public override bool isType(ItemRosterElement itemRosterElement)
	{
		ItemObject item = itemRosterElement.EquipmentElement.Item;
		if (base.isType(itemRosterElement) && item.PrimaryWeapon.WeaponClass == WeaponClass.OneHandedPolearm)
		{
			return true;
		}
		return false;
	}
}

 enum HorseFamilyType {
	Horse = 1,
	Camel = 2,
}
public class CamelSaddleItemCategory : SaddleItemCategory {
	public override string Name
	{
		get { return "Camel"; }
	}
	public override bool isType(ItemRosterElement itemRosterElement)
	{
		return base.isType(itemRosterElement) && itemRosterElement.EquipmentElement.Item.HorseComponent.Monster.FamilyType == (int)HorseFamilyType.Camel;
	}
}

public class HorseSaddleItemCategory : SaddleItemCategory
{
	public override string Name
	{
		get { return "Horse Saddle"; }
	}
	public override bool isType(ItemRosterElement itemRosterElement)
	{
		return base.isType(itemRosterElement) && itemRosterElement.EquipmentElement.Item.HorseComponent.Monster.FamilyType == (int)HorseFamilyType.Horse;
	}
}

public class FoodItemCategory : ExtendedItemCategory
{
	public override string Name
	{
		get { return "Food"; }
	}
	public override bool isType(ItemRosterElement itemRosterElement)
	{
		return EquipmentUtil.IsItemFood(itemRosterElement.EquipmentElement.Item);
	}
}

public class GoodsItemCategory: ExtendedItemCategory
{
	public override string Name
	{
		get { return "Goods"; }
	}
	public override bool isType(ItemRosterElement itemRosterElement)
	{
		return itemRosterElement.EquipmentElement.Item.ItemType.HasAnyFlag(ItemObject.ItemTypeEnum.Goods);
	}
}

public class NonConsumableGoodsItemCategory : GoodsItemCategory
{
	public override string Name
	{
		get { return "Non Consumable Goods"; }
	}
	public override bool isType(ItemRosterElement itemRosterElement)
	{
		return base.isType(itemRosterElement) && !EquipmentUtil.IsItemFood(itemRosterElement.EquipmentElement.Item);
	}
}

public class MuleItemCategory : ExtendedItemCategory
{
	public override string Name
	{
		get { return "Mule"; }
	}
	public override bool isType(ItemRosterElement itemRosterElement)
	{
		return  EquipmentUtil.IsItemMule(itemRosterElement.EquipmentElement.Item);
	}
}

public class HorseItemCategory: ExtendedItemCategory
{
	public override string Name
	{
		get { return "Horse"; }
	}
	public override bool isType(ItemRosterElement itemRosterElement)
	{
		return EquipmentUtil.IsItemHorse(itemRosterElement.EquipmentElement.Item) && !EquipmentUtil.IsItemMule(itemRosterElement.EquipmentElement.Item);
	}
}

public class CamelItemCategory : ExtendedItemCategory
{
	public override string Name
	{
		get { return "Camel"; }
	}
	public override bool isType(ItemRosterElement itemRosterElement)
	{
		return EquipmentUtil.IsItemCamel(itemRosterElement.EquipmentElement.Item);
	}
}

public class MiscellaneousItemCategory : NonConsumableGoodsItemCategory
{ 
	public override string Name
	{
		get { return "Miscellaneous"; }
	}
	public override bool isType(ItemRosterElement itemRosterElement)
	{
		ItemObject item = itemRosterElement.EquipmentElement.Item;
		return base.isType(itemRosterElement) && !item.HasHorseComponent && !item.HasArmorComponent && !item.HasBannerComponent && !item.HasWeaponComponent;
	}
}









/*
 * 
 *	public static List<string> AddItemCategoryNamesFromItemList(List<ItemRosterElement> itemList, List<ExtendedItemCategory> itemCategories, List<string> categoryNames)
	{
		foreach (ItemRosterElement itemRosterElement in itemList)
		{
			foreach (ExtendedItemCategory itemCategory in itemCategories)
			{
				if (itemCategory.isType(itemRosterElement))
				{
					string hasName = categoryNames.Find((name) => {
						return name == itemCategory.Name;
					});
					if (hasName == null)
					{
						categoryNames.Add(itemCategory.Name);
					}
					break;
				}
			}
		};
		return categoryNames;
	}
*/