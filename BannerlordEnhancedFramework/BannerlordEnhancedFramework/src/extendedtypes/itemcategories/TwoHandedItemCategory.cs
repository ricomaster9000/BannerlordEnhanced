using TaleWorlds.Core;

namespace BannerlordEnhancedFramework.extendedtypes.itemcategories;

public class TwoHandedItemCategory : WeaponItemCategory
{
    public override string Name
    {
        get { return "TwoHanded"; }
    }
    public override bool isType(ItemRosterElement itemRosterElement)
    {
        ItemObject item = itemRosterElement.EquipmentElement.Item;
        if (base.isType(itemRosterElement))
        {
            foreach (WeaponComponentData weaponData in item.WeaponComponent.Weapons)
            {
                if (!weaponData.IsMeleeWeapon)
                {
                    continue;
                }
                switch (weaponData.WeaponClass)
                {
                    case WeaponClass.TwoHandedSword:
                    case WeaponClass.TwoHandedPolearm:
                    case WeaponClass.TwoHandedAxe:
                    case WeaponClass.TwoHandedMace:
                        return true;
                }
            }
        }
        return false;
    }
}