using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class UIRaycastDebugger : MonoBehaviour
{
    void Update()
    {
        if (EventSystem.current == null) return;

        // Create pointer data for the current mouse position
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        // Perform a raycast against all UI elements
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        if (results.Count > 0)
        {
            // Log the topmost object hit by the raycast
            Debug.Log("UI Raycast hit: " + results[0].gameObject.name);
        }
    }
}