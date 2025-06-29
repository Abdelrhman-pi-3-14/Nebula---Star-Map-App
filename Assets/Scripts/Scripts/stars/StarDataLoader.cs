using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class StarDataLoader
{
    public static List<Star> LoadData()
    {
        // Define the filename (adjust based on actual file name, e.g., "BSC5.bytes")
        const string filename = "BSC5";

        // Load the binary file as a TextAsset from Unity's Resources folder
        TextAsset textAsset = Resources.Load<TextAsset>(filename);
        if (textAsset == null)
        {
            Debug.LogError($"Failed to load {filename} from Resources. Ensure the file is in the Resources folder.");
            return null;
        }

        // Initialize MemoryStream and BinaryReader to process the binary data
        using MemoryStream stream = new MemoryStream(textAsset.bytes);
        using BinaryReader reader = new BinaryReader(stream);

        try
        {
            // Read the header (28 bytes, 7 Integer*4 fields)
            int star0 = reader.ReadInt32();      // Subtract from star number to get sequence number
            int star1 = reader.ReadInt32();      // First star number in the file
            int starn = reader.ReadInt32();      // Number of stars in the file
            int stnum = reader.ReadInt32();      // Star ID presence/format
            int mprop = reader.ReadInt32();      // Proper motion/radial velocity flag
            int nmag = reader.ReadInt32();       // Number of magnitudes
            int nbent = reader.ReadInt32();      // Bytes per star entry

            // Log header information for debugging
            Debug.Log($"Header: starn={starn}, mprop={mprop}, nmag={nmag}, nbent={nbent}");

            // Determine if coordinates are in J2000 format and calculate number of stars
            bool isJ2000 = starn < 0 || nmag < 0;
            int numStars = Math.Abs(starn);

            // Validate header values
            if (numStars <= 0 || nbent <= 0)
            {
                Debug.LogError("Invalid header values: Number of stars or bytes per entry is invalid.");
                return null;
            }

            // Calculate expected file size for validation
            long expectedSize = 28 + (long)numStars * nbent;
            if (stream.Length < expectedSize)
            {
                Debug.LogError($"Binary file size ({stream.Length} bytes) is smaller than expected ({expectedSize} bytes).");
                return null;
            }

            List<Star> stars = new List<Star>();

            // Read star data for each star
            for (int i = 0; i < numStars; i++)
            {
                // Read catalog number if present
                float catalogNumber = 0f;
                if (stnum >= 0 && stnum != 0)
                {
                    catalogNumber = reader.ReadSingle();
                }

                // Read Right Ascension and Declination (radians)
                double rightAscension = reader.ReadDouble();
                double declination = reader.ReadDouble();

                // Read spectral type (2 characters)
                char spectralTypeChar = (char)reader.ReadByte();
                char spectralIndexChar = (char)reader.ReadByte();
                string spectralType = $"{spectralTypeChar}{spectralIndexChar}";

                // Read magnitudes based on nmag
                short[] magnitudes = new short[Math.Abs(nmag)];
                for (int j = 0; j < magnitudes.Length; j++)
                {
                    magnitudes[j] = reader.ReadInt16();
                }

                // Read proper motion if included
                float raProperMotion = 0f;
                float decProperMotion = 0f;
                if (mprop == 1)
                {
                    raProperMotion = reader.ReadSingle();
                    decProperMotion = reader.ReadSingle();
                }
                else if (mprop == 2)
                {
                    // Radial velocity included, skip for now
                    reader.ReadDouble();
                }

                // Create Star object (assuming Star constructor matches these parameters)
                Star star = new Star(
                    catalogNumber,
                    rightAscension,
                    declination,
                    (byte)spectralTypeChar,
                    (byte)spectralIndexChar,
                    magnitudes.Length > 0 ? magnitudes[0] : (short)0,
                    raProperMotion,
                    decProperMotion
                );
                stars.Add(star);
            }

            return stars;
        }
        catch (Exception e)
        {
            Debug.LogError($"Error reading binary file: {e.Message}");
            return null;
        }
    }
}