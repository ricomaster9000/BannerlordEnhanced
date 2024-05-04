using System.Collections.Generic;
using BannerlordEnhancedFramework.extendedtypes.itemcategories;
using BannerlordEnhancedFramework.src.extendedtypes;
using BannerlordEnhancedFramework.src.utils;
using TaleWorlds.CampaignSystem;

namespace BannerlordEnhancedFramework.extendedtypes;

public class FighterClass : BaseHeroClass
{
    public FighterClass(Hero hero, HeroEquipmentCustomization heroEquipmentCustomization) : base(hero, heroEquipmentCustomization)
    {
        this.mainItemCategories = new List<ExtendedItemCategory>() {
            ExtendedItemCategories.ArmourItemCategory,
            ExtendedItemCategories.WeaponItemCategory,
            ExtendedItemCategories.SaddleItemCategory,
            ExtendedItemCategories.MountItemCategory,
            ExtendedItemCategories.BannerItemCategory,
        };
        this.combatSkills = new CombatSkills(HeroUtil.GetCombatSkills(hero));
    }
    public override bool isClass(Hero hero)
    {
        return true;
    }

}