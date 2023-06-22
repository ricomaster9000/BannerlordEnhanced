using TaleWorlds.Library;

namespace BannerlordEnhancedFramework.utils;

public class WindowUtils
{
    public static void PopupSimpleInquiry(string title, string text, string yesText = "Ok", string noText = "No")
    {
        InquiryData inquiryData = new InquiryData(
            title,
            text,
            true,
            false,
            yesText,
            noText,
            null,
            null
        );
        InformationManager.ShowInquiry(inquiryData);
    }

}