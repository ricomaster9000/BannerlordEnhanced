using BannerlordEnhancedFramework.src.utils;
using TaleWorlds.Core;

namespace BannerlordEnhancedFramework.extendedtypes;

public class NonConsumableGoodsItemCategory : GoodsItemCategory
{
    public override string Name
    {
        get { return "Non Consumable Goods"; }
    }
    public override bool isType(ItemRosterElement itemRosterElement)
    {
        return base.isType(itemRosterElement) && !EquipmentUtil.IsItemFood(itemRosterElement.EquipmentElement.Item);
    }
}