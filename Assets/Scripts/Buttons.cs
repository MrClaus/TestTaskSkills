using UnityEngine;

public class Buttons : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private UI ui;

    // Покупаем новый навык
    public void ClickGetSkill()
    {
        ui.ClickGetSkill();
    }

    // Забываем некогда купленный навык
    public void ClickReturnSkill()
    {
        ui.ClickReturnSkill();
    }

    // Забываем все приобретенные навыке (кроме базовых)
    public void ClickReturnAll()
    {
        ui.ClickReturnAllSkill();
    }

    // Зарабатываем очки
    public void ClickScore()
    {
        ui.ClickAddScore();
    }

    // Выбираем схему навыков
    public void ClickScheme(int id)
    {
        ui.ClickScreen();
        ui.SelectScheme(id);
    }

    // Пустой клик по экрану - закрытие бара
    public void ClickScreen()
    {
        ui.ClickScreen();
    }
}
