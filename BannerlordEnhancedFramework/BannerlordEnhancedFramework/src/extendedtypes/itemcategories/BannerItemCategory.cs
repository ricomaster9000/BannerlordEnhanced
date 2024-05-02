using BannerlordEnhancedFramework.src.utils;
using TaleWorlds.Core;

namespace BannerlordEnhancedFramework.extendedtypes.itemcategories;

public class BannerItemCategory : ExtendedItemCategory
{
    public override string Name
    {
        get { return "Banner"; }
    }
    public override bool isType(ItemRosterElement itemRosterElement)
    {
        return EquipmentUtil.IsItemBanner(itemRosterElement.EquipmentElement.Item);
    }
}