using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class Bencode
{
    private readonly BinaryReader _reader;

    public Bencode(Stream stream)
    {
        // BinaryReader is like C's fread()—it handles the stream for us.
        _reader = new BinaryReader(stream);
    }

    public object Decode()
    {
        // PeekChar is like checking the first byte of a buffer
        char peek = (char)_reader.PeekChar();

        return peek switch
        {
            'd' => DecodeDictionary(),
            'l' => DecodeList(),
            'i' => DecodeInteger(),
            _   => DecodeString() // Strings start with a number (e.g., 4:spam)
        };
    }

    private Dictionary<string, object> DecodeDictionary()
    {
        _reader.ReadChar(); // Pop the 'd'
        var dict = new Dictionary<string, object>();
        while ((char)_reader.PeekChar() != 'e')
        {
            // BitTorrent dictionaries always have strings as keys
            string key = Encoding.UTF8.GetString((byte[])DecodeString());
            object value = Decode();
            dict[key] = value;
        }
        _reader.ReadChar(); // Pop the 'e'
        return dict;
    }

    private byte[] DecodeString()
    {
        // Find the colon (like searching for a char in a C array)
        string lengthStr = "";
        char c;
        while ((c = _reader.ReadChar()) != ':')
        {
            lengthStr += c;
        }
        int length = int.Parse(lengthStr);
        return _reader.ReadBytes(length);
    }

    // TODO: Add DecodeInteger and DecodeList
}