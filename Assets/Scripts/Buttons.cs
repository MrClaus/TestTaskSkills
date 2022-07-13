using UnityEngine;

public class Buttons : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private UI ui;

    // �������� ����� �����
    public void ClickGetSkill()
    {
        ui.ClickGetSkill();
    }

    // �������� ������� ��������� �����
    public void ClickReturnSkill()
    {
        ui.ClickReturnSkill();
    }

    // �������� ��� ������������� ������ (����� �������)
    public void ClickReturnAll()
    {
        ui.ClickReturnAllSkill();
    }

    // ������������ ����
    public void ClickScore()
    {
        ui.ClickAddScore();
    }

    // �������� ����� �������
    public void ClickScheme(int id)
    {
        ui.ClickScreen();
        ui.SelectScheme(id);
    }

    // ������ ���� �� ������ - �������� ����
    public void ClickScreen()
    {
        ui.ClickScreen();
    }
}
