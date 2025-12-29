using Microsoft.VisualStudio.TestTools.UnitTesting;
using CustomTypes.NumberText;
using System.Globalization;

namespace UnitTests;

[TestClass]
public sealed class NumberWordTests
{
    [TestMethod]
    public void IntegerWord()
    {
        NumberWord testObj = new NumberWord(3, NumberWordLookUp.English_UnitedStates);

        Assert.IsTrue(testObj.HasIntegerPart);
        Assert.IsFalse(testObj.HasFractionalPart);
        Assert.AreEqual(3, testObj.IntegerPart);
        Assert.AreEqual("Three", testObj.Text);
    }

    [TestMethod]
    public void CultureCompare()
    {
        Assert.AreEqual(NumberWordLookUp.English_UnitedStates, CultureInfo.InvariantCulture);
    }
}
