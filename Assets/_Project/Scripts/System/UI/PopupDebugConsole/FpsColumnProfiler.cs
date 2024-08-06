using System.Collections.Generic;
using System.Linq;
using CustomInspector;
using UnityEngine;

public class FpsColumnProfiler : MonoBehaviour
{
    [SerializeField] private List<FpsColumn> fpsColumns;

    private Queue<int> _cacheFps;


    [Button]
    public void GetFpsColumns()
    {
        fpsColumns.Clear();
        fpsColumns = GetComponentsInChildren<FpsColumn>().ToList();
    }
    
    private void OnEnable()
    {
        GenerateNewCache();
    }

    private void GenerateNewCache()
    {
        _cacheFps = new Queue<int>();
        foreach (var fpsColumn in fpsColumns)
        {
            fpsColumn.Setup(0,Color.white,null);
        }
    }

    public void TickUpdate()
    {
        var fps = (int)(1f / Time.unscaledDeltaTime);
        _cacheFps.Enqueue(fps);
        
        if (_cacheFps.Count > fpsColumns.Count)
        {
            _cacheFps.Dequeue();
        }
        
        for (int i = 0; i < _cacheFps.Count; i++)
        {
            Color color;
            if (_cacheFps.ElementAt(i) >= 55)
            {
                color = Color.green;
            }
            else if (_cacheFps.ElementAt(i) >= 40)
            {
                color = Color.yellow;
            }
            else
            {
                color = Color.red;
            }
            fpsColumns[i].Setup(Mathf.Min(150,150*(_cacheFps.ElementAt(i)/60f)), color, _cacheFps.ElementAt(i).ToString());
        }
    }
}

