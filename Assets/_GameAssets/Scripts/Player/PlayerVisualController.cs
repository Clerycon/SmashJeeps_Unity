using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerVisualController : NetworkBehaviour
{
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private Transform _playerVisualTransform;
    [SerializeField] private Collider _playerCollider;
    [SerializeField] private Transform _wheelFrontLeft, _wheelFrontRight, _wheelBackLeft, _wheelBackRight;
    [SerializeField] private float _wheelSpinSpeed, _wheelYWhenSpringMin, _wheelYWhenSpringMax;
    

    private Quaternion _wheelFrontLeftRoll;
    private Quaternion _wheelFrontRightRoll;
    private float _forwardSpeed;
    private float _steerInput;
    private Dictionary<WheelType, float> _springsCurrentLength = new()
    {
        {WheelType.FrontLeft, 0f},
        {WheelType.FrontRight, 0f},
        {WheelType.BackLeft, 0f},
        {WheelType.BackRight, 0f},
    };

    public override void OnNetworkSpawn()
    {
        if(!IsOwner) { return; }

        _playerController.OnVehicleCrashed += PlayerController_OnVehicledCrashed;
    }

    private void PlayerController_OnVehicledCrashed()
    {
        enabled = false;
    }

    private void Start()
    {
        _wheelFrontLeftRoll = _wheelFrontLeft.localRotation;
        _wheelFrontRightRoll = _wheelBackRight.localRotation;
    }

    private void Update()
    {
        if(!IsOwner) { return; }
        
        UpdateVisualStates();
        RotateWheels();
        SetSuspension();
    }

    private void UpdateVisualStates()
    {
        _steerInput = Input.GetAxis("Horizontal");
        _forwardSpeed = Vector3.Dot(_playerController.Forward, _playerController.Velocity);

        _springsCurrentLength[WheelType.FrontLeft] = _playerController.GetSpringCurrentLength(WheelType.FrontLeft);
        _springsCurrentLength[WheelType.FrontRight] = _playerController.GetSpringCurrentLength(WheelType.FrontRight);
        _springsCurrentLength[WheelType.BackLeft] = _playerController.GetSpringCurrentLength(WheelType.BackLeft);
        _springsCurrentLength[WheelType.BackRight] = _playerController.GetSpringCurrentLength(WheelType.BackRight);
    }

    private void RotateWheels()
    {
        if(_springsCurrentLength[WheelType.FrontLeft] < _playerController.VehicleSettings.SpringRestLength)
        {
            _wheelFrontLeftRoll *= Quaternion.AngleAxis(_forwardSpeed * _wheelSpinSpeed * Time.deltaTime, Vector3.right);
        }

        if(_springsCurrentLength[WheelType.FrontRight] < _playerController.VehicleSettings.SpringRestLength)
        {
            _wheelFrontRightRoll *= Quaternion.AngleAxis(_forwardSpeed * _wheelSpinSpeed * Time.deltaTime, Vector3.right);
        }

        if(_springsCurrentLength[WheelType.BackLeft] < _playerController.VehicleSettings.SpringRestLength)
        {
            _wheelBackLeft.localRotation *= Quaternion.AngleAxis(_forwardSpeed * _wheelSpinSpeed * Time.deltaTime, Vector3.right);
        }

        if(_springsCurrentLength[WheelType.FrontLeft] < _playerController.VehicleSettings.SpringRestLength)
        {
            _wheelBackRight.localRotation *= Quaternion.AngleAxis(_forwardSpeed * _wheelSpinSpeed * Time.deltaTime, Vector3.right);
        }

        _wheelFrontLeft.localRotation = Quaternion.AngleAxis(_steerInput * _playerController.VehicleSettings.SteerAngle, Vector3.up) * _wheelFrontLeftRoll;
        _wheelFrontRight.localRotation = Quaternion.AngleAxis(_steerInput * _playerController.VehicleSettings.SteerAngle, Vector3.up) * _wheelFrontRightRoll;
    }

    private void SetSuspension()
    {
        float springFrontLeftRatio = _springsCurrentLength[WheelType.FrontLeft] / _playerController.VehicleSettings.SpringRestLength;
        float springFrontRightRatio = _springsCurrentLength[WheelType.FrontRight] / _playerController.VehicleSettings.SpringRestLength;
        float springBackLeftRatio = _springsCurrentLength[WheelType.BackLeft] / _playerController.VehicleSettings.SpringRestLength;
        float springBackRightRatio = _springsCurrentLength[WheelType.BackRight] / _playerController.VehicleSettings.SpringRestLength;

        _wheelFrontLeft.localPosition = new Vector3(_wheelFrontLeft.localPosition.x,
            _wheelYWhenSpringMin + (_wheelYWhenSpringMax - _wheelYWhenSpringMin) * springFrontLeftRatio,
            _wheelFrontLeft.localPosition.z);

        _wheelFrontRight.localPosition = new Vector3(_wheelFrontRight.localPosition.x,
            _wheelYWhenSpringMin + (_wheelYWhenSpringMax - _wheelYWhenSpringMin) * springFrontRightRatio,
            _wheelFrontRight.localPosition.z);

        _wheelBackLeft.localPosition = new Vector3(_wheelBackLeft.localPosition.x,
            _wheelYWhenSpringMin + (_wheelYWhenSpringMax - _wheelYWhenSpringMin) * springBackLeftRatio,
            _wheelBackLeft.localPosition.z);

        _wheelBackRight.localPosition = new Vector3(_wheelBackRight.localPosition.x,
            _wheelYWhenSpringMin + (_wheelYWhenSpringMax - _wheelYWhenSpringMin) * springBackRightRatio,
            _wheelBackRight.localPosition.z);
    }

    public void SetVehicleVisualActive(float delay)
    {
        StartCoroutine(SetVehicleVisualActiveCoroutine(delay));
    }

    private IEnumerator SetVehicleVisualActiveCoroutine(float delay)
    {
        SetVehicleVisualActiveRpc(false);
        _playerCollider.enabled = false;

        yield return new WaitForSeconds(delay);

        SetVehicleVisualActiveRpc(true);
        _playerCollider.enabled = true;
        enabled = true;
        
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SetVehicleVisualActiveRpc(bool isActive)
    {
        _playerVisualTransform.gameObject.SetActive(isActive);
    }
}
