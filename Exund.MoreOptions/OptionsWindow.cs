using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Exund.MoreOptions
{
    class OptionsWindow : MonoBehaviour
    {
        private int ID = 7789;
        private bool visible = false;
        private Rect win = new Rect((Screen.width-500f)/2, (Screen.height - 400f) / 2, 500f, 400f);

        public static bool doProcessFire = true;
        public static bool displayMuzzleFlashes = true;
        public static bool displayBulletsCasing = true;
        public static bool displaySmokeTrails = true;
        public static bool displayHoverEffects = true;
        public static bool displayRemoteChargersEffects = true;
        public static bool displayHolderBeams = true;
        public static bool displayThrustersEffects = true;
        public static bool displayMissileSmoke = true;
        public static bool displayProjectileExplosions = true;
        public static bool displayBlockExplosions = true;
        public static bool displayAntennaGlow = true;

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.O)) visible = !visible;
        }

        public void OnGUI()
        {
            
            if (visible) GUI.Window(ID, win, DoWindow, "Options");
        }

        public void DoWindow(int id)
        {
            //doProcessFire = GUILayout.Toggle(doProcessFire, nameof(doProcessFire));
            displayMuzzleFlashes = GUILayout.Toggle(displayMuzzleFlashes, nameof(displayMuzzleFlashes));
            displayBulletsCasing = GUILayout.Toggle(displayBulletsCasing, nameof(displayBulletsCasing));
            displaySmokeTrails = GUILayout.Toggle(displaySmokeTrails, nameof(displaySmokeTrails));
            displayHoverEffects = GUILayout.Toggle(displayHoverEffects, nameof(displayHoverEffects));
            displayRemoteChargersEffects = GUILayout.Toggle(displayRemoteChargersEffects, nameof(displayRemoteChargersEffects));
            displayHolderBeams = GUILayout.Toggle(displayHolderBeams, nameof(displayHolderBeams));
            displayThrustersEffects = GUILayout.Toggle(displayThrustersEffects, nameof(displayThrustersEffects));
            displayMissileSmoke = GUILayout.Toggle(displayMissileSmoke, nameof(displayMissileSmoke));
            displayProjectileExplosions = GUILayout.Toggle(displayProjectileExplosions, nameof(displayProjectileExplosions));
            displayBlockExplosions = GUILayout.Toggle(displayBlockExplosions, nameof(displayBlockExplosions));
            displayAntennaGlow = GUILayout.Toggle(displayAntennaGlow, nameof(displayAntennaGlow));

            if (GUILayout.Button("Save")) MoreOptionsMod.config.WriteConfigJsonFile();
        }
    }
}
