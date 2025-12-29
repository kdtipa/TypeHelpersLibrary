using CustomTypes.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomTypes.Encryption
{
    public struct TextEncryptionMatrix
    {
        public TextEncryptionMatrix() { }

        public TextEncryptionMatrix(char[] sourceKey)
        {
            if (!TryLoadKey(sourceKey))
            {
                throw new InvalidOperationException("Unable to load given source key");
            }
        }

        private bool TryLoadKey(char[] sourceKey)
        {
            if (!sourceKey.EqualByValue(HandledCharacters)) { return false; }
            

            int srcLen = sourceKey.Length;

            for (int prefix = 0; prefix < srcLen; prefix++)
            {
                for (int suffix = 0; suffix < srcLen; suffix++)
                {
                    var key = new TextEncryptionMatrixKey(HandledCharacters[prefix], HandledCharacters[suffix]);
                    Data.Add(key, sourceKey[GetOffsetIndex(suffix, prefix)]);
                }
            }

            return true;
        }

        private int GetOffsetIndex(int i, int o, int? l = null)
        {
            int len = l ?? HandledCharacters.Length;
            int worker = i + o;

            if (worker >= 0 && worker < len) { return i; }

            if (worker < 0)
            {
                // negative prefix is possible I guess
                while (worker < 0) { worker += len; }
                return worker;
            }

            worker = worker % len;
            return worker;
        }

        public bool TryGetValue(char prefix, char suffix, [NotNullWhen(true)] out char? value)
        {
            value = null;
            var key = new TextEncryptionMatrixKey(prefix, suffix);
            if (Data.TryGetValue(key, out char found))
            {
                value = found;
                return true;
            }
            return false;
        }

        public bool TryGetClear(char value, char previousValue, [NotNullWhen(true)] out char? clear)
        {
            clear = null;
            var result = Data.Where(item => item.Value == value && item.Key.Prefix == previousValue).FirstOrDefault().Key.Suffix;
            if (result != (char)0)
            {
                clear = result;
                return true;
            }
            return false;
        }

        public Dictionary<TextEncryptionMatrixKey, char> Data { get; private set; } = new();

        public char[] Key 
        { 
            get
            {
                char baseChar = HandledCharacters[0];
                char[] result = new char[HandledCharacters.Length];

                for (int i = 0; i < result.Length; i++)
                {
                    result[i] = Data[new(baseChar, HandledCharacters[i])];
                }

                return result;
            } 
        }

        public string KeyString 
        { 
            get
            {
                return Key.ConvertToString();
            } 
        }


        public static char NullChar { get; } = (char)0;

        public static char[] HandledCharacters { get; } =
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

        public static char[] GetRandomOrderChars()
        {
            List<char> randChars = new();
            Random rng = new();
            int len = HandledCharacters.Length;
            int slp = 4;

            foreach (char c in HandledCharacters)
            {
                int currentCount = randChars.Count;
                if (currentCount > 0)
                {
                    int rndI = rng.Next(0, randChars.Count);
                    randChars.Insert(rndI, c);
                }
                else { randChars.Add(c); }
            }

            return randChars.ToArray();
        }

        public static TextEncryptionMatrix CreateNewMatrixWithKey()
        {
            var newKey = GetRandomOrderChars();
            return new TextEncryptionMatrix(newKey);
        }
    }

    public struct TextEncryptionMatrixKey : IEquatable<TextEncryptionMatrixKey>
    {
        public TextEncryptionMatrixKey() { }

        public TextEncryptionMatrixKey(char prefix, char suffix)
        {
            Prefix = prefix;
            Suffix = suffix;
        }

        public char Prefix { get; } = (char)0;

        public char Suffix { get; } = (char)0;

        public static TextEncryptionMatrixKey Empty { get; } = new TextEncryptionMatrixKey();

        public bool Equals(TextEncryptionMatrixKey other)
        {
            return Prefix == other.Prefix && Suffix == other.Suffix;
        }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj is null) { return false; }

            if (obj is TextEncryptionMatrixKey temkObj) { return Equals(temkObj); }

            if (obj is string strObj && strObj.Length == 2) { return Prefix == strObj[0] && Suffix == strObj[1]; }


            return false;
        }

        public override int GetHashCode()
        {
            return (10000 * Prefix) + Suffix;
        }

        public override string ToString()
        {
            return $"{Prefix}{Suffix}";
        }

        public static bool operator ==(TextEncryptionMatrixKey a, TextEncryptionMatrixKey b) { return a.Equals(b); }
        public static bool operator !=(TextEncryptionMatrixKey a, TextEncryptionMatrixKey b) { return !a.Equals(b); }

        public static bool operator ==(TextEncryptionMatrixKey a, string b) { return a.Equals(b); }
        public static bool operator !=(TextEncryptionMatrixKey a, string b) { return !a.Equals(b); }

        public static bool operator ==(string a, TextEncryptionMatrixKey b) { return b.Equals(a); }
        public static bool operator !=(string a, TextEncryptionMatrixKey b) { return !b.Equals(a); }

    }
}
