using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

namespace Shadowfront.Audio
{
    [System.Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;
        public AudioMixerGroup mixerGroup;
        [Range(0f,1f)] public float volume = 1f;
        [Range(0.1f,3f)] public float pitch = 1f;
        public bool loop = false;
        public bool playOnAwake = false;
        [Range(0f,1f)] public float spatialBlend = 1f; // 3D
        public float minDistance = 1f;
        public float maxDistance = 50f;
        [HideInInspector] public AudioSource source;
    }
    
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }
        public Sound[] sounds;
        public AudioMixerGroup masterGroup;
        public AudioMixerGroup musicGroup;
        public AudioMixerGroup sfxGroup;
        
        Dictionary<string, Sound> soundDict = new Dictionary<string, Sound>();
        
        void Awake()
        {
            if(Instance!=null && Instance!=this){ Destroy(gameObject); return; }
            Instance=this; DontDestroyOnLoad(gameObject);
            
            foreach(var s in sounds)
            {
                s.source = gameObject.AddComponent<AudioSource>();
                s.source.clip = s.clip;
                s.source.outputAudioMixerGroup = s.mixerGroup;
                s.source.volume = s.volume;
                s.source.pitch = s.pitch;
                s.source.loop = s.loop;
                s.source.spatialBlend = s.spatialBlend;
                s.source.minDistance = s.minDistance;
                s.source.maxDistance = s.maxDistance;
                s.source.rolloffMode = AudioRolloffMode.Linear;
                if(s.playOnAwake) s.source.Play();
                soundDict[s.name] = s;
            }
        }
        
        public void Play(string name, Vector3? position = null)
        {
            if(!soundDict.TryGetValue(name, out var s)) return;
            if(position.HasValue && s.spatialBlend > 0)
            {
                AudioSource.PlayClipAtPoint(s.clip, position.Value, s.volume);
            }
            else
            {
                s.source.Play();
            }
        }
        
        public void Stop(string name)
        {
            if(soundDict.TryGetValue(name, out var s)) s.source.Stop();
        }
        
        public void SetVolume(string name, float volume)
        {
            if(soundDict.TryGetValue(name, out var s))
            {
                s.volume = volume;
                s.source.volume = volume;
            }
        }
        
        public void SetMasterVolume(float v) => AudioListener.volume = v;
    }
}
