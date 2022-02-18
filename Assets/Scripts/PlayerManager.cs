using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInputManager))]
public class PlayerManager : MonoBehaviour
{ 
    public static PlayerManager instance;
    
    public static Player[] players;
    public static int playerCount;

    public uint maxPlayers;
    //used to view players in the editor
    public Player[] playersDisplay;

    private void Awake()
    {
        if (instance != null)
            Destroy(this.gameObject);
        else
            instance = this;

        players = new Player[maxPlayers];
        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        playersDisplay = players;
    }

    //Event that gets called when new input is detected
    public void OnPlayerJoined(PlayerInput playerInput)
    {
        JoinPlayer(playerInput.GetComponent<Player>());
        //ScoreKeeper.UpdateScores();
    }

    //Event that gets called when player leaves
    public void OnPlayerLeft(PlayerInput playerInput)
    {
        RemovePlayer(playerInput.GetComponent<Player>());
        //ScoreKeeper.UpdateScores();
    }

    //Sets "EnableJoining" on the manager to true, allowing new inputs to join
    public static void SetJoinable(bool enabled)
    {
        if (enabled)
            PlayerInputManager.instance.EnableJoining();
        else
            PlayerInputManager.instance.DisableJoining();
    }

    #region Indexing
    //Gets the index of player in the player array, returns -1 if not there
    public static int GetIndex(Player _player)
    {
        if (playerCount <= 0 || _player == null)
            return -1;
        for(int i = 0; i < players.Length; i++)
            if (players[i] == _player)
                return i;
        return -1;
    }

    //returns true if player is already connected to player manager, false otherwise
    public static bool Contains(Player _player)
    {
        if (playerCount <= 0)
            return false;
        foreach (Player player in players)
            if (player == _player)
                return true;
        return false;
    }

    //returns the next empty slot index in the array (so players can leave and rejoin without creating holes in array),
    //returns -1 if no slots are available
    public static int NextPlayerSlot()
    {
        for (int i = 0; i < players.Length; i++)
            if (players[i] == null)
                return i;
        return -1;
    }
    #endregion

    //Will check coniditons to add a player and add them if conditions hold
    private static void JoinPlayer(Player player)
    {
        
        if (Contains(player))
            return;

        if(NextPlayerSlot() < 0)
        {
            Destroy(player.gameObject);
            return;
        }
        
        AddPlayer(player);

    }
    
    //Properly adds a player to the game
    private static void AddPlayer(Player player, bool inGame = false)
    {
        int playerIndex = NextPlayerSlot();
        players[playerIndex] = player;
        playerCount++;

        player.name = "Player " + (playerIndex + 1);
        player.transform.parent = instance.transform;

        if (inGame)
        {
            //Add Player the the game
        }
    }

    //Properly removes players from the game
    private static void RemovePlayer(Player player)
    {
            
        if (LobbyManager.CanJoinLeave)
        {
            int playerIndex = GetIndex(player);
            if (playerIndex >= 0)
            {
                players[playerIndex] = null;
                Destroy(player.gameObject);
                playerCount--;
            }
        }
    }
}
