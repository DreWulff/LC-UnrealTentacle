using System.Collections.Generic;
using BepInEx.Configuration;
using LevelTypes = LethalLib.Modules.Levels.LevelTypes;

namespace UnrealTentacle.Configuration
{
    public class PluginConfig
    {
        // For more info on custom configs, see https://lethal.wiki/dev/intermediate/custom-configs
        public Dictionary<LevelTypes, int> vanillaRarities;
        public Dictionary<string, int> customRarities;
        public ConfigEntry<int> barbDamage;
        public PluginConfig(ConfigFile cfg)
        {
            // Tentacle spawn weights configuration
            ConfigHelper.Entities.Spawning.GetRarities(
                cfg: cfg,
                defaultWeights: new Dictionary<string, int>
                {
                    {"Vow", 20},
                    {"March", 20},
                    {"Mazon", 35},
                    {"Halation", 10},
                    {"Infernis", 40},
                    {"Junic", 15},
                },
                out vanillaRarities,
                out customRarities);

            // Barb projectile damage configuration
            barbDamage = cfg.Bind(
                "Behaviour",
                "Barb Damage",
                15,
                "Damage dealt by the tentacle's projectiles"
            );

            ConfigHelper.General.ClearUnusedEntries(cfg);
        }
    }
}