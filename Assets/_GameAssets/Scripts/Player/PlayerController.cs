using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public class SpringData
    {
        public float _currentLength;
        public float _currentVelocity;
    }

    private static readonly WheelType[] _wheels = new WheelType[]
    {
        WheelType.FrontLeft,
        WheelType.FrontRight,
        WheelType.BackLeft,
        WheelType.BackRight
    };

    private Dictionary<WheelType, SpringData> _sprintDatas = new Dictionary<WheelType, SpringData>();

    private float _steerInput;
    private float _accelerateInput;

    [Header("References")]
    [SerializeField] private Rigidbody _playerRigidbody;
    [SerializeField] private BoxCollider _playerCollider;
    [SerializeField] private VehicleSettingsSO _vehicleSettings;

    private void Awake()
    {
        foreach(WheelType wheelType in _wheels)
        {
            _sprintDatas.Add(wheelType, new());
        }
    }

    private void Update()
    {
        SetSteerInput(Input.GetAxis("Horizontal"));
        SetAccelerateInput(Input.GetAxis("Vertical"));
    }

    private void FixedUpdate()
    {
        UpdateSuspension();
    }

    private void SetSteerInput(float steerInput)
    {
        _steerInput = Mathf.Clamp(steerInput, -1f, 1f);
    }

    private void SetAccelerateInput(float accelerateInput)
    {
        _accelerateInput = Mathf.Clamp(accelerateInput, -1f, 1f);
    }

    private void UpdateSuspension()
    {
        foreach(WheelType id in _sprintDatas.Keys)
        {
            CastSpring(id);
            
            float currentVelocity = _sprintDatas[id]._currentVelocity;
            float currentLength = _sprintDatas[id]._currentLength;

            float force = SpringMathExtensions.CalculateForceDamped(currentLength, currentVelocity,
                _vehicleSettings.SpringRestLength, _vehicleSettings.SpringStrength, _vehicleSettings.SpringDamper);

            _playerRigidbody.AddForceAtPosition(force * transform.up, GetSpringPosition(id));
        }
    }

    private void CastSpring(WheelType wheelType)
    {
        Vector3 position = GetSpringPosition(wheelType);

        float previousLength = _sprintDatas[wheelType]._currentLength;
        float currentLength;

        if(Physics.Raycast(position, -transform.up, out RaycastHit hit, _vehicleSettings.SpringRestLength))
        {
            currentLength = hit.distance;
        }
        else
        {
            currentLength = _vehicleSettings.SpringRestLength;
        }

        _sprintDatas[wheelType]._currentVelocity = (currentLength - previousLength) / Time.fixedDeltaTime;
        _sprintDatas[wheelType]._currentLength = currentLength;
    }

    private Vector3 GetSpringPosition(WheelType wheelType)
    {
        return transform.localToWorldMatrix.MultiplyPoint3x4(GetSpringRelativePosition(wheelType));
    }

    private Vector3 GetSpringRelativePosition(WheelType wheelType)
    {
        Vector3 boxSize = _playerCollider.size;
        float boxBottom = boxSize.y * -0.5f;

        float paddingX = _vehicleSettings.WheelsPaddingX;
        float paddingZ = _vehicleSettings.WheelsPaddingZ;

        return wheelType switch
        {
            WheelType.FrontLeft => new Vector3(boxSize.x * (paddingX - 0.5f), boxBottom, boxSize.z * (0.5f - paddingZ)),
            WheelType.FrontRight => new Vector3(boxSize.x * (0.5f - paddingX), boxBottom, boxSize.z * (0.5f - paddingZ)),
            WheelType.BackLeft => new Vector3(boxSize.x * (paddingX - 0.5f), boxBottom, boxSize.z * (paddingZ - 0.5f)),
            WheelType.BackRight => new Vector3(boxSize.x * (0.5f - paddingX), boxBottom, boxSize.z * (paddingZ - 0.5f)),
            _ => default
        };
        
    }

    
}

public static class SpringMathExtensions
{
    public static float CalculateForceDamped(float currentLength, float lengthVelocity, 
        float restLength, float strength, float damper)
    {
        float lengthOffset = restLength - currentLength;
        return (lengthOffset * strength) - (lengthVelocity * damper);
    }
}
