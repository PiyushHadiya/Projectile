using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairHandler : MonoBehaviour
{
    public static StairHandler Instance;
    [Header("REF")]
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private GameObject[] StairBox = new GameObject[0];
    [Header("SETTINGS")]
    [SerializeField] private float giftYOffset = 0.25f;
    [SerializeField] private float playerYOffset = 0.5f;
    [SerializeField] private float forwardZOffset = 5f;
    [SerializeField] private float forwardSpeed = 10;
    [Header("CHECKING")]

    private Queue<Transform> GiftCollection = new Queue<Transform>();
    private Action EndFunction;
    private Transform playerTransform;
    private Vector3 startPosition;
    private int stairCount;
    private float startP, endP, tvalue, offSetValue;
    private bool isTimeUpdate = false;
    #region MonoBehaviour CallBacks
    private void Awake() => Instance = this;
    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        if (isTimeUpdate)
            offSetValue = Mathf.Lerp(startP, endP, tvalue);
    }
    #endregion

    #region Public Functions
    public void ActiveStair(int count, Transform playerPosition, Action onComplete)
    {
        transform.position = startPosition;
        EndFunction = onComplete;
        playerTransform = playerPosition;
        stairCount = count;

        StartCoroutine(SetStairPosition());
        StartCoroutine(PlayerTransition());
        StartCoroutine(CameraZoomOut());
    }
    #endregion

    #region IEnumerators
    IEnumerator SetStairPosition()
    {
        float giftPosOffset = 0;
        playerTransform.position = startPosition;

        for (int i = 0; i < stairCount; i++)
        {
            giftPosOffset += giftYOffset;

            int randomBoxID = UnityEngine.Random.Range(0, StairBox.Length);
            Transform currentGiftBox = Instantiate(StairBox[randomBoxID], startPosition, Quaternion.identity).transform;
            currentGiftBox.position = currentGiftBox.position + Vector3.up * giftPosOffset;
            currentGiftBox.parent = transform;
            giftPosOffset += giftYOffset;

            GiftCollection.Enqueue(currentGiftBox);
            yield return new WaitForSeconds(0.084f);
        }
        yield return new WaitForSeconds(0.8f);
        StartCoroutine(MoveForward());
        StartCoroutine(CameraZoomIn());
    }
    IEnumerator PlayerTransition()
    {
        bool isTransition = true;
        float playerOffset = (stairCount) * playerYOffset;
        Vector3 winTarget = startPosition + new Vector3(0, playerOffset, 0);

        while (isTransition)
        {
            Vector3 currentPosition = playerTransform.position;
            playerTransform.position = Vector3.MoveTowards(playerTransform.position, winTarget, Time.deltaTime * 5.5f);

            if (Mathf.Approximately(currentPosition.y, winTarget.y))
                yield break;

            yield return null;
        }
    }
    IEnumerator CameraZoomOut()
    {
        if (virtualCamera == null)
            yield break;

        var cineTransposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        float zoomOutValue = cineTransposer.m_FollowOffset.z - (8f * stairCount * 0.05f);
        startP = cineTransposer.m_FollowOffset.z;
        endP = zoomOutValue;
        tvalue = 0;
        isTimeUpdate = true;
        offSetValue = startP;
        float camSpeed = 0.2f;

        while (true)
        {
            tvalue += camSpeed * Time.deltaTime;
            cineTransposer.m_FollowOffset = new Vector3(0, 5, offSetValue);

            if (tvalue >= 1)
            {
                isTimeUpdate = false;
                yield break;
            }

            yield return null;
        }
    }
    IEnumerator CameraZoomIn()
    {
        if (virtualCamera == null)
            yield break;

        var cineTransposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        float zoomInValue = cineTransposer.m_FollowOffset.z + (10f * stairCount * 0.05f);
        startP = cineTransposer.m_FollowOffset.z;
        endP = -7;
        tvalue = 0;
        isTimeUpdate = true;
        offSetValue = startP;
        float camSpeed = 0.1f;

        while (true)
        {
            tvalue += camSpeed * Time.deltaTime;
            cineTransposer.m_FollowOffset = new Vector3(0, 5, offSetValue);

            if (tvalue >= 1)
            {
                isTimeUpdate = false;
                yield break;
            }
            yield return null;
        }
    }

    IEnumerator MoveForward()
    {
        bool isMoveForward = true;
        float offset = 0;

        while (isMoveForward)
        {
            Vector3 currentPosition = transform.localPosition;
            currentPosition.z += forwardSpeed * Time.deltaTime;

            Vector3 playerPos = playerTransform.position;
            playerPos.z = currentPosition.z;
            playerTransform.position = playerPos;

            currentPosition.x = playerTransform.position.x;
            transform.localPosition = currentPosition;

            if (transform.position.z >= startPosition.z + offset)
            {
                offset += forwardZOffset;
                for (int i = 0; i < 5; i++)
                {
                    if (GiftCollection.Count != 0)
                    {
                        Transform currentGift = GiftCollection.Dequeue();
                        currentGift.parent = null;
                    }
                    else
                    {
                        EndFunction.Invoke();
                        yield break;
                    }
                }
            }

            yield return null;
        }
    }
    #endregion
}
