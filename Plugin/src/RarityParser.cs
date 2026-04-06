using System.Collections.Generic;
using System.Linq;
using LevelTypes = LethalLib.Modules.Levels.LevelTypes;

public static class RarityParser
{
    public static Dictionary<string, LevelTypes> LevelNames = new()
    {
        {"all",             LevelTypes.All},
        {"modded",          LevelTypes.Modded},
        {"vanilla",         LevelTypes.Vanilla},
        {"experimentation", LevelTypes.ExperimentationLevel},
        {"assurance",       LevelTypes.AssuranceLevel},
        {"vow",             LevelTypes.VowLevel},
        {"adamance",        LevelTypes.AdamanceLevel},
        {"march",           LevelTypes.MarchLevel},
        {"offense",         LevelTypes.OffenseLevel},
        {"rend",            LevelTypes.RendLevel},
        {"dine",            LevelTypes.DineLevel},
        {"titan",           LevelTypes.TitanLevel},
        {"embrion",         LevelTypes.EmbrionLevel},
        {"artifice",        LevelTypes.ArtificeLevel},
    };

    /// <summary>
    /// Takes the configuration string with the moon names and their weight, and
    /// creates the required dictionaries for proper registration of an enemy/entity.
    /// </summary>
    /// <param name="raw"></param>
    /// <param name="levelRarities"></param>
    /// <param name="customRarities"></param>
    public static void Parse(
        string raw,
        out Dictionary<LevelTypes, int> levelRarities,
        out Dictionary<string, int> customRarities)
    {
        levelRarities = [];
        customRarities = [];

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

            string name = CleanName(key);
            if (LevelNames.ContainsKey(name))
            {
                levelRarities[LevelNames[name]] = weight;
            }
            else
            {
                customRarities[key] = weight;
            }
        }
    }

    /// <summary>
    /// <para>Takes only the letters from a moon's name in the config for
    /// comparison with the names in the <c>LevelNames</c> enum.</para>
    /// <para>Also removes the <c>Level</c> in cases such as <c>ExperimentationLevel</c></para>
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string CleanName(string name)
    {
        return new string(name.Where(char.IsLetter).ToArray()).ToLower().Replace("level", "");
    }
}
