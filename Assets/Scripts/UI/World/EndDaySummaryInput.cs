using UnityEngine;
using UnityEngine.InputSystem;

public class EndDaySummaryInput : MonoBehaviour
{
    [SerializeField] private InputActionReference action;
    [SerializeField] private EndDaySummaryUI summaryUI;

    private void OnEnable()
    {
        action.action.performed += OnAction;
        action.action.Enable();
    }

    private void OnDisable()
    {
        action.action.performed -= OnAction;
        action.action.Disable();
    }

    private void OnAction(InputAction.CallbackContext ctx)
    {
        if (!DayManager.I.IsAwaitingEndDayConfirmation)
            return;

        if (!summaryUI.IsOpen)
        {
            summaryUI.ShowSummary();
        }
        else
        {
            summaryUI.ConfirmEndDay();
        }
    }
}