using UnityEngine;

public class SkyboxModule : MonoBehaviour
{
    private static readonly int RotationProperty = Shader.PropertyToID("_Rotation");

    [SerializeField] private bool rotateSkybox = true;
    [SerializeField, Range(0f, 360f)] private float rotationAngle;
    [SerializeField] private float rotationSpeed = 1f;

    private Material runtimeSkybox;

    public float RotationAngle
    {
        get => rotationAngle;
        set
        {
            rotationAngle = Mathf.Repeat(value, 360f);
            ApplyRotation();
        }
    }

    public float RotationSpeed
    {
        get => rotationSpeed;
        set => rotationSpeed = value;
    }

    private void Start()
    {
        CreateRuntimeSkybox();
        ApplyRotation();
    }

    private void Update()
    {
        if (!rotateSkybox || Mathf.Approximately(rotationSpeed, 0f))
        {
            return;
        }

        RotationAngle += rotationSpeed * Time.deltaTime;
    }

    private void OnValidate()
    {
        rotationAngle = Mathf.Repeat(rotationAngle, 360f);

        if (Application.isPlaying)
        {
            ApplyRotation();
        }
    }

    private void CreateRuntimeSkybox()
    {
        if (RenderSettings.skybox == null)
        {
            return;
        }

        runtimeSkybox = new Material(RenderSettings.skybox);
        RenderSettings.skybox = runtimeSkybox;
    }

    private void ApplyRotation()
    {
        Material skybox = runtimeSkybox != null ? runtimeSkybox : RenderSettings.skybox;

        if (skybox == null || !skybox.HasProperty(RotationProperty))
        {
            return;
        }

        skybox.SetFloat(RotationProperty, rotationAngle);
        DynamicGI.UpdateEnvironment();
    }
}
