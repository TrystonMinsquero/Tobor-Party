using System.Collections.Generic;

public class RaceData
{
    public int place;
    public float finishTime;
    public List<float> bestLapTimes;
    public Dictionary<Checkpoint, float> bestCheckpointTimes;

    public RaceData(int place, float finishTime, List<float> bestLapTimes = null, Dictionary<Checkpoint, float> bestCheckpointTimes = null)
    {
        this.place = place;
        this.finishTime = finishTime;
        this.bestLapTimes = bestLapTimes;
        this.bestCheckpointTimes = bestCheckpointTimes;
    }

    public RaceData(RaceData raceData)
    {
        place = raceData.place;
        finishTime = raceData.finishTime;
        bestLapTimes = raceData.bestLapTimes;
        bestCheckpointTimes = raceData.bestCheckpointTimes;
    }
}
