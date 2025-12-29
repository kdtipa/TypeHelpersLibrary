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

        TextEncryption testObj = new TextEncryption();
        testObj.Matrix = new(TextEncryptionMatrix.GetRandomOrderChars());

        string encryptedString = testObj.Encrypt(targetStr);

        string decryptedString = testObj.Decrypt(encryptedString);

        Assert.AreNotEqual(targetStr, encryptedString);
        Assert.AreEqual(targetStr, decryptedString);
    }
}
