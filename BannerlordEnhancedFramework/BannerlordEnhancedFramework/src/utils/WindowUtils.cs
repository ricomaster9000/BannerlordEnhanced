using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;

namespace BannerlordEnhancedFramework.utils;

public class WindowUtils
{
    public static void PopupSimpleInquiry(string title, string text, string yesText = "Ok", string noText = "No")
    {
        PopupSimpleInquiry(title, text, yesText, noText,null);
    }
    
    public static void PopupSimpleInquiry(string title, string text, Action consequenceOfClosing)
    {
        PopupSimpleInquiry(title, text, "Ok", null,consequenceOfClosing, consequenceOfClosing);
    }
    
    public static void PopupSimpleInquiry(string title, string text, string yesText, string noText, Action consequenceOfClosing)
    {
        PopupSimpleInquiry(title, text, yesText, noText,consequenceOfClosing, consequenceOfClosing);
    }
    
    public static void PopupSimpleInquiry(string title, string text, string yesText, string noText, Action affirmativeAction, Action negativeAction)
    {
        InquiryData inquiryData = new InquiryData(
            title,
            text,
            true,
			yesText != null ? true : false,
            yesText,
            noText,
            affirmativeAction,
            negativeAction
        );
        InformationManager.ShowInquiry(inquiryData, true);
    }

    public static void DisplayMessageListNameAndTotal(Dictionary<string, int> categoriesDetails, string startLineMessage)
    {
        foreach (KeyValuePair<string, int> item in categoriesDetails)
        {
            startLineMessage += "\n" + item.Key + " " + item.Value;
        }
        InformationManager.DisplayMessage(new InformationMessage(startLineMessage, BannerlordEnhancedFramework.Colors.Yellow));
    }
    
    

}