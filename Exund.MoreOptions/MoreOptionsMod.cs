using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;
using ModHelper;
using Nuterra.NativeOptions;

namespace Exund.MoreOptions
{
    public class MoreOptionsMod : ModBase
    {
        private static readonly FieldInfo line = AccessTools.Field(typeof(SmokeTrail), "line");
        private static readonly FieldInfo m_Trails = AccessTools.Field(typeof(BoosterJet), "m_Trails");
        private static readonly FieldInfo m_SmokeTrailPrefab = AccessTools.Field(typeof(MissileProjectile), "m_SmokeTrailPrefab");
        private static readonly FieldInfo m_MaterialSwapper = AccessTools.Field(typeof(TankBlock), "m_MaterialSwapper");
        private static readonly MethodInfo ClearAllBeams = AccessTools.Method(typeof(ModuleItemHolderBeam), "ClearAllBeams");

        public static ModConfig config;

        private static bool displayMuzzleFlashesBool = true;
        private static bool displayBulletsCasingBool = true;
        private static bool displaySmokeTrailsBool = true;
        private static bool displayHoverEffectsBool = true;
        private static bool displayRemoteChargersEffectsBool = true;
        private static bool displayHolderBeamsBool = true;
        private static bool displayThrustersEffectsBool = true;
        private static bool displayMissileSmokeBool = true;
        public static bool displayProjectileExplosionsBool = true;
        public static bool displayBlockExplosionsBool = true;
        private static bool displayAntennaGlowBool = true;
        private static bool antigravMatSwapBool = true;
        private static bool displayBubblesBool = true;

        private const string HarmonyID = "Exund.MoreOptions";
        private static readonly Harmony harmony = new Harmony(HarmonyID);

        public override void Init()
        {
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        public override void DeInit()
        {
            harmony.UnpatchAll(HarmonyID);
        }

        public override bool HasEarlyInit()
        {
            return true;
        }

        public override void EarlyInit()
        {
            Load();
        }

        public static void Load()
        {
            config = new ModConfig();
            foreach (FieldInfo f in typeof(MoreOptionsMod).GetFields(BindingFlags.Static | BindingFlags.NonPublic).Where(f => f.Name.EndsWith("Bool")))
            {
                config.BindConfig(null, f);
            }

            const string modName = "More Options";

            var displayMuzzleFlashes = new OptionToggle("Muzzle Flashes", modName, displayMuzzleFlashesBool);
            var displayBulletsCasing = new OptionToggle("Bullets Casing", modName, displayBulletsCasingBool);
            var displaySmokeTrails = new OptionToggle("Smoke Trails", modName, displaySmokeTrailsBool);
            var displayHoverEffects = new OptionToggle("Hover Effects", modName, displayHoverEffectsBool);
            var displayRemoteChargersEffects = new OptionToggle("Remote Chargers Effects", modName, displayRemoteChargersEffectsBool);
            var displayHolderBeams = new OptionToggle("Holder Beams", modName, displayHolderBeamsBool);
            var displayThrustersEffects = new OptionToggle("Thrusters Effects", modName, displayThrustersEffectsBool);
            var displayMissileSmoke = new OptionToggle("Missile Smoke", modName, displayMissileSmokeBool);
            var displayProjectileExplosions = new OptionToggle("Projectile Explosions", modName, displayProjectileExplosionsBool);
            var displayBlockExplosions = new OptionToggle("Block Explosions", modName, displayBlockExplosionsBool);
            var displayAntennaGlow = new OptionToggle("Antenna Glow", modName, displayAntennaGlowBool);
            var antigravMatSwap = new OptionToggle("Antigrav color pulse", modName, antigravMatSwapBool);
            var displayBubbles = new OptionToggle("Bubbles (shield, healing)", modName, displayBubblesBool);

            displayMuzzleFlashes.onValueSaved.AddListener(() => { displayMuzzleFlashesBool = displayMuzzleFlashes.SavedValue; });
            displayBulletsCasing.onValueSaved.AddListener(() => { displayBulletsCasingBool = displayBulletsCasing.SavedValue; });
            displaySmokeTrails.onValueSaved.AddListener(() => { displaySmokeTrailsBool = displaySmokeTrails.SavedValue; });
            displayHoverEffects.onValueSaved.AddListener(() => { displayHoverEffectsBool = displayHoverEffects.SavedValue; });
            displayRemoteChargersEffects.onValueSaved.AddListener(() => { displayRemoteChargersEffectsBool = displayRemoteChargersEffects.SavedValue; });
            displayHolderBeams.onValueSaved.AddListener(() => { displayHolderBeamsBool = displayHolderBeams.SavedValue; });
            displayThrustersEffects.onValueSaved.AddListener(() => { displayThrustersEffectsBool = displayThrustersEffects.SavedValue; });
            displayMissileSmoke.onValueSaved.AddListener(() => { displayMissileSmokeBool = displayMissileSmoke.SavedValue; });
            displayProjectileExplosions.onValueSaved.AddListener(() => { displayProjectileExplosionsBool = displayProjectileExplosions.SavedValue; });
            displayBlockExplosions.onValueSaved.AddListener(() => { displayBlockExplosionsBool = displayBlockExplosions.SavedValue; });
            displayAntennaGlow.onValueSaved.AddListener(() => { displayAntennaGlowBool = displayAntennaGlow.SavedValue; });
            antigravMatSwap.onValueSaved.AddListener(() => { antigravMatSwapBool = antigravMatSwap.SavedValue; });
            displayBubbles.onValueSaved.AddListener(() => { displayBubblesBool = displayBubbles.SavedValue; });

            NativeOptionsMod.onOptionsSaved.AddListener(() => { config.WriteConfigJsonFile(); });
        }

        public static void EnableRenderers(Component t, bool enable)
        {
            if (t)
            {
                foreach (Renderer r in t.GetComponentsInChildren<Renderer>())
                {
                    r.enabled = enable;
                }
            }
        }

        internal class Patches
        {
            [HarmonyPatch(typeof(CannonBarrel), "EjectCasing")]
            private static class EjectCasing
            {
                private static bool Prefix()
                {
                    return displayBulletsCasingBool;
                }
            }

            [HarmonyPatch(typeof(MuzzleFlash), "Fire")]
            private static class MuzzleFlashFix
            {
                private static bool Prefix()
                {
                    return displayMuzzleFlashesBool;
                }
            }

            [HarmonyPatch(typeof(SmokeTrail), "Update")]
            private static class SmokeTrailFix
            {
                private static bool Prefix(ref SmokeTrail __instance)
                {
                    var iline = (LineRenderer)line.GetValue(__instance);
                    iline.enabled = displaySmokeTrailsBool;
                    return iline.enabled;
                }
            }

            private static class HoverJetFix
            {
                [HarmonyPatch(typeof(HoverJet), "EnableJetEffects")]
                private static class EnableJetEffects
                {
                    private static void Prefix(ref HoverJet __instance, ref bool active)
                    {
                        active = active && displayHoverEffectsBool;
                    }
                }
            }

            [HarmonyPatch(typeof(ModuleRemoteCharger), "FireNewArcEffect")]
            private static class ModuleRemoteChargerFix
            {
                private static bool Prefix()
                {
                    return displayRemoteChargersEffectsBool;
                }
            }

            private static class ModuleItemHolderBeamFix
            {
                [HarmonyPatch(typeof(ModuleItemHolderBeam), "UpdateBeamEffects")]
                private static class UpdateBeamEffects
                {
                    private static bool Prefix(ref ModuleItemHolderBeam __instance)
                    {
                        if (!displayHolderBeamsBool)
                        {
                            ClearAllBeams.Invoke(__instance, Array.Empty<object>());
                        }

                        return displayHolderBeamsBool;
                    }
                }
            }

            [HarmonyPatch(typeof(BoosterJet), "OnUpdate")]
            private static class BoosterJetFix
            {
                private static bool Prefix(ref BoosterJet __instance)
                {
                    if (!displayThrustersEffectsBool)
                    {
                        var trails = (JetTrail[])m_Trails.GetValue(__instance);
                        foreach (JetTrail jetTrail in trails)
                        {
                            jetTrail.Cease();
                        }
                    }

                    return displayThrustersEffectsBool;
                }
            }

            private static class MissileProjectileFix
            {
                [HarmonyPatch(typeof(MissileProjectile), "ActivateBoosters")]
                private static class ActivateBoosters
                {
                    private static void Prefix(ref MissileProjectile __instance, ref Transform __state)
                    {
                        if (!displayMissileSmokeBool)
                        {
                            __state = (Transform)m_SmokeTrailPrefab.GetValue(__instance);
                            m_SmokeTrailPrefab.SetValue(__instance, null);
                        }
                    }

                    private static void Postfix(ref MissileProjectile __instance, ref Transform __state)
                    {
                        if (__state)
                        {
                            m_SmokeTrailPrefab.SetValue(__instance, __state);
                        }
                    }
                }
            }

            private static readonly MemberInfo EnableRenderers_MI = AccessTools.Method(typeof(MoreOptionsMod), "EnableRenderers");

            [HarmonyPatch(typeof(Projectile), "SpawnExplosion")]
            private static class ProjectileExplosionsFix
            {
                private static readonly MethodInfo Spawn = AccessTools.Method(typeof(ComponentPoolExtensions), "Spawn", new[]
                {
                    typeof(Transform),
                    typeof(Transform),
                    typeof(Vector3)
                });

                public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
                {
                    var codes = instructions.ToList();
                    var spawn_i = codes.FindIndex(ci => ci.Calls(Spawn));
                    var i = codes.FindIndex(spawn_i, 3, ci => ci.IsStloc()) + 1;
                    codes.InsertRange(i, new[]
                    {
                        new CodeInstruction(OpCodes.Ldloc_0),
                        new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(MoreOptionsMod), "displayProjectileExplosionsBool")),
                        new CodeInstruction(OpCodes.Call, EnableRenderers_MI)
                    });
                    return codes;
                }
            }

            [HarmonyPatch(typeof(ModuleDamage), "Explode")]
            private static class ModuleDamageExplosionFix
            {
                private static readonly MethodInfo Spawn = AccessTools.Method(typeof(ComponentPoolExtensions), "Spawn", new[]
                {
                    typeof(Transform),
                    typeof(Transform),
                    typeof(Vector3),
                    typeof(Quaternion)
                });

                public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
                {
                    var codes = instructions.ToList();
                    var spawn_i = codes.FindIndex(ci => ci.Calls(Spawn));
                    var i = codes.FindIndex(spawn_i, 3, ci => ci.IsStloc()) + 1;
                    codes.InsertRange(i, new[]
                    {
                        new CodeInstruction(OpCodes.Ldloc_0),
                        new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(MoreOptionsMod), "displayBlockExplosionsBool")),
                        new CodeInstruction(OpCodes.Call, EnableRenderers_MI)
                    });
                    return codes;
                }
            }

            [HarmonyPatch(typeof(ModuleAntenna), "OnUpdate")]
            private static class ModuleAntennaFix
            {
                private static void Prefix(ref ModuleAntenna __instance)
                {
                    __instance.RequestGlow = displayAntennaGlowBool;
                }
            }

            [HarmonyPatch(typeof(TankBlock), "SwapMaterialAntiGrav")]
            private static class TankBlockFix
            {
                private static bool Prefix(ref TankBlock __instance)
                {
                    if (!antigravMatSwapBool)
                    {
                        ((MaterialSwapper)m_MaterialSwapper.GetValue(__instance)).SwapMaterialAntiGrav(false);
                    }

                    return antigravMatSwapBool;
                }
            }

            [HarmonyPatch(typeof(BubbleShield), "Update")]
            private static class BubbleShieldFix
            {
                private static void Prefix(ref BubbleShield __instance)
                {
                    var renderers = __instance.GetComponentsInChildren<Renderer>(true);
                    foreach (var r in renderers)
                    {
                        r.enabled = displayBubblesBool;
                    }
                }
            }
        }
    }
}
