using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image _gameOverBackgroundImage;
    [SerializeField] private RectTransform _gameOverTextTransform;
    [SerializeField] private RectTransform _scoreTableBackgroundTransform;
    [SerializeField] private TMP_Text _winnerText;
    [SerializeField] private Button _mainMenuButton;

    private RectTransform _mainMenuButtonTransform;
    private RectTransform _winnerTextTransform;

    [Header("Settings")]
    [SerializeField] private float _animationDuration;
    [SerializeField] private float _scaleDuration;

    private void Awake()
    {
        _mainMenuButtonTransform = _mainMenuButton.GetComponent<RectTransform>();
        _winnerTextTransform = _winnerText.GetComponent<RectTransform>();
    }

    private void Start()
    {
        _scoreTableBackgroundTransform.gameObject.SetActive(false);
        _scoreTableBackgroundTransform.localScale = Vector3.zero;

        GameManager.Instance.OnGameStateChanged += GameManager_OnGameStateChanged;
    }

    private void GameManager_OnGameStateChanged(GameState gameState)
    {
        if(gameState == GameState.GameOver)
        {
            AnimateGameOver();
        }
    }

    private void AnimateGameOver()
    {
        _gameOverBackgroundImage.DOFade(0.8f, _animationDuration / 2);
        _gameOverTextTransform.DOAnchorPosY(0f, _animationDuration).SetEase(Ease.OutBounce).OnComplete(() =>
        {
           _gameOverTextTransform.GetComponent<TMP_Text>().DOFade(0f, _animationDuration / 2).SetDelay(1f).OnComplete(() =>
           {
                _scoreTableBackgroundTransform.gameObject.SetActive(true);
                _scoreTableBackgroundTransform.DOScale(0.8f, _scaleDuration).SetEase(Ease.OutBack).OnComplete(() =>
                {
                    _mainMenuButtonTransform.DOScale(1f, _scaleDuration).SetEase(Ease.OutBack).OnComplete(() =>
                    {
                       _winnerTextTransform.DOScale(1f, _scaleDuration).SetEase(Ease.OutBack); 
                    });
                });
           });
        });
    }
}
