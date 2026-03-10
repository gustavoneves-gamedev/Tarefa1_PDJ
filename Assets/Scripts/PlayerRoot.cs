using UnityEngine;

public class PlayerRoot : MonoBehaviour
{

    private float lastTapTime;
    private int tapCount;
    private Vector2 startTouch;
    [SerializeField] private float swipeDistance = 100f;

    [SerializeField] private bool isJumping;
    [SerializeField] private float verticalSpeed = 10f;
    private float defaultVerticalSpeed;    
    private CharacterController cc;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cc = GetComponent<CharacterController>();
        defaultVerticalSpeed = verticalSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        DetectTaps();
        DetectSwipes();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            isJumping = true;
        }

        if (isJumping) Jump();

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
        //Colocar a troca de cena aqui
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

    private void Jump()
    {
        
        Vector3 vertical = Vector3.up * verticalSpeed * Time.deltaTime;

        verticalSpeed -= .1f;

        cc.Move(vertical);

        if (cc.isGrounded)
        {
            isJumping = false;
            verticalSpeed = defaultVerticalSpeed;
        }
    }


}
