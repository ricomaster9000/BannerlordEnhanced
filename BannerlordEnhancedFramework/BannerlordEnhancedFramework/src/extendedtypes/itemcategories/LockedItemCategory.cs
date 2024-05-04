using BannerlordEnhancedFramework.src.utils;
using TaleWorlds.Core;

namespace BannerlordEnhancedFramework.extendedtypes.itemcategories;

public class LockedItemCategory : ExtendedItemCategory
{
    public override string Name
    {
        get { return "Locked Item"; }
    }
    public override bool isType(ItemRosterElement itemRosterElement)
    {
        return EquipmentUtil.IsEquipmentElementLocked(itemRosterElement.EquipmentElement);
    }
}