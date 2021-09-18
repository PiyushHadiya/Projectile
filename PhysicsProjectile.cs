using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsProjectile : MonoBehaviour
{
    public bool isBulletFollowPath = false;
    public Transform bullet;
    public float bulletSimulationSpeed = 1f;
    public GameObject ProjectilePoint;
    public int PointStep = 50;
    public float PointSpace = 0.05f;
    public float InitialVelocity = 10;
    [Range(0, 360)]
    public float Angle = 45;

    private List<Transform> ballList = new List<Transform>();
    private List<float> timeIntervalList = new List<float>();

    private float horizontalVelocity;
    private float verticalVelocity;
    private float remainingTime = 0;
    private void Start()
    {
        CreatePointsAndTimeStep();
    }

    private void CreatePointsAndTimeStep()
    {
        if (ProjectilePoint != null)
        {
            for (int i = 0; i < PointStep; i++)
            {
                Transform currentBallTransform = Instantiate(ProjectilePoint).transform;
                ballList.Add(currentBallTransform);
            }
        }

        float timeStep = 0;
        for (int i = 0; i < PointStep; i++)
        {
            timeIntervalList.Add(timeStep);
            timeStep += PointSpace;
        }
    }

    private void Projectile()
    {
        // Here "44 to 50.35" is "0 to 360"
        // So convert custom angle to -> 44 to 50.35
        // Here "givenAngle" value is between ( 44 -> 50.35 )
        float givenAngle = (Mathf.InverseLerp(0, 360, Angle) * 6.35f) + 44;
        
        horizontalVelocity = InitialVelocity * Mathf.Cos(givenAngle);
        verticalVelocity = InitialVelocity * Mathf.Sin(givenAngle);

        float x, y, t;

        for (int i = 0; i < PointStep; i++)
        {
            t = timeIntervalList[i];

            x = horizontalVelocity * t;
            y = verticalVelocity * t - (0.5f * 9.8f * t * t);

            if (ProjectilePoint != null)
                ballList[i].position = new Vector2(x, y);
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
        if (Input.GetMouseButtonDown(1))
        {
            isBulletFollowPath = false;
        }
    }
}
