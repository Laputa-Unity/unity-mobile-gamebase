using System;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using JetBrains.Annotations;
using Pancake.Common;
using UnityEngine;

public class MoveObject : MonoBehaviour
{
    public GameObject TargetMoveGO;
    private bool isRun = true;

    public bool IsRun
    {
        get => isRun;
        set
        {
            OnIsRunChanged(value);
            isRun = value;
        }
    }
    public bool IsPlayOnAwake = true;
    public bool IsLoop = true;
    public bool IsLookAtTarget = true;
    [Range(0.1f,1000f)] public float Speed = 5f;
    [Range(0,100f)] public float MoveDelay;
    [Range(0,100f)] public float RotateDelay;
    [Header("Create circle point", order = 0)]
    [Range(0f,100f)] public float PointNumb = 3f;
    [Range(0.1f,1000f)] public float Radius = 2f;
    public CircleSpawnType CircleSpawnType;
    
    [Header("The path must be at least 2 points.", order=1)]
    public List<Transform> Path;

    private Queue<Vector3> PathQueue = new Queue<Vector3>();
    private List<GameObject> circleListPoint;
    private Vector3 currentPoint;
    private Vector3 nextPoint;
    private TweenerCore<Vector3,Vector3,VectorOptions> sequence;
    private Sequence nextSequence;
    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform item in Path)
        {
            PathQueue.Enqueue(item.position);
        }

        if (PathQueue.Count < 2) return;

        TargetMoveGO.transform.position = PathQueue.Peek();
        
        if (IsPlayOnAwake) StartMove();
    }

    private void OnDestroy()
    {
        sequence?.Kill();
        nextSequence?.Kill();
    }

    public void StartMove()
    {
        Vector3 tempPoint = PathQueue.Peek();
        currentPoint = tempPoint;
        PathQueue.Dequeue();
        PathQueue.Enqueue(tempPoint);
        nextPoint = PathQueue.Peek();
        
        if (IsLookAtTarget) TargetMoveGO.transform.DOLookAt(nextPoint, RotateDelay);
        sequence = TargetMoveGO.transform.DOMove(nextPoint, Speed).SetSpeedBased(true).SetEase(Ease.Linear).OnComplete(
        () =>
        {
            if (IsLoop)
            {
                nextSequence?.Play();
            }
        }).OnStart(() =>
        {
            nextSequence = DOTween.Sequence().AppendInterval(MoveDelay).AppendCallback(() => StartMove());
            nextSequence.Pause();
        });
    }

    public void ActivateStartMove()
    {
        if (!IsPlayOnAwake)
        {
            StartMove();
        }
    }

    public void OnIsRunChanged(bool value)
    {
        if (value) sequence.Play();
        else sequence.Pause();
    }

    private void ClearCircleList()
    {
        if (circleListPoint.IsNullOrEmpty()) return;
        circleListPoint.ForEach(item=>DestroyImmediate(item));
        circleListPoint.Clear();
        circleListPoint = new List<GameObject>();
    }

    private void OnDrawGizmos()
    {
        if (!Path.IsNullOrEmpty())
        {
            try
            {
                foreach (Transform item in Path)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawSphere(item.transform.position, 0.2f);
                }
            }
            catch (Exception e)
            {
                circleListPoint = new List<GameObject>();
                Path = new List<Transform>();
                throw;
            }
        }
    }

    public void CreateCirclePoint()
    {
        ClearCircleList();
        for (int i = 0; i < PointNumb; i++)
        {
            float theta = i * 2 * Mathf.PI / PointNumb;
            float x = Mathf.Sin(theta)*Radius;
            float z = Mathf.Cos(theta)*Radius;
  
            GameObject obj = new GameObject();
            obj.name = $"Point {i}";
            obj.transform.parent = transform;
            obj.transform.position = new Vector3(x + transform.position.x, 0, z + transform.position.z);

            circleListPoint.Add(obj);
        }
    }
}

public enum CircleSpawnType
{
    XY,
    XZ,
    YZ
}
