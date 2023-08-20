using System;
using System.Collections.Generic;
using System.Linq;
using BannerlordEnhancedFramework.extendedtypes;
using BannerlordEnhancedFramework.src.utils;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.ViewModelCollection.Inventory;
using TaleWorlds.Core;
using TaleWorlds.Library;
using static TaleWorlds.Core.Equipment;

namespace BannerlordEnhancedFramework.extendedtypes;

public abstract class HeroEquipmentCustomization
{
	public EquipmentType equipmentType = EquipmentType.Battle;
	public HeroEquipmentCustomization() { }
	public virtual (List<ItemRosterElement> removals, List<ItemRosterElement> additions) customizeAndAssignEquipment(List<ItemRosterElement> items, List<ItemCategory> itemCategories, Hero hero)
	{
		items = ItemCategory.OrderItemRosterByEffectiveness(items);

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
			EquipmentIndex equipmentIndex = EquipmentUtil.GetItemTypeWithItemObject(newItem);
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

	public virtual List<ItemRosterElement> removeEquipment(List<ItemCategory> itemCategories, Hero hero, Predicate<EquipmentElement> canRemove = null)
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
	public static List<ItemRosterElement> getItemsByCategories(List<ItemRosterElement> itemList, ItemCategory byItemCategory)
	{
		return getItemsByCategories(itemList, new List<ItemCategory>() { byItemCategory });
	}

	public static List<ItemRosterElement> getItemsByCategories(List<ItemRosterElement> itemList, List<ItemCategory> byItemCategories)
	{
		return itemList.FindAll(itemRosterElement =>
		{
			foreach (ItemCategory itemCategory in byItemCategories)
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
	public override (List<ItemRosterElement> removals, List<ItemRosterElement> additions) customizeAndAssignEquipment(List<ItemRosterElement> items, List<ItemCategory> itemCategories, Hero hero)
	{
		items = getItemsByCategories(items, itemCategories);
		return base.customizeAndAssignEquipment(items, itemCategories, hero);
	}

	// Remove equipment that is item is not by this class
	public override List<ItemRosterElement> removeEquipment(List<ItemCategory> itemCategories, Hero hero, Predicate<EquipmentElement> canRemove = null)
	{
		Predicate<EquipmentElement> canRemoveEquipment = (equipmentElement) =>
		{
			if (canRemove != null && canRemove(equipmentElement) == true)
			{
				return true;
			}
			bool shouldRemove = true;
			foreach (ItemCategory itemCategory in itemCategories)
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
	public override (List<ItemRosterElement> removals, List<ItemRosterElement> additions) customizeAndAssignEquipment(List<ItemRosterElement> items, List<ItemCategory> itemCategories, Hero hero)
	{
		if (this.cultureCode != CultureCode.AnyOtherCulture)
		{
			items = getItemsByCulture(items, this.cultureCode);
		}
		return base.customizeAndAssignEquipment(items, itemCategories, hero);
	}
	// Remove equipment item that is this class culture
	public override List<ItemRosterElement> removeEquipment(List<ItemCategory> itemCategories, Hero hero, Predicate<EquipmentElement> canRemove = null)
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
	protected string heroClass;
	public HeroEquipmentCustomization heroEquipmentCustomization;

	protected List<ItemCategory> mainItemCategories = new List<ItemCategory>();
	protected Skills skills;

	public List<ItemCategory> MainItemCategories
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
		this.mainItemCategories = new List<ItemCategory>() {
			ItemCategory.ArmorItemCategory,
			ItemCategory.WeaponItemCategory,
			ItemCategory.SaddleItemCategory,
			ItemCategory.MountItemCategory,
			ItemCategory.BannerItemCategory,
		};
		this.skills = new CombatSkills(hero);
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
		this.mainItemCategories = new List<ItemCategory>() {
			ItemCategory.MountItemCategory,
		};

		this.skills = new CombatSkills(hero);
	}
	public override bool isClass(Hero hero)
	{
		return true;
	}
}

public abstract class ItemCategory
{
	public static readonly ItemCategory ArmorItemCategory = new ArmorItemCategory();
	public static readonly ItemCategory WeaponItemCategory = new WeaponItemCategory();
	public static readonly ItemCategory SaddleItemCategory = new SaddleItemCategory();
	public static readonly ItemCategory MountItemCategory = new MountItemCategory();
	public static readonly ItemCategory RangedWeaponItemCategory = new RangedWeaponItemCategory();
	public static readonly ItemCategory BannerItemCategory = new BannerItemCategory();
	public static readonly ItemCategory ShieldItemCategory = new ShieldItemCategory();
	public static readonly ItemCategory OneHandedItemCategory = new OneHandedItemCategory();
	public static readonly ItemCategory TwoHandedItemCategory = new TwoHandedItemCategory();
	public static readonly ItemCategory PolearmItemCategory = new PolearmItemCategory();

	public abstract string Name { get; }
	public abstract bool isType(ItemRosterElement itemRosterElement);

	public enum OrderByEffectiveness
	{
		MOST_EFFECTIVE,
		LEAST_EFFECTIVE,
		ABOVE_AVERAGE_EFFECTIVE
	}

	public static List<ItemRosterElement> OrderItemRosterByEffectiveness(List<ItemRosterElement> itemRosterElementList, OrderByEffectiveness orderByEffectiveness = OrderByEffectiveness.MOST_EFFECTIVE)
	{
		switch (orderByEffectiveness)
		{
			case OrderByEffectiveness.MOST_EFFECTIVE:
				return itemRosterElementList.OrderByDescending(itemRosterElement => itemRosterElement.EquipmentElement.Item.Effectiveness).ToList();

			case OrderByEffectiveness.LEAST_EFFECTIVE:
				return itemRosterElementList.OrderBy(itemRosterElement => itemRosterElement.EquipmentElement.Item.Effectiveness).ToList();
		}
		return itemRosterElementList.OrderBy(itemRosterElement => itemRosterElement.EquipmentElement.Item.Effectiveness).ToList();
	}
	public static List<string> AddItemCategoryNamesFromItemList(List<ItemRosterElement> itemList, List<ItemCategory> itemCategories, List<string> categoryNames)
	{
		foreach (ItemRosterElement itemRosterElement in itemList)
		{
			foreach (ItemCategory itemCategory in itemCategories)
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
}

public class MountItemCategory : ItemCategory
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

public class ArmorItemCategory : ItemCategory
{
	public override string Name
	{
		get { return "Armor"; }
	}
	public override bool isType(ItemRosterElement itemRosterElement)
	{
		return EquipmentUtil.IsItemArmour(itemRosterElement.EquipmentElement.Item);
	}
}

public class WeaponItemCategory : ItemCategory
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

public class SaddleItemCategory : ItemCategory
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
public class BannerItemCategory : ItemCategory
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

public class LightArmorItemCategory : ArmorItemCategory
{
	public override string Name
	{
		get { return "LightArmor"; }
	}
	public override bool isType(ItemRosterElement itemRosterElement)
	{
		return base.isType(itemRosterElement) && itemRosterElement.EquipmentElement.Item.Weight < 10; // TODO change number
	}
}

// TODO shield must also fall under OneHandedItemCategory
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

public struct Skill
{
	public int value { get; set; }
	public SkillObject skillObject { get; set; }

	public Skill(CharacterObject character, SkillObject skillObject)
	{
		value = character.GetSkillValue(skillObject);
		this.skillObject = skillObject;
	}
}

public abstract class Skills
{
	public List<Skill> allSkills = new List<Skill>();

	protected Hero hero;

	public Skills(Hero hero)
	{
		this.hero = hero;
	}

	public void OrderByDescending()
	{
		allSkills.OrderByDescending(skill => skill.value);
	}

	public void OrderByAscending()
	{
		allSkills.OrderBy(skill => skill.value);
	}

	public bool CompareSkillObjects(SkillObject skillObject1, SkillObject skillObject2)
	{
		return EquipmentUtil.CompareSkillObjects(hero.CharacterObject, skillObject1, skillObject2);
	}
}

public class CombatSkills : Skills
{
	public CombatSkills(Hero hero): base(hero) {
		CharacterObject character = hero.CharacterObject;
		allSkills = new List<Skill>()
		{
		    new Skill(character, DefaultSkills.OneHanded),
			new Skill(character, DefaultSkills.TwoHanded),
			new Skill(character, DefaultSkills.Polearm),
			new Skill(character, DefaultSkills.Throwing),
			new Skill(character, DefaultSkills.Crossbow),
			new Skill(character, DefaultSkills.Bow),
		};
		OrderByDescending();
	}
}

/*
 public override List<ItemRosterElement> removeEquipment(List<ItemCategory> itemCategories, Hero hero, Predicate<EquipmentElement> canRemove = null)
{
	Predicate<EquipmentElement> canRemoveEquipment = (equipmentElement) =>
	{
		bool shouldRemove = canRemove != null ? canRemove(equipmentElement) : true;
		foreach (ItemCategory itemCategory in itemCategories)
		{
			if(itemCategory.isType(new ItemRosterElement(equipmentElement.Item)))
			{
				shouldRemove = false;
				break;
			}
		}
		return shouldRemove;
	};
	return base.removeEquipment(itemCategories, hero, canRemoveEquipment);
}*/

// Battania Culture
/*
public static readonly ItemCategory BattaniaArmorItemCategory = new BattaniaArmorItemCategory();
public static readonly ItemCategory BattaniaWeaponItemCategory = new BattaniaWeaponItemCategory();
public static readonly ItemCategory BattaniaSaddleItemCategory = new BattaniaSaddleItemCategory();
public static readonly ItemCategory BattaniaMountItemCategory = new BattaniaMountItemCategory();
public static readonly ItemCategory BattaniaRangedWeaponItemCategory = new BattaniaRangedWeaponItemCategory();
public static readonly ItemCategory BattaniaBannerItemCategory = new BattaniaBannerItemCategory();
*/


/*
public class BattaniaArmorItemCategory : ArmorItemCategory
{
	public override string Name
	{
		get { return "Battania Armor"; }
	}
	public override bool isType(ItemRosterElement itemRosterElement)
	{
		return base.isType(itemRosterElement) && itemRosterElement.EquipmentElement.Item.Culture != null && itemRosterElement.EquipmentElement.Item.Culture.GetCultureCode() == CultureCode.Battania;
	}
}
public class BattaniaWeaponItemCategory : WeaponItemCategory
{
	public override string Name
	{
		get { return "Battania Weapon"; }
	}
	public override bool isType(ItemRosterElement itemRosterElement)
	{
		return base.isType(itemRosterElement) && itemRosterElement.EquipmentElement.Item.Culture != null && itemRosterElement.EquipmentElement.Item.Culture.GetCultureCode() == CultureCode.Battania;
	}
}
public class BattaniaMountItemCategory : MountItemCategory
{
	public override string Name
	{
		get { return "Battania Mount"; }
	}
	public override bool isType(ItemRosterElement itemRosterElement)
	{
		return base.isType(itemRosterElement) && itemRosterElement.EquipmentElement.Item.Culture != null && itemRosterElement.EquipmentElement.Item.Culture.GetCultureCode() == CultureCode.Battania;
	}
}

public class BattaniaBannerItemCategory : BannerItemCategory
{
	public override string Name
	{
		get { return "Battania Banner"; }
	}
	public override bool isType(ItemRosterElement itemRosterElement)
	{
		return base.isType(itemRosterElement) && itemRosterElement.EquipmentElement.Item.Culture != null && itemRosterElement.EquipmentElement.Item.Culture.GetCultureCode() == CultureCode.Battania;
	}
}

public class BattaniaSaddleItemCategory : SaddleItemCategory
{
	public override string Name
	{
		get { return "Battania Saddle"; }
	}
	public override bool isType(ItemRosterElement itemRosterElement)
	{
		return base.isType(itemRosterElement) && itemRosterElement.EquipmentElement.Item.Culture != null && itemRosterElement.EquipmentElement.Item.Culture.GetCultureCode() == CultureCode.Battania;
	}
}

public class BattaniaRangedWeaponItemCategory : RangedWeaponItemCategory
{
	public override string Name
	{
		get { return "Battania Ranged Weapon"; }
	}
	public override bool isType(ItemRosterElement itemRosterElement)
	{
		return base.isType(itemRosterElement) && itemRosterElement.EquipmentElement.Item.Culture != null && itemRosterElement.EquipmentElement.Item.Culture.GetCultureCode() == CultureCode.Battania;
	}
}
*/



/*
public class BattaniaClass : FighterClass
{
	public BattaniaClass(Hero hero) : base(hero)
	{
		cultureCode = CultureCode.Battania;
	}
	public override bool isClass(Hero hero)
	{
		return true;
	}
}*/

/*
public class BattaniaClass : BaseHeroClass
{
	public BattaniaClass(Hero hero) : base(hero)
	{
		this.mainItemCategories = new List<ItemCategory>() {
			ItemCategory.BattaniaArmorItemCategory,
			ItemCategory.BattaniaWeaponItemCategory,
			ItemCategory.BattaniaSaddleItemCategory,
			ItemCategory.BattaniaMountItemCategory,
			ItemCategory.BattaniaBannerItemCategory,
		};

		this.skills = new CombatSkills(hero);
	}
	public override bool isClass(Hero hero)
	{
		return true;
	}
}*/

/*
Skill primarSkill = this.skills.allSkills.FirstOrDefault();
				if (primarSkill.skillObject == newItem.RelevantSkill)
				{

				}
				if (newItem.Effectiveness < currentEquipmentElement.Item.Effectiveness)
{
	continue;
}*/

/*
public abstract class BaseHeroClass
{
	protected Hero hero;
	protected string heroClass;
	protected HeroEquipmentCustomization heroEquipmentCustomization;
	// protected CultureCode cultureCode = CultureCode.AnyOtherCulture;

	protected List<ItemCategory> mainItemCategories = new List<ItemCategory>();
	protected Skills skills;
	public BaseHeroClass(Hero hero)
	{
		this.hero = hero;
	}
	public abstract bool isClass(Hero hero);


	public (List<ItemRosterElement> removals, List<ItemRosterElement> additions) assignCivilianEquipment(List<ItemRosterElement> items)
	{
		items = EquipmentUtil.FilterByItemFlags(items, ItemFlags.Civilian);
		return assignEquipment(items, EquipmentType.Civilian);
	}

	public (List<ItemRosterElement> removals, List<ItemRosterElement> additions) assignBattleEquipment(List<ItemRosterElement> items)
	{
		return assignEquipment(items, EquipmentType.Battle);
	}
	private (List<ItemRosterElement> removals, List<ItemRosterElement> additions) assignEquipment(List<ItemRosterElement> items, EquipmentType equipmentType)
	{
		if (this.cultureCode != CultureCode.AnyOtherCulture)
		{
			items = getItemsByCulture(items, this.cultureCode);
		}
		items = getItemsByCategories(items, mainItemCategories);
		items = ItemCategory.OrderItemRosterByEffectiveness(items);

		List<ItemRosterElement> additionItems = new List<ItemRosterElement>();
		List<ItemRosterElement> removalItems = new List<ItemRosterElement>();

		foreach (ItemRosterElement itemRosterElement in items)
		{
			ItemObject newItem = itemRosterElement.EquipmentElement.Item;
			Equipment equipment = equipmentType == EquipmentType.Battle ? hero.BattleEquipment : hero.CivilianEquipment;

			if (newItem == null || !canUseItem(itemRosterElement.EquipmentElement) || itemRosterElement.EquipmentElement.IsEmpty || itemRosterElement.Amount <= 0)
			{
				continue;
			}

			EquipmentIndex equipmentIndex = EquipmentUtil.GetItemTypeWithItemObject(newItem);
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
				}else if (!hasItemBySkill)
				{
					equipmentIndex = EquipmentUtil.GetOpenWeaponEquipmentIndex(equipment);
					isOpenSlot = equipmentIndex != EquipmentIndex.None;
					if (!isOpenSlot)
					{
						equipmentIndex = EquipmentUtil.GetLowestWeaponEquipmentIndexBySkillRank(equipment, this.hero.CharacterObject);
						currentEquipmentElement = equipment[equipmentIndex]; 

						if (this.skills.CompareSkillObjects(newItem.RelevantSkill, currentEquipmentElement.Item.RelevantSkill))
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

	public bool canUseItem(EquipmentElement equipmentElement)
	{
		ItemObject item = equipmentElement.Item;
		bool canUseByGender = EquipmentUtil.CanUseItemByGender(this.hero.IsFemale, item);
		if (canUseByGender == false) return false;

		bool hasEnoughSkills = CharacterHelper.CanUseItemBasedOnSkill(this.hero.CharacterObject, equipmentElement);
		if (hasEnoughSkills == false) return false;

		bool isUsable = !item.HasHorseComponent || item.HorseComponent.IsRideable;
		isUsable = isUsable && equipmentElement.IsQuestItem == false;
		if (isUsable == false) return false;

		return true;
	}

	public static List<ItemRosterElement> getItemsByCategories(List<ItemRosterElement> itemList, ItemCategory byItemCategory)
	{
		return getItemsByCategories(itemList, new List<ItemCategory>() { byItemCategory });
	}

	public static List<ItemRosterElement> getItemsByCategories(List<ItemRosterElement> itemList, List<ItemCategory> byItemCategories)
	{
		return itemList.FindAll(itemRosterElement =>
		{
			foreach (ItemCategory itemCategory in byItemCategories)
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

	public List<string> AddItemCategoryNamesFromItemList(List<ItemRosterElement> itemList, List<string> categoryNames)
	{
		foreach (ItemRosterElement itemRosterElement in itemList)
		{
			foreach (ItemCategory itemCategory in this.mainItemCategories)
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
}
*/