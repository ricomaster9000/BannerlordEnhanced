using TaleWorlds.Core;

namespace BannerlordEnhancedFramework.extendedtypes.itemcategories;

public class PolearmItemCategory : WeaponItemCategory
{
    public override string Name
    {
        get { return "Polearm"; }
    }
    public override bool isType(ItemRosterElement itemRosterElement)
    {
        ItemObject item = itemRosterElement.EquipmentElement.Item;
        if (base.isType(itemRosterElement))
        {
            foreach (WeaponComponentData weaponData in item.WeaponComponent.Weapons)
            {
                if (weaponData.IsMeleeWeapon && weaponData.IsPolearm)
                {
                    return true;
                }
            }
        }
        return false;
    }
}