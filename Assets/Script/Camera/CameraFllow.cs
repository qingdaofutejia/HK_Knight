using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFllow : MonoBehaviour
{
    public Transform target;
    public float smoothTime = 0.2f;
    public Vector3 offset;

    [Header("Y轴缓冲（Dead Zone）")]
    public float yDeadZone = 1.5f;

    private Vector3 velocity = Vector3.zero;


    void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPosition = transform.position;

        //X轴始终跟随
        targetPosition.x = target.position.x + offset.x;

        //超过范围才移动
        float deltaY = target.position.y - transform.position.y;

        if (Mathf.Abs(deltaY) > yDeadZone)
        {
            targetPosition.y = target.position.y + offset.y;
        }

        // 锁定Z轴
        targetPosition.z = transform.position.z;

        //平滑移动
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref velocity,
            smoothTime
        );
    }
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        SnapToTarget();
    }
    private void SnapToTarget( )
    {
        if (target == null) return;

        Vector3 pos = transform.position;
        pos.x = target.position.x + offset.x;
        pos.y = target.position.y + offset.y;
        // z 保持相机原来的值
        transform.position = pos;
    }
}
