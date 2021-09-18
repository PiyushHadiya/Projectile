using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsProjectile : MonoBehaviour
{
    public bool isBulletFollowPath = false;
    public Transform bullet;
    public float bulletSimulationSpeed = 0.5f;
    public GameObject ProjectilePoint;
    public int PointStep = 20;
    public float timeInterval = 0.025f;
    public float initialVelocity = 10;
    [Range(0, 360)]
    public float angle = 45;

    private List<Transform> BallList = new List<Transform>();
    private List<float> timeIntervalList = new List<float>();

    private float horizontalVelocity;
    private float verticalVelocity;
    private float remainingTime = 0;
    private void Start()
    {
        CreateBall();
    }

    private void CreateBall()
    {
        if (ProjectilePoint != null)
        {
            for (int i = 0; i < PointStep; i++)
            {
                Transform currentBallTransform = Instantiate(ProjectilePoint).transform;
                BallList.Add(currentBallTransform);
            }
        }
        float timeint = 0;
        for (int i = 0; i < PointStep; i++)
        {
            timeIntervalList.Add(timeint);
            timeint += timeInterval;
        }
    }

    private void Projectile()
    {
        float currentValue = (Mathf.InverseLerp(0, 360, angle) * 6.35f) + 44;
        horizontalVelocity = initialVelocity * Mathf.Cos(currentValue);
        verticalVelocity = initialVelocity * Mathf.Sin(currentValue);

        float x, y, t;

        for (int i = 0; i < PointStep; i++)
        {
            t = timeIntervalList[i];

            x = horizontalVelocity * t;
            y = verticalVelocity * t - (0.5f * 9.8f * t * t);

            if (ProjectilePoint != null)
                BallList[i].position = new Vector2(x, y);
        }
    }

    private void Update()
    {
        Projectile();

        if (Input.GetMouseButtonDown(0))
        {
            isBulletFollowPath = true;
            remainingTime = 0;
        }
        if (isBulletFollowPath)
        {
            remainingTime += Time.deltaTime * bulletSimulationSpeed;
            float x = horizontalVelocity * remainingTime;
            float y = verticalVelocity * remainingTime - (0.5f * 9.8f * remainingTime * remainingTime);
            bullet.position = new Vector2(x, y);
        }
    }
}
