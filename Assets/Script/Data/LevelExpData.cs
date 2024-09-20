using UnityEngine;
using System.Collections.Generic;

public class LevelExpData
{
    private readonly Dictionary<int, float> levelTargetExpDic;
    private readonly float maxTargetExp;
    private readonly float minTargetExp;

    public LevelExpData(string rawCsvData)
    {
        levelTargetExpDic = new();

        string[] lines = rawCsvData.Split('\n');

        int maxLevel = int.MinValue;
        int minLevel = int.MaxValue;
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] values = lines[i].Split(',');
            if (values.Length != 2) continue;

            if (int.TryParse(values[0], out int level) && float.TryParse(values[1], out float exp))
            {
                if (levelTargetExpDic.TryGetValue(level, out float prevExp))
                {
                    Debug.LogWarning($"Level {level} already has exp {prevExp} on dictionary");
                    continue;
                }

                levelTargetExpDic[level] = exp;

                if (level > maxLevel)
                {
                    maxLevel = level;
                    maxTargetExp = exp;
                }

                if (level < minLevel)
                {
                    minLevel = level;
                    minTargetExp = exp;
                }
            }
        }
    }

    public float GetTargetExp(int level)
    {
        // Exp is on the table
        if (levelTargetExpDic.TryGetValue(level, out float exp))
        {
            return exp;
        }

        // level is more than expected: return highest level's exp
        if (level >= levelTargetExpDic.Count)
        {
            return maxTargetExp;
        }

        // should not be occurred (table value missing, etc.)
        return minTargetExp;
    }
}
