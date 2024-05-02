using BannerlordEnhancedFramework.extendedtypes.itemcategories;
using BannerlordEnhancedFramework.src.utils;
using TaleWorlds.Core;

namespace BannerlordEnhancedFramework.extendedtypes;

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