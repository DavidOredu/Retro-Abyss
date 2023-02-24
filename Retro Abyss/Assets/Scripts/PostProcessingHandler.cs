using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
//using UnityEngine.Rendering.PostProcessing;
public class PostProcessingHandler : SingletonDontDestroy<PostProcessingHandler>
{
    public UniversalRendererData rendererData;
    public Volume volume;
    public Vignette vignette;
    public Bloom bloom;
    public ChromaticAberration chromaticAberration;
    public FilmGrain filmGrain;
    public MotionBlur motionBlur;
    public LensDistortion lensDistortion;

    private ScriptableRendererFeature feature;
    private MobileFrostUrp frostFeature;

    [Header("FROST SETTINGS")]
    public string featureName = null;
    [Range(0, 1)]
    public float VignetteIntensity = 1f;
    [Range(0, 1)]
    public float Transparency = 1f;

    public SettingsData settingsData;
    // Start is called before the first frame update
    void Start()
    {
        volume.profile.TryGet(out bloom);
        volume.profile.TryGet(out vignette);
        volume.profile.TryGet(out filmGrain);
        volume.profile.TryGet(out motionBlur);
        volume.profile.TryGet(out chromaticAberration);
        volume.profile.TryGet(out lensDistortion);

        TryGetFeature(out feature);

        frostFeature = feature as MobileFrostUrp;
        featureName = frostFeature.name;

        SetPostProcessing();
    }
    private void Update()
    {
        frostFeature.settings.Vignette = VignetteIntensity;
        frostFeature.settings.Transparency = Transparency;
        rendererData.SetDirty();
    }
    public void SetPostProcessing()
    {
        bloom.active = settingsData.bloom;
        filmGrain.active = settingsData.filmGrain;
        motionBlur.active = settingsData.motionBlur;
    }
    private bool TryGetFeature(out ScriptableRendererFeature feature)
    {
        feature = rendererData.rendererFeatures.Where((f) => f.name == featureName).FirstOrDefault();
        return feature != null;
        // Update is called once per frame
    }
    public void StopFrostEffect()
    {
        feature.SetActive(false);
        rendererData.SetDirty();
    }
    
}
