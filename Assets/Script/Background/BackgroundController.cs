using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    /// <summary>
    /// 0 1 2 /
    /// 3 4 5 /
    /// 6 7 8 /
    /// </summary>
    [SerializeField] private List<BackgroundPlane> planes = new();

    private void Awake()
    {
        foreach (var plane in planes)
        {
            plane.TriggerCallback = OnPlaneTriggerEnter;
        }
    }

    private void OnPlaneTriggerEnter(BackgroundPlane plane)
    {
        int index = planes.IndexOf(plane);

        if (index.Equals(1)) // goes up
        {
            // 6 7 8 > 0 1 2
            for (int i = 6; i <= 8; ++i)
            {
                MovePlanesByAt(i, 0, 3);
            }
        }
        else if (index.Equals(7)) // goes down
        {
            // 0 1 2 > 6 7 8
            for (int i = 0; i <= 2; ++i)
            {
                MovePlanesByAt(i, 0, -3);
            }
        }
        else if (index.Equals(3)) // goes left
        {
            // 2 > 0, 5 > 3, 8 > 6
            for (int i = 2; i <= 8; i += 3)
            {
                MovePlanesByAt(i, -3, 0);
            }
        }
        else if (index.Equals(5)) // goes right
        {
            // 0 > 2, 3 > 5, 6 > 8
            for (int i = 0; i <= 6; i += 3)
            {
                MovePlanesByAt(i, 3, 0);
            }
        }

        // rearrange index of plane with position
        planes.Sort((p1, p2) =>
        {
            var pos1 = p1.transform.localPosition;
            var pos2 = p2.transform.localPosition;

            float val1 = pos1.x - pos1.y * 3.5f;
            float val2 = pos2.x - pos2.y * 3.5f;

            return val1.CompareTo(val2);
        });
    }

    private void MovePlanesByAt(int index, float xDelta, float yDelta)
    {
        var plane = planes[index];
        var dest = plane.transform.localPosition;
        var size = plane.PlaneSize;

        dest.x += xDelta * size.x;
        dest.y += yDelta * size.y;

        plane.transform.localPosition = dest;
    }
}
