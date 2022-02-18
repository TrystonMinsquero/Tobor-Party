using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    //static variables
    public static LobbyManager instance;

    public static bool CanJoinLeave
    {
        get { return instance; }
    }
    //true if in lobby menu (not controls, etc.)

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
        //MusicManager.StartMusic();
    }

    public void StartGame()
    {
        SceneManager.LoadScene("TestCar");
    }

}
