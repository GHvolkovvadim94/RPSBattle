using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class BattleSystem : MonoBehaviour
{
    private ScreenManager screenManager;
    public Text roundStatusText;
    public Text timerText;
    public Button rockButton;
    public Button paperButton;
    public Button scissorsButton;

    public Sprite rockSprite;    // Спрайт для камня
    public Sprite paperSprite;   // Спрайт для бумаги
    public Sprite scissorsSprite; // Спрайт для ножниц
    public Sprite emptySprite; // Спрайт пустого выбора


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

    public static event Action<string> OnPlayerMadeChoice;

    // Для хранения всех раундов
    private List<RoundResult> roundResults = new List<RoundResult>();

    // UI для отображения списка действий
    public GameObject roundResultPrefab; // Префаб для отображения результата раунда
    public Transform roundResultsContent; // Контейнер для элементов списка
    public ScrollRect scrollView; // ScrollView для скроллинга списка


    private bool isPlayerReady = false; // Флаг для проверки, что игрок сделал выбор
    private bool isEnemyReady = false; // Флаг для соперника

    private bool isGameOver = false; // Флаг для отслеживания состояния игры
    private void Awake()
    {
        ActionButtonsInputAvailable(false); // Отключаем кнопки на время подготовки
        screenManager = FindObjectOfType<ScreenManager>();
        scrollView = scrollView.GetComponent<ScrollRect>();
    }

    private void OnEnable()
    {
        // Подписка на событие нажатия кнопки "Старт"
        ScreenManager.OnStartButtonPressed += StartPreparation;
        ScreenManager.OnEndMatchButtonPressed += RestartGame;

    }

    private void OnDisable()
    {
        // Отписка от события при отключении BattleSystem
        ScreenManager.OnStartButtonPressed -= StartPreparation;
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

            // Сохраняем результат раунда
            roundResults.Add(new RoundResult(playerAction, enemyAction));

            // Обновляем список на экране
            AddRoundResultToUI(playerAction, enemyAction);

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
    private void AddRoundResultToUI(ICombatantAction playerAction, ICombatantAction enemyAction)
    {
        // Создаем новый элемент UI на основе префаба
        GameObject roundResultObject = Instantiate(roundResultPrefab, roundResultsContent);

        // Находим текстовые компоненты и изображения в префабе
        //Text roundText = roundResultObject.GetComponentInChildren<Text>();
        Image[] images = roundResultObject.GetComponentsInChildren<Image>();
        Debug.Log(images.Length);

        Image playerActionImage = null;
        Image playerWinFrame = null;
        Image enemyActionImage = null;
        Image enemyWinFrame = null;

        // Ищем нужные компоненты для спрайтов действий и корон
        foreach (Image image in images)
        {
            if (image.gameObject.name == "PlayerActionImage")
            {
                playerActionImage = image;
                Debug.Log("Назначил PlayerActionImage");
            }
            else if (image.gameObject.name == "PlayerWinFrame")
            {
                playerWinFrame = image;
                Debug.Log("Назначил PlayerWinFrame");
            }
            else if (image.gameObject.name == "EnemyActionImage")
            {
                enemyActionImage = image;
                Debug.Log("Назначил EnemyActionImage");
            }
            else if (image.gameObject.name == "EnemyWinFrame")
            {
                enemyWinFrame = image;
                Debug.Log("Назначил EnemyWinFrame");
            }
        }

        // Назначаем текст для раунда
        //roundText.text = $"Раунд {roundNumber}";

        // Назначаем спрайты для действий игрока и противника
        playerActionImage.sprite = GetActionSprite(playerAction.GetActionName());
        enemyActionImage.sprite = GetActionSprite(enemyAction.GetActionName());

        // Скрываем обе короны по умолчанию
        playerWinFrame.gameObject.SetActive(false);
        enemyWinFrame.gameObject.SetActive(false);

        // Определяем, кто победил, и включаем корону
        if (playerAction.CurrentChoice == enemyAction.CurrentChoice)
        {
            //roundText.text += " — Ничья!";
        }
        else if ((playerAction.CurrentChoice == "Rock" && enemyAction.CurrentChoice == "Scissors") ||
                 (playerAction.CurrentChoice == "Paper" && enemyAction.CurrentChoice == "Rock") ||
                 (playerAction.CurrentChoice == "Scissors" && enemyAction.CurrentChoice == "Paper"))
        {
            //roundText.text += " — Победа!";
            // Показываем корону над игроком
            playerWinFrame.gameObject.SetActive(true);
        }
        else
        {
            //roundText.text += " — Поражение!";
            // Показываем корону над противником
            enemyWinFrame.gameObject.SetActive(true);
        }

        Canvas.ForceUpdateCanvases();  // Форсируем обновление UI
        ScrollToBottom();
    }

    private Sprite GetActionSprite(string actionName)
    {
        switch (actionName)
        {
            case "Rock":
                return rockSprite;
            case "Paper":
                return paperSprite;
            case "Scissors":
                return scissorsSprite;
            default:
                return emptySprite;
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
        StopAllCoroutines();
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
        roundResults.Clear();
        ClearScrollViewContent();
        // Обновляем UI здоровья
        UpdateHealthUI();

        // Сбрасываем текст статуса раунда
        roundNumber = 0;
        roundStatusText.text = "Начало нового матча!";
        timerText.text = "";

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
    public void ClearScrollViewContent()
    {
        // Удаляем всех детей внутри scrollViewContent
        foreach (Transform child in roundResultsContent)
        {
            Destroy(child.gameObject);
        }
    }
    // Метод для прокрутки ScrollView до самого конца
    private void ScrollToBottom()
    {
        // Устанавливаем вертикальный скроллбар на 0 (самый низ)
        scrollView.verticalNormalizedPosition = 0f;

        // Форсируем обновление после прокрутки
        Canvas.ForceUpdateCanvases();
    }
}
