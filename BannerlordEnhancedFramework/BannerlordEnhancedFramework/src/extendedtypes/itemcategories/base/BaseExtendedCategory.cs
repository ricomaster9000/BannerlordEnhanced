using System.Collections.Generic;
using System.Linq;
using BannerlordEnhancedFramework.src.utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace BannerlordEnhancedFramework.extendedtypes.itemcategories;

public abstract partial class ExtendedItemCategory
{
	
	public abstract string Name { get; }
	public abstract bool isType(ItemRosterElement itemRosterElement);

	public static List<ItemRosterElement> OrderItemRosterByEffectiveness(List<ItemRosterElement> itemRosterElementList, OrderByEffectiveness orderByEffectiveness = OrderByEffectiveness.MOST_EFFECTIVE)
	{
		switch (orderByEffectiveness)
		{
			case OrderByEffectiveness.MOST_EFFECTIVE:
				return itemRosterElementList.OrderByDescending(itemRosterElement => itemRosterElement.EquipmentElement.Item.Effectiveness).ToList();
			case OrderByEffectiveness.ABOVE_AVERAGE_EFFECTIVE:
			default:
				return itemRosterElementList.OrderBy(itemRosterElement => itemRosterElement.EquipmentElement.Item.Effectiveness).ToList();
		}
	}
	public static List<ItemRosterElement> OrderItemRoster(List<ItemRosterElement> itemRosterElementList, OrderBy orderBy)
	{
		switch (orderBy) {
			case OrderBy.HEAVIEST_TO_LIGHTEST:
				return itemRosterElementList.OrderByDescending(itemRosterElement => itemRosterElement.EquipmentElement.Item.Weight).ToList();
			case OrderBy.LIGHTEST_TO_HEAVIEST:
				return itemRosterElementList.OrderBy(itemRosterElement => itemRosterElement.EquipmentElement.Item.Weight).ToList();
			default:
				return itemRosterElementList;
		}
	}
	public static Dictionary<string, int> GetItemCategoryToTotalWorthForCultureAndCategoryFromItems(
		List<ItemRosterElement> itemList,
		List<ExtendedItemCategory> itemCategories,
		ExtendedCultureCode cultureCode
	)
	{
		Dictionary<string, int> result = new Dictionary<string, int>();
		foreach (ItemRosterElement itemRosterElement in itemList)
		{
			foreach (ExtendedItemCategory itemCategory in itemCategories)
			{
				if (itemCategory.isType(itemRosterElement) == false || !cultureCode.IsItemOfCulture(itemRosterElement))
				{
					continue;
				}

				string itemCategoryToNameKey = cultureCode.getName() + " " + itemCategory.Name;
				if (result.ContainsKey(itemCategoryToNameKey))
				{
					result[itemCategoryToNameKey] += itemRosterElement.Amount;
				} else
				{
					result.Add(itemCategoryToNameKey, itemRosterElement.Amount);
				}
			}
		}
		return result;
	}
	
	public static Dictionary<string, int> GetAllItemCategoryNamesByItemsAndCategories(List<ItemRosterElement> itemList, List<ExtendedItemCategory> itemCategories, Dictionary<string, int> categoryNames)
	{
		foreach (ItemRosterElement itemRosterElement in itemList)
		{
			foreach (ExtendedItemCategory itemCategory in itemCategories)
			{
				if (itemCategory.isType(itemRosterElement))
				{
					if (categoryNames.ContainsKey(itemCategory.Name))
					{
						categoryNames[itemCategory.Name] += 1;
					}
					else
					{
						categoryNames.Add(itemCategory.Name, 1);
					}
					break;
				}
			}
		};
		return categoryNames;
	}
}