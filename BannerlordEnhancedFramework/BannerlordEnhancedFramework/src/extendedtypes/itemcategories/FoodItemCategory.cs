using BannerlordEnhancedFramework.extendedtypes.itemcategories;
using BannerlordEnhancedFramework.src.utils;
using TaleWorlds.Core;

namespace BannerlordEnhancedFramework.extendedtypes;

public class FoodItemCategory : ExtendedItemCategory
{
    public override string Name
    {
        get { return "Food"; }
    }
    public override bool isType(ItemRosterElement itemRosterElement)
    {
        return EquipmentUtil.IsItemFood(itemRosterElement.EquipmentElement.Item);
    }
}