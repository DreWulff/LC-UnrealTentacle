using System.Collections.Generic;
using System.Reflection;
using BepInEx.Configuration;

namespace UnrealTentacle.Configuration
{
    public class PluginConfig
    {
        // For more info on custom configs, see https://lethal.wiki/dev/intermediate/custom-configs
        public ConfigEntry<string> Rarity;
        public PluginConfig(ConfigFile cfg)
        {
            Rarity = cfg.Bind(
                "Spawning",
                "Rarity",
                "Vow:20,March:20,Mazon:35,Halation:10,Infernis:40,Junic:15",
                "Spawn weights per moon.\n" +
                "Format: Key:Weight,Key:Weight\n" +
                "Keys can be:\n" +
                "- Names of vanilla moons (e.g., 'Experimentation', '41-experimentation', 'ExperimentationLevel')\n" +
                "- Custom moon names ('Junic', '60 Mazon', etc)\n" +
                "- Blanket tags: 'All', 'Vanilla', 'Modded'"
            );

            ClearUnusedEntries(cfg);
        }

        private void ClearUnusedEntries(ConfigFile cfg)
        {
            // Normally, old unused config entries don't get removed, so we do it with this piece of code. Credit to Kittenji.
            PropertyInfo orphanedEntriesProp = cfg.GetType().GetProperty("OrphanedEntries", BindingFlags.NonPublic | BindingFlags.Instance);
            var orphanedEntries = (Dictionary<ConfigDefinition, string>)orphanedEntriesProp.GetValue(cfg, null);
            orphanedEntries.Clear(); // Clear orphaned entries (Unbinded/Abandoned entries)
            cfg.Save(); // Save the config file to save these changes
        }
    }
}