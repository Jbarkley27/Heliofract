using UnityEngine;

public class PlanetRotationModule : MonoBehaviour
{
    [SerializeField] private Vector3 rotationAxis = Vector3.up;
    [SerializeField, Range(0f, 360f)] private float rotationAngle;
    [SerializeField] private float rotationSpeed = 10f;

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

    public Vector3 RotationAxis
    {
        get => rotationAxis;
        set
        {
            rotationAxis = value == Vector3.zero ? Vector3.up : value.normalized;
            ApplyRotation();
        }
    }

    private void Start()
    {
        ApplyRotation();
    }

    private void Update()
    {
        if (Mathf.Approximately(rotationSpeed, 0f))
        {
            return;
        }

        RotationAngle += rotationSpeed * Time.deltaTime;
    }

    private void OnValidate()
    {
        if (rotationAxis == Vector3.zero)
        {
            rotationAxis = Vector3.up;
        }

        rotationAxis.Normalize();
        rotationAngle = Mathf.Repeat(rotationAngle, 360f);
        ApplyRotation();
    }

    private void ApplyRotation()
    {
        transform.localRotation = Quaternion.AngleAxis(rotationAngle, rotationAxis);
    }
}
