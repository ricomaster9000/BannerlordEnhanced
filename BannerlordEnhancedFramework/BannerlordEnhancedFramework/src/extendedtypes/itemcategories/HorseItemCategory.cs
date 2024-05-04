using BannerlordEnhancedFramework.extendedtypes.itemcategories;
using BannerlordEnhancedFramework.src.utils;
using TaleWorlds.Core;

namespace BannerlordEnhancedFramework.extendedtypes;

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