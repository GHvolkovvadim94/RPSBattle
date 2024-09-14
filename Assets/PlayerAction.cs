public class PlayerAction : ICombatantAction
{
    private string choice;
    public string CurrentChoice => choice;

    public event System.Action OnActionChosen;

    public void ChooseAction()
    {
        // ������������� �� ������� ������ ������ ����� BattleSystem
        BattleSystem.OnPlayerMadeChoice += HandlePlayerChoice;
    }

    private void HandlePlayerChoice(string playerChoice)
    {
        choice = playerChoice;
        OnActionChosen?.Invoke(); // �������� �������, ��� �������� �������
        BattleSystem.OnPlayerMadeChoice -= HandlePlayerChoice; // ������������
    }
}
