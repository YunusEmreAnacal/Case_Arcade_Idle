using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Hareket Ayarlarý")]
    public float moveSpeed = 6f; 
    public float rotationSpeed = 15f; 

    [Header("UI Referanslarý")]
    public Canvas canvas; 
    public RectTransform joystickBase; 
    public RectTransform joystickKnob; 
    public float joystickRadius = 125f; 

    private CharacterController controller;
    private Vector2 touchStartPosition;
    private Vector2 moveInput;
    private bool isTouching = false;
    private Camera mainCamera;

  
    private Animator animator;
    private int animSpeedID;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        mainCamera = Camera.main;

        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Player nesnesinde Animator bileþeni bulunamadý!");
        }
        animSpeedID = Animator.StringToHash("Speed");

        joystickBase.gameObject.SetActive(false);
    }

    void Update()
    {
        bool touchStarted = false;
        bool touchEnded = false;
        Vector2 currentPosition = Vector2.zero;


#if UNITY_EDITOR
        // ==== EDÝTÖR ÝÇÝN MOUSE (FARE) KONTROLÜ ====
        if (Mouse.current != null)
        {
            currentPosition = Mouse.current.position.ReadValue();
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                touchStarted = true;
            }
            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                touchEnded = true;
            }
        }
#else
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.isInContact)
        {
            currentPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            if (Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            {
                touchStarted = true;
            }
            if (Touchscreen.current.primaryTouch.press.wasReleasedThisFrame)
            {
                touchEnded = true;
            }
        }
#endif
       

        if (touchStarted)
        {
      
            StartTouch(currentPosition);
        }
        else if (isTouching) 
        {
            if (touchEnded)
            {
               
                EndTouch();
            }
            else
            {
         
                DragTouch(currentPosition);
            }
        }

        HandleMovement();
    }

    private void StartTouch(Vector2 screenPosition)
    {
        isTouching = true;

        Vector2 canvasPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            screenPosition,
            canvas.worldCamera,
            out canvasPos);

        joystickBase.anchoredPosition = canvasPos;
        joystickKnob.anchoredPosition = Vector2.zero;
        joystickBase.gameObject.SetActive(true);
    }

    private void DragTouch(Vector2 screenPosition)
    {
        Vector2 canvasPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            screenPosition,
            canvas.worldCamera,
            out canvasPos);

        Vector2 direction = canvasPos - joystickBase.anchoredPosition;

        if (direction.magnitude > joystickRadius)
        {
            direction = direction.normalized * joystickRadius;
        }

        joystickKnob.anchoredPosition = direction;
        moveInput = direction / joystickRadius;
    }

    private void EndTouch()
    {
        isTouching = false;
        joystickBase.gameObject.SetActive(false);
        moveInput = Vector2.zero;
        animator.SetFloat(animSpeedID, 0f);
    }

    private void HandleMovement()
    {
        Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y);
        float currentSpeed = moveInput.magnitude;
        animator.SetFloat(animSpeedID, currentSpeed);

        if (currentSpeed > 0.1f)
        {
            Vector3 movement = moveDirection.normalized * moveSpeed * currentSpeed * Time.deltaTime;
            controller.Move(movement);

            Quaternion targetRotation = Quaternion.LookRotation(moveDirection.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        else
        {
            controller.Move(Vector3.down * 0.1f * Time.deltaTime);
        }
    }
}