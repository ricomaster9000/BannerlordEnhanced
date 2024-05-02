using BannerlordEnhancedFramework.extendedtypes.itemcategories;
using TaleWorlds.Core;

namespace BannerlordEnhancedFramework.extendedtypes;

public class RangedWeaponItemCategory : WeaponItemCategory
{
    public override string Name
    {
        get { return "Ranged Weapon"; }
    }
    public override bool isType(ItemRosterElement itemRosterElement)
    {
        ItemObject item = itemRosterElement.EquipmentElement.Item;
        if (base.isType(itemRosterElement))
        {
            foreach (WeaponComponentData weaponData in item.WeaponComponent.Weapons)
            {
                if (weaponData.IsRangedWeapon || weaponData.IsAmmo)
                {
                    return true;
                }
            }
        }
        return false;
    }
}