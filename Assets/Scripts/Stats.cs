using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using Builds;

public class Stats : NetworkBehaviour
{
    public Text name;
    public Text score;

    [SerializeField]
    public readonly SyncList<PlayerStats> stats = new SyncList<PlayerStats>();

    void Update()
    {
        name.text = "»гроки:\n";
        score.text = "—чЄт:\n";
        foreach (PlayerStats playerStats in stats)
        {
            name.text += playerStats.name + "\n";
            score.text += playerStats.count + "\n";
        }
    }
}

[System.Serializable]
public class PlayerStats
{
    public string name;
    public int count;
}