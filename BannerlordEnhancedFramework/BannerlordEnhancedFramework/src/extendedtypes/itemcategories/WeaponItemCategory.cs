using BannerlordEnhancedFramework.src.utils;
using TaleWorlds.Core;

namespace BannerlordEnhancedFramework.extendedtypes.itemcategories;

public class WeaponItemCategory : ExtendedItemCategory
{
    public override string Name
    {
        get { return "Weapon"; }
    }
    public override bool isType(ItemRosterElement itemRosterElement)
    {
        return EquipmentUtil.IsItemWeapon(itemRosterElement.EquipmentElement.Item);
    }
}