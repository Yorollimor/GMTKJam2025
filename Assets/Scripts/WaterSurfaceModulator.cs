using UnityEngine;

[System.Serializable]
public struct WaterSurfaceSettings
{
    public float wave_amplitude;
    public float wave_frequency;
    public float wave_speed;
    public float water_level;
    public Color water_color;
    public float surface_thickness;
    public Color surface_color;
}


public class WaterSurfaceModulator : MonoBehaviour
{
    bool useMaterialAsStartSettings = true; // Use the material's current settings as the starting point
    public WaterSurfaceSettings startSettings;
    public WaterSurfaceSettings maxSettings;

    SpriteRenderer spreiteRenderer;
    Vector2 previousVelocity;

    public float dieDownSpeed = 0.5f; // Speed at which the wave state dies down
    public float velocityMultiplier = 1;
    float currentWaveState = 0f; // Current wave state, used for animation or modulation
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spreiteRenderer = GetComponent<SpriteRenderer>();
        if(useMaterialAsStartSettings) FillFromMaterial(out startSettings);
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 currentVelocity = GameManager.Instance.currentTank.GetTankVelocity();
        Vector2 deltaVelocity = currentVelocity - previousVelocity;
        float strength = deltaVelocity.magnitude;

        currentWaveState += strength * velocityMultiplier;
        currentWaveState -= dieDownSpeed * Time.deltaTime;


        currentWaveState = Mathf.Clamp01(currentWaveState); // Ensure it stays between 0 and 1
        //Debug.Log($"Current Wave State: {currentWaveState} (Strength: {strength})");
        LerpMaterial(spreiteRenderer.material, startSettings, maxSettings, currentWaveState);

        previousVelocity = currentVelocity;
    }

    private void LerpMaterial(Material m, WaterSurfaceSettings from, WaterSurfaceSettings to, float ratio)
    {
        m.SetColor("_Color_Water", Color.Lerp(from.water_color, to.water_color, ratio));
        m.SetColor("_Color_Surface", Color.Lerp(from.surface_color, to.surface_color, ratio));
        m.SetFloat("_wave_amplitude", Mathf.Lerp(from.wave_amplitude, to.wave_amplitude, ratio));
        m.SetFloat("_wave_frequency", Mathf.Lerp(from.wave_frequency, to.wave_frequency, ratio));
        m.SetFloat("_wave_speed", Mathf.Lerp(from.wave_speed, to.wave_speed, ratio));
        m.SetFloat("_waterLevel", Mathf.Lerp(from.water_level, to.water_level, ratio));
        m.SetFloat("_surfaceThickness", Mathf.Lerp(from.surface_thickness, to.surface_thickness, ratio));
    }

    private void FillFromMaterial(out WaterSurfaceSettings settings)
    {
        settings.water_color = spreiteRenderer.material.GetColor("_Color_Water");
        settings.surface_color = spreiteRenderer.material.GetColor("_Color_Surface");
        settings.wave_amplitude = spreiteRenderer.material.GetFloat("_wave_amplitude");
        settings.wave_frequency = spreiteRenderer.material.GetFloat("_wave_frequency");
        settings.wave_speed = spreiteRenderer.material.GetFloat("_wave_speed");
        settings.water_level = spreiteRenderer.material.GetFloat("_waterLevel");
        settings.surface_thickness = spreiteRenderer.material.GetFloat("_surfaceThickness");
    }
}
