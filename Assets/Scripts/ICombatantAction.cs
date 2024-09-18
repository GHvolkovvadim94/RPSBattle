public interface ICombatantAction
{
    string CurrentChoice { get; }
    event System.Action OnActionChosen;
    void ChooseAction(); // ������ ������ ��������
    public string GetActionName()
    {
        return CurrentChoice;
    }
}
