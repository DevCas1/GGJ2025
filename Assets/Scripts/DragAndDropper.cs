using UnityEngine;
using UnityEngine.InputSystem;

public class DragAndDropper : MonoBehaviour
{
    [Header("Required")]
    [SerializeField] private Camera _camera;
    [SerializeField] private LayerMask _draggableLayer;
    [SerializeField] private LayerMask _placeableLayer;
    [SerializeField] private float _rotationIncrement = 45;
    [Header("Cursor")]
    [SerializeField] private Texture2D _openHandCursor;
    [SerializeField] private Vector2 _openHandCursorHotspot = Vector2.zero;
    [SerializeField] private Texture2D _closedHandCursor;
    [SerializeField] private Vector2 _closedHandCursorHotspot = Vector2.zero;

    private Vector2 _pointerPos;
    private RaycastHit _raycastHit;
    private Transform _selectedDraggable;
    private Transform _pickedUpDraggable;
    private Obstacle _pickedUpObstacle;
    private Vector3 _draggableOrigin;
    private Quaternion _draggableOriginRotation;
    private bool _hasRotated;

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

        if (carryingDraggable == true && _currentRightClick == true && _hasRotated == false)
        {
            // Dragging something and gotta rotate the draggable!

            _pickedUpDraggable.Rotate(new (0, _rotationIncrement, 0));
            _hasRotated = true;
        }


        Ray ray = _camera.ScreenPointToRay(_pointerPos);

        if (Physics.Raycast(
            ray,
            out _raycastHit,
            _camera.farClipPlane,
            carryingDraggable ? _placeableLayer : _draggableLayer
        ) == false)
        {
            // Not hitting anything
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

            if (carryingDraggable == true)
            {
                // Still wanna know where the ray from camera intersects world pos y = 0
                Plane intersectTestPlane = new (Vector3.up, Vector3.zero);
                // Create temporary intersection plane
                Vector3 groundPoint;
                // This is the point where the ray points, and y == 0

                if (intersectTestPlane.Raycast(ray, out float tempDistance))
                {
                    // Get intersection from camera to test plane
                    groundPoint = ray.GetPoint(tempDistance);
                    // Then store point where the ray points, and y == 0
                    _pickedUpDraggable.position = groundPoint;
                    // And assign position to picked up draggable
                }
                // Still dragging something


                // Find some way to place indication of draggable location
                // by calculating intersection of camera ray and camera angle?

                if (_currentLeftClick == false)
                {
                    // Letting go of object before we could place it
                    // Debug.Log("Nowhere to place \"" + _pickedUpDraggable.name + "\", dropping it to it's origin " + _draggableOrigin);

                    // Return picked up draggable to whence it came
                    ReturnDraggableToOrigin();

                    // Play some effects?
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

                if (_pickedUpObstacle.CanBePlaced == false)
                {
                    // Debug.Log("Draggable \"" + _pickedUpDraggable.name + "\" intersects with another collider, and has been returned to it's origin");
                    ReturnDraggableToOrigin();
                }
                else
                {
                    // Debug.Log("Let go of \"" + _pickedUpDraggable.name + "\" and placed it at " + _raycastHit.point);
                    LetGoOfDraggable();
                }
            }
        }
        else
        {
            // Not dragging anything, and hovering over something selectable!
            Cursor.SetCursor(_openHandCursor, _openHandCursorHotspot, CursorMode.Auto);

            if (_raycastHit.transform != _selectedDraggable)
            {
                if (_selectedDraggable != null)
                {
                    // Selected something else!

                    // Deselect old draggable effects
                }

                // Select new one
                _selectedDraggable = _raycastHit.transform;
                // Debug.Log("Selected \"" + _selectedDraggable.name + "\"");
                // Play effects
            }

            if (_currentLeftClick == true)
            {
                // Gotta pick up the draggable!
                // Debug.Log("Picked up \"" + _selectedDraggable.name + "\"");

                // Play pick up effects

                PickUpDraggable();
            }
        }
    }

    private void PickUpDraggable()
    {
        _pickedUpDraggable = _selectedDraggable;
        _pickedUpObstacle = _pickedUpDraggable.GetComponent<Obstacle>();
        _draggableOrigin = _pickedUpDraggable.position;
        _draggableOriginRotation = _pickedUpDraggable.rotation;

        // Make collider trigger so player will pass through it while dragging
        _pickedUpDraggable.GetComponentInChildren<Collider>().isTrigger = true;

        Cursor.SetCursor(_closedHandCursor, _closedHandCursorHotspot, CursorMode.Auto);

        // Play deselect effects?

        _selectedDraggable = null;
    }

    private void ReturnDraggableToOrigin()
    {
        _pickedUpDraggable.position = _draggableOrigin;
        _pickedUpDraggable.rotation = _draggableOriginRotation;

        LetGoOfDraggable();
    }

    private void LetGoOfDraggable()
    {
        _pickedUpDraggable.GetComponentInChildren<Collider>().isTrigger = false;

        // Place draggable effect?
        if (_pickedUpDraggable.TryGetComponent(out Obstacle obstacle))
        {
            obstacle.OnObstaclePlaced();
        }
        else
        {
            Debug.LogWarning($"Somehow picked up {_pickedUpDraggable.name} without it having the Obstacle component!");
        }

        _pickedUpObstacle = null;
        _pickedUpDraggable = null;
        _draggableOrigin = Vector3.zero;

        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
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
        _hasRotated = false;
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
