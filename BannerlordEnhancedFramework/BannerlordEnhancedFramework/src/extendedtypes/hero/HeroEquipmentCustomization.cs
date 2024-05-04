using System;
using System.Collections.Generic;
using BannerlordEnhancedFramework.extendedtypes.itemcategories;
using BannerlordEnhancedFramework.src.utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

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
    public static List<ItemRosterElement> getItemsByCategories(List<ItemRosterElement> itemList, ExtendedItemCategory byItemCategories)
    {
        return getItemsByCategories(itemList, new List<ExtendedItemCategory>() { byItemCategories });
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