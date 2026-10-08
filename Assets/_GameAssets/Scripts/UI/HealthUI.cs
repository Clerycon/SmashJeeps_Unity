using System;
using DG.Tweening;
using UnityEngine;

public class HealthUI : MonoBehaviour
{
    public static HealthUI Instance { get; private set; }
    [SerializeField] private RectTransform _healthBarTransform;
    
    [SerializeField] private float _animationDuration;

    private void Awake()
    {
        Instance = this;
    }
    public void SetHealth(int health, int maxHealth)
    {
        _healthBarTransform.DOScaleX((float)health / maxHealth, _animationDuration).SetEase(Ease.Linear);
    }
}
