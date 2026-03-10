using UnityEngine;

public class PlayerRoot : MonoBehaviour
{

    private float lastTapTime;
    private int tapCount;
    private Vector2 startTouch;
    [SerializeField] private float swipeDistance = 100f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        DetectTaps();
        DetectSwipes();

    }

    private void DetectSwipes()
    {
        if (Input.touchCount == 1)
        {
            Touch t = Input.GetTouch(0);

            if (t.phase == TouchPhase.Began)
            {
                startTouch = t.position;
            }
            else if (t.phase == TouchPhase.Ended)
            {
                Vector2 delta = t.position - startTouch;

                if (delta.magnitude > swipeDistance)
                {
                    if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                    {
                        if (delta.x > 0) Debug.Log("Swipe Right");
                        else Debug.Log("Swipe Left");
                    }
                    else
                    {
                        if (delta.y > 0) Debug.Log("Swipe Up");
                        else Debug.Log("Swipe Down");
                    }
                }
            }
        }
    }

    private void DetectTaps()
    {
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began || Input.GetKeyDown(KeyCode.Mouse0))
        {
            float timeNow = Time.time;

            if (timeNow - lastTapTime < 0.5f)
                tapCount++;
            else
                tapCount = 1;

            lastTapTime = timeNow;

            if (tapCount == 1)
            {
                Invoke("SingleTap", 0.5f);
            }

            if (tapCount == 2)
            {
                CancelInvoke();
                Invoke("DoubleTap", 0.5f);
            }
            if (tapCount == 3)
            {
                CancelInvoke();
                Invoke("TripleTap", 0.5f);
            }
        }
    }

    private void SingleTap()
    {
        Debug.Log("Single Tap");
    }

    private void DoubleTap()
    {
        Debug.Log("Double Tap");
    }

    private void TripleTap()
    {
        Debug.Log("Triple Tap");
    }
}
