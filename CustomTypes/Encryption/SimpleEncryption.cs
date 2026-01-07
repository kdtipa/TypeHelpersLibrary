using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomTypes.Encryption;

public static class SimpleEncryption
{
    public static SimpleEncryptionKey GetNewKey() { return new SimpleEncryptionKey(); }

    public static SimpleEncryptionKey GetKeyFrom(string strSource) { return new SimpleEncryptionKey(strSource); }

    public static string Encrypt(this string sourceStr, SimpleEncryptionKey key)
    {
        return Encrypt(key, sourceStr);
    }

    public static string Encrypt(SimpleEncryptionKey key, string clearStr)
    {
        int shift;
        int keyLen = key.KeyLength;
        var result = new StringBuilder();

        for (int i = 0; i < clearStr.Length; i++)
        {
            char c = clearStr[i];
            shift = (i % 7) + 3; // 3 to 9;
            int ki = key.Key.IndexOf(c);
            if (ki != -1)
            {
                ki += shift;
                if (ki > keyLen) { ki = ki % keyLen; }

                result.Append(key.Key[ki]);
            }
            else { result.Append(c); }
        }
        
        return result.ToString();
    }

    public static string Decrypt(this string sourceStr, SimpleEncryptionKey key)
    {
        return Decrypt(key, sourceStr);
    }

    public static string Decrypt(SimpleEncryptionKey key, string blurredStr)
    {
        int shift;
        int keyLen = key.KeyLength;
        var result = new StringBuilder();

        for (int i = 0; i < blurredStr.Length; i++)
        {
            char c = blurredStr[i];
            shift = (i % 7) + 3; // 3 to 9;
            int ki = key.Key.IndexOf(c);
            if (ki != -1)
            {
                ki -= shift;
                if (ki < 0) { ki += keyLen; }

                result.Append(key.Key[ki]);
            }
            else { result.Append(c); }
        }

        return result.ToString();
    }
}
