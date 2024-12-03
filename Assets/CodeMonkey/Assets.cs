/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading the Code Monkey Utilities
    I hope you find them useful in your projects
    If you have any questions use the contact form
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using GameEvents;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace CodeMonkey {

    /*
     * Global Asset references
     * Edit Asset references in the prefab CodeMonkey/Resources/CodeMonkeyAssets
     * */
    public class Assets : MonoBehaviour {

        // Internal instance reference
        private static Assets _i; 

        // Instance reference
        public static Assets i {
            get {
                if (_i == null) _i = Instantiate(Resources.Load<Assets>("CodeMonkeyAssets")); 
                return _i; 
            }
        }


        // All references
        
        public Sprite s_White;
        public Sprite s_Circle;

        public Material m_White;

        public Transform damagePopup;
        public Transform playerDamagePopup;
        public Transform zombieDamagePopup;
        public Transform explosion;

        public ScreenShakeProfile bigExplosionSSP;
        public ScreenShakeProfile smallExplosionSSP;
        public ScreenShakeProfile playerDamagedSSP;

        public AudioClip bigExplosionSound;
        public AudioClip smallExplosionSound;

        public AudioClip buttonDown;
        public AudioClip buttonUp;
        public AudioClip openLocker;
        public AudioClip openBackpack;

        public VoidEvent onPlayerDied;

        public CollectibleContainerData handsData;
        public CollectibleContainerData headData;
        public CollectibleContainerData backpackData;
        public FloorContainerData vanFloorData;

        public GameObject effectPrefab;

        public ItemData[] zoneUnlockItems;

        public GameObject[] smallBloodPools;
        public GameObject[] mediumBloodPools;
        public GameObject[] largeBloodPools;

        public GameObject[] enemyPrefabs;

        public GameObject miniZombie;

        public ItemData ZBGData;

        public PlayerData activePlayerData;

        public TMP_Text marketDaysText;
    }

}
