using UnityEngine;

public class HexagonEvent : MonoBehaviour
{
    public string eventStage;
    public string eventNumber;
    public Vector3 center;
    public float radius = 2f;
    public bool playerInside = false;
    public int randomPoolIndex = 0;
    public bool isTransition = false;
    public int transitionMapIndex;
    public int respawnPointIndex;
    void Start()
    {
        center = transform.position;
    }
}