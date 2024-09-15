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

    private bool isPlayerReady = false; // Флаг для проверки, что игрок сделал выбор
    private bool isEnemyReady = false; // Флаг для соперника

    private bool isGameOver = false; // Флаг для отслеживания состояния игры
    private void Awake()
    {
        ActionButtonsInputAvailable(false); // Отключаем кнопки на время подготовки
        screenManager = FindObjectOfType<ScreenManager>();
    }

    void Start()
    {

        // Инициализация игрока и соперника
        player = new Player(100);
        enemy = new Enemy(100);

        // Устанавливаем максимальные значения здоровья на UI
        PlayerMainHealthSlider.maxValue = player.MaxHealth;
        EnemyMainHealthSlider.maxValue = enemy.MaxHealth;
        PlayerDamageHealthSlider.maxValue = player.MaxHealth;
        EnemyDamageHealthSlider.maxValue = enemy.MaxHealth;

        // Устанавливаем текущие значения здоровья
        UpdateHealthUI();

        // Создаем действия для игрока и соперника
        playerAction = new PlayerAction();
        enemyAction = new EnemyAction();

        roundStatusText.text = "Начало матча...";
        timerText.text = "";

        // Подписываемся на события выбора действия
        playerAction.OnActionChosen += OnPlayerActionChosen;
        enemyAction.OnActionChosen += OnEnemyActionChosen;

        // Привязываем кнопки
        rockButton.onClick.AddListener(() => PlayerMakesChoice("Rock"));
        paperButton.onClick.AddListener(() => PlayerMakesChoice("Paper"));
        scissorsButton.onClick.AddListener(() => PlayerMakesChoice("Scissors"));

        // Начинаем игру
        Invoke("StartPreparation", delay);
    }

    void StartPreparation()
    {
        if (isGameOver) return; // Если игра окончена, выходим из цикла

        timer = preparationTime;
        roundNumber++;
        ActionButtonsInputAvailable(false); // Отключаем кнопки на время подготовки
        StartCoroutine(PreparationPhase());
    }

    IEnumerator PreparationPhase()
    {
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            roundStatusText.text = $"Раунд {roundNumber}";
            timerText.text = $"{Mathf.Ceil(timer)}";
            yield return null;
        }

        StartDecisionPhase();
    }

    void StartDecisionPhase()
    {
        if (isGameOver) return; // Если игра окончена, выходим

        roundStatusText.text = $"Сделайте выбор!";
        timer = decisionTime;
        isPlayerReady = false; // Обнуляем выбор игрока
        isEnemyReady = false;  // Обнуляем выбор соперника

        ActionButtonsInputAvailable(true); // Включаем кнопки для выбора

        // Ожидаем выбор игрока и соперника
        playerAction.ChooseAction();
        enemyAction.ChooseAction();

        StartCoroutine(DecisionPhase()); // Запускаем фазу принятия решения
    }

    IEnumerator DecisionPhase()
    {
        while (timer > 0)
        {
            if (isPlayerReady && isEnemyReady)
                break; // Если оба выбрали действие - выходим

            timer -= Time.deltaTime;
            timerText.text = $"{Mathf.Ceil(timer)}";
            yield return null;
        }

        // Если время вышло, игрок ничего не выбрал
        if (!isPlayerReady)
        {
            OnPlayerMadeChoice?.Invoke("None"); // Если игрок не сделал выбор
        }

        EvaluateRoundResult();
    }

    private void PlayerMakesChoice(string choice)
    {
        OnPlayerMadeChoice?.Invoke(choice); // Сообщаем системе, что игрок сделал выбор
        ActionButtonsInputAvailable(false);
    }

    private void OnPlayerActionChosen()
    {
        isPlayerReady = true; // Игрок сделал выбор
    }

    private void OnEnemyActionChosen()
    {
        isEnemyReady = true; // Противник сделал выбор
    }

    void EvaluateRoundResult()
    {
        if (isPlayerReady && isEnemyReady) // Проверяем, что оба выбрали действия
        {
            Debug.Log($"Результат: {playerAction.CurrentChoice} vs {enemyAction.CurrentChoice}");

            if (playerAction.CurrentChoice == enemyAction.CurrentChoice)
            {
                roundStatusText.text = "Ничья!";
            }
            else if ((playerAction.CurrentChoice == "Rock" && enemyAction.CurrentChoice == "Scissors") ||
                     (playerAction.CurrentChoice == "Paper" && enemyAction.CurrentChoice == "Rock") ||
                     (playerAction.CurrentChoice == "Scissors" && enemyAction.CurrentChoice == "Paper"))
            {
                roundStatusText.text = "Вы выиграли!";
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
                roundStatusText.text = "Вы проиграли!";
                TakeDamage(PlayerDamageHealthSlider, PlayerMainHealthSlider, damageAmount, player);
                StartCoroutine(UpdateHealthSlider(PlayerMainHealthSlider, player.CurrentHealth));

                if (player.CurrentHealth <= 0)
                {
                    GameOver(false);
                    return;
                }
            }

            // Задержка перед следующим раундом
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
        // Обновляем значения на UI для здоровья
        PlayerMainHealthSlider.value = player.CurrentHealth;
        PlayerDamageHealthSlider.value = player.CurrentHealth;

        EnemyMainHealthSlider.value = enemy.CurrentHealth;
        EnemyDamageHealthSlider.value = enemy.CurrentHealth;

        // Обновляем текст здоровья
        GetHealthTextComponent(PlayerMainHealthSlider).text = $"{player.CurrentHealth}/{player.MaxHealth}";
        GetHealthTextComponent(EnemyMainHealthSlider).text = $"{enemy.CurrentHealth}/{enemy.MaxHealth}";
    }

    private Text GetHealthTextComponent(Slider slider)
    {
        Text[] texts = slider.GetComponentsInChildren<Text>();
        foreach (Text text in texts)
        {
            if (text.gameObject.name == "valueText") // Ищем компонент текста с именем "valueText"
            {
                return text;
            }
        }
        Debug.LogError("Не найден компонент текста для здоровья!");
        return null;
    }

    void GameOver(bool playerWon)
    {
        if (playerWon)
        {
            roundStatusText.text = "Матч окончен!";
        }
        else
        {
            roundStatusText.text = "Матч окончен!";
        }

        // Отключаем кнопки
        ActionButtonsInputAvailable(false);

        // Показать попап завершения игры
        screenManager.ShowMatchEndPopup(playerWon);  // Показываем попап

        // Останавливаем таймеры и действия
    }


    void RestartGame()
    {
        // Сбрасываем состояние игры
        isGameOver = false;

        // Сбрасываем здоровье игрока и соперника
        player.ResetHealth();
        enemy.ResetHealth();

        // Обновляем UI здоровья
        UpdateHealthUI();

        // Сбрасываем текст статуса раунда
        roundNumber = 0;
        roundStatusText.text = "Начало нового матча!";
        timerText.text = "";

        // Запускаем первый раунд после небольшого ожидания
        Invoke("StartPreparation", delay);
    }
    public IEnumerator ShowDamageEffect(Slider damageSlider, Slider mainSlider, int previousHealth)
    {
        yield return new WaitForSeconds(0.5f);  // Ждем 1 секунду перед анимацией

        float elapsedTime = 0f;
        float duration = 2f;  // Длительность анимации
        float targetValue = previousHealth - damageAmount;  // Целевое значение (здоровье после урона)

        // Плавное уменьшение значения слайдера
        while (damageSlider.value > targetValue)
        {
            elapsedTime += Time.deltaTime;
            // Уменьшаем значение damageSlider с использованием Mathf.MoveTowards
            damageSlider.value = Mathf.MoveTowards(damageSlider.value, targetValue, (damageSlider.maxValue / duration) * Time.deltaTime);
            yield return null;
        }

        // Убедитесь, что значение слайдера урона совпадает с основным слайдером
        damageSlider.value = targetValue;
    }

    public void TakeDamage(Slider damageSlider, Slider mainSlider, int amount, ICombatant combatant)
    {
        combatant.TakeDamage(amount); // уменьшаем здоровье

        // Обновляем основной слайдер здоровья
        StartCoroutine(UpdateHealthSlider(mainSlider, combatant.CurrentHealth));

        // Передаем начальное значение здоровья до получения урона
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
