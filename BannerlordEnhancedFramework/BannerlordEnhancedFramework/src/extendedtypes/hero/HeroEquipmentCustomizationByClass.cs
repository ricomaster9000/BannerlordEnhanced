using System;
using System.Collections.Generic;
using BannerlordEnhancedFramework.extendedtypes.itemcategories;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace BannerlordEnhancedFramework.extendedtypes;

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