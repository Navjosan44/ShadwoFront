using UnityEngine;

namespace Shadowfront.Weapons
{
    [CreateAssetMenu(menuName="Shadowfront/Weapon Data")]
    public class WeaponData : ScriptableObject
    {
        public string weaponName = "Rifle";
        public GameObject bulletPrefab;
        public GameObject muzzleFlashPrefab;
        public AudioClip fireSound;
        public AudioClip reloadSound;
        public int damage = 25;
        public float fireRate = 0.1f; // rounds per second
        public int magazineSize = 30;
        public int maxAmmo = 120;
        public float reloadTime = 2f;
        public float range = 100f;
        public float recoilVertical = 2f;
        public float recoilHorizontal = 1f;
        public float aimRecoilMultiplier = 0.5f;
        public LayerMask hitLayers = -1;
    }
}
