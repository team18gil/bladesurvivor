using System.Collections.Generic;
using UnityEngine;

public class ItemProbabilityData
{
    private readonly Dictionary<string, int> itemCounts = new();

    public ItemProbabilityData(Dictionary<string, int> itemCounts)
    {
        this.itemCounts = itemCounts;
    }

    public string FindItemTypeName()
    {
        int totalCount = 0;
        foreach (var count in itemCounts.Values)
        {
            totalCount += count;
        }

        if (totalCount == 0)
        {
            Debug.LogWarning("No items available.");
            return null;
        }

        int randomValue = Random.Range(0, totalCount + 1);

        // Loop through each item type and return one based on probability
        int cumulativeProbability = 0;
        foreach (var kvp in itemCounts)
        {
            cumulativeProbability += kvp.Value;
            if (randomValue <= cumulativeProbability)
            {
                if (kvp.Key.Equals("NoItem"))
                {
                    return null;
                }
                return kvp.Key;
            }
        }

        return null;
    }

    public override string ToString()
    {
        string retString = "";

        foreach (var kvp in itemCounts)
        {
            if (!string.IsNullOrEmpty(retString))
            {
                retString += ",";
            }
            retString += $"{kvp.Key}:{kvp.Value}";
        }
        return retString;
    }
}
