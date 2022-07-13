using UnityEngine;

public class Block : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private UI ui;

    [Header("ID Block")]
    [SerializeField] private int id;
    [SerializeField] private int[] connects;

    public int Id { get => id; }
    public int[] InId { get => connects; }    

    public void ClickBlock(int id)
    {
        ui.ClickBlock(id);
    }
}
