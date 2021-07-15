using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using HarmonyLib;
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

		//private static bool doProcessFireBool;
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
		private static bool displayBubblesBool;

		public static void Load()
        {
            config = new ModConfig();
            foreach (FieldInfo f in typeof(MoreOptionsMod).GetFields(BindingFlags.Static | BindingFlags.NonPublic).Where(f => f.Name.EndsWith("Bool")))
            {
				config.BindConfig(null, f);
            }

            var harmony = new Harmony("exund.moreoptions");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

			var modName = "More Options";
			//doProcessFire = new OptionToggle("Process Fire", modName, doProcessFireBool);
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
			displayBubbles.onValueSaved.AddListener(() =>
			{
				displayBubblesBool = displayBubbles.SavedValue;
			});

			NativeOptionsMod.onOptionsSaved.AddListener(() =>
			{
				config.WriteConfigJsonFile();
			});
        }

        /*private static bool isCoroutineExecuting = false;

        static IEnumerator<WaitForSeconds> ExecuteAfterTime(float time, Action task)
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
				[HarmonyPatch(typeof(HoverJet), "Update")]
				private static class Update
				{
					private static void Postfix(ref HoverJet __instance)
					{
						EnableJetEffects.Invoke(__instance, new object[] { displayHoverEffectsBool });
					}
				}

				[HarmonyPatch(typeof(HoverJet), "OnAttach")]
				private static class OnAttach
				{
					private static void Postfix(ref HoverJet __instance)
					{
						EnableJetEffects.Invoke(__instance, new object[] { displayHoverEffectsBool });
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
				private static Dictionary<ModuleItemHolderBeam, GameObject> ItemHolderQuads = new Dictionary<ModuleItemHolderBeam, GameObject>();
				[HarmonyPatch(typeof(ModuleItemHolderBeam), "UpdateBeamEffects")]
				private static class UpdateBeamEffects
				{
					private static void Prefix(ref ModuleItemHolderBeam __instance)
					{
						var prefab = (GameObject)m_BeamQuadPrefab.GetValue(__instance);
						if (!ItemHolderQuads.ContainsKey(__instance)) ItemHolderQuads.Add(__instance, prefab);

						if (displayHolderBeamsBool)
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
                    if(!displayThrustersEffectsBool)
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
				private static Dictionary<MissileProjectile, Transform> MissileSmokePrefab = new Dictionary<MissileProjectile, Transform>();
				[HarmonyPatch(typeof(MissileProjectile), "ActivateBoosters")]
				private static class ActivateBoosters
				{
					private static void Prefix(ref MissileProjectile __instance)
					{
						var prefab = (Transform)m_SmokeTrailPrefab.GetValue(__instance);
						if (prefab != null && !MissileSmokePrefab.ContainsKey(__instance)) MissileSmokePrefab.Add(__instance, prefab);

						if (displayMissileSmokeBool)
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
                    if (!displayProjectileExplosionsBool)
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

                    return displayProjectileExplosionsBool;
                }
            }

            [HarmonyPatch(typeof(ModuleDamage), "Explode")]
            private static class ModuleDamageExplosionFix
            {
                private static bool Prefix(ref ModuleDamage __instance, ref bool withDamage)
                {
                    if (!displayBlockExplosionsBool)
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

                    return displayBlockExplosionsBool;
                }
            }

            [HarmonyPatch(typeof(ModuleAntenna), "Update")]
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
				static FieldInfo m_MaterialSwapper = typeof(TankBlock).GetField("m_MaterialSwapper", BindingFlags.NonPublic | BindingFlags.Instance);
				private static bool Prefix(ref TankBlock __instance)
				{
					if(!antigravMatSwapBool)
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
					foreach(var r in renderers)
                    {
						r.enabled = displayBubblesBool;
                    }
				}
			}
		}
    }
}
