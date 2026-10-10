using UnityEngine;

namespace Shadowfront.Weapons
{
    public class WeaponManager : MonoBehaviour
    {
        public Gun[] weapons;
        int currentIndex = 0;
        Gun currentGun;
        
        void Awake()
        {
            if(weapons.Length > 0) Equip(0);
        }
        
        void Update()
        {
            var input = Shadowfront.Input.InputManager.Instance;
            if(input == null) return;
            
            if(input.WeaponSwitchNext) SwitchNext();
            if(input.WeaponSwitchPrev) SwitchPrev();
            
            if(input.FirePressed && currentGun != null)
            {
                var pc = Shadowfront.Player.PlayerController.Instance;
                if(pc != null && pc.cameraPivot != null)
                {
                    var cam = Camera.main;
                    if(currentGun.CanFire)
                        currentGun.TryFire(cam.transform.position, cam.transform.forward, out _);
                }
                input.ConsumeFire();
            }
            
            if(input.ReloadPressed && currentGun != null)
                currentGun.TryReload();
        }
        
        public void SwitchNext()
        {
            if(weapons.Length == 0) return;
            int next = (currentIndex + 1) % weapons.Length;
            Equip(next);
        }
        
        public void SwitchPrev()
        {
            if(weapons.Length == 0) return;
            int prev = (currentIndex - 1 + weapons.Length) % weapons.Length;
            Equip(prev);
        }
        
        void Equip(int index)
        {
            if(index < 0 || index >= weapons.Length) return;
            if(currentGun != null) currentGun.gameObject.SetActive(false);
            currentIndex = index;
            currentGun = weapons[index];
            currentGun.gameObject.SetActive(true);
        }
        
        public Gun CurrentGun => currentGun;
    }
}
