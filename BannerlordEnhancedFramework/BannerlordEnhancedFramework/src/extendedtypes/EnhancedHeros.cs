using System.Collections.Generic;
using System.Linq;
using BannerlordEnhancedFramework.src.utils;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace BannerlordEnhancedFramework.extendedtypes;

/*
 * 		public static List<HeroEnhanced> getPartyHeroes(MobileParty mobileParty, List<BaseHeroClass> heroClasses) 
		{
			return new List<HeroEnhanced>();
		}

		public static void GiveBestEquipmentFromItemRoster()
		{

			List<HeroEnhanced> partyHeroes = getPartyHeroes(PlayerUtils.PlayerParty(), new List<BaseHeroClass> { new FighterClass() });
 */

public class HeroEnhanced
{
	private Hero hero;
	private BaseHeroClass heroClass;

	public HeroEnhanced(Hero hero, BaseHeroClass heroClass)
	{
		this.hero = hero;
		this.heroClass = heroClass;
	}
}

public enum EquipmentType
{
	Battle,
	Civilian
}

public abstract class BaseHeroClass
{
	protected Hero hero;

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
		List<ItemRosterElement> itemsByCategory = getItemsByCategories(items, mainItemCategories);
		itemsByCategory = ItemCategory.OrderItemRosterByEffectiveness(itemsByCategory);

		List<ItemRosterElement> additionItems = new List<ItemRosterElement>();
		List<ItemRosterElement> removalItems = new List<ItemRosterElement>();

		foreach (ItemRosterElement itemRosterElement in itemsByCategory)
		{
			ItemObject newItem = itemRosterElement.EquipmentElement.Item;
			Equipment equipment = equipmentType == EquipmentType.Battle ? hero.BattleEquipment : hero.CivilianEquipment;

			if (!canUseItem(itemRosterElement.EquipmentElement) || itemRosterElement.EquipmentElement.IsEmpty || itemRosterElement.Amount <= 0)
			{
				continue;
			}

			EquipmentIndex equipmentIndex = EquipmentUtil.GetItemTypeWithItemObject(newItem);
			EquipmentElement currentEquipmentElement = equipment[equipmentIndex];

			bool isOpenSlot = EquipmentUtil.IsItemEquipped(currentEquipmentElement) == false; 
			bool canReplace = false;

			if (equipmentIndex == EquipmentIndex.HorseHarness && !EquipmentUtil.HasHorseForHorseHarness(equipment[EquipmentIndex.Horse]))
			{
				continue;
			}

			if (!isOpenSlot)
			{
				if (EquipmentUtil.IsItemHorse(newItem))
				{
					if (EquipmentUtil.CalculateHorseEffectiveness(newItem) > currentEquipmentElement.Item.Effectiveness)
					{
						canReplace = true;
					}
				}
				else if (EquipmentUtil.IsItemArmour(newItem))
				{
					if (EquipmentUtil.CalculateArmourEffectiveness(newItem) > currentEquipmentElement.Item.Effectiveness)
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
		// TODO female/male checks
		ItemObject item = equipmentElement.Item;
		bool hasEnoughSkills = CharacterHelper.CanUseItemBasedOnSkill(this.hero.CharacterObject, equipmentElement);
		bool isUsable = !item.HasHorseComponent || item.HorseComponent.IsRideable;
		isUsable = isUsable && equipmentElement.IsQuestItem == false;
		return hasEnoughSkills && isUsable;
	}

	public static List<ItemRosterElement> getItemsByCategories(List<ItemRosterElement> itemList, ItemCategory byItemCategory)
	{
		return getItemsByCategories(itemList, new List<ItemCategory>() { byItemCategory });
	}

	public static List<ItemRosterElement> getItemsByCategories(List<ItemRosterElement> itemList, List<ItemCategory> byItemCategories)
	{
		return itemList.FindAll(item =>
		{
			foreach (ItemCategory itemCategory in byItemCategories)
			{
				if (itemCategory.isType(item))
				{
					return true;
				}
			}
			return false;
		});
	}
}

public class FighterClass : BaseHeroClass
{

	public FighterClass(Hero hero) : base(hero)
	{
		this.mainItemCategories = new List<ItemCategory>() {
			ItemCategory.ArmorItemCategory,
			ItemCategory.OneHandedItemCategory,
			ItemCategory.TwoHandedItemCategory,
			ItemCategory.PolearmItemCategory,
			ItemCategory.ShieldItemCategory,
			ItemCategory.SaddleItemCategory,
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
	public static readonly ItemCategory OneHandedItemCategory = new OneHandedItemCategory();
	public static readonly ItemCategory TwoHandedItemCategory = new TwoHandedItemCategory();
	public static readonly ItemCategory PolearmItemCategory = new PolearmItemCategory();
	public static readonly ItemCategory ShieldItemCategory = new ShieldItemCategory();
	public static readonly ItemCategory SaddleItemCategory = new SaddleItemCategory();

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
}

public class HorseItemCategory : ItemCategory
{
	public override bool isType(ItemRosterElement itemRosterElement)
	{
		throw new System.NotImplementedException();
	}
}

public class ArmorItemCategory : ItemCategory
{
	public override bool isType(ItemRosterElement itemRosterElement)
	{
		return EquipmentUtil.IsItemArmour(itemRosterElement.EquipmentElement.Item);
	}
}

public class WeaponItemCategory : ItemCategory
{
	public override bool isType(ItemRosterElement itemRosterElement)
	{
		return EquipmentUtil.IsItemWeapon(itemRosterElement.EquipmentElement.Item);
	}
}

public class ShieldItemCategory : WeaponItemCategory
{
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
	public override bool isType(ItemRosterElement itemRosterElement)
	{
		return base.isType(itemRosterElement) && itemRosterElement.EquipmentElement.Item.Weight < 10; // TODO change number
	}
}

public class OneHandedItemCategory : WeaponItemCategory
{
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

public class OneHandedPolearmItemCategory : WeaponItemCategory
{
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

public class SaddleItemCategory : ItemCategory
{
	public override bool isType(ItemRosterElement itemRosterElement)
	{
		return itemRosterElement.EquipmentElement.Item.Type == ItemObject.ItemTypeEnum.HorseHarness;
	}
}
 enum HorseFamilyType {
	Horse = 1,
	Camel = 2,
}
public class CamelSaddleItemCategory : SaddleItemCategory {
	public override bool isType(ItemRosterElement itemRosterElement)
	{
		return base.isType(itemRosterElement) && itemRosterElement.EquipmentElement.Item.HorseComponent.Monster.FamilyType == (int)HorseFamilyType.Camel;
	}
}

public class HorseSaddleItemCategory : SaddleItemCategory
{
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

	public void OrderByDescending()
	{
		allSkills.OrderByDescending(skill => skill.value);
	}

	public void OrderByAscending()
	{
		allSkills.OrderBy(skill => skill.value);
	}
}

public class CombatSkills : Skills
{
	public CombatSkills(Hero hero) {
		CharacterObject character = hero.CharacterObject;
		allSkills = new List<Skill>()
		{
		    new Skill(character, DefaultSkills.OneHanded),
			new Skill(character, DefaultSkills.TwoHanded),
			new Skill(character, DefaultSkills.Polearm),
			new Skill(character, DefaultSkills.Throwing),
			new Skill(character, DefaultSkills.Riding),
			new Skill(character, DefaultSkills.Crossbow),
			new Skill(character, DefaultSkills.Bow),
		};
		OrderByDescending();
	}
}