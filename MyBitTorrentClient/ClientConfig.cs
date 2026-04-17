using System;
using System.Linq; // Required for Concat and ToArray
using System.Security.Cryptography; // Required for RandomNumberGenerator
using System.Text;


public class ClientConfig
{
    // The unique 20-byte ID for this instance of Riptide
    public static byte[] PeerID { get; private set; }

    // Static constructor that runs automatically once when the app starts
    static ClientConfig()
    {
        string prefix = "-RT0001-";
        // Convert the "prefix" string into a byte[] type
        byte[] prefixByte = Encoding.UTF8.GetBytes(prefix);

        // Generates 16 random bytes
        byte[] randomBytes = RandomNumberGenerator.GetBytes(12);

        PeerID = prefixByte.Concat(randomBytes).ToArray();
    }
}