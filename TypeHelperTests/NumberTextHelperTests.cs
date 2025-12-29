using System.Collections.Generic;
using TypeHelpers;

namespace TypeHelperTests;

[TestClass]
public class NumberTextHelperTests
{
    [TestMethod]
    public void DigitIntToWordTest()
    {
        var result0 = NumberTextHelper.GetWordsFromInt(0, TargetCaseForNumberText.ProperCase);
        Assert.AreEqual("Zero", result0);

        var result7 = NumberTextHelper.GetWordsFromInt(7, TargetCaseForNumberText.ProperCase);
        Assert.AreEqual("Seven", result7);

        var result9 = NumberTextHelper.GetWordsFromInt(9, TargetCaseForNumberText.ProperCase);
        Assert.AreEqual("Nine", result9);

        var result_n4 = NumberTextHelper.GetWordsFromInt(-4, TargetCaseForNumberText.ProperCase);
        Assert.AreEqual("Negative Four", result_n4);
    }

    [TestMethod]
    public void TeenIntToWordTest()
    {
        var result11 = NumberTextHelper.GetWordsFromInt(11, TargetCaseForNumberText.ProperCase);
        Assert.AreEqual("Eleven", result11);

        var result13 = NumberTextHelper.GetWordsFromInt(13, TargetCaseForNumberText.ProperCase);
        Assert.AreEqual("Thirteen", result13);

        var result15 = NumberTextHelper.GetWordsFromInt(15, TargetCaseForNumberText.ProperCase);
        Assert.AreEqual("Fifteen", result15);

        var result19 = NumberTextHelper.GetWordsFromInt(19, TargetCaseForNumberText.ProperCase);
        Assert.AreEqual("Nineteen", result19);
    }


    [TestMethod]
    public void BiggerIntToWordTest()
    {
        var result42 = NumberTextHelper.GetWordsFromInt(42, TargetCaseForNumberText.ProperCase);
        Assert.AreEqual("Forty-Two", result42);

        var result100 = NumberTextHelper.GetWordsFromInt(100, TargetCaseForNumberText.ProperCase);
        Assert.AreEqual("One Hundred", result100);

        var result417 = NumberTextHelper.GetWordsFromInt(417, TargetCaseForNumberText.ProperCase);
        Assert.AreEqual("Four Hundred Seventeen", result417);

        var result999 = NumberTextHelper.GetWordsFromInt(999, TargetCaseForNumberText.ProperCase);
        Assert.AreEqual("Nine Hundred Ninety-Nine", result999);

        var result1234567890 = NumberTextHelper.GetWordsFromInt(1234567890, TargetCaseForNumberText.ProperCase);
        Assert.AreEqual("One Billion Two Hundred Thirty-Four Million Five Hundred Sixty-Seven Thousand Eight Hundred Ninety", result1234567890);
    }

    [TestMethod]
    public void GetDigitListTest()
    {
        var testValues = new Dictionary<int, List<int>>()
        {
            { 9, [9] },
            { 25, [2, 5] },
            { 1234567, [1, 2, 3, 4, 5, 6, 7] },
            { -475, [-4, 7, 5] }
        };

        foreach (var item in testValues)
        {
            var parsedArray = NumberTextHelper.GetDigitValueList(item.Key);
            Assert.IsTrue(ListHelper.EqualToListByValue(item.Value, parsedArray));
        }
    }



}