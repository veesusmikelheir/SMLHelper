﻿namespace SMLHelper.V2
{
    using System;
    using System.Reflection;
    using HarmonyLib;
    using Patchers;
    using Patchers.EnumPatching;
    using QModManager.API.ModLoading;
    using QModManager.Utility;
    using UnityEngine;

    /// <summary>
    /// WARNING: This class is for use only by QModManager.
    /// </summary>
    [QModCore]
    public class Initializer
    {
        internal static readonly Harmony harmony = new Harmony("com.ahk1221.smlhelper");

        /// <summary>
        /// WARNING: This method is for use only by QModManager.
        /// </summary>
        [QModPrePatch]
        [Obsolete("This method is for use only by QModManager.", true)]
        public static void PrePatch()
        {


            Logger.Initialize();
#if SUBNAUTICA
            Logger.Log($"Loading v{Assembly.GetExecutingAssembly().GetName().Version} for Subnautica", LogLevel.Info);
#elif BELOWZERO
            Logger.Log($"Loading v{Assembly.GetExecutingAssembly().GetName().Version} for BelowZero", LogLevel.Info);
#endif

            Logger.Debug("Loading TechType Cache");
            TechTypePatcher.cacheManager.LoadCache();
            Logger.Debug("Loading CraftTreeType Cache");
            CraftTreeTypePatcher.cacheManager.LoadCache();

            PrefabDatabasePatcher.PrePatch(harmony);
            EnumInfoPatch.Patch(harmony);

            var t = TechTypePatcher.AddTechType("TESTTYPE");
            EnumInfoPatch.ClearCache(typeof(TechType));
            Logger.Debug(t.ToString());
            Logger.Debug(Enum.GetName(typeof(TechType), t));
           
        }

        /// <summary>
        /// WARNING: This method is for use only by QModManager.
        /// </summary>
        [QModPostPatch("E3DC72597463233E62D01BD222AD0C96")]
        [Obsolete("This method is for use only by QModManager.", true)]
        public static void PostPatch()
        {
            FishPatcher.Patch(harmony);

            TechTypePatcher.Patch();
            CraftTreeTypePatcher.Patch();
            PingTypePatcher.Patch();
            //EnumPatcher.Patch(harmony);

            CraftDataPatcher.Patch(harmony);
            CraftTreePatcher.Patch(harmony);
            ConsoleCommandsPatcher.Patch(harmony);
            LanguagePatcher.Patch(harmony);
            PrefabDatabasePatcher.PostPatch(harmony);
            SpritePatcher.Patch();
            KnownTechPatcher.Patch(harmony);
            BioReactorPatcher.Patch();
            OptionsPanelPatcher.Patch(harmony);
            ItemsContainerPatcher.Patch(harmony);
            PDAPatcher.Patch(harmony);
            PDAEncyclopediaPatcher.Patch(harmony);
            ItemActionPatcher.Patch(harmony);
            LootDistributionPatcher.Patch(harmony);
            WorldEntityDatabasePatcher.Patch(harmony);
            IngameMenuPatcher.Patch(harmony);
            TooltipPatcher.Patch(harmony);



            Logger.Debug("Saving TechType Cache");
            TechTypePatcher.cacheManager.SaveCache();
            Logger.Debug("Saving CraftTreeType Cache");
            CraftTreeTypePatcher.cacheManager.SaveCache();
        }
    }
}
