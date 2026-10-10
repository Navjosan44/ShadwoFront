using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Shadowfront.Graphics
{
    [System.Serializable]
    public class QualityPreset
    {
        public string name;
        public int targetFPS = 60;
        public int renderResolutionScale = 100;
        public int shadowResolution = 1024;
        public int msaa = 2;
        public float viewDistance = 200f;
        public bool useShadows = true;
        public bool usePostProcessing = true;
        public bool useFog = true;
        public int maxParticles = 50;
        public float lodBias = 1f;
    }
    
    public class GraphicsManager : MonoBehaviour
    {
        public static GraphicsManager Instance { get; private set; }
        
        public QualityPreset[] presets;
        public int currentPresetIndex = 1; // Medium default
        
        void Awake()
        {
            if(Instance!=null && Instance!=this){ Destroy<GameObject>(); return; }
            Instance=this; DontDestroyOnLoad(transform.root);
            InitializePresets();
        }
        
        void InitializePresets()
        {
            if(presets != null && presets.Length > 0) return;
            
            presets = new QualityPreset[3];
            presets[0] = new QualityPreset { name=\"Low\", targetFPS=30, renderResolutionScale=70, shadowResolution=512, msaa=1, viewDistance=100f, useShadows=false, usePostProcessing=false, useFog=false, maxParticles=20, lodBias=0.5f };
            presets[1] = new QualityPreset { name=\"Medium\", targetFPS=60, renderResolutionScale=100, shadowResolution=1024, msaa=2, viewDistance=200f, useShadows=true, usePostProcessing=true, useFog=true, maxParticles=50, lodBias=1f };
            presets[2] = new QualityPreset { name=\"High\", targetFPS=60, renderResolutionScale=150, shadowResolution=2048, msaa=4, viewDistance=400f, useShadows=true, usePostProcessing=true, useFog=true, maxParticles=100, lodBias=1.5f };
        }
        
        public void SetQuality(int index)
        {
            if(presets == null || index < 0 || index >= presets.Length) return;
            currentPresetIndex = index;
            var p = presets[index];
            
            // Render resolution
            Screen.SetResolution(Screen.width, Screen.height, Screen.fullScreen, p.targetFPS);
            QualitySettings.SetQualityLevel(index);
            Application.targetFrameRate = p.targetFPS;
            
            // URP settings
            var urp = QualitySettings.renderPipeline as UniversalRenderPipelineAsset;
            if(urp != null)
            {
                urp.m_MSAA = (Rendering.Antialiasing)p.msaa;
                urp.m_RequireDepthTexture = p.usePostProcessing;
                urp.m_RequireOpaqueTexture = p.usePostProcessing;
            }
            
            // Dynamic resolution
            Screen.SetResolution((int)(Screen.width * p.renderResolutionScale / 100f), (int)(Screen.height * p.renderResolutionScale / 100f), Screen.fullScreen, p.targetFPS);
            
            Debug.Log($\"[Graphics] Quality set to {p.name}\");
        }
        
        public QualityPreset Current => presets[currentPresetIndex];
        public string[] GetQualityNames()
        {
            if(presets == null) return new string[0];
            var names = new string[presets.Length];
            for(int i=0;i<presets.Length;i++) names[i] = presets[i].name;
            return names;
        }
        
        public void SetAdaptiveFPS(int targetFPS)
        {
            Application.targetFrameRate = targetFPS;
            QualitySettings.VSyncCount = targetFPS >= 60 ? 0 : 1;
        }
    }
}
