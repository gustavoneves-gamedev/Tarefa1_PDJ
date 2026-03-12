using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerRoot : MonoBehaviour
{
    [Header("Touch config")]
    [SerializeField] private float swipeDistance = 100f;
    private float lastTapTime;
    private int tapCount;
    private Vector2 startTouch;
    private bool hasSwipe;
    private float touchTime;

    [Header("Player Movement")]
    [SerializeField] private bool isJumping;
    [SerializeField] private float verticalSpeed = 10f;
    private float defaultVerticalSpeed;
    [SerializeField] private float gravity = 5f;

    [Header("Clone Skill")]
    [SerializeField] private float cloneSpeed = 20f;
    private Vector3 defaultPosition;
    private bool canCloneMove;
    private bool isCloneMoving;
    private bool isCloneOut; //Para um controle mais refinado, eu teria que criar variáveis para cada clone, mas dada a 
                             //natureza do exercício, decidi manter simples


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
        defaultPosition = playerObj.transform.position;

    }


    void Update()
    {
        DetectSwipes();
        DetectPinch();
        DetectTaps();

        //Jump 
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isJumping = true;
        }

        if (isJumping)
        {
            Jump();
            return;
        }

        //Clone Skill
        if (Input.GetKeyDown(KeyCode.C) && !isCloneMoving && playerCloneR != null)
        {
            canCloneMove = true;
            isCloneMoving = true;

            if (!isCloneOut)
            {
                playerCloneL.SetActive(true);
                playerCloneR.SetActive(true);
            }

        }

        Clone();


    }

    private void DetectSwipes()
    {
        if (Input.touchCount == 1)
        {
            Touch t = Input.GetTouch(0);

            if (t.phase == TouchPhase.Began)
            {
                startTouch = t.position;
                touchTime = Time.time;
                hasSwipe = false;
            }
            else if (t.phase == TouchPhase.Ended)
            {
                Vector2 delta = t.position - startTouch;

                if (delta.magnitude > swipeDistance)
                {
                    hasSwipe = true;

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
                            tapCount = 0;
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

    private void DetectPinch()
    {
        if (Input.touchCount == 2)
        {
            Touch t0 = Input.GetTouch(0);
            Touch t1 = Input.GetTouch(1);

            Vector2 start0 = t0.position - t0.deltaPosition;
            Vector2 start1 = t1.position - t1.deltaPosition;

            float startDist = (start0 - start1).magnitude;
            float atualDist = (t0.position - t1.position).magnitude;

            if (atualDist > startDist && !isCloneOut && playerCloneR != null)
            {
                Debug.Log("Pinch Out (Zoom In)");
                canCloneMove = true;
                isCloneMoving = true;

                if (!isCloneOut)
                {
                    playerCloneL.SetActive(true);
                    playerCloneR.SetActive(true);
                }
            }
            else if (atualDist < startDist && isCloneOut && playerCloneR != null)
            {
                Debug.Log("Pinch In (Zoom Out)");
                canCloneMove = true;
                isCloneMoving = true;

                if (!isCloneOut)
                {
                    playerCloneL.SetActive(true);
                    playerCloneR.SetActive(true);
                }
            }
        }
    }

    #region Taps
    private void DetectTaps()
    {
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended) //|| Input.GetKeyDown(KeyCode.Mouse0)
        {
            if (Time.time - touchTime > 0.15f || hasSwipe)
            {
                tapCount = 0;
                return;
            }


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

    private void Clone() //Pinch Out
    {
        if (!canCloneMove) return;
        CloneMovement(playerCloneL, 0);
        CloneMovement(playerCloneR, 1);

    }

    private void CloneMovement(GameObject clone, int code)
    {
        int i = 1;
        if (isCloneOut) i = -1;

        if (code == 0)
        {
            clone.transform.Translate(Vector3.left * cloneSpeed * i * Time.deltaTime);

            if (isCloneOut && clone.transform.position.x >= 0)
            {
                canCloneMove = false;
                isCloneOut = false;
                isCloneMoving = false;
                RestoreClone();

            }
            else if (clone.transform.position.x <= -2)
            {
                canCloneMove = false;
                isCloneOut = true;
                isCloneMoving = false;
            }
        }

        if (code == 1)
        {
            clone.transform.Translate(Vector3.right * cloneSpeed * i * Time.deltaTime);

            if (isCloneOut && clone.transform.position.x <= 0)
            {
                canCloneMove = false;
                isCloneOut = false;
                isCloneMoving = false;
                RestoreClone();

            }
            else if (clone.transform.position.x >= 2)
            {
                canCloneMove = false; //Estou repetindo aqui, mas acho que não precisaria uma vez que a bool que
                                      //controla os dois clones já está sendo definida no if acima
                isCloneOut = true;
                isCloneMoving = false;
            }

        }
    }

    private void RestoreClone()
    {
        playerCloneL.SetActive(false);
        playerCloneR.SetActive(false);

        playerCloneL.transform.position = defaultPosition;
        playerCloneR.transform.position = defaultPosition;
    }

}
