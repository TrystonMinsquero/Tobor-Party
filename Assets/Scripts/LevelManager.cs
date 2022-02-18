using UnityEngine;
using UnityEngine.InputSystem;

public class LevelManager : MonoBehaviour
{

    private void Awake()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject playerObj in players)
        {
            Player player = playerObj.GetComponent<Player>();
        }
        PlayerInputManager.instance.JoinPlayer();
    }
}
