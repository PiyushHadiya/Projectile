using UnityEngine;

[RequireComponent(typeof(LineRenderer))]

public class Projectile3D : MonoBehaviour
{
    public bool xAxisForward = false;
    public GameObject CrossHair;
    [Range(0.1f, 100)]
    public float Velocity = 10;
    [Range(0.01f, 0.5f)]
    public float PointDistance = 0.04f;
    [Range(3, 100)]
    public int MaxPoint = 40;
    public LayerMask layerMask;
    public Vector3 gravity = new Vector3(0, -Physics.gravity.y, 0);
    internal bool hasFired = false;

    private float horizontalVelocity, verticalVelocity, depthVelocity;
    private float xDisplacement, yDisplacement, zDisplacement;

    private float Angle;
    private float PIAngle;

    private LineRenderer lineRenderer;
    private Vector3 CurrentPoint, lastPoint;
    private float currentTime;

    private void Awake()
    {
        lineRenderer = this.GetComponent<LineRenderer>();
    }

    private void Update()
    {
        bool isProjectileUpdate = Time.frameCount % 3 == 0;

        if (!isProjectileUpdate)
            return;

        CurrentAngle();
        CalculateVelocity();

        SetProjectileDirectionPath();

        currentTime = 0;

        CurrentPoint = this.transform.position;
        lineRenderer.positionCount = MaxPoint;

        SetProjectilePath();
    }

    #region DrawProjectile
    private void SetProjectilePath()
    {
        for (int i = 0; i < MaxPoint; i++)
        {
            if (!Physics.CheckSphere(CurrentPoint, 0.1f, layerMask, QueryTriggerInteraction.Collide))
            {
                lastPoint = CurrentPoint;
                currentTime += PointDistance;

                yDisplacement = (float)(verticalVelocity * currentTime + 0.5f * -gravity.y * (currentTime * currentTime)) + this.transform.position.y;
                xDisplacement = horizontalVelocity * currentTime + 0.5f * gravity.x * (currentTime * currentTime) + this.transform.position.x;

                if (Angle >= 90 && Angle < 270 || Velocity < 0)
                    zDisplacement = -(depthVelocity * currentTime + 0.5f * -gravity.z * (currentTime * currentTime)) + this.transform.position.z;
                else
                    zDisplacement = depthVelocity * currentTime + 0.5f * gravity.z * (currentTime * currentTime) + this.transform.position.z;

                CurrentPoint = new Vector3(xDisplacement, yDisplacement, zDisplacement);

                lineRenderer.SetPosition(i, CurrentPoint);
            }
            else
            {
                lineRenderer.positionCount = i;

                if (CrossHair == null)
                    break;

                RaycastHit rayHit;
                Vector3 direction = (CurrentPoint - lastPoint).normalized;
                Physics.Raycast(lastPoint, direction, out rayHit, 10, layerMask);

                CrossHair.transform.position = rayHit.point;
                CrossHair.transform.rotation = Quaternion.FromToRotation(Vector3.up, rayHit.normal);

                break;
            }
        }
    }
    #endregion

    #region Private Functions
    private void CurrentAngle()
    {
        if (xAxisForward)
        {
            Angle = transform.rotation.eulerAngles.z;
            PIAngle = transform.rotation.eulerAngles.y + 90;
        }
        else
        {
            Angle = -transform.rotation.eulerAngles.x;
            PIAngle = transform.rotation.eulerAngles.y;
        }
    }

    private void CalculateVelocity()
    {
        verticalVelocity = Velocity * Mathf.Sin(Angle * Mathf.Deg2Rad);
        horizontalVelocity = Velocity * Mathf.Cos(Angle * Mathf.Deg2Rad) * Mathf.Sin(PIAngle * Mathf.Deg2Rad);
    }

    private void SetProjectileDirectionPath()
    {
        if (((PIAngle - 90 > 180 && xAxisForward == true) || ((PIAngle < 90 || PIAngle > 270) && xAxisForward == false)) && Velocity != verticalVelocity)
            depthVelocity = Mathf.Sqrt((Velocity * Velocity) - (horizontalVelocity * horizontalVelocity) - (verticalVelocity * verticalVelocity));
        else if (Velocity != verticalVelocity)
            depthVelocity = -Mathf.Sqrt((Velocity * Velocity) - (horizontalVelocity * horizontalVelocity) - (verticalVelocity * verticalVelocity));
        else
            depthVelocity = 0;


        //If "depthVelocity" is an imaginary number (as sometimes happens when used in 2D/near 2D situations) it sets "depthVelocity" to a new equation
        if (float.IsNaN(depthVelocity))
            depthVelocity = Velocity * Mathf.Cos(PIAngle * Mathf.Deg2Rad);
    }


    #endregion
}