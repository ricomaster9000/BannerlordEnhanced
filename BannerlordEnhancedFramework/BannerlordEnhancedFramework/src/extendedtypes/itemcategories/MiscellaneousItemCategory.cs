using TaleWorlds.Core;

namespace BannerlordEnhancedFramework.extendedtypes;

public class MiscellaneousItemCategory : NonConsumableGoodsItemCategory
{ 
    public override string Name
    {
        get { return "Miscellaneous"; }
    }
    public override bool isType(ItemRosterElement itemRosterElement)
    {
        ItemObject item = itemRosterElement.EquipmentElement.Item;
        return base.isType(itemRosterElement) && !item.HasHorseComponent && !item.HasArmorComponent && !item.HasBannerComponent && !item.HasWeaponComponent;
    }
}