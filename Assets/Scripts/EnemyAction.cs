public class EnemyAction : ICombatantAction
{
    private string choice;
    public string CurrentChoice => choice;

    public event System.Action OnActionChosen;

    public void ChooseAction()
    {
        // ѕростой рандомный выбор дл€ соперника
        string[] choices = { "Rock", "Paper", "Scissors" };
        choice = choices[UnityEngine.Random.Range(0, choices.Length)];
        OnActionChosen?.Invoke(); // —игнализируем, что действие выбрано
    }
}
