using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Level Data")]
public class LevelData : ScriptableObject
{
    public int levelNumber;
    public int attemptToBeat;
    public int numOfTargets;
    public int numOfBarriers;
}
