using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

[RequireComponent(typeof(GraphicRaycaster))]
public class DebugUIRaycaster : MonoBehaviour
{
    private GraphicRaycaster _raycaster;
    private PointerEventData _pointerData;
    private EventSystem _eventSystem;

    void Awake()
    {
        _raycaster = GetComponent<GraphicRaycaster>();
        _eventSystem = EventSystem.current;
        _pointerData = new PointerEventData(_eventSystem);
    }

    void Update()
    {
        _pointerData.position = Input.mousePosition;
        var results = new List<RaycastResult>();
        _raycaster.Raycast(_pointerData, results);

        if (results.Count > 0 && Input.GetMouseButtonDown(0))
        {
            var topHit = results[0];
            Debug.Log($"[UI HIT] '{topHit.gameObject.name}' at screen {_pointerData.position}");
        }
        else if (Input.GetMouseButtonDown(0))
        {
            Debug.Log($"[UI MISS] at screen {_pointerData.position}");
        }
    }
}