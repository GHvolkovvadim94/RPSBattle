public class RoundResult
{
    public ICombatantAction PlayerAction { get; private set; }
    public ICombatantAction EnemyAction { get; private set; }

    public RoundResult(ICombatantAction playerAction, ICombatantAction enemyAction)
    {
        PlayerAction = playerAction;
        EnemyAction = enemyAction;
    }
}