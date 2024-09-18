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

    /// <summary>
    /// Automatically find class with className and make with using Get() method
    /// </summary>
    public T GetByName<T>(string className, Transform parent) where T : MonoBehaviour
    {
        if (string.IsNullOrEmpty(className))
        {
            Debug.LogWarning($"className is null");
            return null;
        }

        System.Type type = System.Type.GetType(className);
        if (type is null)
        {
            Debug.LogWarning($"Class name {className} does not exist. Check the class name or namespace.");
            return null;
        }

        // Ensure the type can be assigned to T (e.g., it inherits from T)
        if (!typeof(T).IsAssignableFrom(type))
        {
            Debug.LogWarning($"Class {className} does not inherit from {typeof(T)}.");
            return null;
        }

        System.Reflection.MethodInfo method = GetType().GetMethod("Get").MakeGenericMethod(type);

        if (method.Invoke(this, new object[] { parent }) is not T result)
        {
            Debug.LogWarning($"Result not found on generic result with name {className}");
            return null;
        }

        return result;
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
