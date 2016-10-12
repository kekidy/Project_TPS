using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CatmullRomSpline : MonoBehaviour {

    public static List<Vector3> CreateCatmullRomList(List<Vector3> vectorList, float timeCycle)
    {
        int loops = Mathf.FloorToInt(1f / timeCycle);
        List<Vector3> catmullromList = new List<Vector3>();

        for (int i = 1; i < vectorList.Count - 2; i++)
        {
            Vector3 p0 = vectorList[i - 1];
            Vector3 p1 = vectorList[i];
            Vector3 p2 = vectorList[i + 1];
            Vector3 p3 = vectorList[i + 2];

            for  (int j = 0; j < loops; j++)
            {
                float t = j * timeCycle;
                catmullromList.Add(GetCatmullRomPosition(t, p0, p1, p2, p3));
            }
        }

        return catmullromList;
    }

    public static Vector3 GetCatmullRomPosition(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
	{
		Vector3 a = 2f * p1;
		Vector3 b = p2 - p0;
		Vector3 c = 2f * p0 - 5f * p1 + 4f * p2 - p3;
		Vector3 d = -p0 + 3f * p1 - 3f * p2 + p3;
        Vector3 pos = 0.5f * (a + (b * t) + (c * t * t) + (d * t * t * t));

		return pos;
	}
}
