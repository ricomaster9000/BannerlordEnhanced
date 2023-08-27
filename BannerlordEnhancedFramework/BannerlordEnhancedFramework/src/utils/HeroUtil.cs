using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BannerlordEnhancedFramework.extendedtypes;
using BannerlordEnhancedFramework.src.extendedtypes;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.ViewModelCollection.CharacterDeveloper;
using TaleWorlds.Core;

namespace BannerlordEnhancedFramework.src.utils
{
	internal class HeroUtil
	{
		public static List<Skill> GetCombatSkills(Hero hero)
		{
			return new List<Skill>()
			{
			new Skill(hero.CharacterObject.GetSkillValue(DefaultSkills.OneHanded), DefaultSkills.OneHanded),
			new Skill(hero.CharacterObject.GetSkillValue(DefaultSkills.TwoHanded), DefaultSkills.TwoHanded),
			new Skill(hero.CharacterObject.GetSkillValue(DefaultSkills.Polearm), DefaultSkills.Polearm),
			new Skill(hero.CharacterObject.GetSkillValue(DefaultSkills.Throwing), DefaultSkills.Throwing),
			new Skill(hero.CharacterObject.GetSkillValue(DefaultSkills.Crossbow), DefaultSkills.Crossbow),
			new Skill(hero.CharacterObject.GetSkillValue(DefaultSkills.Bow), DefaultSkills.Bow),
		};
		}
	}
}
