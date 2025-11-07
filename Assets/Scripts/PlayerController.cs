using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

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


        // Dokunmatik giriþini (varsa) al
        TouchControl primaryTouch = null;
        if (Touchscreen.current != null)
        {
            primaryTouch = Touchscreen.current.primaryTouch;
        }

#if UNITY_EDITOR
        // ==== EDÝTÖR ÝÇÝN MOUSE (FARE) KONTROLÜ ====
        if (Mouse.current != null)
        {
            // FARE BU FRAME MÝ TIKLADI?
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                currentPosition = Mouse.current.position.ReadValue();
                touchStarted = true;
            }

            // FARE BU FRAME MÝ BIRAKILDI?
            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                touchEnded = true;
            }

            // FARE BASILI MI TUTULUYOR?
            if (Mouse.current.leftButton.isPressed)
            {
                currentPosition = Mouse.current.position.ReadValue();
            }
        }
#else
        // ==== MOBÝL ÝÇÝN DOKUNMATÝK KONTROLÜ ====
        if (primaryTouch != null)
        {
            // PARMAK BU FRAME MÝ DOKUNDU?
            if (primaryTouch.press.wasPressedThisFrame)
            {
                currentPosition = primaryTouch.position.ReadValue();
                touchStarted = true;
            }

            // PARMAK BU FRAME MÝ BIRAKILDI? (ASIL DÜZELTME BURADA)
            if (primaryTouch.press.wasReleasedThisFrame)
            {
                touchEnded = true;
            }

            // PARMAK EKRANA BASILI MI TUTULUYOR?
            if (primaryTouch.press.isPressed)
            {
                currentPosition = primaryTouch.position.ReadValue();
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