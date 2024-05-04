using BannerlordEnhancedFramework.extendedtypes.itemcategories;
using BannerlordEnhancedFramework.src.utils;
using TaleWorlds.Core;

namespace BannerlordEnhancedFramework.extendedtypes;

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