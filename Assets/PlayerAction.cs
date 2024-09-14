public class PlayerAction : ICombatantAction
{
    private string choice;
    public string CurrentChoice => choice;

    public event System.Action OnActionChosen;

    public void ChooseAction()
    {
        // Подписываемся на событие выбора игрока через BattleSystem
        BattleSystem.OnPlayerMadeChoice += HandlePlayerChoice;
    }

    private void HandlePlayerChoice(string playerChoice)
    {
        choice = playerChoice;
        OnActionChosen?.Invoke(); // Вызываем событие, что действие выбрано
        BattleSystem.OnPlayerMadeChoice -= HandlePlayerChoice; // Отписываемся
    }
}
