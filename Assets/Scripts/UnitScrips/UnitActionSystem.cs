using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitActionSystem : MonoBehaviour
{
    public static UnitActionSystem Instance {get; private set;}
    public event EventHandler OnSelectedUnitChanged;
    public event EventHandler OnSelectedActionChanged;
    public event EventHandler<bool> OnBusyChanged;
    public event EventHandler OnActionStarted;

    [SerializeField] private Unit selectedUnit;
    [SerializeField] private LayerMask unitLayerMask;

    private BaseAction selectedAction;

    private bool isBusy;
    private bool offline;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one UnitActionSystem! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    private void Start()
    {
        UnitManager.Instance.OnAllEnemiesDefeated += UnitManager_OnAllEnemiesDefeated;
        UnitManager.Instance.OnKingTaken += UnitManager_OnKingTaken;
        UnitSpawner.Instance.OnSpawningFinished += UnitSpawner_OnSpawningFinished;
        SetSelectedUnit(selectedUnit);
    }

    private void Update()
    {
        if (offline) return;
        
        if (isBusy) return;
        
        if (!TurnSystem.Instance.IsPlayerTurn()) return; 
        
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        if (TryHandleUnitSelection())
        {
            return;
        } 

        HandleSelectedAction();
    }

    private void HandleSelectedAction()
    {
        if (InputManager.Instance.IsMouseButtonDownThisFrame())
        {
            GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition2D());
            if (!selectedAction.IsValidActionGridPosition(mouseGridPosition))
            {
                return;  
            }
            SetBusy();
            selectedAction.TakeAction(mouseGridPosition, ClearBusy);
            OnActionStarted?.Invoke(this, EventArgs.Empty);
            /*switch(selectedAction)
            {
                case MoveAction moveAction:
                    if (moveAction.IsValidActionGridPosition(mouseGridPosition))
                    {
                        SetBusy();
                        moveAction.Move(mouseGridPosition, ClearBusy);
                    }
                    break;
                case SpinAction spinAction:
                    SetBusy();
                    spinAction.Spin(ClearBusy);
                    break;
            }*/
        }
    }
    private void SetBusy()
    {
        isBusy = true;
        OnBusyChanged?.Invoke(this, true);
    }

    private void ClearBusy()
    {
        isBusy = false;
        OnBusyChanged?.Invoke(this, false);
    }

    private void SetOffline()
    {
        offline = true;
    }

    private void ClearOffline()
    {
        offline = false;
    }

    public bool GetOffline()
    {
        return offline;
    }

    private bool TryHandleUnitSelection()
    {
        if (InputManager.Instance.IsMouseButtonDownThisFrame())
        {
            Collider2D point = Physics2D.OverlapPoint(MouseWorld.GetPosition2D(), unitLayerMask);
            //Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetMouseScreenPosition());
            if (point != null)
            {
                if (point.transform.TryGetComponent<Unit>(out Unit unit))
                {
                    if (unit == selectedUnit)
                    {
                        // Unit is already selected
                        return false;
                    }

                    if (unit.IsEnemy())
                    {
                        return false;
                    }

                    BaseAction selectedAction = selectedUnit.GetUnitAction();
                    List<GridPosition> allyTargetList = selectedAction.GetValidAllyTargetGridPositionList();

                    // Unit is a target of an effect
                    if (allyTargetList.Contains(unit.GetGridPosition()))
                    {
                        return false;
                    }
                    
                    SetSelectedUnit(unit);
                    return true;
                }
            }
        }
        return false;
    }

    private void SetSelectedUnit(Unit unit)
    {
        selectedUnit = unit;
        SetSelectedAction(unit.GetUnitAction());

        OnSelectedUnitChanged?.Invoke(this, EventArgs.Empty);
    }

    public Unit GetSelectedUnit()
    {
        return selectedUnit;
    }

    public void SetSelectedAction(BaseAction baseAction)
    {
        selectedAction = baseAction;

        OnSelectedActionChanged?.Invoke(this, EventArgs.Empty);
    }

    public BaseAction GetSelectedAction()
    {
        return selectedAction;
    }

    private void UnitManager_OnAllEnemiesDefeated(object sender, EventArgs e)
    {
        SetOffline();
    }

    private void UnitManager_OnKingTaken(object sender, EventArgs e)
    {
        SetOffline();
    }

    private void UnitSpawner_OnSpawningFinished(object sender, EventArgs e)
    {
        List<Unit> friendlyUnits = UnitManager.Instance.GetFriendlyUnitList();

        foreach (Unit unit in friendlyUnits)
        {
            if (unit.TryGetComponent<KingAction>(out KingAction kingAction))
            {
                SetSelectedUnit(unit);
                break;
            }
        }
        ClearOffline();
    }
}
