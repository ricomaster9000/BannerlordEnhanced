using System;
using System.Collections.Generic;
using System.Linq;
using BannerlordEnhancedFramework.extendedtypes.itemcategories;
using BannerlordEnhancedFramework.src.extendedtypes;
using BannerlordEnhancedFramework.src.utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using static TaleWorlds.Core.Equipment;

namespace BannerlordEnhancedFramework.extendedtypes;

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