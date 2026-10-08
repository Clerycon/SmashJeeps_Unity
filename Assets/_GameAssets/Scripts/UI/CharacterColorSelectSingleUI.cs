using System;
using UnityEngine;
using UnityEngine.UI;

public class CharacterColorSelectSingleUI : MonoBehaviour
{
    [SerializeField] private int _colorId;
    [SerializeField] private Image _colorImage;
    [SerializeField] private GameObject _selectedGameObject;
    [SerializeField] private Button _button;

    private void Awake()
    {
        _button.onClick.AddListener(() =>
        {
           MultiplayerGameManager.Instance.ChangePlayColor(_colorId); 
        });
    }

    private void Start()
    {
        MultiplayerGameManager.Instance.OnPlayerDataNetworkListChanged += 
            MultiplayerGameManager_OnPlayerDataNetworkListChanged;
        _colorImage.color = MultiplayerGameManager.Instance.GetPlayerColor(_colorId);
        UpdateIsSelected();
    }

    private void MultiplayerGameManager_OnPlayerDataNetworkListChanged()
    {
        UpdateIsSelected();
    }

    private void UpdateIsSelected()
    {
        if(MultiplayerGameManager.Instance.GetPlayerData().ColorId == _colorId)
        {
            _selectedGameObject.SetActive(true);
        }
        else
        {
            _selectedGameObject.SetActive(false);
        }
    }

}
