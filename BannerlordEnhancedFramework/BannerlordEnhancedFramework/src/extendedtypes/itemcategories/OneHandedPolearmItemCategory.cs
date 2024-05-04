using BannerlordEnhancedFramework.extendedtypes.itemcategories;
using TaleWorlds.Core;

namespace BannerlordEnhancedFramework.extendedtypes;

public class OneHandedPolearmItemCategory : WeaponItemCategory
{
    public override string Name
    {
        get { return "OneHanded Polearm"; }
    }
    public override bool isType(ItemRosterElement itemRosterElement)
    {
        ItemObject item = itemRosterElement.EquipmentElement.Item;
        if (base.isType(itemRosterElement) && item.PrimaryWeapon.WeaponClass == WeaponClass.OneHandedPolearm)
        {
            return true;
        }
        return false;
    }
}