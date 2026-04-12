using System.Reflection;
using UnityEngine;
using BepInEx;
using LethalLib.Modules;
using LevelTypes = LethalLib.Modules.Levels.LevelTypes;
using BepInEx.Logging;
using System.IO;
using System.Collections.Generic;
using UnrealTentacle.Configuration;

namespace UnrealTentacle
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency(LethalLib.Plugin.ModGUID)]
    public class Plugin : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger = null!;
        internal static PluginConfig BoundConfig { get; private set; } = null!;
        public static AssetBundle? ModAssets;

        private void Awake()
        {
            Logger = base.Logger;

            // This should be ran before Network Prefabs are registered.
            InitializeNetworkBehaviours();

            // We load the asset bundle that should be next to our DLL file, with the specified name.
            var bundleName = "unreal-tentacle-assets";
            ModAssets = AssetBundle.LoadFromFile(Path.Combine(Path.GetDirectoryName(Info.Location), bundleName));
            if (ModAssets == null)
            {
                Logger.LogError($"Failed to load custom assets.");
                return;
            }

            // We load our assets from our asset bundle. Remember to rename them both here and in our Unity project.
            var UnrealTentacle = ModAssets.LoadAsset<EnemyType>("NaliTentacle");
            var UnrealTentacleTN = ModAssets.LoadAsset<TerminalNode>("NaliTentacleTN");
            var UnrealTentacleTK = ModAssets.LoadAsset<TerminalKeyword>("NaliTentacleTK");
            var TentacleBarb = ModAssets.LoadAsset<GameObject>("Barb");

            // Network Prefabs need to be registered. See https://docs-multiplayer.unity3d.com/netcode/current/basics/object-spawning/
            // LethalLib registers prefabs on GameNetworkManager.Start.
            NetworkPrefabs.RegisterNetworkPrefab(UnrealTentacle.enemyPrefab);
            NetworkPrefabs.RegisterNetworkPrefab(TentacleBarb);

            // Parses and registers the spawn configuration
            BoundConfig = new PluginConfig(base.Config);
            Enemies.RegisterEnemy(
                UnrealTentacle,
                BoundConfig.vanillaRarities,
                BoundConfig.customRarities,
                UnrealTentacleTN,
                UnrealTentacleTK
            );

            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }

        private static void InitializeNetworkBehaviours()
        {
            // See https://github.com/EvaisaDev/UnityNetcodePatcher?tab=readme-ov-file#preparing-mods-for-patching
            var types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (var type in types)
            {
                var methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                foreach (var method in methods)
                {
                    var attributes = method.GetCustomAttributes(typeof(RuntimeInitializeOnLoadMethodAttribute), false);
                    if (attributes.Length > 0)
                    {
                        method.Invoke(null, null);
                    }
                }
            }
        }
    }
}