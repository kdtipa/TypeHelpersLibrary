using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CustomTypes;

public class HTMLColor : IEquatable<HTMLColor>
{
    public HTMLColor() { }

    /// <summary>
    /// Use integers from 0 to 255 or you'll get an exception.
    /// </summary>
    public HTMLColor(int red, int green, int blue, int? alpha)
    {
        Red = red;
        Green = green;
        Blue = blue;
        Alpha = alpha;
    }

    /// <summary>
    /// Use strings from "0" to "FF" or you'll get an exception.
    /// </summary>
    public HTMLColor(string redHex, string greenHex, string blueHex, string? alphaHex)
    {
        RedHex = redHex;
        GreenHex = greenHex;
        BlueHex = blueHex;
        AlphaHex = alphaHex;
    }

    /// <summary>
    /// Attempts to break apart a standard HTML hex color string into the 
    /// information required for this class.
    /// </summary>
    /// <param name="rgbaHex">
    /// Can be preceeded by the pound/number symbol (#), but the rest should 
    /// be 3, 4, 6, or 8 hex characters.
    /// </param>
    /// <exception cref="ArgumentException">
    /// Throws an exception if the input cannot be parsed into an HTML color.
    /// </exception>
    public HTMLColor(string rgbaHex)
    {
        if (string.IsNullOrWhiteSpace(rgbaHex) || rgbaHex.Length < 3)
        {
            throw new ArgumentException("parameter is empty", nameof(rgbaHex));
        }

        var cleanSrc = rgbaHex.Trim();
        cleanSrc = cleanSrc.TrimStart('#');

        if (TryParseHexString(cleanSrc, out var r, out var g, out var b, out var a))
        {
            Red = r;
            Green = g;
            Blue = b;
            Alpha = a;
        }
        else
        {
            throw new ArgumentException($"Unable to parse [{rgbaHex}] as an HTML color.", nameof(rgbaHex));
        }
    }


    private HexaDecimal _red = new();

    private HexaDecimal _green = new();

    private HexaDecimal _blue = new();

    private HexaDecimal? _alpha = null;

    private int _minValue = 0;
    private int _maxValue = 255;


    public int Red
    {
        get { return (int)_red.DecimalValue; }
        set
        {
            if (value >= _minValue && value <= _maxValue)
            {
                _red.DecimalValue = value;
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(value), $"must be a value from {_minValue} to {_maxValue}.  Received: {value}");
            }
        }
    }

    public int Green
    {
        get { return (int)_green.DecimalValue; }
        set
        {
            if (value >= _minValue && value <= _maxValue)
            {
                _green.DecimalValue = value;
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(value), $"must be a value from {_minValue} to {_maxValue}.  Received: {value}");
            }
        }
    }

    public int Blue
    {
        get { return (int)_blue.DecimalValue; }
        set
        {
            if (value >= _minValue && value <= _maxValue)
            {
                _blue.DecimalValue = value;
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(value), $"must be a value from {_minValue} to {_maxValue}.  Received: {value}");
            }
        }
    }

    public int? Alpha
    {
        get 
        {
            if (_alpha is null) { return null; }
            return (int)_alpha.Value.DecimalValue;
        }
        set
        {
            if (value is null)
            {
                _alpha = null;
            }
            else if (value >= _minValue && value <= _maxValue)
            {
                _alpha = new(value.Value);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(value), $"must be a value from {_minValue} to {_maxValue}.  Received: {value}");
            }
        }
    }


    public string RedHex
    {
        get { return _red.Value.PadLeft(2, '0'); }
        set
        {
            if (HexaDecimal.TryParse(value, out HexaDecimal parsedRed) 
             && parsedRed.DecimalValue >= _minValue 
             && parsedRed.DecimalValue <= _maxValue)
            {
                _red = parsedRed;
            }
            else
            {
                throw new ArgumentException($"Given value [{value}] could not be parsed as a hexadecimal value");
            }
        }
    }

    public string BlueHex
    {
        get { return _blue.Value.PadLeft(2, '0'); }
        set
        {
            if (HexaDecimal.TryParse(value, out HexaDecimal parsedBlue)
             && parsedBlue.DecimalValue >= _minValue
             && parsedBlue.DecimalValue <= _maxValue)
            {
                _blue = parsedBlue;
            }
            else
            {
                throw new ArgumentException($"Given value [{value}] could not be parsed as a hexadecimal value");
            }
        }
    }

    public string GreenHex
    {
        get { return _green.Value.PadLeft(2, '0'); }
        set
        {
            if (HexaDecimal.TryParse(value, out HexaDecimal parsedGreen)
             && parsedGreen.DecimalValue >= _minValue
             && parsedGreen.DecimalValue <= _maxValue)
            {
                _green = parsedGreen;
            }
            else
            {
                throw new ArgumentException($"Given value [{value}] could not be parsed as a hexadecimal value");
            }
        }
    }

    public string? AlphaHex
    {
        get 
        { 
            if (_alpha is null) { return null; }
            else
            {
                // one value to get the value nullable variable value, and one value to get the hex string from the underlying variable
                return _alpha.Value.Value.PadLeft(2, '0');
            }
        }
        set
        {
            if (value is null) { _alpha = null; }
            else if (HexaDecimal.TryParse(value, out HexaDecimal parsedAlpha)
             && parsedAlpha.DecimalValue >= _minValue
             && parsedAlpha.DecimalValue <= _maxValue)
            {
                _alpha = parsedAlpha;
            }
            else
            {
                throw new ArgumentException($"Given value [{value}] could not be parsed as a hexadecimal value");
            }
        }
    }

    public string HexadecimalColorString
    {
        get
        {
            var retVal = new StringBuilder();
            retVal.Append(RedHex);
            retVal.Append(GreenHex);
            retVal.Append(BlueHex);
            if (!string.IsNullOrEmpty(AlphaHex)) { retVal.Append(AlphaHex); }
            return retVal.ToString();
        }
    }

    public string HTMLColorName
    {
        get
        {
            var hcs = HexadecimalColorString; // just to have the string and not have to build it repeatedly
            if (HTMLColorNames.Any(hcn => string.Equals(hcn.Value, hcs, StringComparison.OrdinalIgnoreCase)))
            {
                return HTMLColorNames.Where(hcn => string.Equals(hcn.Value, hcs, StringComparison.OrdinalIgnoreCase)).FirstOrDefault().Key;
            }
            else
            {
                return string.Empty;
            }
        }
    }

    public static bool TryParseHexString(string rgbaHex, out int intRed, out int intGreen, out int intBlue, out int? intAlpha)
    {
        // set up the default _values
        intRed = 0;
        intGreen = 0;
        intBlue = 0;
        intAlpha = null;

        if (string.IsNullOrWhiteSpace(rgbaHex)) { return false; }

        // now try to dig through the source string
        var rgbaLen = rgbaHex.Length;
        var parts = new List<string>();

        switch (rgbaLen)
        {
            // length 3 is shorthand Hex like "7CF" that means "77CCFF"
            case 3:
                parts.Add($"{rgbaHex[0]}{rgbaHex[0]}");
                parts.Add($"{rgbaHex[1]}{rgbaHex[1]}");
                parts.Add($"{rgbaHex[2]}{rgbaHex[2]}");
                break;
            // length 4 is shorthand Hex like "7CF8" that means "77CCFF88" (which includes the alpha value)
            case 4:
                parts.Add($"{rgbaHex[0]}{rgbaHex[0]}");
                parts.Add($"{rgbaHex[1]}{rgbaHex[1]}");
                parts.Add($"{rgbaHex[2]}{rgbaHex[2]}");
                parts.Add($"{rgbaHex[3]}{rgbaHex[3]}");
                break;
            // length 6 is the standard color hex for HTML like "78C4F0"
            case 6:
                parts.Add(rgbaHex[0..1]);
                parts.Add(rgbaHex[2..3]);
                parts.Add(rgbaHex[4..5]);
                break;
            // length 8 is the standard plus alpha like "78C4F07D"
            case 8:
                parts.Add(rgbaHex[0..1]);
                parts.Add(rgbaHex[2..3]);
                parts.Add(rgbaHex[4..5]);
                parts.Add(rgbaHex[6..7]);
                break;
            // length other than those doesn't work
            default:
                break;
        }

        if (parts.Count < 3) { return false; }

        var hexParts = new List<HexaDecimal>();

        foreach (var part in parts)
        {
            if (HexaDecimal.TryParse(part, out var parseResult))
            {
                hexParts.Add(parseResult);
            }
            else
            {
                // if the text string is wrong, the parse failed
                return false;
            }
        }

        // we should have the same number of hex _values as source _values, otherwise we would have returned.
        // what we need now is to be sure they all equate to 0 to 255
        if (hexParts.Count >= 3)
        {
            var srcRed = hexParts[0].DecimalValue;
            if (srcRed >= 0 && srcRed <= 255) { intRed = (int)srcRed; }

            var srcGreen = hexParts[1].DecimalValue;
            if (srcGreen >= 0 && srcGreen <= 255) { intGreen = (int)srcGreen; }

            var srcBlue = hexParts[2].DecimalValue;
            if (srcBlue >= 0 && srcBlue <= 255) { intBlue = (int)srcBlue; }

            // the possible alpha...
            if (hexParts.Count >= 4)
            {
                var srcAlpha = hexParts[3].DecimalValue;
                if (srcAlpha >= 0 && (int)srcAlpha <= 255) { intAlpha = (int)srcAlpha; }
            }

            return true;
        }

        // if we got this far and haven't returned yet, the parse failed
        return false;
    }

    public bool Equals(HTMLColor? other)
    {
        if (other is null) { return false; }

        return _red.DecimalValue == other._red.DecimalValue
            && _green.DecimalValue == other._green.DecimalValue
            && _blue.DecimalValue == other._blue.DecimalValue
            && ((_alpha is null && other._alpha is null) || (_alpha is not null && other._alpha is not null && _alpha.Value.DecimalValue == other._alpha.Value.DecimalValue));
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) { return false; }

        if (obj is HTMLColor htmlc) { return Equals(htmlc); }

        if (obj is long longObj)
        {
            try
            {
                var hObj = new HexaDecimal((long)obj);
                var hcObj = new HTMLColor(hObj.Value);
                return Equals(hcObj);
            }
            catch { return false; }
        }

        if (obj is string strObj)
        {
            try
            {
                var hcObj = new HTMLColor(strObj);
                return Equals(hcObj);
            }
            catch { return false; }
        }

        return false;
    }

    public override int GetHashCode()
    {
        return (Red * 1000000) + (Green * 1000) + Blue;
    }

    public override string ToString()
    {
        var result = new StringBuilder();
        result.Append(RedHex);
        result.Append(GreenHex);
        result.Append(BlueHex);
        if (AlphaHex is not null) { result.Append(AlphaHex); }

        return result.ToString();
    }

    public string ToCSSHexString()
    {
        return $"#{ToString()}";
    }

    public string ToCSSRGBAString()
    {
        var result = new StringBuilder();
        result.Append("rgb");
        if (_alpha is not null) { result.Append('a'); }
        result.Append('(');

        result.Append(_red.DecimalValue);
        result.Append(", ");
        result.Append(_green.DecimalValue);
        result.Append(", ");
        result.Append(_blue.DecimalValue);

        if (_alpha is not null)
        {
            result.Append(", ");
            result.Append(AlphaHex);
        }

        result.Append(')');
        return result.ToString();
    }




    public static Dictionary<string, string> HTMLColorNames = new()
    {
        { "IndianRed", "CD5C5C" },
        { "LightCoral", "F08080" },
        { "Salmon", "FA8072" },
        { "DarkSalmon", "E9967A" },
        { "LightSalmon", "FFA07A" },
        { "Crimson", "DC143C" },
        { "Red", "FF0000" },
        { "FireBrick", "B22222" },
        { "DarkRed", "8B0000" },
        { "Pink", "FFC0CB" },
        { "LightPink", "FFB6C1" },
        { "HotPink", "FF69B4" },
        { "DeepPink", "FF1493" },
        { "MediumVioletRed", "C71585" },
        { "PaleVioletRed", "DB7093" },
        { "LightSalmon", "FFA07A" },
        { "Coral", "FF7F50" },
        { "Tomato", "FF6347" },
        { "OrangeRed", "FF4500" },
        { "DarkOrange", "FF8C00" },
        { "Orange", "FFA500" },
        { "Gold", "FFD700" },
        { "Yellow", "FFFF00" },
        { "LightYellow", "FFFFE0" },
        { "LemonChiffon", "FFFACD" },
        { "LightGoldenrodYellow", "FAFAD2" },
        { "PapayaWhip", "FFEFD5" },
        { "Moccasin", "FFE4B5" },
        { "PeachPuff", "FFDAB9" },
        { "PaleGoldenrod", "EEE8AA" },
        { "Khaki", "F0E68C" },
        { "DarkKhaki", "BDB76B" },
        { "Lavender", "E6E6FA" },
        { "Thistle", "D8BFD8" },
        { "Plum", "DDA0DD" },
        { "Violet", "EE82EE" },
        { "Orchid", "DA70D6" },
        { "Fuchsia", "FF00FF" },
        { "Magenta", "FF00FF" },
        { "MediumOrchid", "BA55D3" },
        { "MediumPurple", "9370DB" },
        { "RebeccaPurple", "663399" },
        { "BlueViolet", "8A2BE2" },
        { "DarkViolet", "9400D3" },
        { "DarkOrchid", "9932CC" },
        { "DarkMagenta", "8B008B" },
        { "Purple", "800080" },
        { "Indigo", "4B0082" },
        { "SlateBlue", "6A5ACD" },
        { "DarkSlateBlue", "483D8B" },
        { "MediumSlateBlue", "7B68EE" },
        { "GreenYellow", "ADFF2F" },
        { "Chartreuse", "7FFF00" },
        { "LawnGreen", "7CFC00" },
        { "Lime", "00FF00" },
        { "LimeGreen", "32CD32" },
        { "PaleGreen", "98FB98" },
        { "LightGreen", "90EE90" },
        { "MediumSpringGreen", "00FA9A" },
        { "SpringGreen", "00FF7F" },
        { "MediumSeaGreen", "3CB371" },
        { "SeaGreen", "2E8B57" },
        { "ForestGreen", "228B22" },
        { "Green", "008000" },
        { "DarkGreen", "006400" },
        { "YellowGreen", "9ACD32" },
        { "OliveDrab", "6B8E23" },
        { "Olive", "808000" },
        { "DarkOliveGreen", "556B2F" },
        { "MediumAquamarine", "66CDAA" },
        { "DarkSeaGreen", "8FBC8B" },
        { "LightSeaGreen", "20B2AA" },
        { "DarkCyan", "008B8B" },
        { "Teal", "008080" },
        { "Aqua", "00FFFF" },
        { "Cyan", "00FFFF" },
        { "LightCyan", "E0FFFF" },
        { "PaleTurquoise", "AFEEEE" },
        { "Aquamarine", "7FFFD4" },
        { "Turquoise", "40E0D0" },
        { "MediumTurquoise", "48D1CC" },
        { "DarkTurquoise", "00CED1" },
        { "CadetBlue", "5F9EA0" },
        { "SteelBlue", "4682B4" },
        { "LightSteelBlue", "B0C4DE" },
        { "PowderBlue", "B0E0E6" },
        { "LightBlue", "ADD8E6" },
        { "SkyBlue", "87CEEB" },
        { "LightSkyBlue", "87CEFA" },
        { "DeepSkyBlue", "00BFFF" },
        { "DodgerBlue", "1E90FF" },
        { "CornflowerBlue", "6495ED" },
        { "MediumSlateBlue", "7B68EE" },
        { "RoyalBlue", "4169E1" },
        { "Blue", "0000FF" },
        { "MediumBlue", "0000CD" },
        { "DarkBlue", "00008B" },
        { "Navy", "000080" },
        { "MidnightBlue", "191970" },
        { "Cornsilk", "FFF8DC" },
        { "BlanchedAlmond", "FFEBCD" },
        { "Bisque", "FFE4C4" },
        { "NavajoWhite", "FFDEAD" },
        { "Wheat", "F5DEB3" },
        { "BurlyWood", "DEB887" },
        { "Tan", "D2B48C" },
        { "RosyBrown", "BC8F8F" },
        { "SandyBrown", "F4A460" },
        { "Goldenrod", "DAA520" },
        { "DarkGoldenrod", "B8860B" },
        { "Peru", "CD853F" },
        { "Chocolate", "D2691E" },
        { "SaddleBrown", "8B4513" },
        { "Sienna", "A0522D" },
        { "Brown", "A52A2A" },
        { "Maroon", "800000" },
        { "White", "FFFFFF" },
        { "Snow", "FFFAFA" },
        { "HoneyDew", "F0FFF0" },
        { "MintCream", "F5FFFA" },
        { "Azure", "F0FFFF" },
        { "AliceBlue", "F0F8FF" },
        { "GhostWhite", "F8F8FF" },
        { "WhiteSmoke", "F5F5F5" },
        { "SeaShell", "FFF5EE" },
        { "Beige", "F5F5DC" },
        { "OldLace", "FDF5E6" },
        { "FloralWhite", "FFFAF0" },
        { "Ivory", "FFFFF0" },
        { "AntiqueWhite", "FAEBD7" },
        { "Linen", "FAF0E6" },
        { "LavenderBlush", "FFF0F5" },
        { "MistyRose", "FFE4E1" },
        { "Gainsboro", "DCDCDC" },
        { "LightGray", "D3D3D3" },
        { "Silver", "C0C0C0" },
        { "DarkGray", "A9A9A9" },
        { "Gray", "808080" },
        { "DimGray", "696969" },
        { "LightSlateGray", "778899" },
        { "SlateGray", "708090" },
        { "DarkSlateGray", "2F4F4F" },
        { "Black", "000000" }
    };

}
