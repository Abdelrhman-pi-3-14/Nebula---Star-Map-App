using System;
using System.Collections.Generic;
using UnityEngine;


    [Serializable]
    public class ConstellationLinesData
    {
        public string constellationAbbr;
        public int starCount;
        public List<int> bscNumbers;
    }

    public static class ConstellationLinesParser
    {
        public static Dictionary<string, List<int>> ParseConstellationLines(TextAsset file)
        {
            Dictionary<string, List<int>> constellationDict = new Dictionary<string, List<int>>();
            string[] lines = file.text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string line in lines)
            {
                if (line.StartsWith("#")) continue;

                string[] tokens = line.Trim().Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                if (tokens.Length < 3) continue;

                string abbr = tokens[0];
                List<int> starIds = new List<int>();

                for (int i = 2; i < tokens.Length; i++)
                {
                    if (int.TryParse(tokens[i], out int bsc))
                        starIds.Add(bsc);
                }

                // Merge entries for constellations with multiple lines
                if (constellationDict.ContainsKey(abbr))
                    constellationDict[abbr].AddRange(starIds);
                else
                    constellationDict[abbr] = starIds;
            }
            return constellationDict;
        }
    }
