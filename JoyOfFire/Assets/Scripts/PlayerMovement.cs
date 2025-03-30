using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{
    private NavMeshAgent agent;
    private Vector3 targetPosition;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    void Update()
    {
        bool status = SetTargetPosition();
        if (status)
        {
            Move(targetPosition);
        }
            
        
    }

    private bool SetTargetPosition()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return false;
            }
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPosition = new Vector3(mousePosition.x, mousePosition.y, 0);
            return true;
        }
        return false;
        
        
    }

    void Move(Vector3 targetPosition)
    {
        agent.SetDestination(targetPosition);
    }
    
}