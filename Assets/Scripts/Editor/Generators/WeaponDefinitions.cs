#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Shadowfront.Weapons;

public static class WeaponDefinitions
{
    [MenuItem(\"Shadowfront/Generate/Weapon Definitions\")]
    public static void Generate()
    {
        CreateWeapon(\"Pistol\", 20, 0.25f, 12, 48, 1.5f, 30f, 1.5f, 1f, 0.5f);
        CreateWeapon(\"Rifle\", 25, 0.1f, 30, 120, 2f, 100f, 2f, 1f, 0.5f);
        CreateWeapon(\"SMG\", 15, 0.06f, 40, 160, 1.8f, 50f, 2.5f, 1.5f, 0.6f);
        CreateWeapon(\"Shotgun\", 40, 0.8f, 6, 24, 2.5f, 25f, 4f, 3f, 0.7f);
        CreateWeapon(\"Sniper\", 80, 1.2f, 5, 20, 2.8f, 200f, 1f, 0.5f, 0.3f);
        
        AssetDatabase.Refresh();
        Debug.Log(\"[Shadowfront] Weapon definitions generated\");
    }
    
    static void CreateWeapon(string name, int damage, float fireRate, int magSize, int maxAmmo, float reloadTime, float range, float vRecoil, float hRecoil, float aimMult)
    {
        var data = ScriptableObject.CreateInstance<WeaponData>();
        data.weaponName = name;
        data.damage = damage;
        data.fireRate = fireRate;
        data.magazineSize = magSize;
        data.maxAmmo = maxAmmo;
        data.reloadTime = reloadTime;
        data.range = range;
        data.recoilVertical = vRecoil;
        data.recoilHorizontal = hRecoil;
        data.aimRecoilMultiplier = aimMult;
        data.hitLayers = ~LayerMask.GetMask(\"Ignore Raycast\");
        
        string path = $\"Assets/Resources/Weapons/{name}.asset\";
        System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
        AssetDatabase.CreateAsset(data, path);
    }
}
#endif
