using UnityEngine;

namespace Shadowfront.Weapons
{
    public class MuzzleFlash : MonoBehaviour
    {
        public ParticleSystem ps;
        public Light flashLight;
        public float duration = 0.05f;
        
        void Awake()
        {
            ps = GetComponent<ParticleSystem>();
            flashLight = GetComponent<Light>();
        }
        
        public void Play()
        {
            if(ps != null) ps.Play();
            if(flashLight != null) StartCoroutine(FlashLight());
        }
        
        System.Collections.IEnumerator FlashLight()
        {
            flashLight.enabled = true;
            yield return new WaitForSeconds(duration);
            flashLight.enabled = false;
        }
    }
}
