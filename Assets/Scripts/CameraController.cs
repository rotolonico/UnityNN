using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;
        
    public bool isActive;

    public Camera thisCam;

    public float minOrthoSize;

    public float maxOrthoSize;

    int prevTouchCount;

    private Vector3 touchStart;
    private Vector3 startPosition = Vector3.zero;

    private Vector3 originalStartPoint;

    private Vector2 midPoint;

    float prevTouchDistance;

    bool isScrolling;

    float buffer = 200f;

    float initSize = 3;

    private Vector3 oldCameraPosition;
    private float oldCameraSize;

    private void Awake() => Instance = this;

    Vector2 CalcMidPoint()
    {
        int touchCount = Input.touchCount;

        Vector2 newMidPoint = new Vector2();

        for (int i = 0; i < touchCount; i++)
        {
            newMidPoint += Input.touches[i].position;
        }

        newMidPoint /= touchCount;

        return newMidPoint;
    }

    float CalcSpread()
    {
        int touchCount = Input.touchCount;

        float touchDistance = 0f;

        for (int i = 0; i < touchCount; i++)
        {
            touchDistance += (midPoint - Input.touches[i].position).magnitude;
        }

        return touchDistance;
    }

    private void Update()
    {
        if (!isActive)
            return;

        if (Input.touchCount != prevTouchCount)
        {
            touchStart = thisCam.ScreenToWorldPoint(Input.mousePosition);
            prevTouchCount = Input.touchCount;
            midPoint = CalcMidPoint();
            prevTouchDistance = CalcSpread();
            return;
        }

        if (Input.touchCount >= 2 && prevTouchCount == Input.touchCount)
        {
            midPoint = CalcMidPoint();
            var touchDistance = CalcSpread();

            if (prevTouchDistance < float.Epsilon || float.IsNaN(prevTouchDistance) ||
                float.IsInfinity(touchDistance))
            {
                prevTouchDistance = touchDistance;
                return;
            }

            float magnification = touchDistance / prevTouchDistance;

            prevTouchDistance = touchDistance;

            if (float.IsNaN(magnification))
                return;

            thisCam.orthographicSize *= 1 / magnification;
            thisCam.orthographicSize = Mathf.Clamp(thisCam.orthographicSize, minOrthoSize, maxOrthoSize);
        }

        float scrollMagnification = 0.08f;

        if (Math.Abs(Input.mouseScrollDelta.y) > 0)
        {
            float newSize =
                Mathf.Max(
                    thisCam.orthographicSize * (1 - scrollMagnification * Math.Sign(Input.mouseScrollDelta.y)),
                    minOrthoSize);

            if (Mathf.Abs(newSize - thisCam.orthographicSize) < float.Epsilon)
                return;

            float scaleFactor = thisCam.orthographicSize / newSize;

            thisCam.orthographicSize = Mathf.Clamp(newSize, minOrthoSize, maxOrthoSize);

            if (Math.Abs(thisCam.orthographicSize - maxOrthoSize) > 0.1f)
            {
                var camPosition = thisCam.transform.position;
                camPosition += (thisCam.ScreenToWorldPoint(Input.mousePosition) - camPosition) * (scaleFactor - 1);
                thisCam.transform.position = camPosition;
            }
            
            Main.Instance.GraphNetwork();
        }

        var wasScrolling = isScrolling;
        isScrolling = Mathf.Abs(Input.mouseScrollDelta.y) > 0f;

        if (wasScrolling == false && isScrolling)
            touchStart = thisCam.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonDown(2)) touchStart = thisCam.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButton(2))
        {
            var camPosition = thisCam.transform.position;

            midPoint = Input.mousePosition;

            var direction = touchStart - thisCam.ScreenToWorldPoint(midPoint);

            Vector3 position = camPosition;

            position += direction;

            camPosition = position;
            camPosition.z = -10f;
            thisCam.transform.position = camPosition;
            
            Main.Instance.GraphNetwork();
        }

        prevTouchCount = Input.touchCount;
    }
}