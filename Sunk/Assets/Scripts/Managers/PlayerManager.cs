using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInputManager))]
public class PlayerManager : Singelton<PlayerManager>
{
    private bool playerIsActive = false;

    private int activePlayerIndex = 0;
    private Player activePlayer;
    private Player firstPlayer;
    private Player secondPlayer;

    protected override void Awake()
    {
        if (!PlayerInputManager.instance.playerPrefab)
            PlayerInputManager.instance.playerPrefab = GetPlayerPrefab();

        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        if (playerInput.playerIndex == 0)
        {
            firstPlayer = playerInput.GetComponent<Player>();
            if (activePlayerIndex == 0)
                SetActivePlayer(0);
        }
        else if (playerInput.playerIndex == 1)
        {
            secondPlayer = playerInput.GetComponent<Player>();
            if (activePlayerIndex == 1)
                SetActivePlayer(1);
        }
    }

    private GameObject GetPlayerPrefab()
    {
        GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Player/Player.prefab", typeof(GameObject));
        return prefab;
    }

    private void SetActivePlayer(int index)
    {
        activePlayer = activePlayerIndex == 0 ? firstPlayer : secondPlayer;
        activePlayer.SetActive(playerIsActive);
    }

    public void DeactivatePlayer()
    {
        playerIsActive = false;

        activePlayer.SetActive(false);
    }

    public void ActivatePlayer()
    {
        playerIsActive = true;

        if(activePlayer)
            activePlayer.SetActive(true);
    }

    public void SwitchActivePlayer()
    {
        playerIsActive = true;
        activePlayerIndex = activePlayerIndex == 0 ? 1 : 0;

        activePlayer = activePlayerIndex == 0 ? firstPlayer : secondPlayer;

        if (activePlayer)
            activePlayer.SetActive(true);
    }
}
