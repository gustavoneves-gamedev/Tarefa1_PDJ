using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerRoot : MonoBehaviour
{
    [Header("Touch config")]
    [SerializeField] private float swipeDistance = 100f;
    private float lastTapTime;
    private int tapCount;
    private Vector2 startTouch;

    [Header("Player Movement")]
    [SerializeField] private bool isJumping;
    [SerializeField] private float verticalSpeed = 10f;
    private float defaultVerticalSpeed;
    [SerializeField] private float gravity = 5f;
    private bool canCloneMove;


    [Header("References")]
    [SerializeField] private GameObject playerObj;
    [SerializeField] private GameObject playerCloneR;
    [SerializeField] private GameObject playerCloneL;
    [SerializeField] private Material[] materials; //Eu iria criar materiais para fazer a troca,
                                                   //mas descobri que a unity tem cores padrões
    private CharacterController cc;


    void Start()
    {
        cc = GetComponent<CharacterController>();
        defaultVerticalSpeed = verticalSpeed;

    }


    void Update()
    {
        DetectSwipes();
        DetectPinch();
        DetectTaps();

        if (Input.GetKeyDown(KeyCode.C))
        {
            canCloneMove = true;
            playerCloneL.SetActive(true);
            playerCloneR.SetActive(true);
        }

        Clone();


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
                        if (delta.x > 0)
                        {
                            Debug.Log("Swipe Right");
                        }
                        else
                        {
                            Debug.Log("Swipe Left");
                        }
                    }
                    else
                    {
                        if (delta.y > 0)
                        {
                            Debug.Log("Swipe Up");
                            isJumping = true;
                        }
                        else
                        {
                            Debug.Log("Swipe Down");
                        }
                    }
                }
            }
        }
    }

    private static void DetectPinch()
    {
        if (Input.touchCount == 2)
        {
            Touch t0 = Input.GetTouch(0);
            Touch t1 = Input.GetTouch(1);

            Vector2 start0 = t0.position - t0.deltaPosition;
            Vector2 start1 = t1.position - t1.deltaPosition;

            float startDist = (start0 - start1).magnitude;
            float atualDist = (t0.position - t1.position).magnitude;

            if (atualDist > startDist)
            {
                Debug.Log("Pinch Out (Zoom In)");
            }
            else if (atualDist < startDist)
            {
                Debug.Log("Pinch In (Zoom Out)");
            }
        }
    }

    #region Taps
    private void DetectTaps()
    {
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began || Input.GetKeyDown(KeyCode.Mouse0))
        {
            float timeNow = Time.time;

            if (timeNow - lastTapTime < 0.3f)
                tapCount++;
            else
                tapCount = 1;

            lastTapTime = timeNow;

            if (tapCount == 1)
            {
                Invoke("SingleTap", 0.3f);
            }

            if (tapCount == 2)
            {
                CancelInvoke();
                Invoke("DoubleTap", 0.3f);
            }
            if (tapCount == 3)
            {
                CancelInvoke();
                Invoke("TripleTap", 0.2f); //Queria colocar instantâneo para o triple, mas decidi só reduzir o tempo para manter o paralelismo
            }
        }
    }

    private void SingleTap()
    {
        Debug.Log("Single Tap");

        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(0)) SceneManager.LoadScene(1);
        else SceneManager.LoadScene(0);
    }

    private void DoubleTap()
    {
        Debug.Log("Double Tap");
        ChangeMaterial1();
    }

    private void TripleTap()
    {
        Debug.Log("Triple Tap");
        ChangeMaterial2();
    }

    #endregion

    private void Jump()
    {

        Vector3 vertical = Vector3.up * verticalSpeed * Time.deltaTime;

        verticalSpeed = verticalSpeed - gravity * Time.deltaTime;

        cc.Move(vertical);

        if (cc.isGrounded)
        {
            isJumping = false;
            verticalSpeed = defaultVerticalSpeed;
        }
    } //Swipe Up

    private void ChangeMaterial1()
    {
        playerObj.GetComponent<MeshRenderer>().material.color = Color.yellow;
    } //Double Tap

    private void ChangeMaterial2()
    {
        playerObj.GetComponent<MeshRenderer>().material.color = Color.crimson;
    } //Triple Tap

    private void Clone()
    {
        if (!canCloneMove) return;
        CloneMovement(playerCloneL, 0);
        CloneMovement(playerCloneR, 1);

    }

    private void CloneMovement(GameObject clone, int code)
    {
        if (code == 0)
        {
            clone.transform.Translate(Vector3.left * 20f * Time.deltaTime);
            if (clone.transform.position.x <= -2) canCloneMove = false;
        }

        if (code == 1)
        {
            clone.transform.Translate(Vector3.right * 20f * Time.deltaTime);
            if (clone.transform.position.x >= 2) canCloneMove = false;
        }
    }

}
