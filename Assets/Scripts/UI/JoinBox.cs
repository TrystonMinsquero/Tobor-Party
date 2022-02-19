using UnityEngine;

public class JoinBox : MonoBehaviour
{
    [Header("Data")]
    public int slot;
    public bool hasPlayer;

    [Header("UI Draggables")]
    public Canvas joined;
    public Canvas empty;

    public void AddPlayer(Player player)
    {
        empty.enabled = false;
        joined.enabled = true;
        hasPlayer = true;
        
    }

    public void RemovePlayer(Player player)
    {
        joined.enabled = false;
        empty.enabled = true;
        hasPlayer = false;
    }
    
}
