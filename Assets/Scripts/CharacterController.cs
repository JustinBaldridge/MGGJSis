using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [SerializeField] float moveSpeed;

    private HealthSystem healthSystem;
    GridPosition gridPosition;
    // Start is called before the first frame update
    void Start()
    {
        healthSystem = GetComponent<HealthSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    
        GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        if (newGridPosition != gridPosition)
        {
            GridPosition oldGridPosition = gridPosition;
            gridPosition = newGridPosition;
            //LevelGrid.Instance.UnitMovedGridPosition(this, oldGridPosition, newGridPosition);
        }
    }

    void Move()
    {
        Vector2 input = InputManager.Instance.GetMoveVector();
        Vector3 movementVector = new Vector3(input.x, input.y, 0);
        transform.position += movementVector * moveSpeed * Time.deltaTime;
    }
}
