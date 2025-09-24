using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInputManager))]
public class PlayerManager : Singelton<PlayerManager>
{
    public bool PlayerIsActive { get; private set; } = true;

    private int activePlayerIndex = 0;
    private Player activePlayer;
    private Player firstPlayer;
    private Player secondPlayer;

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

    private void SetActivePlayer(int index)
    {
        activePlayer = activePlayerIndex == 0 ? firstPlayer : secondPlayer;
        activePlayer.SetActive(PlayerIsActive);
    }

    public void DeactivatePlayer()
    {
        PlayerIsActive = false;

        activePlayer.SetActive(false);
    }

    public void ActivatePlayer()
    {
        activePlayer.SetActive(true);
    }

    public void SwitchActivePlayer()
    {
        PlayerIsActive = true;
        activePlayerIndex = activePlayerIndex == 0 ? 1 : 0;

        activePlayer = activePlayerIndex == 0 ? firstPlayer : secondPlayer;

        if (activePlayer)
            activePlayer.SetActive(true);
    }
}
