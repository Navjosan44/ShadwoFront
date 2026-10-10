using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Shadowfront.UI.Screens
{
    public class WeaponSelectScreen : MonoBehaviour
    {
        public GameObject panel;
        public Transform weaponsContainer;
        public GameObject weaponSlotPrefab;
        public Button closeButton;
        
        Shadowfront.Weapons.WeaponManager weaponManager;
        
        void Awake()
        {
            panel?.SetActive(false);
            closeButton?.onClick.AddListener(Close);
            weaponManager = FindObjectOfType<Shadowfront.Weapons.WeaponManager>();
        }
        
        public void Open()
        {
            panel?.SetActive(true);
            Refresh();
            Time.timeScale = 0f;
        }
        
        public void Close()
        {
            panel?.SetActive(false);
            Time.timeScale = 1f;
        }
        
        public void Refresh()
        {
            if(weaponsContainer == null || weaponManager == null) return;
            
            foreach(Transform child in weaponsContainer) Destroy(child.gameObject);
            
            for(int i=0;i<weaponManager.weapons.Length;i++)
            {
                int idx = i;
                var gun = weaponManager.weapons[i];
                var slot = Instantiate(weaponSlotPrefab, weaponsContainer);
                var txt = slot.GetComponentInChildren<TextMeshProUGUI>();
                if(txt != null && gun.data != null)
                    txt.text = $"{gun.data.weaponName}\nAmmo: {gun.currentAmmo}/{gun.reserveAmmo}";
                slot.GetComponent<Button>().onClick.AddListener(() => weaponManager.Equip(idx));
            }
        }
        
        void Update()
        {
            if(panel != null && panel.activeSelf && Shadowfront.Input.InputManager.Instance?.PausePressed == true)
                Close();
        }
    }
}
