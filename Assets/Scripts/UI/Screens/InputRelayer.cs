using System.Collections.Generic;
using NUnit.Framework.Constraints;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Maps normalized cursor positions to canvas interactions.
/// </summary>
public class InputRelayer : MonoBehaviour
{
    public RectTransform CanvasTransform;
    // the raycaster for the canvas thbat handles mouse inputs.
    public GraphicRaycaster Raycaster;


    List<GameObject> DragTargets = new();

    /// <summary>
    // receive cursor input
    // DisplayScreen relays input to this
    /// </summary>
    /// <param name="normalizedPos"></param>
    public void OnCursorInput(Vector2 normalizedPos)
    {

        // Get input position in canvas space
        Vector3 canvasInputPos = new Vector3(
            CanvasTransform.sizeDelta.x * normalizedPos.x,
         CanvasTransform.sizeDelta.y * normalizedPos.y,
         0);

        // contains info that the ui needs to interpret the event
        PointerEventData pointerEvent = new PointerEventData(EventSystem.current);
        pointerEvent.position = canvasInputPos;


        // use graphics raycaster to cast pointer event to the screen
        // write results to the results list. 
        List<RaycastResult> raycastResults = new();
        Raycaster.Raycast(pointerEvent, raycastResults);

#if ENABLE_LEGACY_INPUT_MANAGER
            bool bMouseDownThisFrame = Input.GetMouseButtonDown(0);
            bool bMouseUpThisFrame = Input.GetMouseButtonUp(0);
            bool bIsMouseDown = Input.GetMouseButton(0);
#else
        // new input system
        bool bMouseDownThisFrame = UnityEngine.InputSystem.Mouse.current.leftButton.wasPressedThisFrame;
        bool bMouseUpThisFrame = UnityEngine.InputSystem.Mouse.current.leftButton.wasReleasedThisFrame;
        bool bIsMouseDown = UnityEngine.InputSystem.Mouse.current.leftButton.isPressed;
#endif

        if (bIsMouseDown)
        {
            Debug.Log("Left mouse down");
        }

        // clear drag targets if mouse goes up
        if (bMouseUpThisFrame)
        {
            foreach (var target in DragTargets)
            {
                if (ExecuteEvents.Execute(target, pointerEvent, ExecuteEvents.endDragHandler))
                    break;
            }
            DragTargets.Clear();
        }

        // process hit results
        foreach (RaycastResult raycastResult in raycastResults)
        {
            //inject pointer event data onto the canvas

            // create new event data
            PointerEventData pointerEventForResult = new PointerEventData(EventSystem.current);
            pointerEventForResult.position = canvasInputPos;
            pointerEventForResult.pointerCurrentRaycast = raycastResult;
            pointerEventForResult.pointerPressRaycast = raycastResult;

            // is button down
            if (bIsMouseDown)
            {

                pointerEventForResult.button = PointerEventData.InputButton.Left;
            }


            // interacting with slider?>
            // retreive a slider if hit
            Slider hitSlider = raycastResult.gameObject.GetComponentInParent<Slider>();

            // new drag targets?
            if (bMouseDownThisFrame)
            {
                if (ExecuteEvents.Execute(raycastResult.gameObject, pointerEventForResult, ExecuteEvents.beginDragHandler))
                {
                    // whatever hit has handled that
                    DragTargets.Add(raycastResult.gameObject);
                }
                if (hitSlider != null)
                {
                    hitSlider.OnInitializePotentialDrag(pointerEventForResult);

                    // make sure its the slider gameobject
                    if (!DragTargets.Contains(raycastResult.gameObject))
                    {
                        DragTargets.Add(raycastResult.gameObject);
                    }

                }

            }
            // update current drag targets?
            else if (DragTargets.Contains(raycastResult.gameObject))
            {
                pointerEventForResult.dragging = true;
                ExecuteEvents.Execute(raycastResult.gameObject, pointerEventForResult, ExecuteEvents.dragHandler);

                if (hitSlider != null)
                {
                    hitSlider.OnDrag(pointerEventForResult);
                }
            }

            if (bMouseDownThisFrame)
            {
                if (ExecuteEvents.Execute(raycastResult.gameObject, pointerEventForResult, ExecuteEvents.pointerDownHandler))
                {
                    break;
                }
            }
            else if (bMouseUpThisFrame)
            {
                bool bDidRun = false;
                bDidRun |= ExecuteEvents.Execute(raycastResult.gameObject, pointerEventForResult, ExecuteEvents.pointerUpHandler);
                bDidRun |= ExecuteEvents.Execute(raycastResult.gameObject, pointerEventForResult, ExecuteEvents.pointerClickHandler);

                if (bDidRun)
                {
                    //consumed input
                    break;
                }
            }




        }

    }


}