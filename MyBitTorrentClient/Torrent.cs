using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class Torrent
{
    // These are the properties Riptide will need to access later
    public string TrackerUrl { get; private set; }
    // We will add more properties here soon (like FileName, PieceLength, etc.)

    // This is the Constructor. It runs immediately when you create a 'new Torrent()'
    public Torrent(string filePath)
    {
        if (File.Exists(filePath))
        {
            try
            {
                // Reads the entire file into a byte array
                byte[] fileBytes = File.ReadAllBytes(filePath);

                // Sends said byte array to the Bencode decoder and saves the result,
                // We cast it to a Dictionary because we know the root of a torrent file is always a dictionary.
                var decodedDictionary = (Dictionary<string, object>)Bencode.Decode(fileBytes);

                byte[] decodedString = (byte[])decodedDictionary;

                if (decodedDictionary.Contains("announce"))
                    string announce =  Encoding.UTF8.GetString(decodedString.ToArray());

                if (decodedDictionary.Contains("info"))
                    string info =  Encoding.UTF8.GetString(decodedString.ToArray());
            }

            // Debugging messages
            catch (IOException e)
            {
                Console.WriteLine($"An I/O error occurred: {e.Message}");
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine($"Access to the path is denied: {e.Message}");
            }
        }
        else
        {
            Console.WriteLine($"File not found at path: {filePath}");
        }
    }
}