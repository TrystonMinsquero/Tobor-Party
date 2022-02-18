using UnityEngine;

public abstract class Player : MonoBehaviour
{
    private void Awake()
    {
        transform.tag = "Player";
        DontDestroyOnLoad(this.gameObject);
    }
}
