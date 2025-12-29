using Microsoft.VisualStudio.TestTools.UnitTesting;
using CustomTypes.Helpers;

namespace UnitTests;

[TestClass]
public class ArrayHelperTests
{
    [TestMethod]
    public void SortTest()
    {
        string[] colors =
        {
            "red", "orange", "yellow", "green", "blue", "purple"
        };

        string[] expected =
        {
            "blue", "green", "orange", "purple", "red", "yellow"
        };

        string[] test = colors.Sort();

        Assert.IsTrue(expected.EqualByValueAndOrder(test));
    }


    [TestMethod]
    public void EqualByValueTest()
    {
        string[] colorsRainbow =
        {
            "red", "orange", "yellow", "green", "blue", "purple"
        };

        string[] colorsAlphabetic =
        {
            "blue", "green", "orange", "purple", "red", "yellow"
        };

        Assert.IsTrue(colorsRainbow.EqualByValue(colorsAlphabetic));
    }

    [TestMethod]
    public void EqualByValueAndOrderTest()
    {
        string[] colorsRainbow =
        {
            "red", "orange", "yellow", "green", "blue", "purple"
        };

        string[] colorsRainbow2 =
        {
            "red", "orange", "yellow", "green", "blue", "purple"
        };



        Assert.IsTrue(colorsRainbow.EqualByValueAndOrder(colorsRainbow2));
    }
}
