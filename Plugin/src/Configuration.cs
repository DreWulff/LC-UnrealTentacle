using System.Collections.Generic;
using System.Reflection;
using BepInEx.Configuration;

namespace UnrealTentacle.Configuration {
    public class PluginConfig
    {
        // For more info on custom configs, see https://lethal.wiki/dev/intermediate/custom-configs
        public ConfigEntry<string> Rarity;
        public PluginConfig(ConfigFile cfg)
        {
            Rarity = cfg.Bind(
            "Spawning",
            "Rarity",
            "All:15,6 Mazon:200,Halation:100,Vow:100,46 Infernis:200",
            "Spawn weights per moon.\n" +
            "Format: Key:Weight,Key:Weight\n" +
            "Keys can be LevelTypes (All, Modded, ExperimentationLevel, etc)\n" +
            "or custom moon names (Junic, Infernis, etc)."
        );
            
            ClearUnusedEntries(cfg);
        }

        private void ClearUnusedEntries(ConfigFile cfg) {
            // Normally, old unused config entries don't get removed, so we do it with this piece of code. Credit to Kittenji.
            PropertyInfo orphanedEntriesProp = cfg.GetType().GetProperty("OrphanedEntries", BindingFlags.NonPublic | BindingFlags.Instance);
            var orphanedEntries = (Dictionary<ConfigDefinition, string>)orphanedEntriesProp.GetValue(cfg, null);
            orphanedEntries.Clear(); // Clear orphaned entries (Unbinded/Abandoned entries)
            cfg.Save(); // Save the config file to save these changes
        }
    }
}