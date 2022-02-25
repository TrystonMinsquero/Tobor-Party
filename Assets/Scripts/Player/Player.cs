using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerObject playerObject;

    public RaceData raceData;

    public void SetUp(PlayerObject playerObject)
    {
        this.playerObject = playerObject;
    }

    public void AddRaceData(RaceData _raceData)
    {
        raceData = new RaceData(_raceData);
    }


}
