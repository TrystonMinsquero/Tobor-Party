using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoints : MonoBehaviour
{
    public static Checkpoints Instance { get; private set; }
    public List<Checkpoint> checkpoints = new List<Checkpoint>();

    void Awake()
    {
        foreach (var c in checkpoints)
        {
            var name = c.transform.name;
            var sub = name.Substring(name.IndexOf('(') + 1, name.IndexOf(')') - name.IndexOf('(') - 1);
            var pos = int.Parse(sub);
            c.nameIndex = pos;
        }

        checkpoints.Sort((a, b) => a.nameIndex.CompareTo(b.nameIndex));
        for (int i = 0; i < checkpoints.Count; i++)
        {
            checkpoints[i].index = i;
        }

        Instance = this;
    }

    void Start()
    {

    }
}
