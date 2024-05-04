using BannerlordEnhancedFramework.src.utils;
using TaleWorlds.Core;

namespace BannerlordEnhancedFramework.extendedtypes.itemcategories;

public class BodyArmourItemCategory : ArmourItemCategory
{
    public override string Name
    {
        get { return "Body Armour"; }
    }
    public override bool isType(ItemRosterElement itemRosterElement)
    {
        return base.isType(itemRosterElement) && !EquipmentUtil.IsItemSaddle(itemRosterElement.EquipmentElement.Item);
    }
}