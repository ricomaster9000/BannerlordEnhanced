using System;
using System.Collections.Generic;
using BannerlordEnhancedFramework.extendedtypes.itemcategories;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace BannerlordEnhancedFramework.extendedtypes;

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