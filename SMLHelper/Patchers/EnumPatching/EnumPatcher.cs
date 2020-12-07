namespace SMLHelper.V2.Patchers.EnumPatching
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using HarmonyLib;
    using SMLHelper.V2.Utility;

    internal static class EnumInfoPatch
    {
        public static void Patch(Harmony harmony)
        {
            harmony.Patch(TargetMethod(), null, null, new HarmonyMethod(AccessTools.Method(typeof(EnumInfoPatch), nameof(Transpiler))));
        }
        static MethodBase TargetMethod()
        {
            return AccessTools.Method(Type.GetType("System.Enum"), "GetCachedValuesAndNames");
            

        }

        internal static void ClearCache(Type enumType)
        {
            AccessTools.Field(Type.GetType("System.RuntimeType"), "GenericCache").SetValue(enumType, null); 
        }

        static void FixEnum(object type, ref ulong[] oldValues, ref string[] oldNames)
        {

            
            var enumType = type as Type;

            if (!GetArrays(enumType, out string[] toBePatchedNames, out ulong[] toBePatchedValues)) return;
            Array.Resize(ref oldNames, toBePatchedNames.Length + oldNames.Length);
            Array.Resize(ref oldValues, toBePatchedValues.Length + oldValues.Length); 
            Array.Copy(toBePatchedNames, 0, oldNames, oldNames.Length- toBePatchedNames.Length, toBePatchedNames.Length);
            Array.Copy(toBePatchedValues, 0, oldValues, oldValues.Length- toBePatchedValues.Length, toBePatchedValues.Length);

            Array.Sort<ulong, string>(oldValues, oldNames, Comparer<ulong>.Default);
            
        }

        static bool GetArrays(Type type, out string[] names, out ulong[] values)
        {
            names = null;
            values = null;
            if (type == typeof(TechType))
            {
                if (TechTypePatcher.cacheManager == null) return false;
                names = TechTypePatcher.cacheManager.ModdedNames;
                values = TechTypePatcher.cacheManager.ModdedValues;
                return true;
            }
            else if (type == typeof(PingType))
            {
                if (PingTypePatcher.cacheManager == null) return false;

                names = PingTypePatcher.cacheManager.ModdedNames;
                values = PingTypePatcher.cacheManager.ModdedValues;
                return true;
            }
            else if (type == typeof(CraftTree.Type))
            {
                if (CraftTreeTypePatcher.cacheManager == null) return false;

                names = CraftTreeTypePatcher.cacheManager.ModdedNames;
                values = CraftTreeTypePatcher.cacheManager.ModdedValues;
                return true;
            }

            return false;
        }

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            using (var enumerator = instructions.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    var v = enumerator.Current;
                    if (v.operand is MethodInfo me && me.Name == "Sort")
                    {
                        yield return v;
                        enumerator.MoveNext();
                        v = enumerator.Current;
                        var labels = v.labels;
                        v.labels = new List<Label>();
                        yield return new CodeInstruction(OpCodes.Ldarg_0) { labels = labels };
                        yield return new CodeInstruction(OpCodes.Ldloca, 1);
                        yield return new CodeInstruction(OpCodes.Ldloca, 2);
                        yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(EnumInfoPatch), "FixEnum"));
                        yield return v;
                    }
                    else
                    {
                        yield return v;
                    }
                }
            }
        }
    }

    
}
