using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using Unity.Netcode;
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
    [SerializeField] private LeaderboardUI _leaderboardUI;
    [SerializeField] private ScoreTablePlayer _scoreTablePlayerPrefab;
    [SerializeField] private Transform _scoreTableParentTransform;

    private RectTransform _mainMenuButtonTransform;
    private RectTransform _winnerTextTransform;

    [Header("Settings")]
    [SerializeField] private float _animationDuration;
    [SerializeField] private float _scaleDuration;

    private void Awake()
    {
        _mainMenuButtonTransform = _mainMenuButton.GetComponent<RectTransform>();
        _winnerTextTransform = _winnerText.GetComponent<RectTransform>();

        _mainMenuButton.onClick.AddListener(OnMainMenuButtonClick);
    }

    private void OnMainMenuButtonClick()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            HostSingleton.Instance.HostGameManager.ShutDown();
        }

        ClientSingleton.Instance.ClientGameManager.Disconnect();
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

                PopulateGameOverLeaderboard();
           });
        });
    }

    private void PopulateGameOverLeaderboard()
    {
        var leaderboardData = _leaderboardUI.GetLeaderboardData().OrderByDescending(x => x.Score).ToList();

        HashSet<ulong> existingClientIds = new HashSet<ulong>();

        for(int i = 0; i < leaderboardData.Count; i++)
        {
            var entry = leaderboardData[i];

            if (existingClientIds.Contains(entry.ClientId))
            {
                continue;
            }

            ScoreTablePlayer scoreTablePlayerInstance
                = Instantiate(_scoreTablePlayerPrefab, _scoreTableParentTransform);

            bool isOwner = entry.ClientId == NetworkManager.Singleton.LocalClientId;
            int rank = i + 1;

            scoreTablePlayerInstance.SetScoreTableData(
                rank.ToString(), entry.PlayerName, 
                entry.Score.ToString(),isOwner);

            existingClientIds.Add(entry.ClientId);
        }

        SetWinnersName();
    }

    private void SetWinnersName()
    {
        string winnersName = _leaderboardUI.GetWinnersName();
        _winnerText.text = winnersName + " SMASHED Y'ALL!";
    }
}
