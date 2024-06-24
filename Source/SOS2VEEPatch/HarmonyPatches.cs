using HarmonyLib;
using RimWorld;
using ShowHair;
using System;
using System.Reflection;
using Verse;
using Verse.AI;

namespace SOS2ShowHairPatch
{
    [StaticConstructorOnStartup]
    public static class HarmonyPatches
    {
        private static Harmony _harmonyInstance;
        public static Harmony HarmonyInstance { get { return _harmonyInstance; } }

        static HarmonyPatches()
        {
            _harmonyInstance = new Harmony("com.ionfrigate12345.sos2showhairpatch");
            _harmonyInstance.PatchAll();
        }

        [HarmonyPatch]
        public static class Patch_ShowHair_Settings_TryGetPawnHatState
        {
            static MethodBase TargetMethod()
            {
                Type showHairSettingsType = AccessTools.TypeByName("ShowHair.Settings");
                if (showHairSettingsType == null)
                {
                    Log.Error("[SOS2ShowHairPatch] Cannot find ShowHair.Settings type.");
                    return null;
                }

                var method = AccessTools.Method(showHairSettingsType, "TryGetPawnHatState");
                if (method == null)
                {
                    Log.Error("[SOS2ShowHairPatch] Cannot find TryGetPawnHatState method in ShowHair.Settings.");
                    return null;
                }

                return method;
            }

            static bool Prefix(Pawn pawn, ThingDef def, out HatEnum hatEnum)
            {
                if (pawn!= null && pawn.Map != null && HarmonyUtils.IsSOS2SpaceMap(pawn.Map) && pawn.AmbientTemperature <= pawn.Map.mapTemperature.OutdoorTemp)
                {
                    hatEnum = HatEnum.HidesAllHair;
                    return false; 
                }

                hatEnum = default;
                return true;
            }
        }
    }
}
