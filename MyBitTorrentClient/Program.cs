using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System;
using System.IO;


public class Bencode
{
    public Bencode(Stream stream)
    {
    }

    public object Decode()
    {
    }

    private Dictionary<string, object> DecodeDictionary()
    {
    }

    // Function for decoding arrays
    private static byte[] DecodeByteArray(IEnumerator<byte> enumerator)
    {
        // Creates a dynamic collection for the length characters
        List<byte> lengthBytes = new List<byte>();
        
        // Loop continuously adds the current byte to the list until it reaches ":".
        do
        {
            if (enumerator.Current == (byte)':')
                break;

            lengthBytes.Add(enumerator.Current);
        }
        while (enumerator.MoveNext());

        // Converts lengthBytes to a string, then parse it into an integer.
        string lengthString = Encoding.UTF8.GetString(lengthBytes.ToArray());
        int length = int.Parse(lengthString);

        // Creates the final size byte array using the length.
        byte[] bytes = new byte[length];

        // Loops exactly 'length' times.
        for (int i = 0; length > i; i++)
        {
            enumerator.MoveNext();
            bytes[i] = enumerator.Current;
        } 
        
        // Return the filled byte array
        return bytes;
    }

    // Function for decoding integers
    private static long DecodeInteger(IEnumerator<byte> enumerator)
    {
        // Creates a dynamic collection to hold the bytes we read
        List<byte> numberBytes = new List<byte>();

        // loop that continuously advances the enumerator
        while (enumerator.MoveNext())
        {
            // Checks if the current byte is the end marker: (byte)'e'
            if (enumerator.Current == (byte)'e')
                break; // Breaks because we've hit the end of the integer

            // Otherwise, adds the current byte (enumerator.Current) to your list.
            numberBytes.Add(enumerator.Current);
        }

        // Converts the list of bytes into a string.
        string convertedText = Encoding.UTF8.GetString(numberBytes.ToArray());

        // Parse said string into a long int and return it.
        return Int64.Parse(convertedText);
    }

    private List<object> DecodeList()
    {
    }
}