using UnityEngine;

public class HexagonEvent : MonoBehaviour
{
    public string eventStage;
    public string eventNumber;
    public Vector3 center;
    public float radius = 2f;
    public bool playerInside = false;
    void Start()
    {
        center = transform.position;
    }
}