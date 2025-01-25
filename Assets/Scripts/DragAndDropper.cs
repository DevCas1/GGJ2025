using UnityEngine;
using UnityEngine.InputSystem;

public class DragAndDropper : MonoBehaviour
{
    private enum DragStatus { Free, PickUp, Drag, Release }

    [SerializeField] private Camera _camera;
    [SerializeField] private LayerMask _draggableLayer;
    [SerializeField] private LayerMask _placeableLayer;

    private Vector2 _pointerPos;
    private Vector3 _worldPos;
    private DragStatus _currentDragStatus;
    private RaycastHit _raycastHit;
    private Transform _lastHitTransform;
    private Transform _selectedDraggable;
    private Transform _pickedUpDraggable;
    private Vector3 _draggableOrigin;

    private bool _currentLeftClick;

    private bool _currentRightClick;

    private bool _currentMiddleClick;

    public void Update()
    {
        CheckForObjects();
    }

    private void CheckForObjects()
    {
        bool carryingDraggable = _pickedUpDraggable != null;

        Ray ray = _camera.ScreenPointToRay(_pointerPos);

        if (Physics.Raycast(
            ray,
            out _raycastHit,
            _camera.farClipPlane,
            carryingDraggable ? _placeableLayer : _draggableLayer
        ) == false)
        {
            // Not hitting anything

            if (carryingDraggable == true)
            {
                // Still dragging something

                // Find some way to place indication of draggable location
                // by calculating intersection of camera ray and camera angle?

                if (_currentLeftClick == false)
                {
                    // Letting go of object before we could place it
                    Debug.Log("Nowhere to place \"" + _pickedUpDraggable.name + "\", dropping it to it's origin " + _draggableOrigin);

                    // Return picked up draggable to whence it came
                    _pickedUpDraggable.position = _draggableOrigin;
                    
                    // Play some effects?

                    _pickedUpDraggable = null;
                }
            }

            return;
        }

        // When we do actually hit something :o

        if (carryingDraggable == true)
        {
            // Currently draggint something, and hitting placeable-object, meaning we can put it down

            // Check if draggable can be dropped here in terms of overlapping colliders?

            // Move draggable to mouse position in world space
            _pickedUpDraggable.position = _raycastHit.point;

            if (_currentLeftClick == false)
            {
                // Gotta let go of draggable
                Debug.Log("Let go of \"" + _pickedUpDraggable.name + "\" and placed it at " + _raycastHit.point);

                // Place draggable effect?

                _pickedUpDraggable = null;
                _draggableOrigin = Vector3.zero;
            }

        }
        else
        {
            // Not dragging anything, and hovering over something selectable!

            if (_raycastHit.transform != _selectedDraggable)
            {
                if (_selectedDraggable != null)
                {
                    // Selected something else!

                    // Deselect old draggable effects
                }

                // Select new one
                _selectedDraggable = _raycastHit.transform;
                Debug.Log("Selected \"" + _selectedDraggable.name + "\"");
                // Play effects
            }

            if (_currentLeftClick == true)
            {
                // Gotta pick up the draggable!
                Debug.Log("Picked up \"" + _selectedDraggable.name + "\"");

                // Play pick up effects

                _pickedUpDraggable = _selectedDraggable;
                _draggableOrigin = _pickedUpDraggable.position;
                _pickedUpDraggable.GetComponent<Collider>().isTrigger = true;
                
                _selectedDraggable = null;
            }
        }
    }

    private void OnPoint(InputValue pointValue)
    {
        Vector2 input = pointValue.Get<Vector2>();

        if (input != _pointerPos)
        {
            _pointerPos = input;
        }
        else
            Debug.Log("pointValue == _pointerPos ?!");
    }

    private void OnClick(InputValue clickValue)
    {
        bool input = clickValue.isPressed;

        if (_currentLeftClick == input)
            Debug.LogWarning("_currentLeftClick set to the same value it already was!");

        _currentLeftClick = input;

    }

    private void OnRightClick(InputValue rightClicklValue)
    {
        bool input = rightClicklValue.isPressed;

        if (_currentRightClick == input)
            Debug.LogWarning("_currentRightClick set to the same value it already was!");

        _currentRightClick = input;
    }

    private void OnMiddleClick(InputValue middleClickValue)
    {
        bool input = middleClickValue.isPressed;

        if (_currentMiddleClick == input)
            Debug.LogWarning("_currentMiddleClick set to the same value it already was!");

        _currentMiddleClick = input;
    }

    private void OnScrollWheel(InputValue scrollValue)
    {
        Vector2 input = scrollValue.Get<Vector2>();
        // Debug.Log("OnScrollWheel called! Value: " + input);
    }
}
