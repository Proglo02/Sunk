using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInputManager))]
public class PlayerManager : Singleton<PlayerManager>
{
    private PlayerInputManager playerInputManager;

    private int activePlayerIndex = 0;
    private Player activePlayer;
    private Player firstPlayer;
    private Player secondPlayer;

    private bool playerIsActive = false;

    protected override void Awake()
    {
        playerInputManager = GetComponent<PlayerInputManager>();

        if (!playerInputManager.playerPrefab && !PlayerInputManager.instance.playerPrefab)
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

    /// <summary>
    /// Deactivates the currently active player
    /// </summary>
    public void DeactivatePlayer()
    {
        playerIsActive = false;

        activePlayer.SetActive(false);
    }

    /// <summary>
    /// Activates the currently active player
    /// </summary>
    public void ActivatePlayer()
    {
        playerIsActive = true;

        if (activePlayer)
            activePlayer.SetActive(true);
    }

    /// <summary>
    /// Switches the active player to the other player and activates them
    /// </summary>
    public void SwitchActivePlayer()
    {
        playerIsActive = true;
        activePlayerIndex = activePlayerIndex == 0 ? 1 : 0;

        activePlayer = activePlayerIndex == 0 ? firstPlayer : secondPlayer;

        if (activePlayer)
            activePlayer.SetActive(true);
    }

    private GameObject GetPlayerPrefab()
    { 
        GameObject prefab = Resources.Load<GameObject>("Prefabs/Player/Player");
        return prefab;
    }

    private void SetActivePlayer(int index)
    {
        activePlayer = activePlayerIndex == 0 ? firstPlayer : secondPlayer;
        activePlayer.SetActive(playerIsActive);
    }
}
