using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class Bencode
{
    // The main entry point for Riptide to decode a torrent file
    public static object Decode(byte[] bytes)
    {
        IEnumerator<byte> enumerator = ((IEnumerable<byte>)bytes).GetEnumerator();
        enumerator.MoveNext();
        return DecodeNextObject(enumerator);
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

    // Function for decoding a list of BEncoded objects
    private static List<object> DecodeList(IEnumerator<byte> enumerator)
    {
        // Creates a dynamic collection to hold the items that need decoding
        List<object> listItems = new List<object>();

        while (enumerator.MoveNext())
        {
            // We break if the current byte is an e.
            if (enumerator.Current == (byte)'e') break;

            // Decode the current byte type
            var decodedItem = DecodeNextObject(enumerator);
        
            // Append the decoded byte to the new list
            listItems.Add(decodedItem);
        }
        return listItems;
    }

    // Function for routing the correct methods for each byte
    private static object DecodeNextObject(IEnumerator<byte> enumerator)
    {
        // Checks if it's an integer marker
        if (enumerator.Current == (byte)'i') return DecodeInteger(enumerator);

        // Checks if it's a list marker
        if (enumerator.Current == (byte)'l') return DecodeList(enumerator); 

        // CheckS if it's a dictionary marker
        if (enumerator.Current == (byte)'d') return DecodeDictionary(enumerator);

        // If it's none of the above, it's a number indicating
        return DecodeByteArray(enumerator);
    }

    // Function for decoding dictionaries
    private static Dictionary<string, object> DecodeDictionary(IEnumerator<byte> enumerator)
    {
        // Creates a dynamic dictionary ???
        Dictionary<string, object> dictionary = new Dictionary<string, object>();

        while (enumerator.MoveNext())
        {
            // We break if the current byte is an e.
            if (enumerator.Current == (byte)'e') break;

            // Decode the key
            byte[] keyBytes = DecodedByteArray(enumerator.Current);

            // Convert the key's raw bytes into a string
            string key = Encoding.UTF8.GetString(keyBytes);

            // Advance the enumerator
            enumerator.MoveNext();

            // Call DecodeNextObject to get the value
        var value = DecodeNextObject(enumerator);

            // Append both vars into the dictionary
            dictionary.Add(key, value);
        }
        return dictionary;
    }
}