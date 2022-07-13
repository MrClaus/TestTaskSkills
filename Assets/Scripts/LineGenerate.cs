using System.Collections.Generic;
using UnityEngine;

public class LineGenerate : MonoBehaviour
{
    [Header("Prefab Line")]
    public GameObject line;

    private List<GameObject> childs = new List<GameObject>();
    public List<GameObject> BlockList { get => childs; }

    private void Awake()
    {
        foreach (Transform child in transform)
            if (child.CompareTag("Block"))
                childs.Add(child.gameObject);

        childs.Sort(delegate (GameObject o1, GameObject o2)
        { return o1.GetComponent<Block>().Id.CompareTo(o2.GetComponent<Block>().Id); });
        Generate(childs);
    }

    // Генерация линий для текущей активной блок-схемы
    void Generate(List<GameObject> childs)
    {
        foreach (GameObject child in childs)
        {
            int id = child.GetComponent<Block>().Id;
            int[] arr = child.GetComponent<Block>().InId;
            Vector2 fromPos = child.transform.position;

            if (arr.Length > 0)
            {
                for (int i = 0; i < arr.Length; i++)
                {
                    int inId = arr[i];
                    foreach (GameObject toChild in childs)
                    {
                        if (toChild.GetComponent<Block>().Id == inId)
                        {
                            Vector2 toPos = toChild.transform.position;
                            GameObject mLine = Instantiate(line, transform);
                            mLine.GetComponent<Line>().SetPositions(fromPos, toPos);
                            break;
                        }
                    }
                }                
            }            
        }
    }
}
