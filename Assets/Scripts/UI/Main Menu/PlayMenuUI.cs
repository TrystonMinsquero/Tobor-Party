using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayMenuUI : MonoBehaviour
{
    public void PlayToborRacing()
    {
        SceneManager.LoadScene("Lobby");
    }
}