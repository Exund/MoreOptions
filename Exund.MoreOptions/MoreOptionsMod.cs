using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Harmony;
using UnityEngine;
using ModHelper.Config;

namespace Exund.MoreOptions
{
    public class MoreOptionsMod
    {
        public static MethodInfo EnableJetEffects;
        public static FieldInfo line;
        public static FieldInfo recoiling;
        public static FieldInfo recoilAnim;
        public static FieldInfo animState;
        public static FieldInfo m_BeamQuadPrefab;
        public static FieldInfo m_Trails;
        public static FieldInfo m_SmokeTrailPrefab;
        public static FieldInfo m_Explosion;

        public static ModConfig config;

        public static void Load()
        {
            config = new ModConfig();
            foreach (FieldInfo f in typeof(OptionsWindow).GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                config.BindConfig<OptionsWindow>(null, f.Name);
            }


            EnableJetEffects = typeof(HoverJet).GetMethod("EnableJetEffects", BindingFlags.Instance | BindingFlags.NonPublic);
            line = typeof(SmokeTrail).GetField("line", BindingFlags.Instance | BindingFlags.NonPublic);
            recoiling = typeof(CannonBarrel).GetField("recoiling", BindingFlags.Instance | BindingFlags.NonPublic);
            recoilAnim = typeof(CannonBarrel).GetField("recoilAnim", BindingFlags.Instance | BindingFlags.NonPublic);
            animState = typeof(CannonBarrel).GetField("animState", BindingFlags.Instance | BindingFlags.NonPublic);
            m_BeamQuadPrefab = typeof(ModuleItemHolderBeam).GetField("m_BeamQuadPrefab", BindingFlags.Instance | BindingFlags.NonPublic);
            m_Trails = typeof(BoosterJet).GetField("m_Trails", BindingFlags.Instance | BindingFlags.NonPublic);
            m_SmokeTrailPrefab = typeof(MissileProjectile).GetField("m_SmokeTrailPrefab", BindingFlags.Instance | BindingFlags.NonPublic);
            m_Explosion = typeof(Projectile).GetField("m_Explosion", BindingFlags.Instance | BindingFlags.NonPublic);

            var harmony = HarmonyInstance.Create("exund.prodcedural.blocks");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            var _holder = new GameObject();
            _holder.AddComponent<OptionsWindow>();
            GameObject.DontDestroyOnLoad(_holder);

            
        }

        private static bool isCoroutineExecuting = false;

        static IEnumerator<WaitForSeconds> ExecuteAfterTime(float time, Action task)
        {
            if (isCoroutineExecuting)
                yield break;

            isCoroutineExecuting = true;

            yield return new WaitForSeconds(time);
            task();
            

            isCoroutineExecuting = false;
        }

        internal class Patches
        {
            [HarmonyPatch(typeof(CannonBarrel), "ProcessFire")]
            private static class ProcessFire
            {
                private static bool Prefix(ref CannonBarrel __instance, ref bool __result)
                {
                    /*if (recoilAnim.GetValue(__instance) != null && !OptionsWindow.doProcessFire) recoiling.SetValue(__instance, true);
                    __result = true;
                    var anim = (AnimationState)animState.GetValue(__instance);
                    var i = __instance;
                    __instance.StartCoroutine(ExecuteAfterTime(anim.length, () =>
                    {
                        recoiling.SetValue(i, false);
                    }));
                    return OptionsWindow.doProcessFire;*/
                    return true;
                }
            }

            [HarmonyPatch(typeof(CannonBarrel), "Update")]
            private static class Update
            {
                private static bool Prefix()
                {
                    return OptionsWindow.doProcessFire;
                }
            }

            [HarmonyPatch(typeof(CannonBarrel), "EjectCasing")]
            private static class EjectCasing
            {
                private static bool Prefix()
                {
                    return OptionsWindow.displayBulletsCasing;
                }
            }

            [HarmonyPatch(typeof(MuzzleFlash), "Fire")]
            private static class MuzzleFlashFix
            {
                private static bool Prefix()
                {
                    return OptionsWindow.displayMuzzleFlashes;
                }
            }

            [HarmonyPatch(typeof(SmokeTrail), "Update")]
            private static class SmokeTrailFix
            {
                private static bool Prefix(ref SmokeTrail __instance)
                {
                    var iline = (LineRenderer)line.GetValue(__instance);
                    iline.enabled = OptionsWindow.displaySmokeTrails;
                    return iline.enabled;
                }
            }

            [HarmonyPatch(typeof(HoverJet), "Update")]
            private static class HoverJet1
            {
                private static void Postfix(ref HoverJet __instance)
                {
                    EnableJetEffects.Invoke(__instance, new object[] { OptionsWindow.displayHoverEffects });
                }
            }
            [HarmonyPatch(typeof(HoverJet), "OnAttach")]
            private static class HoverJet2
            {
                private static void Postfix(ref HoverJet __instance)
                {
                    EnableJetEffects.Invoke(__instance, new object[] { OptionsWindow.displayHoverEffects });
                }
            }

            [HarmonyPatch(typeof(ModuleRemoteCharger), "FireNewArcEffect")]
            private static class ModuleRemoteChargerFix
            {
                private static bool Prefix()
                {
                    return OptionsWindow.displayRemoteChargersEffects;
                }
            }

            private static Dictionary<ModuleItemHolderBeam, GameObject> ItemHolderQuads = new Dictionary<ModuleItemHolderBeam, GameObject>();
            [HarmonyPatch(typeof(ModuleItemHolderBeam), "UpdateBeamEffects")]
            private static class ModuleItemHolderBeamFix
            {
                private static void Prefix(ref ModuleItemHolderBeam __instance)
                {
                    var prefab = (GameObject)m_BeamQuadPrefab.GetValue(__instance);
                    if (!ItemHolderQuads.ContainsKey(__instance)) ItemHolderQuads.Add(__instance, prefab);

                    if (OptionsWindow.displayHolderBeams)
                    {
                        if (ItemHolderQuads[__instance] != prefab) m_BeamQuadPrefab.SetValue(__instance, ItemHolderQuads[__instance]);
                    }
                    else if (prefab != null)
                    {
                        m_BeamQuadPrefab.SetValue(__instance, null);
                    }
                }
            }
            [HarmonyPatch(typeof(ModuleItemHolderBeam), "OnRecycle")]
            private static class ModuleItemHolderBeamFix2
            {
                private static void Prefix(ref ModuleItemHolderBeam __instance)
                {
                    if (ItemHolderQuads.ContainsKey(__instance))
                    {
                        m_BeamQuadPrefab.SetValue(__instance, ItemHolderQuads[__instance]);
                        ItemHolderQuads.Remove(__instance);
                    }
                }
            }

            [HarmonyPatch(typeof(BoosterJet), "Update")]
            private static class BoosterJetFix
            {
                private static bool Prefix(ref BoosterJet __instance)
                {
                    if(!OptionsWindow.displayThrustersEffects)
                    {
                        var trails = (JetTrail[])m_Trails.GetValue(__instance);
                        foreach (JetTrail jetTrail in trails)
                        {
                            jetTrail.Cease();
                        }
                    }

                    return OptionsWindow.displayThrustersEffects;
                }
            }

            private static Dictionary<MissileProjectile, Transform> MissileSmokePrefab = new Dictionary<MissileProjectile, Transform>();
            [HarmonyPatch(typeof(MissileProjectile), "ActivateBoosters")]
            private static class MissileProjectileActivateBoostersFix
            {
                private static void Prefix(ref MissileProjectile __instance)
                {
                    var prefab = (Transform)m_SmokeTrailPrefab.GetValue(__instance);
                    if (prefab != null && !MissileSmokePrefab.ContainsKey(__instance)) MissileSmokePrefab.Add(__instance, prefab);

                    if (OptionsWindow.displayMissileSmoke)
                    {
                        if (MissileSmokePrefab[__instance] != prefab) m_SmokeTrailPrefab.SetValue(__instance, MissileSmokePrefab[__instance]);
                    }
                    else if (prefab != null)
                    {
                        m_SmokeTrailPrefab.SetValue(__instance, null);
                    }
                }
            }

            [HarmonyPatch(typeof(MissileProjectile), "OnRecycle")]
            private static class MissileProjectileRecycleFix
            {
                private static void Prefix(ref MissileProjectile __instance)
                {
                    if (MissileSmokePrefab.ContainsKey(__instance))
                    {
                        m_SmokeTrailPrefab.SetValue(__instance, MissileSmokePrefab[__instance]);
                        MissileSmokePrefab.Remove(__instance);
                    }
                }
            }

            [HarmonyPatch(typeof(Projectile), "SpawnExplosion")]
            private static class ProjectileExplosionsFix
            {
                private static bool Prefix(ref Projectile __instance, ref Vector3 explodePos)
                {
                    if (!OptionsWindow.displayProjectileExplosions)
                    {
                        var exp = (Transform)m_Explosion.GetValue(__instance);
                        if (exp)
                        {
                            Transform transform = exp.Spawn(Singleton.dynamicContainer, explodePos);
                            foreach (ParticleSystem ps in transform.GetComponentsInChildren<ParticleSystem>())
                            {
                                ps.maxParticles = 0;
                            }
                            Explosion component = transform.GetComponent<Explosion>();
                            if (component != null)
                            {
                                component.SetDamageSource(__instance.Shooter);
                            }
                        }
                    }

                    return OptionsWindow.displayProjectileExplosions;
                }
            }

            [HarmonyPatch(typeof(ModuleDamage), "Explode")]
            private static class ModuleDamageExplosionFix
            {
                private static bool Prefix(ref ModuleDamage __instance, ref bool withDamage)
                {
                    if (!OptionsWindow.displayBlockExplosions)
                    {
                        Transform transform = __instance.deathExplosion.Spawn(Singleton.dynamicContainer, __instance.block.centreOfMassWorld, __instance.block.trans.rotation);
                        foreach (ParticleSystem ps in transform.GetComponentsInChildren<ParticleSystem>())
                        {
                            ps.maxParticles = 0;
                        }
                        Explosion component = transform.GetComponent<Explosion>();
                        if (component)
                        {
                            component.SetDamageSource(__instance.block.DamageInEffect.SourceTank);
                            component.DoDamage = withDamage;
                            component.SetCorpType(Singleton.Manager<ManSpawn>.inst.GetCorporation((BlockTypes)__instance.block.visible.ItemType));
                        }
                    }

                    return OptionsWindow.displayBlockExplosions;
                }
            }

            [HarmonyPatch(typeof(ModuleAntenna), "Update")]
            private static class ModuleAntennaFix
            {
                private static void Prefix(ref ModuleAntenna __instance)
                {
                    __instance.RequestGlow = OptionsWindow.displayAntennaGlow;
                }
            }
        }
    }
}
