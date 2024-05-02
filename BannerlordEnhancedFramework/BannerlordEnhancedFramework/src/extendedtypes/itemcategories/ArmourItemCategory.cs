using BannerlordEnhancedFramework.src.utils;
using TaleWorlds.Core;

namespace BannerlordEnhancedFramework.extendedtypes.itemcategories;

public class ArmourItemCategory : ExtendedItemCategory
{
    public override string Name
    {
        get { return "Armour"; }
    }
    public override bool isType(ItemRosterElement itemRosterElement)
    {
        return EquipmentUtil.IsItemArmour(itemRosterElement.EquipmentElement.Item);
    }
}