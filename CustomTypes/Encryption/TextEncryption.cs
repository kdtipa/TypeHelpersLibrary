using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomTypes.Encryption;

public class TextEncryption
{



    public TextEncryptionMatrix Matrix { get; set; }

    public string Encrypt(string clearText)
    {
        var result = new StringBuilder();

        char preceedingChar = ' ';

        foreach (char c in clearText)
        {
            if (Matrix.TryGetValue(preceedingChar, c, out var enc))
            {
                result.Append(enc);
                preceedingChar = enc.Value;
            }
        }

        return result.ToString();
    }

    public string Decrypt(string blurredText)
    {
        var result = new StringBuilder();

        // start at the end and work back
        int blurLen = blurredText.Length;
        for (int i = blurLen - 1; i >= 0; i--)
        {
            if (i > 0)
            {
                if (Matrix.TryGetClear(blurredText[i-1], blurredText[i], out var dcr))
                {
                    result.Append(dcr);
                }
            }
            else
            {
                if (Matrix.TryGetClear(' ', blurredText[i], out var dcr))
                {
                    result.Append(dcr);
                }
            }
        }

        return result.ToString();
    }
}
