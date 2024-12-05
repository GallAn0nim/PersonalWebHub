using System.Text;
using PersonalWebHub.Models.Tools;

namespace PersonalWebHub.Services.Tools;

public class ObfuscateService : IObfuscateService
{
    public string Obfuscate(ObfuscateTextRequest request)
    {
        var textUtf8Bytes = Encoding.UTF8.GetBytes(request.Text);
        var keyUtf8Bytes = Encoding.UTF8.GetBytes(request.Key);
            
        var resultBytes = XorWithKey(textUtf8Bytes, keyUtf8Bytes);
            
        var resultHexString = Convert.ToHexString(resultBytes);
            
        return resultHexString;
    }

    public string Deobfuscate(DeobfuscateTextRequest request)
    {
        var decodedResultBytes = DecodeFromHexString(request.Text);
        var keyUtf8Bytes = Encoding.UTF8.GetBytes(request.Key);
            
        var originalBytes = XorWithKey(decodedResultBytes, keyUtf8Bytes);
             
        var originalText = Encoding.UTF8.GetString(originalBytes);
            
        return originalText;
    }
    
    private static byte[] XorWithKey(byte[] textBytes, byte[] keyBytes)
    {
        return textBytes
            .Select((byteValue, i) => (byte)(byteValue ^ keyBytes[i % keyBytes.Length]))
            .ToArray();
    }
    
    private static byte[] DecodeFromHexString(string hex)
    {
        if (hex.Length % 2 != 0)
            throw new ArgumentException("Hex string must have an even length.", nameof(hex));

        return Enumerable.Range(0, hex.Length / 2)
            .Select(i => Convert.ToByte(hex.Substring(i * 2, 2), 16))
            .ToArray();
    }
}