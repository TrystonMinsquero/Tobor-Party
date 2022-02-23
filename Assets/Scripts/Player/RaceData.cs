using System.Collections.Generic;

public class RaceData
{
    public int place;
    public List<float> lapTimes;
    public Dictionary<Checkpoint, float> checkPointTimes;

    public RaceData(int place, List<float> lapTimes = null, Dictionary<Checkpoint, float> checkPointTimes = null)
    {
        this.place = place;
        this.lapTimes = lapTimes;
        this.checkPointTimes = checkPointTimes;
    }

    public RaceData(RaceData raceData)
    {
        place = raceData.place;
        lapTimes = raceData.lapTimes;
        checkPointTimes = raceData.checkPointTimes;
    }
}
