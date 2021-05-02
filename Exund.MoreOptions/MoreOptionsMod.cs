using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Harmony;
using UnityEngine;
using ModHelper.Config;
using Nuterra.NativeOptions;

namespace Exund.MoreOptions
{
    public class MoreOptionsMod
    {
		static readonly BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;  
        public static MethodInfo EnableJetEffects = typeof(HoverJet).GetMethod("EnableJetEffects", flags);
        public static FieldInfo line = typeof(SmokeTrail).GetField("line", flags);
		static readonly Type T_CannonBarrel = typeof(CannonBarrel);
		public static FieldInfo recoiling = T_CannonBarrel.GetField("recoiling", flags);
		public static FieldInfo recoilAnim = T_CannonBarrel.GetField("recoilAnim", flags);
		public static FieldInfo animState = T_CannonBarrel.GetField("animState", flags);
		public static FieldInfo m_BeamQuadPrefab = typeof(ModuleItemHolderBeam).GetField("m_BeamQuadPrefab", flags);
		public static FieldInfo m_Trails = typeof(BoosterJet).GetField("m_Trails", flags);
		public static FieldInfo m_SmokeTrailPrefab = typeof(MissileProjectile).GetField("m_SmokeTrailPrefab", flags);
		public static FieldInfo m_Explosion = typeof(Projectile).GetField("m_Explosion", flags);

		public static ModConfig config;

		private static OptionToggle doProcessFire;
		private static OptionToggle displayMuzzleFlashes;
		private static OptionToggle displayBulletsCasing;
		private static OptionToggle displaySmokeTrails;
		private static OptionToggle displayHoverEffects;
		private static OptionToggle displayRemoteChargersEffects;
		private static OptionToggle displayHolderBeams;
		private static OptionToggle displayThrustersEffects;
		private static OptionToggle displayMissileSmoke;
		private static OptionToggle displayProjectileExplosions;
		private static OptionToggle displayBlockExplosions;
		private static OptionToggle displayAntennaGlow;
		private static OptionToggle antigravMatSwap;

		private static bool doProcessFireBool;
		private static bool displayMuzzleFlashesBool;
		private static bool displayBulletsCasingBool;
		private static bool displaySmokeTrailsBool;
		private static bool displayHoverEffectsBool;
		private static bool displayRemoteChargersEffectsBool;
		private static bool displayHolderBeamsBool;
		private static bool displayThrustersEffectsBool;
		private static bool displayMissileSmokeBool;
		private static bool displayProjectileExplosionsBool;
		private static bool displayBlockExplosionsBool;
		private static bool displayAntennaGlowBool;
		private static bool antigravMatSwapBool;

		public static void Load()
        {
            config = new ModConfig();
            foreach (FieldInfo f in typeof(MoreOptionsMod).GetFields(BindingFlags.Static | BindingFlags.NonPublic).Where(f => f.Name.EndsWith("Bool")))
            {
				config.BindConfig(null, f);
            }

            var harmony = HarmonyInstance.Create("exund.moreoptions");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

			//doProcessFire = new OptionToggle("Process Fire", "More Options", doProcessFireBool);
			displayMuzzleFlashes = new OptionToggle("Muzzle Flashes", "More Options", displayMuzzleFlashesBool);
			displayBulletsCasing = new OptionToggle("Bullets Casing", "More Options", displayBulletsCasingBool);
			displaySmokeTrails = new OptionToggle("Smoke Trails", "More Options", displaySmokeTrailsBool);
			displayHoverEffects = new OptionToggle("Hover Effects", "More Options", displayHoverEffectsBool);
			displayRemoteChargersEffects = new OptionToggle("Remote Chargers Effects", "More Options", displayRemoteChargersEffectsBool);
			displayHolderBeams = new OptionToggle("Holder Beams", "More Options", displayHolderBeamsBool);
			displayThrustersEffects = new OptionToggle("Thrusters Effects", "More Options", displayThrustersEffectsBool);
			displayMissileSmoke = new OptionToggle("Missile Smoke", "More Options", displayMissileSmokeBool);
			displayProjectileExplosions = new OptionToggle("Projectile Explosions", "More Options", displayProjectileExplosionsBool);
			displayBlockExplosions = new OptionToggle("Block Explosions", "More Options", displayBlockExplosionsBool);
			displayAntennaGlow = new OptionToggle("Antenna Glow", "More Options", displayAntennaGlowBool);
			antigravMatSwap = new OptionToggle("Antigrav color pulse", "More Options", antigravMatSwapBool);


			/*doProcessFire.onValueSaved.AddListener(() =>
			{
				doProcessFireBool = doProcessFire.SavedValue;
			});*/
			displayMuzzleFlashes.onValueSaved.AddListener(() =>
			{
				displayMuzzleFlashesBool = displayMuzzleFlashes.SavedValue;
			});
			displayBulletsCasing.onValueSaved.AddListener(() =>
			{
				displayBulletsCasingBool = displayBulletsCasing.SavedValue;
			});
			displaySmokeTrails.onValueSaved.AddListener(() =>
			{
				displaySmokeTrailsBool = displaySmokeTrails.SavedValue;
			});
			displayHoverEffects.onValueSaved.AddListener(() =>
			{
				displayHoverEffectsBool = displayHoverEffects.SavedValue;
			});
			displayRemoteChargersEffects.onValueSaved.AddListener(() =>
			{
				displayRemoteChargersEffectsBool = displayRemoteChargersEffects.SavedValue;
			});
			displayHolderBeams.onValueSaved.AddListener(() =>
			{
				displayHolderBeamsBool = displayHolderBeams.SavedValue;
			});
			displayThrustersEffects.onValueSaved.AddListener(() =>
			{
				displayThrustersEffectsBool = displayThrustersEffects.SavedValue;
			});
			displayMissileSmoke.onValueSaved.AddListener(() =>
			{
				displayMissileSmokeBool = displayMissileSmoke.SavedValue;
			});
			displayProjectileExplosions.onValueSaved.AddListener(() =>
			{
				displayProjectileExplosionsBool = displayProjectileExplosions.SavedValue;
			});
			displayBlockExplosions.onValueSaved.AddListener(() =>
			{
				displayBlockExplosionsBool = displayBlockExplosions.SavedValue;
			});
			displayAntennaGlow.onValueSaved.AddListener(() =>
			{
				displayAntennaGlowBool = displayAntennaGlow.SavedValue;
			});
			antigravMatSwap.onValueSaved.AddListener(() =>
			{
				antigravMatSwapBool = antigravMatSwap.SavedValue;
			});

			NativeOptionsMod.onOptionsSaved.AddListener(() =>
			{
				config.WriteConfigJsonFile();
			});
        }

        private static bool isCoroutineExecuting = false;

        /*static IEnumerator<WaitForSeconds> ExecuteAfterTime(float time, Action task)
        {
            if (isCoroutineExecuting)
                yield break;

            isCoroutineExecuting = true;

            yield return new WaitForSeconds(time);
            task();
            

            isCoroutineExecuting = false;
        }*/

        internal class Patches
        {
            /*[HarmonyPatch(typeof(CannonBarrel), "ProcessFire")]
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
                    return OptionsWindow.doProcessFire;
                    return true;
                }
            }

            [HarmonyPatch(typeof(CannonBarrel), "Update")]
            private static class Update
            {
                private static bool Prefix()
                {
                    return doProcessFire.SavedValue;
                }
            }*/

            [HarmonyPatch(typeof(CannonBarrel), "EjectCasing")]
            private static class EjectCasing
            {
                private static bool Prefix()
                {
                    return displayBulletsCasing.SavedValue;
                }
            }

            [HarmonyPatch(typeof(MuzzleFlash), "Fire")]
            private static class MuzzleFlashFix
            {
                private static bool Prefix()
                {
                    return displayMuzzleFlashes.SavedValue;
                }
            }

            [HarmonyPatch(typeof(SmokeTrail), "Update")]
            private static class SmokeTrailFix
            {
                private static bool Prefix(ref SmokeTrail __instance)
                {
                    var iline = (LineRenderer)line.GetValue(__instance);
                    iline.enabled = displaySmokeTrails.SavedValue;
                    return iline.enabled;
                }
            }

			private static class HoverJetFix
			{
				[HarmonyPatch(typeof(HoverJet), "Update")]
				private static class Update
				{
					private static void Postfix(ref HoverJet __instance)
					{
						EnableJetEffects.Invoke(__instance, new object[] { displayHoverEffects.SavedValue });
					}
				}

				[HarmonyPatch(typeof(HoverJet), "OnAttach")]
				private static class OnAttach
				{
					private static void Postfix(ref HoverJet __instance)
					{
						EnableJetEffects.Invoke(__instance, new object[] { displayHoverEffects.SavedValue });
					}
				}
			} 

            [HarmonyPatch(typeof(ModuleRemoteCharger), "FireNewArcEffect")]
            private static class ModuleRemoteChargerFix
            {
                private static bool Prefix()
                {
                    return displayRemoteChargersEffects.SavedValue;
                }
            }

			private static class ModuleItemHolderBeamFix
			{
				private static Dictionary<ModuleItemHolderBeam, GameObject> ItemHolderQuads = new Dictionary<ModuleItemHolderBeam, GameObject>();
				[HarmonyPatch(typeof(ModuleItemHolderBeam), "UpdateBeamEffects")]
				private static class UpdateBeamEffects
				{
					private static void Prefix(ref ModuleItemHolderBeam __instance)
					{
						var prefab = (GameObject)m_BeamQuadPrefab.GetValue(__instance);
						if (!ItemHolderQuads.ContainsKey(__instance)) ItemHolderQuads.Add(__instance, prefab);

						if (displayHolderBeams.SavedValue)
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
				private static class OnRecycle
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
			}

            [HarmonyPatch(typeof(BoosterJet), "Update")]
            private static class BoosterJetFix
            {
                private static bool Prefix(ref BoosterJet __instance)
                {
                    if(!displayThrustersEffects.SavedValue)
                    {
                        var trails = (JetTrail[])m_Trails.GetValue(__instance);
                        foreach (JetTrail jetTrail in trails)
                        {
                            jetTrail.Cease();
                        }
                    }

                    return displayThrustersEffects.SavedValue;
                }
            }

			private static class MissileProjectileFix
			{
				private static Dictionary<MissileProjectile, Transform> MissileSmokePrefab = new Dictionary<MissileProjectile, Transform>();
				[HarmonyPatch(typeof(MissileProjectile), "ActivateBoosters")]
				private static class ActivateBoosters
				{
					private static void Prefix(ref MissileProjectile __instance)
					{
						var prefab = (Transform)m_SmokeTrailPrefab.GetValue(__instance);
						if (prefab != null && !MissileSmokePrefab.ContainsKey(__instance)) MissileSmokePrefab.Add(__instance, prefab);

						if (displayMissileSmoke.SavedValue)
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
				private static class Recycle
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
			}

            [HarmonyPatch(typeof(Projectile), "SpawnExplosion")]
            private static class ProjectileExplosionsFix
            {
                private static bool Prefix(ref Projectile __instance, ref Vector3 explodePos)
                {
                    if (!displayProjectileExplosions.SavedValue)
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

                    return displayProjectileExplosions.SavedValue;
                }
            }

            [HarmonyPatch(typeof(ModuleDamage), "Explode")]
            private static class ModuleDamageExplosionFix
            {
                private static bool Prefix(ref ModuleDamage __instance, ref bool withDamage)
                {
                    if (!displayBlockExplosions.SavedValue)
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

                    return displayBlockExplosions.SavedValue;
                }
            }

            [HarmonyPatch(typeof(ModuleAntenna), "Update")]
            private static class ModuleAntennaFix
            {
                private static void Prefix(ref ModuleAntenna __instance)
                {
                    __instance.RequestGlow = displayAntennaGlow.SavedValue;
                }
            }

			[HarmonyPatch(typeof(TankBlock), "SwapMaterialAntiGrav")]
			private static class TankBlockFix
			{
				static FieldInfo m_MaterialSwapper = typeof(TankBlock).GetField("m_MaterialSwapper", BindingFlags.NonPublic | BindingFlags.Instance);
				private static bool Prefix(ref TankBlock __instance)
				{
					if(!antigravMatSwap.SavedValue)
					{
						((MaterialSwapper)m_MaterialSwapper.GetValue(__instance)).SwapMaterialAntiGrav(false);
					}
					return antigravMatSwap.SavedValue;
				}
			}
        }
    }
}
