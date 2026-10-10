using UnityEngine;

namespace Shadowfront.Weapons
{
    public class Gun : MonoBehaviour
    {
        public WeaponData data;
        public Transform muzzle;
        public Transform ejectPort;
        public AudioSource audioSource;
        public ParticleSystem muzzleFlash;
        
        [Header(\"State\")]
        public int currentAmmo;
        public int reserveAmmo;
        public bool isReloading;
        float nextFireTime;
        Vector3 recoilOffset;
        
        void Awake()
        {
            if(data != null)
            {
                currentAmmo = data.magazineSize;
                reserveAmmo = data.maxAmmo;
            }
        }
        
        public bool CanFire => Time.time >= nextFireTime && !isReloading && currentAmmo > 0;
        
        public void TryFire(Vector3 origin, Vector3 direction, out RaycastHit hit)
        {
            hit = default;
            if(!CanFire || data == null) return;
            
            nextFireTime = Time.time + data.fireRate;
            currentAmmo--;
            
            Events.GameEvents.ShotFired();
            
            if(muzzleFlash != null) muzzleFlash.Play();
            if(data.muzzleFlashPrefab != null && muzzle != null)
                Instantiate(data.muzzleFlashPrefab, muzzle.position, muzzle.rotation, muzzle);
            
            if(audioSource != null && data.fireSound != null)
                audioSource.PlayOneShot(data.fireSound);
            
            Vector3 spreadDir = direction;
            float vRecoil = data.recoilVertical * (Shadowfront.Player.PlayerController.Instance?.IsAiming == true ? data.aimRecoilMultiplier : 1f);
            float hRecoil = data.recoilHorizontal * (Shadowfront.Player.PlayerController.Instance?.IsAiming == true ? data.aimRecoilMultiplier : 1f);
            spreadDir = Quaternion.Euler(Random.Range(-vRecoil, vRecoil), Random.Range(-hRecoil, hRecoil), 0) * spreadDir;
            
            if(Physics.Raycast(origin, spreadDir, out hit, data.range, data.hitLayers))
            {
                var h = hit.collider.GetComponent<Health>();
                if(h != null)
                {
                    h.TakeDamage(data.damage);
                    Events.GameEvents.EnemyDamaged(data.damage);
                }
            }
            
            ApplyRecoil(vRecoil, hRecoil);
        }
        
        void ApplyRecoil(float v, float h)
        {
            var pc = Shadowfront.Player.PlayerController.Instance;
            if(pc != null && pc.cameraPivot != null)
            {
                recoilOffset += new Vector3(-v * 0.5f, Random.Range(-h, h) * 0.5f, 0);
                StartCoroutine(ResetRecoil());
            }
        }
        
        System.Collections.IEnumerator ResetRecoil()
        {
            yield return new WaitForSeconds(data.fireRate * 2f);
            float t = 0;
            while(t < 0.2f)
            {
                recoilOffset = Vector3.Lerp(recoilOffset, Vector3.zero, t / 0.2f);
                t += Time.deltaTime;
                yield return null;
            }
            recoilOffset = Vector3.zero;
        }
        
        public void TryReload()
        {
            if(isReloading || currentAmmo >= data.magazineSize || reserveAmmo <= 0) return;
            StartCoroutine(ReloadRoutine());
        }
        
        System.Collections.IEnumerator ReloadRoutine()
        {
            isReloading = true;
            Events.GameEvents.Reload();
            if(audioSource != null && data.reloadSound != null)
                audioSource.PlayOneShot(data.reloadSound);
            
            yield return new WaitForSeconds(data.reloadTime);
            
            int needed = data.magazineSize - currentAmmo;
            int taken = Mathf.Min(needed, reserveAmmo);
            currentAmmo += taken;
            reserveAmmo -= taken;
            isReloading = false;
        }
        
        public void AddAmmo(int amount)
        {
            reserveAmmo = Mathf.Min(reserveAmmo + amount, data.maxAmmo);
        }
    }
}
