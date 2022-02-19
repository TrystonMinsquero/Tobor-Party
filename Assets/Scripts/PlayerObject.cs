using UnityEngine;

public abstract class PlayerObject : MonoBehaviour
{
    public abstract bool AssignController(PlayerController playerController);

    public abstract bool HasController();
}
