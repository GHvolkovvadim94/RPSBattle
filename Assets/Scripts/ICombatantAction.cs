public interface ICombatantAction
{
    string CurrentChoice { get; }
    event System.Action OnActionChosen;
    void ChooseAction(); // Логика выбора действия
}
