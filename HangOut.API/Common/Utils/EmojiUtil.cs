using System.Globalization;

namespace HangOut.API.Common.Utils;

public static class EmojiUtil
{
    public static bool IsEmoji(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return false;

        var enumerator = StringInfo.GetTextElementEnumerator(input);
        int elementCount = 0;

        while (enumerator.MoveNext())
        {
            string element = enumerator.GetTextElement();
            elementCount++;

            int codepoint = char.ConvertToUtf32(element, 0);

            // Emoji ranges (simplified)
            if ((codepoint >= 0x1F300 && codepoint <= 0x1F6FF) || // Misc symbols & pictographs
                (codepoint >= 0x1F900 && codepoint <= 0x1F9FF) || // Supplemental symbols
                (codepoint >= 0x1F680 && codepoint <= 0x1F6FF) || // Transport & map symbols
                (codepoint >= 0x2600 && codepoint <= 0x26FF))     // Misc symbols
            {
                return true;
            }
        }

        return false;
    }
}