using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerObject playerObject;

    public RaceData raceData;

    public int skinIndex;

    public void SetUp(PlayerObject playerObject)
    {
        this.playerObject = playerObject;
        playerObject.GetComponent<PlayerSkin>().SetIndex(skinIndex);
    }

    public void AddRaceData(RaceData _raceData)
    {
        raceData = new RaceData(_raceData);
    }


}
