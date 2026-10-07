using System;
using NUnit.Framework;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterSelectUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Button _mainMenuButton;
    [SerializeField] private Button _readyButton;
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _copyButton;
    [SerializeField] private TMP_Text _readyText;
    [SerializeField] private Sprite _greenButtonSprite;
    [SerializeField] private Sprite _redButtonSprite;

    private bool _isPlayerReady;

    private void Awake()
    {
        _mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);
        _readyButton.onClick.AddListener(OnReadyButtonClicked);
        _startButton.onClick.AddListener(OnStartButtonClicked);
    }
    private void Start()
    {
        _startButton.gameObject.SetActive(NetworkManager.Singleton.IsServer);

        CharacterSelectReady.Instance.OnAllPlayersReady += CharacterSelectReady_OnAllPlayersReady;
        CharacterSelectReady.Instance.OnUnreadyChanged += CharacterSelectReady_OnUnreadyChanged;
        MultiplayerGameManager.Instance.OnPlayerDataNetworkListChanged += MultiplayerGameManager_OnPlayerDataNetworkListChanged;
    }

    private void MultiplayerGameManager_OnPlayerDataNetworkListChanged()
    {
        SetStartButtonInteractable(true);
    }

    private void CharacterSelectReady_OnUnreadyChanged()
    {
        SetStartButtonInteractable(false);
    }

    private void CharacterSelectReady_OnAllPlayersReady()
    {
        if (CharacterSelectReady.Instance.AreAllPlayersReady())
        {
            SetStartButtonInteractable(true);
        }
        else
        {
            SetStartButtonInteractable(false);
        }
    }

    private void OnStartButtonClicked()
    {
        NetworkManager.Singleton.SceneManager.LoadScene(Consts.SceneNames.GAME_SCENE, LoadSceneMode.Single);
    }

    private void OnReadyButtonClicked()
    {
        _isPlayerReady = !_isPlayerReady;

        if (_isPlayerReady)
        {
            SetPlayerReady();
        }
        else
        {
            SetPlayerUnready();
        }
    }

    private void OnMainMenuButtonClicked()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            HostSingleton.Instance.HostGameManager.ShutDown();
        }
        else
        {
            ClientSingleton.Instance.ClientGameManager.Disconnect();
        }
    }

    private void SetPlayerReady()
    {
        CharacterSelectReady.Instance.SetPlayerReady();
        _readyText.text = "Ready";
        _readyButton.image.sprite = _greenButtonSprite;
    }

    private void SetPlayerUnready()
    {
        CharacterSelectReady.Instance.SetPlayerUnready();
        _readyText.text = "Unready";
        _readyButton.image.sprite = _redButtonSprite;

    }

    private void SetStartButtonInteractable(bool isActive)
    {
        if(_startButton != null)
        {
            _startButton.interactable = isActive;
        }
    }
}
