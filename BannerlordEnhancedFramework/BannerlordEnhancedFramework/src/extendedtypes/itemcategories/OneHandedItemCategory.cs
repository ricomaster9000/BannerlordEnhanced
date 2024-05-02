using TaleWorlds.Core;

namespace BannerlordEnhancedFramework.extendedtypes.itemcategories;

public class OneHandedItemCategory : WeaponItemCategory
{
    public override string Name
    {
        get { return "OneHanded"; }
    }
    public override bool isType(ItemRosterElement itemRosterElement)
    {
        ItemObject item = itemRosterElement.EquipmentElement.Item;
        if (base.isType(itemRosterElement))
        {
            foreach(WeaponComponentData weaponData in item.WeaponComponent.Weapons)
            {
                if (!weaponData.IsMeleeWeapon)
                {
                    continue;
                }
                switch (weaponData.WeaponClass)
                {
                    case WeaponClass.OneHandedSword:
                    case WeaponClass.OneHandedPolearm:
                    case WeaponClass.OneHandedAxe:
                        return true;
                }
            }
        }
        return false;
    }
}