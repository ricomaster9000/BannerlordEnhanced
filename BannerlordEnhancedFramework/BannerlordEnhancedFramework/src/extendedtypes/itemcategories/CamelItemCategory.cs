using BannerlordEnhancedFramework.extendedtypes.itemcategories;
using BannerlordEnhancedFramework.src.utils;
using TaleWorlds.Core;

namespace BannerlordEnhancedFramework.extendedtypes;

public class CamelItemCategory : ExtendedItemCategory
{
    public override string Name
    {
        get { return "Camel"; }
    }
    public override bool isType(ItemRosterElement itemRosterElement)
    {
        return EquipmentUtil.IsItemCamel(itemRosterElement.EquipmentElement.Item);
    }
}