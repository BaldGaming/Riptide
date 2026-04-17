using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class Torrent
{
    // Properties that Riptide will use throughout the lifecycle of the download
    public string TrackerUrl { get; private set; }
    public string FileName { get; private set; }
    public long FileSize { get; private set; }
    public long PieceLength { get; private set; }
    public byte[] PiecesHashes { get; private set; } // The entire data
    public List<byte[]> PieceHashesList { get; private set; } = new List<byte[]>(); // Stores the metadata in 20-byte chunks
    public byte[] InfoHash { get; private set; }

    public Torrent(string filePath)
    {
        if (File.Exists(filePath))
        {
            try
            {
                byte[] fileBytes = File.ReadAllBytes(filePath);
                
                // Locate the starting index of the info
                int infoValueStart = FindByteArray(fileBytes, Encoding.UTF8.GetBytes("4:info")) + 6;
                // We set the current index (the one to be eventually the last "e") to be the starting position
                int currentIndex = infoValueStart;
                int amount = 0; // This tracks nesting depth

                // Counter logic for nested dictionaries
                do
                {
                    byte current = fileBytes[currentIndex];

                    // Skip strings
                    if (char.IsDigit((char)current))
                    {
                        // Find where the colon is
                        int colonIndex = Array.IndexOf(fileBytes, (byte)':', currentIndex);
                        
                        // Extract the number before the colon to get the length
                        string lengthStr = Encoding.UTF8.GetString(fileBytes[currentIndex..colonIndex]);
                        int length = int.Parse(lengthStr);

                        // Move the index to the last byte of the string content
                        currentIndex = colonIndex + length;
                    }

                    // Skip integers
                    else if (current == (byte)'i')
                        // Finds the "e" that ends this integer and jump to it
                        currentIndex = Array.IndexOf(fileBytes, (byte)'e', currentIndex);

                    // Detect nesting
                    else if (current == (byte)'d' || current == (byte)'l')
                        amount++;
                    
                    // Detect nesting end
                    else if (current == (byte)'e')
                        amount--;

                } while (amount > 0);

                int infoValueEnd = currentIndex;

                // SHA1 Hash calculation
                // Use the found range to get the raw info bytes and hash them
                byte[] rawInfoDict = fileBytes[infoValueStart..infoValueEnd];
                InfoHash = SHA1.HashData(rawInfoDict);

                // Decode the file into a generic dictionary
                var decodedDictionary = (Dictionary<string, object>)Bencode.Decode(fileBytes);

                // Extract the Tracker URL
                if (decodedDictionary.ContainsKey("announce"))
                {
                    // Cast to byte array, then convert to string, then save to property
                    byte[] announceBytes = (byte[])decodedDictionary["announce"];
                    TrackerUrl = Encoding.UTF8.GetString(announceBytes);
                }

                // Extract the "info" dictionary
                if (decodedDictionary.ContainsKey("info"))
                {
                    // We cast this as its own dictionary
                    var infoDict = (Dictionary<string, object>)decodedDictionary["info"];

                    // Extract the File Name
                    if (infoDict.ContainsKey("name"))
                    {
                        byte[] nameBytes = (byte[])infoDict["name"];
                        FileName = Encoding.UTF8.GetString(nameBytes);
                    }

                    // Extract the Piece Length
                    if (infoDict.ContainsKey("piece length"))
                        PieceLength = (long)infoDict["piece length"];

                    // Extract the Total File Size
                    if (infoDict.ContainsKey("length"))
                        FileSize = (long)infoDict["length"];

                    // Extract the Piece Hashes
                    if (infoDict.ContainsKey("pieces"))
                        PiecesHashes = (byte[])infoDict["pieces"];
                }

                // Appends individual 20-byte chunks from PiecesHashes into the PiecesHashesList.
                for (int i = 0; i < PieceHashes.Length; i += 20)
                    PieceHashesList.Add(PiecesHashes[i..(i + 20)]);
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occurred during parsing: {e.Message}");
            }
        }
        else
        {
            Console.WriteLine($"File not found at path: {filePath}");
        }
    }

    private int FindByteArray(byte[] source, byte[] pattern)
    {
        int i, j;

        for (i = 0; i <= source.Length - pattern.Length; i++)
            {
                bool found = true;
                for (j = 0; j < pattern.Length; j++)
                {
                    if (source[i + j] != pattern[j])
                        {
                            found = false;
                            break;
                        }
                }

                if (found)
                    return i;
            }
        return -1;
    }

    
}