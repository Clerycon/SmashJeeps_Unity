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
        
    }

    private void SetSteerInput(float steerInput)
    {
        _steerInput = Mathf.Clamp(steerInput, -1f, 1f);
    }

    private void SetAccelerateInput(float accelerateInput)
    {
        _accelerateInput = Mathf.Clamp(accelerateInput, -1f, 1f);
    }

    
}
