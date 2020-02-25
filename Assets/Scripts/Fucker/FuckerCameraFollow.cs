using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Utils;

public class FuckerCameraFollow : MonoBehaviour
{
    public Transform 目标;
    public Transform 自己;

    [Space]
    public float speed = 0.1f;

    [Space]
    public Vector3Item 轴;

    Vector3 current;
    void Update()
    {
        自己.position = 自己.position.Item(轴).Merge(Vector3.SmoothDamp(自己.position, 目标.position, ref current, speed)) ;
    }
}
