using System;
using System.Collections.Generic;
using LethalLib.Modules;

public static class RarityParser
{
    public static void Parse(
        string raw,
        out Dictionary<Levels.LevelTypes, int> levelRarities,
        out Dictionary<string, int> customRarities)
    {
        levelRarities = new();
        customRarities = new();

        if (string.IsNullOrWhiteSpace(raw))
            return;

        var entries = raw.Split(',');

        foreach (var entry in entries)
        {
            var pair = entry.Split(':');
            if (pair.Length != 2)
                continue;

            var key = pair[0].Trim();
            if (!int.TryParse(pair[1].Trim(), out var weight))
                continue;

            if (Enum.TryParse(key, true, out Levels.LevelTypes levelType))
            {
                levelRarities[levelType] = weight;
            }
            else
            {
                customRarities[key] = weight;
            }
        }
    }
}