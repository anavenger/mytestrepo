using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;

[Serializable]
struct SerializedTupple
{
    public int _min;
    public int _max;
    private SerializedTupple((int _min, int _max) pair)
    {
        _min = pair._min;
        _max = pair._max;
    }

    public static implicit operator SerializedTupple((int _min, int _max) pair)
    {
        return new SerializedTupple(pair);
    }
}

[RequireComponent(typeof(LineRenderer))]
public class GridGenerator : MonoBehaviour
{
    public List<GameObject> prefabList;
    
    [Header("Кол-во итераций генератора предметов")]
    [SerializeField] private int iterations;
    [Header("Рамки")]
    [SerializeField] private SerializedTupple areaX;
    [SerializeField] private SerializedTupple areaY;
    [SerializeField] private Vector2Int ramka;
    [SerializeField] private float yHeight;
    [SerializeField] private float yPosition;
    private LineRenderer areaRenderer;
    private async void Start()
    {
        areaRenderer = GetComponent<LineRenderer>();
        RenderArea();

        for (int i = 0; i < 3; i++)
        {
            await Task.Delay(100);
            InitRandomPrefabsInArea();
        }
    }

    private async void InitRandomPrefabsInArea()
    {
        int count = iterations;
        while (count-- > 0)
        {
            for (int x = areaX._min; x < areaX._max; x++)
            {
                for (int y = areaY._min; y < areaY._max; y++)
                {
                    if (x <= ramka.x || y >= ramka.y) continue;

                    int randRange = UnityEngine.Random.Range(0, prefabList.Count);
                    var toy = Instantiate(prefabList[randRange], new Vector3(x, yPosition, y),
                        prefabList[randRange].transform.rotation);
                    toy.transform.SetParent(transform);
                }
            }

            await Task.Delay(400);
        }
    }
    private void RenderArea()
    {
        areaRenderer.positionCount = 8;

        int index = 0;

        areaRenderer.SetPosition(index++, new Vector3(areaX._min, yHeight, areaY._min));
        areaRenderer.SetPosition(index++, new Vector3(areaX._min, yHeight, areaY._max));
        
        areaRenderer.SetPosition(index++, new Vector3(areaX._min, yHeight, areaY._max));
        areaRenderer.SetPosition(index++, new Vector3(areaX._max, yHeight, areaY._max));
        
        areaRenderer.SetPosition(index++, new Vector3(areaX._max, yHeight, areaY._max));
        areaRenderer.SetPosition(index++, new Vector3(areaX._max, yHeight, areaY._min));
        
        areaRenderer.SetPosition(index++, new Vector3(areaX._max, yHeight, areaY._min));
        areaRenderer.SetPosition(index++, new Vector3(areaX._min, yHeight, areaY._min));
    }

}
