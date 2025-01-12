using UnityEngine;

public class HexagonEvent : MonoBehaviour
{
    public string eventStage;
    public string eventNumber;
    public Vector3 center;
    public float radius = 2f;

    void Start()
    {
        center = transform.position;
    }
}