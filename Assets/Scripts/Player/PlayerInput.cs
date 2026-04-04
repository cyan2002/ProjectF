using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    public static PlayerInput Instance { get; private set; }

    public string ActiveInventory { get; set; } = "NA";
    public bool canOpen { get; set; } = true;

    public static event Action<Vector2> onMove;
    public static event Action HandleB;
    public static event Action HandleI;
    public static event Action HandleR;
    public static event Action HandleLeftClick;
    public static event Action HandleJ;
    public static event Action HandleK;
    public static event Action HandleE;
    public static event Action HandleEscape;
    public static event Action HandleShiftClick;

    private PlayerInputActions actions;
    private Vector2 lastMove;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        actions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        actions.Player.Enable();

        actions.Player.Move.performed += OnMovePerformed;
        actions.Player.Move.canceled += OnMoveCanceled;

        actions.Player.B.performed += _ => HandleB?.Invoke();
        actions.Player.I.performed += _ => HandleI?.Invoke();
        actions.Player.R.performed += _ => HandleR?.Invoke();
        actions.Player.Click.performed += OnClick;
        actions.Player.ShiftClick.performed += _ => HandleShiftClick?.Invoke();
        actions.Player.J.performed += _ => HandleJ?.Invoke();
        actions.Player.K.performed += _ => HandleK?.Invoke();
        actions.Player.E.performed += _ => HandleE?.Invoke();
        actions.Player.Escape.performed += _ => HandleEscape?.Invoke();
    }

    private void OnDisable()
    {
        actions.Player.Move.performed -= OnMovePerformed;
        actions.Player.Move.canceled -= OnMoveCanceled;

        actions.Player.Click.performed -= OnClick;

        actions.Player.Disable();
    }

    private void OnMovePerformed(InputAction.CallbackContext ctx)
    {
        Vector2 move = ctx.ReadValue<Vector2>();
        if (move == lastMove) return;
        lastMove = move;
        onMove?.Invoke(move);
    }

    private void OnMoveCanceled(InputAction.CallbackContext ctx)
    {
        lastMove = Vector2.zero;
        onMove?.Invoke(Vector2.zero);
    }

    // Shift+Click is a separate action in the asset, so plain Click
    // should only fire when Shift is NOT held.
    private void OnClick(InputAction.CallbackContext ctx)
    {
        if (Keyboard.current.leftShiftKey.isPressed) return;
        HandleLeftClick?.Invoke();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            lastMove = Vector2.zero;
            onMove?.Invoke(Vector2.zero);
        }
    }
}