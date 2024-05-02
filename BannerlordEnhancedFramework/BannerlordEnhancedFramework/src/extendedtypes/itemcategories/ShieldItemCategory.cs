using TaleWorlds.Core;

namespace BannerlordEnhancedFramework.extendedtypes.itemcategories;

public class ShieldItemCategory : WeaponItemCategory
{
    public override string Name
    {
        get { return "Shield"; }
    }
    public override bool isType(ItemRosterElement itemRosterElement)
    {
        ItemObject item = itemRosterElement.EquipmentElement.Item;
        if (base.isType(itemRosterElement))
        {
            switch (item.PrimaryWeapon.WeaponClass)
            {
                case WeaponClass.SmallShield:
                case WeaponClass.LargeShield:
                    return true;
            }
        }
        return false;
    }
}