using CustomTypes.Encryption;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests;

[TestClass]
public class TextEncryptionTests
{
    [TestMethod]
    public void TestEncDec()
    {
        string targetStr = "I love Lucy";

        SimpleEncryptionKey testKey = new SimpleEncryptionKey();

        string encryptedString = targetStr.Encrypt(testKey);

        string decryptedString = encryptedString.Decrypt(testKey);

        Assert.AreNotEqual(targetStr, encryptedString);
        Assert.AreEqual(targetStr, decryptedString);
    }
}
