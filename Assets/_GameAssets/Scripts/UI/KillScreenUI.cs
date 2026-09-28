using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class KillScreenUI : MonoBehaviour
{
    public static KillScreenUI Instance { get; private set;}
    public event Action OnRespawnTimerFinished;

    [Header("Smash UI")]
    [SerializeField] private RectTransform _smashUITransform;
    [SerializeField] private TMP_Text _smashedPlayerText;

    [Header("Smashed UI")]
    [SerializeField] private RectTransform _smashedUITransform;
    [SerializeField] private TMP_Text _smashedByPlayerText;
    [SerializeField] private TMP_Text _respawnTimerText;

    [Header("Settings")]
    [SerializeField] private float _scaleDuration;
    [SerializeField] private float _smashUIStayDuration;

    private float _timer;
    private bool _isTimerActive;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        _smashUITransform.gameObject.SetActive(false);
        _smashedUITransform.gameObject.SetActive(false);

        _smashUITransform.localScale = Vector3.zero;
        _smashedUITransform.localScale = Vector3.zero;
    }

    private void Update()
    {
        if (_isTimerActive)
        {
            _timer -= Time.deltaTime;
            int timer = (int)_timer;
            _respawnTimerText.text = timer.ToString();

            if(_timer <= 0f)
            {
                _smashedUITransform.localScale = Vector3.zero;
                _smashedUITransform.gameObject.SetActive(false); 
                _smashedByPlayerText.text = string.Empty;
                _isTimerActive = false;
                OnRespawnTimerFinished?.Invoke();
            }
        }
    }

    public void SetSmashUI(string playerName)
    {
        StartCoroutine(SetSmashUICoroutine(playerName));
    }

    private IEnumerator SetSmashUICoroutine(string playerName)
    {
        _smashUITransform.gameObject.SetActive(true);
        _smashUITransform.DOScale(1f, _scaleDuration).SetEase(Ease.OutBack);
        _smashedPlayerText.text = playerName;

        yield return new WaitForSeconds(_smashUIStayDuration);

        _smashUITransform.DOScale(0f, _scaleDuration).SetEase(Ease.Linear).OnComplete(() =>
        {
            _smashUITransform.localScale = Vector3.zero;
            _smashUITransform.gameObject.SetActive(false);
            _smashedPlayerText.text = string.Empty;
        });
    }

    public void SetSmashedUI(string playerName, int respawnTimeCounter)
    {
        _smashedUITransform.gameObject.SetActive(true);
        _smashedUITransform.DOScale(1f, _scaleDuration).SetEase(Ease.OutBack);
        _smashedByPlayerText.text = playerName;
        _respawnTimerText.text = respawnTimeCounter.ToString();

        _isTimerActive = true;
        _timer = respawnTimeCounter;
    }

    
}
