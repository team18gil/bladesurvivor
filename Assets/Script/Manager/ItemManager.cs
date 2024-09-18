using UnityEngine;
using System.Collections.Generic;

public class ItemManager : MonoBehaviour
{
    [SerializeField] private TextAsset itemDataCSVText;
    private Dictionary<string, ItemProbabilityData> itemProbabilityDataDic = new();

    private void Awake()
    {
        if (itemDataCSVText == null || string.IsNullOrEmpty(itemDataCSVText.text))
        {
            Debug.LogError($"ItemData CSV is not found");
            return;
        }

        LoadItemProbabilityDataDic(itemDataCSVText.text);

        GameManager.Instance.AddEvent(EEvent.MonsterDead, OnMonsterDead);
        GameManager.Instance.AddEvent(EEvent.ItemCollected, OnItemCollected);
    }

    private void LoadItemProbabilityDataDic(string rawData)
    {
        string[] lines = rawData.Split('\n');
        string[] headers = lines[0].Split(',');

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] values = lines[i].Split(',');
            string monsterType = values[0];

            var itemCounts = new Dictionary<string, int>();

            // Loop through the columns dynamically (skip the first column which is the monster type)
            for (int j = 1; j < values.Length; j++)
            {
                if (string.IsNullOrEmpty(values[j])) continue;

                if (int.TryParse(values[j].Trim(), out int itemCount) && itemCount > 0)
                {
                    string itemType = headers[j].Trim();
                    itemCounts[itemType] = itemCount;
                }
            }

            if (itemProbabilityDataDic.TryGetValue(monsterType, out var prevItemProbabilityData))
            {
                Debug.LogWarning($"ItemProbabilityData already exists on {monsterType}:\n\t{prevItemProbabilityData}");
            }
            else
            {
                itemProbabilityDataDic[monsterType] = new ItemProbabilityData(itemCounts);
            }
        }
    }



    private void OnMonsterDead(object param)
    {
        if (param is MonsterBase monster)
        {
            string typeString = $"{monster.GetType()}";
            if (!itemProbabilityDataDic.TryGetValue(typeString, out var itemProbabilityData))
            {
                Debug.LogWarning($"ItemProbabilityData is not found on the type {typeString}");
                return;
            }

            string itemTypeName = itemProbabilityData.FindItemTypeName();
            if (!string.IsNullOrEmpty(itemTypeName))
            {
                MakeItem(itemTypeName, monster);
            }
        }
    }

    private void MakeItem(string itemTypeName, MonsterBase monster)
    {
        if (monster is null)
        {
            Debug.LogWarning($"Monster is not found");
            return;
        }

        var gm = GameManager.Instance;
        var newItem = gm.GameObjectPool.GetByName<ItemBase>(itemTypeName, gm.PlaygroundParent);

        if (newItem is not null)
        {
            newItem.transform.localPosition = monster.transform.localPosition;
            // add item to collection if needed
        }
    }



    private void OnItemCollected(object param)
    {
        if (param is ItemBase item)
        {
            GameManager.Instance.GameObjectPool.Release(item);
            // remove item from collection if needed
        }
    }
}
