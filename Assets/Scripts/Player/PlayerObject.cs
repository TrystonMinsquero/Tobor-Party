using UnityEngine;

[RequireComponent(typeof(PlayerSkin))]
public abstract class PlayerObject : MonoBehaviour
{
    public abstract bool AssignController(PlayerController playerController);

    public abstract bool HasController();

    public void SetSkin(int index)
    {
        GetComponent<PlayerSkin>().SetIndex(index);
    }
}
