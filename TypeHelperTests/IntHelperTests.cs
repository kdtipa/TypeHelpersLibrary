using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using TypeHelpers;

namespace TypeHelperTests;

[TestClass]
public class IntHelperTests
{
    [TestMethod]
    public void RootTest()
    {
        int[] TestVals = { 1, 2, 3, 9, 27, -4 };
        int[] ExpVals = { 3, 2, 2, 2, 3, 2 };
        decimal?[] ExpectedRootVals = { 1.00000m, 1.41420m, 1.73203m, 3.00000m, 3.00000m, null };
        List<decimal?> ActualResults = new();

        var testCount = TestVals.Length;
        for (int t = 0; t < testCount; t++)
        {
            var result = TestVals[t].Root(ExpVals[t]);
            ActualResults.Add(result);
        }

        // doing this in a separate loop so that I can break and look at all the values
        for (int r = 0; r < testCount; r++)
        {
            Assert.AreEqual(ExpectedRootVals[r], ActualResults[r]);
        }

    }
}
