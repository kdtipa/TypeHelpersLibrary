using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomTypes.Encryption;

public struct SimpleEncryptionKey : IEquatable<SimpleEncryptionKey>
{
    public SimpleEncryptionKey()
    {
        // in the empty constructor, we need to build the key
        Key = BuildKey();
    }

    public SimpleEncryptionKey(string srcKey)
    {
        // in this case we need to check to make sure there are no repeats at least 
        // and that all the characters are within the allowed character set.
        LoadKey(srcKey);
    }

    public SimpleEncryptionKey(List<char> srcKey)
    {
        // in this case we need to check to make sure there are no repeats at least 
        // and that all the characters are within the allowed character set.
        LoadKey(srcKey);
    }



    /// <summary>
    /// The primary value of this object.  It is used by the Encryption/Decryption 
    /// code to do the work.
    /// </summary>
    public List<char> Key { get; private set; } = new();

    public int KeyLength { get { return Key.Count; } }

    /// <summary>
    /// Useful mostly when you're ready to store the key, so you can later create 
    /// the same key object for use in decrypting your encrypted stuff.
    /// </summary>
    /// <returns></returns>
    public string GetKeyString()
    {
        var result = new StringBuilder();
        foreach (char c in Key)
        {
            result.Append(c);
        }
        return result.ToString();
    }


    public bool Equals(SimpleEncryptionKey other)
    {
        int keyLen = Key.Count;
        int otherLen = other.Key.Count;

        if (keyLen != otherLen) { return false; }

        for (int i = 0; i < keyLen; i++)
        {
            if (Key[i] != other.Key[i]) { return false; }
        }

        return true;
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is null) { return false; }

        if (obj is SimpleEncryptionKey sekObj) { return Equals(sekObj); }

        if (obj is string strObj)
        {
            try
            {
                var sek = new SimpleEncryptionKey(strObj);
                return Equals(sek);
            } catch { return false; }
        }

        return false;
    }

    public override int GetHashCode()
    {
        return Key.GetHashCode();
    }

    public override string ToString()
    {
        return GetKeyString();
    }

    private List<char> BuildKey()
    {
        var result = new List<char>();
        var rng = new SlowRandom();

        foreach (char c in CoveredChars)
        {
            int ri = rng.Next(0, result.Count);
            result.Insert(ri, c);
        }

        return result;
    }

    private void LoadKey(string srcKey)
    {
        var srcList = new List<char>();
        foreach (char c in srcKey)
        {
            srcList.Add(c);
        }

        LoadKey(srcList);
    }

    private void LoadKey(List<char> srcKey)
    {
        if (srcKey.Count < 20)
        {
            throw new Exception("Invalid Key Length");
        }

        if (srcKey.GroupBy(item => item).Select(grp => new { ch = grp.Key, count = grp.Count() }).Any(item => item.count > 1))
        {
            throw new Exception("Invalid Key Form");
        }

        foreach (char c in srcKey)
        {
            if (!CoveredChars.Contains(c))
            {
                throw new Exception("Invalid Key Contents");
            }
        }

        Key = srcKey;
    }




    public static SimpleEncryptionKey GenerateKey()
    {
        return new SimpleEncryptionKey();
    }

    public static char[] CoveredChars { get; } = new char[]
    {
        '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
        (char)9, (char)10, (char)11, (char)12, (char)13,
        ' ', '!', '"', '#', '$', '%', '&', '\'', '(', ')',
        '*', '+', ',', '-', '.', '/',
        ':', ';', '<', '=', '>', '?', '@',
        'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M',
        'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
        '[', '\\', ']', '^', '_', '`',
        'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
        'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
        '{', '|', '}', '~',
        'α', 'ß', 'Γ', 'π', 'Σ', 'σ', 'µ', 'τ', 'Φ', 'Θ', 'Ω', 'δ', '∞',
        'φ', 'ε', '∩', '≡', '±', '≥', '≤', '⌠', '⌡', '÷', '≈',
        '€', 'ƒ', 'Š', 'Œ', 'Ž', '‘', '’', '”', '”', '–', 'š', 'œ',
        'ž', 'Ÿ', '¢', '£', '¤', '¥', '¦', '§', 'µ', '¶',
        '¼', '½', '¾', '¿', 'À', 'Á', 'Â', 'Ã', 'Ä', 'Å',
        'Æ', 'Ç', 'È', 'É', 'Ê', 'Ë', 'Ì', 'Í', 'Î', 'Ï',
        'Ð', 'Ñ', 'Ò', 'Ó', 'Ô', 'Õ', 'Ö', '×', 'Ø', 'Ù',
        'Ú', 'Û', 'Ü', 'Ý', 'Þ', 'ß', 'à', 'á', 'â', 'ã',
        'ä', 'å', 'æ', 'ç', 'è', 'é', 'ê', 'ë', 'ì', 'í',
        'î', 'ï', 'ð', 'ñ', 'ò', 'ó', 'ô', 'õ', 'ö',
        'ø', 'ù', 'ú', 'û', 'ü', 'ý', 'þ', 'ÿ'
    };

    public static bool operator ==(SimpleEncryptionKey a, SimpleEncryptionKey b) { return a.Equals(b); }
    public static bool operator !=(SimpleEncryptionKey a, SimpleEncryptionKey b) { return !a.Equals(b); }

    public static bool operator ==(SimpleEncryptionKey a, string b) { return a.Equals(b); }
    public static bool operator !=(SimpleEncryptionKey a, string b) { return !a.Equals(b); }

    public static bool operator ==(string a, SimpleEncryptionKey b) { return b.Equals(a); }
    public static bool operator !=(string a, SimpleEncryptionKey b) { return !b.Equals(a); }

}
