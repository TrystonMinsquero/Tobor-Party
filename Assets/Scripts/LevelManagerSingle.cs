using UnityEngine;

public class LevelManagerSingle : MonoBehaviour
{
    public static LevelManagerSingle Instance { get; private set; }
    
    // only necessary for order of players
    public PlayerSpawner playerSpawner;
    public PlayerController playerController;

    public static AudioSource gameMusic;

    private void Awake()
    {
        Instance = this;
        
        playerSpawner.TryToSpawnWith(playerController);

        gameMusic = GetComponent<AudioSource>();
    }

    private void OnDestroy()
    {
        gameMusic = null;
    }
}