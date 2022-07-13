using UnityEngine;

public class Line : MonoBehaviour
{
    [SerializeField] private LineRenderer line;

    void Start()
    {
        line.startWidth = 0.1f;
        line.endWidth = 0.1f;        
    }

    public void SetPositions(Vector2 pos1, Vector2 pos2)
    {
        line.material.color = Color.black;
        line.SetPosition(0, pos1);
        line.SetPosition(1, pos2);
    }
}
