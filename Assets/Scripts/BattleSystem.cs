using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class BattleSystem : MonoBehaviour
{
    private ScreenManager screenManager;
    public Text roundStatusText;
    public Text timerText;
    public Button rockButton;
    public Button paperButton;
    public Button scissorsButton;

    public Slider PlayerMainHealthSlider;
    public Slider PlayerDamageHealthSlider;

    public Slider EnemyMainHealthSlider;
    public Slider EnemyDamageHealthSlider;

    private Player player;
    private Enemy enemy;

    private ICombatantAction playerAction;
    private ICombatantAction enemyAction;

    private int damageAmount = 20;
    private int roundNumber = 0;

    private float preparationTime = 2.0f;
    private float decisionTime = 5.0f;
    private float delay = 1.0f;

    private float timer = 0f;

    public static event System.Action<string> OnPlayerMadeChoice;

    private bool isPlayerReady = false; // ���� ��� ��������, ��� ����� ������ �����
    private bool isEnemyReady = false; // ���� ��� ���������

    private bool isGameOver = false; // ���� ��� ������������ ��������� ����
    private void Awake()
    {
        ActionButtonsInputAvailable(false); // ��������� ������ �� ����� ����������
        screenManager = FindObjectOfType<ScreenManager>();
    }

    void Start()
    {

        // ������������� ������ � ���������
        player = new Player(100);
        enemy = new Enemy(100);

        // ������������� ������������ �������� �������� �� UI
        PlayerMainHealthSlider.maxValue = player.MaxHealth;
        EnemyMainHealthSlider.maxValue = enemy.MaxHealth;
        PlayerDamageHealthSlider.maxValue = player.MaxHealth;
        EnemyDamageHealthSlider.maxValue = enemy.MaxHealth;

        // ������������� ������� �������� ��������
        UpdateHealthUI();

        // ������� �������� ��� ������ � ���������
        playerAction = new PlayerAction();
        enemyAction = new EnemyAction();

        roundStatusText.text = "������ �����...";
        timerText.text = "";

        // ������������� �� ������� ������ ��������
        playerAction.OnActionChosen += OnPlayerActionChosen;
        enemyAction.OnActionChosen += OnEnemyActionChosen;

        // ����������� ������
        rockButton.onClick.AddListener(() => PlayerMakesChoice("Rock"));
        paperButton.onClick.AddListener(() => PlayerMakesChoice("Paper"));
        scissorsButton.onClick.AddListener(() => PlayerMakesChoice("Scissors"));

        // �������� ����
        Invoke("StartPreparation", delay);
    }

    void StartPreparation()
    {
        if (isGameOver) return; // ���� ���� ��������, ������� �� �����

        timer = preparationTime;
        roundNumber++;
        ActionButtonsInputAvailable(false); // ��������� ������ �� ����� ����������
        StartCoroutine(PreparationPhase());
    }

    IEnumerator PreparationPhase()
    {
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            roundStatusText.text = $"����� {roundNumber}";
            timerText.text = $"{Mathf.Ceil(timer)}";
            yield return null;
        }

        StartDecisionPhase();
    }

    void StartDecisionPhase()
    {
        if (isGameOver) return; // ���� ���� ��������, �������

        roundStatusText.text = $"�������� �����!";
        timer = decisionTime;
        isPlayerReady = false; // �������� ����� ������
        isEnemyReady = false;  // �������� ����� ���������

        ActionButtonsInputAvailable(true); // �������� ������ ��� ������

        // ������� ����� ������ � ���������
        playerAction.ChooseAction();
        enemyAction.ChooseAction();

        StartCoroutine(DecisionPhase()); // ��������� ���� �������� �������
    }

    IEnumerator DecisionPhase()
    {
        while (timer > 0)
        {
            if (isPlayerReady && isEnemyReady)
                break; // ���� ��� ������� �������� - �������

            timer -= Time.deltaTime;
            timerText.text = $"{Mathf.Ceil(timer)}";
            yield return null;
        }

        // ���� ����� �����, ����� ������ �� ������
        if (!isPlayerReady)
        {
            OnPlayerMadeChoice?.Invoke("None"); // ���� ����� �� ������ �����
        }

        EvaluateRoundResult();
    }

    private void PlayerMakesChoice(string choice)
    {
        OnPlayerMadeChoice?.Invoke(choice); // �������� �������, ��� ����� ������ �����
        ActionButtonsInputAvailable(false);
    }

    private void OnPlayerActionChosen()
    {
        isPlayerReady = true; // ����� ������ �����
    }

    private void OnEnemyActionChosen()
    {
        isEnemyReady = true; // ��������� ������ �����
    }

    void EvaluateRoundResult()
    {
        if (isPlayerReady && isEnemyReady) // ���������, ��� ��� ������� ��������
        {
            Debug.Log($"���������: {playerAction.CurrentChoice} vs {enemyAction.CurrentChoice}");

            if (playerAction.CurrentChoice == enemyAction.CurrentChoice)
            {
                roundStatusText.text = "�����!";
            }
            else if ((playerAction.CurrentChoice == "Rock" && enemyAction.CurrentChoice == "Scissors") ||
                     (playerAction.CurrentChoice == "Paper" && enemyAction.CurrentChoice == "Rock") ||
                     (playerAction.CurrentChoice == "Scissors" && enemyAction.CurrentChoice == "Paper"))
            {
                roundStatusText.text = "�� ��������!";
                TakeDamage(EnemyDamageHealthSlider, EnemyMainHealthSlider, damageAmount, enemy);
                StartCoroutine(UpdateHealthSlider(EnemyMainHealthSlider, enemy.CurrentHealth));

                if (enemy.CurrentHealth <= 0)
                {
                    GameOver(true);
                    return;
                }
            }
            else
            {
                roundStatusText.text = "�� ���������!";
                TakeDamage(PlayerDamageHealthSlider, PlayerMainHealthSlider, damageAmount, player);
                StartCoroutine(UpdateHealthSlider(PlayerMainHealthSlider, player.CurrentHealth));

                if (player.CurrentHealth <= 0)
                {
                    GameOver(false);
                    return;
                }
            }

            // �������� ����� ��������� �������
            Invoke("StartPreparation", delay);
        }
    }

    IEnumerator UpdateHealthSlider(Slider healthSlider, int newHealth)
    {
        float elapsedTime = 0f;
        float duration = 0.5f;
        float startValue = healthSlider.value;
        float endValue = newHealth;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float currentValue = Mathf.Lerp(startValue, endValue, elapsedTime / duration);
            healthSlider.value = currentValue;
            GetHealthTextComponent(healthSlider).text = $"{Mathf.RoundToInt(currentValue)}/{healthSlider.maxValue}";
            yield return null;
        }

        healthSlider.value = endValue;
        GetHealthTextComponent(healthSlider).text = $"{endValue}/{healthSlider.maxValue}";
    }

    void UpdateHealthUI()
    {
        // ��������� �������� �� UI ��� ��������
        PlayerMainHealthSlider.value = player.CurrentHealth;
        PlayerDamageHealthSlider.value = player.CurrentHealth;

        EnemyMainHealthSlider.value = enemy.CurrentHealth;
        EnemyDamageHealthSlider.value = enemy.CurrentHealth;

        // ��������� ����� ��������
        GetHealthTextComponent(PlayerMainHealthSlider).text = $"{player.CurrentHealth}/{player.MaxHealth}";
        GetHealthTextComponent(EnemyMainHealthSlider).text = $"{enemy.CurrentHealth}/{enemy.MaxHealth}";
    }

    private Text GetHealthTextComponent(Slider slider)
    {
        Text[] texts = slider.GetComponentsInChildren<Text>();
        foreach (Text text in texts)
        {
            if (text.gameObject.name == "valueText") // ���� ��������� ������ � ������ "valueText"
            {
                return text;
            }
        }
        Debug.LogError("�� ������ ��������� ������ ��� ��������!");
        return null;
    }

    void GameOver(bool playerWon)
    {
        if (playerWon)
        {
            roundStatusText.text = "���� �������!";
        }
        else
        {
            roundStatusText.text = "���� �������!";
        }

        // ��������� ������
        ActionButtonsInputAvailable(false);

        // �������� ����� ���������� ����
        screenManager.ShowMatchEndPopup(playerWon);  // ���������� �����

        // ������������� ������� � ��������
    }


    void RestartGame()
    {
        // ���������� ��������� ����
        isGameOver = false;

        // ���������� �������� ������ � ���������
        player.ResetHealth();
        enemy.ResetHealth();

        // ��������� UI ��������
        UpdateHealthUI();

        // ���������� ����� ������� ������
        roundNumber = 0;
        roundStatusText.text = "������ ������ �����!";
        timerText.text = "";

        // ��������� ������ ����� ����� ���������� ��������
        Invoke("StartPreparation", delay);
    }
    public IEnumerator ShowDamageEffect(Slider damageSlider, Slider mainSlider, int previousHealth)
    {
        yield return new WaitForSeconds(0.5f);  // ���� 1 ������� ����� ���������

        float elapsedTime = 0f;
        float duration = 2f;  // ������������ ��������
        float targetValue = previousHealth - damageAmount;  // ������� �������� (�������� ����� �����)

        // ������� ���������� �������� ��������
        while (damageSlider.value > targetValue)
        {
            elapsedTime += Time.deltaTime;
            // ��������� �������� damageSlider � �������������� Mathf.MoveTowards
            damageSlider.value = Mathf.MoveTowards(damageSlider.value, targetValue, (damageSlider.maxValue / duration) * Time.deltaTime);
            yield return null;
        }

        // ���������, ��� �������� �������� ����� ��������� � �������� ���������
        damageSlider.value = targetValue;
    }

    public void TakeDamage(Slider damageSlider, Slider mainSlider, int amount, ICombatant combatant)
    {
        combatant.TakeDamage(amount); // ��������� ��������

        // ��������� �������� ������� ��������
        StartCoroutine(UpdateHealthSlider(mainSlider, combatant.CurrentHealth));

        // �������� ��������� �������� �������� �� ��������� �����
        int previousHealth = combatant.CurrentHealth + amount;
        StartCoroutine(ShowDamageEffect(damageSlider, mainSlider, previousHealth));
    }

    void ActionButtonsInputAvailable(bool available)
    {
        rockButton.interactable = available;
        paperButton.interactable = available;
        scissorsButton.interactable = available;
    }
}
