using BannerlordEnhancedFramework.extendedtypes.itemcategories;
using TaleWorlds.Core;

namespace BannerlordEnhancedFramework.extendedtypes;

public class CamelSaddleItemCategory : SaddleItemCategory {
    public override string Name
    {
        get { return "Camel"; }
    }
    public override bool isType(ItemRosterElement itemRosterElement)
    {
        return base.isType(itemRosterElement) && itemRosterElement.EquipmentElement.Item.HorseComponent.Monster.FamilyType == (int)MountFamilyType.Camel;
    }
}