using UnityEngine;
using System.Collections;

public class BallLauncher : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Rigidbody ball;
    public Transform StartPoint;
    public Transform target;
    public LayerMask layerMask;

    public float h = 25;
    public float gravity = -18;
    [Range(3, 30)]
    public int resolution = 10;
    public bool debugPath;

    void Start()
    {
        ball.useGravity = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Launch();
        }

        if (debugPath)
        {
            DrawPath();
        }
    }

    void Launch()
    {
        Physics.gravity = Vector3.up * gravity;
        ball.useGravity = true;
        ball.velocity = CalculateLaunchData().initialVelocity;
    }

    LaunchData CalculateLaunchData()
    {

        float displacementY = target.position.y - StartPoint.position.y;
        Vector3 displacementXZ = new Vector3(target.position.x - StartPoint.position.x, 0, target.position.z - StartPoint.position.z);
        float time = Mathf.Sqrt(-2 * h / gravity) + Mathf.Sqrt(2 * (displacementY - h) / gravity);
        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * h);
        Vector3 velocityXZ = displacementXZ / time;

        return new LaunchData(velocityXZ + velocityY * -Mathf.Sign(gravity), time);
    }

    void DrawPath()
    {
        lineRenderer.positionCount = resolution + 1;

        LaunchData launchData = CalculateLaunchData();
        Vector3 previousDrawPoint = StartPoint.position;


        for (int i = 0; i <= resolution; i++)
        {
            float simulationTime = i / (float)resolution * launchData.timeToTarget;
            Vector3 displacement = launchData.initialVelocity * simulationTime + Vector3.up * gravity * simulationTime * simulationTime / 2f;
            Vector3 drawPoint = StartPoint.position + displacement;
            Debug.DrawLine(previousDrawPoint, drawPoint, Color.green);
            previousDrawPoint = drawPoint;
            lineRenderer.SetPosition(i, previousDrawPoint);

            //if (Physics.CheckSphere(drawPoint, 0.5f, layerMask))
            //{
            //    lineRenderer.positionCount = i + 1;
            //    break;
            //}
        }
    }

    struct LaunchData
    {
        public readonly Vector3 initialVelocity;
        public readonly float timeToTarget;

        public LaunchData(Vector3 initialVelocity, float timeToTarget)
        {
            this.initialVelocity = initialVelocity;
            this.timeToTarget = timeToTarget;
        }

    }
}