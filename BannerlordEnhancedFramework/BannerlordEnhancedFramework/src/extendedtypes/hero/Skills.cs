using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BannerlordEnhancedFramework.src.utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace BannerlordEnhancedFramework.src.extendedtypes
{
	public struct Skill
	{
		public int value { get; set; }
		public SkillObject skillObject { get; set; }

		public Skill(int skillValue, SkillObject skillObject)
		{
			value = skillValue;
			this.skillObject = skillObject;
		}
	}

	public abstract class Skills
	{
		public List<Skill> allSkills = new List<Skill>();

		public Skills()
		{

		}

		public void OrderByDescending()
		{
			allSkills.OrderByDescending(skill => skill.value);
		}

		public void OrderByAscending()
		{
			allSkills.OrderBy(skill => skill.value);
		}

		public bool CompareSkillObjects(Hero hero, SkillObject skillObject1, SkillObject skillObject2)
		{
			return EquipmentUtil.CompareSkillObjects(hero.CharacterObject, skillObject1, skillObject2);
		}
	}
}
