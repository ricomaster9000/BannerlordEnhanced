using TaleWorlds.Core;

namespace BannerlordEnhancedFramework.extendedtypes.itemcategories;

public class SaddleItemCategory : ExtendedItemCategory
{
    public override string Name
    {
        get { return "Saddle"; }
    }
    public override bool isType(ItemRosterElement itemRosterElement)
    {
        return itemRosterElement.EquipmentElement.Item.Type == ItemObject.ItemTypeEnum.HorseHarness;
    }
}