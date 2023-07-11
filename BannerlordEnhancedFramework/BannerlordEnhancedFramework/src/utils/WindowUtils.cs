using System;
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
            false,
            yesText,
            noText,
            affirmativeAction,
            negativeAction
        );
        InformationManager.ShowInquiry(inquiryData, true);
    }
    
    

}