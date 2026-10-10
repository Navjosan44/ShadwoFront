using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

namespace Shadowfront.UI
{
    public class HUD : MonoBehaviour
    {
        [Header(\"Health\")]
        public Slider healthBar;
        public TextMeshProUGUI healthText;
        
        [Header(\"Ammo\")]
        public TextMeshProUGUI ammoText;
        public TextMeshProUGUI reserveAmmoText;
        public Image reloadIndicator;
        
        [Header(\"Mission\")]
        public TextMeshProUGUI missionNameText;
        public TextMeshProUGUI objectiveText;
        public TextMeshProUGUI killCountText;
        public Slider extractionProgressBar;
        public GameObject extractionPanel;
        
        [Header(\"Weapon\")]
        public TextMeshProUGUI weaponNameText;
        public Image weaponIcon;
        
        [Header(\"Crosshair\")]
        public RectTransform crosshair;
        public float crosshairSpread = 20f;
        
        [Header(\"XP/Level\")]
        public TextMeshProUGUI levelText;
        public Slider xpBar;
        
        void Update()
        {
            UpdateHealth();
            UpdateAmmo();
            UpdateMission();
            UpdateWeapon();
            UpdateCrosshair();
            UpdateProgression();
        }
        
        void UpdateHealth()
        {
            var player = GameObject.FindGameObjectWithTag(\"Player\");
            if(player == null) return;
            var h = player.GetComponent<Shadowfront.Core.Health>();
            if(h != null && healthBar != null)
            {
                healthBar.value = h.Fraction;
                if(healthText != null) healthText.text = $"{h.current}/{h.max}";
            }
        }
        
        void UpdateAmmo()
        {
            var wm = FindObjectOfType<Shadowfront.Weapons.WeaponManager>();
            var gun = wm?.CurrentGun;
            if(gun != null && gun.data != null)
            {
                if(ammoText != null) ammoText.text = gun.currentAmmo.ToString();
                if(reserveAmmoText != null) reserveAmmoText.text = $"/{gun.reserveAmmo}";
                if(reloadIndicator != null) reloadIndicator.gameObject.SetActive(gun.isReloading);
            }
            else
            {
                if(ammoText != null) ammoText.text = \"--\";
                if(reserveAmmoText != null) reserveAmmoText.text = \"\";
                if(reloadIndicator != null) reloadIndicator.gameObject.SetActive(false);
            }
        }
        
        void UpdateMission()
        {
            var mm = Shadowfront.Missions.MissionManager.Instance;
            if(mm == null) return;
            
            if(missionNameText != null && mm.currentMission != null)
                missionNameText.text = mm.currentMission.missionName;
            
            if(objectiveText != null)
            {
                var incomplete = mm.Objectives.Where(o => !o.isComplete).ToList();
                if(incomplete.Count > 0)
                    objectiveText.text = incomplete[0].description;
                else
                    objectiveText.text = \"All objectives complete\";
            }
            
            if(killCountText != null)
                killCountText.text = $\"Kills: {mm.KillCount}\";
            
            if(extractionPanel != null)
                extractionPanel.SetActive(mm.State == Shadowfront.Missions.MissionState.Extracting);
            
            if(extractionProgressBar != null)
                extractionProgressBar.value = mm.ExtractionProgress;
        }
        
        void UpdateWeapon()
        {
            var wm = FindObjectOfType<Shadowfront.Weapons.WeaponManager>();
            var gun = wm?.CurrentGun;
            if(gun != null && gun.data != null)
            {
                if(weaponNameText != null) weaponNameText.text = gun.data.weaponName;
            }
        }
        
        void UpdateCrosshair()
        {
            var pc = Shadowfront.Player.PlayerController.Instance;
            if(pc == null || crosshair == null) return;
            
            float spread = pc.IsAiming ? 0 : crosshairSpread;
            Vector2 offset = new Vector2(spread, spread);
            crosshair.anchoredPosition = Vector2.Lerp(crosshair.anchoredPosition, offset, Time.deltaTime * 10f);
        }
        
        void UpdateProgression()
        {
            var prog = Shadowfront.Data.Progression.Instance;
            if(prog == null) return;
            
            if(levelText != null) levelText.text = $\"Lv {prog.level}\";
            if(xpBar != null) xpBar.value = (float)prog.xp / prog.xpToNextLevel;
        }
    }
}
