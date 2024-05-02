using BannerlordEnhancedFramework.extendedtypes.itemcategories;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace BannerlordEnhancedFramework.extendedtypes;

public class GoodsItemCategory: ExtendedItemCategory
{
    public override string Name
    {
        get { return "Goods"; }
    }
    public override bool isType(ItemRosterElement itemRosterElement)
    {
        return itemRosterElement.EquipmentElement.Item.ItemType.HasAnyFlag(ItemObject.ItemTypeEnum.Goods);
    }
}