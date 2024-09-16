using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameObjectPool", menuName = "ScriptableObjects/GameObjectPool")]
public class GameObjectPool : ScriptableObject
{
    [SerializeField] private List<MonoBehaviour> prefabs;
    private Dictionary<System.Type, MonoBehaviour> prefabDic = new();
    private Dictionary<System.Type, List<MonoBehaviour>> poolDic = new();

    public void Initialize()
    {
        foreach (var prefab in prefabs)
        {
            var type = prefab.GetType();
            if (prefabDic.TryGetValue(type, out var prevPrefab))
            {
                Debug.LogWarning($"Prefab {prevPrefab.name} already exists for {type}");
                continue;
            }
            prefabDic.Add(type, prefab);
        }
    }

    public T Get<T>(Transform parent) where T : MonoBehaviour
    {
        var type = typeof(T);
        if (!prefabDic.TryGetValue(type, out var prefab))
        {
            Debug.LogWarning($"Prefab not found for {type}");
            return null;
        }

        T component;
        if (!poolDic.TryGetValue(type, out var pool) || pool.Count.Equals(0))
        {
            component = Instantiate(prefab) as T;
            component.name = $"{type}";
        }
        else
        {
            component = pool[0] as T;
            component.gameObject.SetActive(true);
            pool.RemoveAt(0);
        }
        component.transform.SetParent(parent, true);
        return component;
    }

    public void Release(MonoBehaviour component)
    {
        var type = component.GetType();
        if (!poolDic.TryGetValue(type, out var pool))
        {
            pool = new List<MonoBehaviour>();
            poolDic.Add(type, pool);
        }
        component.gameObject.SetActive(false);
        pool.Add(component);
    }
}
