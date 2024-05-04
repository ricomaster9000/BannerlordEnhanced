using BannerlordEnhancedFramework.extendedtypes.itemcategories;
using TaleWorlds.Core;

namespace BannerlordEnhancedFramework.extendedtypes;

public class HorseSaddleItemCategory : SaddleItemCategory
{
    public override string Name
    {
        get { return "Horse Saddle"; }
    }
    public override bool isType(ItemRosterElement itemRosterElement)
    {
        return base.isType(itemRosterElement) && itemRosterElement.EquipmentElement.Item.HorseComponent.Monster.FamilyType == (int)HorseFamilyType.Horse;
    }
}