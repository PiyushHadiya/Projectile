using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchJoystick : MonoBehaviour
{
    public Transform target;
    public float MouseSensitivity = 1f;

    private Vector2 mouseStartPosition;
    private Vector2 mouseEndPosition;
    [SerializeField] private Vector2 direction = Vector2.zero;

    private float MouseX;
    private float MouseY;
    private void Update()
    {
        if (Input.touchCount > 0)
            TouchControl();

        MouseX += direction.x * MouseSensitivity * Time.deltaTime;
        MouseY -= direction.y * MouseSensitivity * Time.deltaTime;

        target.eulerAngles = new Vector3(MouseY, MouseX, 0);
    }
    private void TouchControl()
    {
        Touch touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Began)
        {
            mouseStartPosition = Input.mousePosition;
        }

        if (touch.phase == TouchPhase.Moved)
        {
            mouseEndPosition = Input.mousePosition;

            if (Vector2.Distance(mouseStartPosition, mouseEndPosition) > 100)
            {
                SwipeDetect();
                mouseStartPosition = mouseEndPosition;
            }

        }

        if (touch.phase == TouchPhase.Ended)
        {
            direction = Vector2.zero;
        }
    }
    private void SwipeDetect()
    {
        direction = (mouseEndPosition - mouseStartPosition).normalized;

        

        //if (direction.x > 0)
        //{
        //    float value = Vector2.Dot(Vector2.up, direction);

        //    if (value > 0.5f) direction.y = 1f;
        //    else if (value < 0.5f && value > -0.5f) direction.x = 1f;
        //    else if (value < -0.5f) direction.y = -1f;
        //}
        //if (direction.x < 0)
        //{
        //    float value = Vector2.Dot(Vector2.up, direction);

        //    if (value > 0.5f) direction.y = 1f;
        //    else if (value < 0.5f && value > -0.5f) direction.x = -1f;
        //    else if (value < -0.5f) direction.y = -1f;
        //}
    }
}
