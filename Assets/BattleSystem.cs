using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BattleSystem : MonoBehaviour
{
    public Text roundStatusText;
    public Text timerText;
    public Button rockButton;
    public Button paperButton;
    public Button scissorsButton;

    public Slider PlayerMainHealthSlider;
    public Slider PlayerDamageHealthSlider;

    public Slider EnemyMainHealthSlider;
    public Slider EnemyDamageHealthSlider;

    private int playerStartHealth = 100;
    private int enemyStartHealth = 100;
    private int playerCurrentHealth;
    private int enemyCurrentHealth;
    private int roundNumber = 0;
    private int damageAmount = 20;


    private float timer = 0f;
    private float preparationTime = 2.0f;
    private float decisionTime = 5.0f;
    private float delay = 1.0f;



    private string playerChoice = "";
    private string enemyChoice = "";
    private bool hasPlayerChosen = false;


    void Start()
    {
        playerCurrentHealth = playerStartHealth;
        enemyCurrentHealth = enemyStartHealth;

        PlayerMainHealthSlider.maxValue = playerStartHealth;
        EnemyMainHealthSlider.maxValue = enemyStartHealth;
        PlayerDamageHealthSlider.maxValue = playerStartHealth;
        EnemyDamageHealthSlider.maxValue = enemyStartHealth;

        PlayerMainHealthSlider.value = playerStartHealth;
        EnemyMainHealthSlider.value = enemyStartHealth;
        PlayerDamageHealthSlider.value = playerStartHealth;
        EnemyDamageHealthSlider.value = enemyStartHealth;

        roundStatusText.text = "������ �����...";
        timerText.text = "";
        Invoke("StartPreparation", delay);

        rockButton.onClick.AddListener(() => PlayerMakesChoice("Rock"));
        paperButton.onClick.AddListener(() => PlayerMakesChoice("Paper"));
        scissorsButton.onClick.AddListener(() => PlayerMakesChoice("Scissors"));
    }

    void StartPreparation()
    {
        ResetChoices();
        timer = preparationTime;
        roundNumber++;
        ActionButtonsInputAvailable(false);
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
        roundStatusText.text = $"�������� �����!";
        timer = decisionTime;
        ActionButtonsInputAvailable(true);
        StartCoroutine(DecisionPhase());
    }

    IEnumerator DecisionPhase()
    {
        while (timer > 0)
        {
            if (hasPlayerChosen)
            {
                break;
            }
            timer -= Time.deltaTime;
            timerText.text = $"{Mathf.Ceil(timer)}";
            yield return null;
        }
        if (!hasPlayerChosen)
        {
            playerChoice = "None";
        }
        CalculateEnemyChoice();
        EvaluateRoundResult();
    }

    void PlayerMakesChoice(string choice)
    {
        if (!hasPlayerChosen)
        {
            hasPlayerChosen = true;
            playerChoice = choice;
            Debug.Log($"����� ������: {choice}");
            ActionButtonsInputAvailable(false);
        }
    }

    void CalculateEnemyChoice()
    {
        string[] choices = { "Rock", "Paper", "Scissors" };
        enemyChoice = choices[Random.Range(0, choices.Length)];
        Debug.Log($"��������� ������: {enemyChoice}");
    }

    void EvaluateRoundResult()
    {
        Debug.Log($"���������: {playerChoice} vs {enemyChoice}");

        if (playerChoice == enemyChoice)
        {
            roundStatusText.text = "�����!";
        }
        else if ((playerChoice == "Rock" && enemyChoice == "Scissors") ||
                 (playerChoice == "Paper" && enemyChoice == "Rock") ||
                 (playerChoice == "Scissors" && enemyChoice == "Paper"))
        {
            roundStatusText.text = "�� ��������!";
            enemyCurrentHealth -= damageAmount;
            TakeDamage(EnemyDamageHealthSlider, EnemyMainHealthSlider, enemyCurrentHealth, damageAmount);

            // ��������, ���� �������� ���������� <= 0
            if (enemyCurrentHealth <= 0)
            {
                GameOver(true); // ������ ������
                return; // ��������� ���������� ������
            }
        }
        else
        {
            roundStatusText.text = "�� ���������!";
            playerCurrentHealth -= damageAmount;
            TakeDamage(PlayerDamageHealthSlider, PlayerMainHealthSlider, playerCurrentHealth, damageAmount);

            // ��������, ���� �������� ������ <= 0
            if (playerCurrentHealth <= 0)
            {
                GameOver(false); // ������ ����������
                return; // ��������� ���������� ������
            }
        }

        // �������� ����� ������� ���������� ������
        Invoke("StartPreparation", delay);
    }

    void ResetChoices()
    {
        playerChoice = null;
        enemyChoice = null;
        hasPlayerChosen = false;

    }
    public void TakeDamage(Slider damageSlider, Slider mainSlider, int currentHealth, int damageAmount)
    {
        Debug.Log($"�������� �����: {damageAmount}");
        // ��������� �������� ������� ��������
        StartCoroutine(UpdateHealthSlider(mainSlider, currentHealth));

        // �������� ��������� �������� �������� �� ��������� �����
        int previousHealth = currentHealth + damageAmount;
        StartCoroutine(ShowDamageEffect(damageSlider, mainSlider, previousHealth));
    }

    private IEnumerator ShowDamageEffect(Slider damageSlider, Slider mainSlider, int previousHealth)
    {
        // ���� ������������ ����� ����� ������� ��������
        yield return new WaitForSeconds(1f);

        float elapsedTime = 0f;
        float duration = 2f; // ����������������� ��������
        float targetValue = previousHealth - damageAmount;    // ������� �������� ��������� �������� ����� ��������� �����

        Debug.Log($"�������� ����� � {damageSlider.value} �� {targetValue}");

        // ������ ��������� �������� �������� ����� �� ������ ��������� ��������
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



    IEnumerator UpdateHealthSlider(Slider healthSlider, int newHealth)
    {
        Debug.Log($"UpdateHealthSlider");
        float elapsedTime = 0f;
        float duration = 0.5f; // ����������������� ��������
        float startValue = healthSlider.value;
        float endValue = newHealth;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float currentValue = Mathf.Lerp(startValue, endValue, elapsedTime / duration);
            healthSlider.value = currentValue;
            GetHealthTextComponent(healthSlider).text = $"{Mathf.RoundToInt(currentValue).ToString()}/{healthSlider.maxValue.ToString()}";
            yield return null;
        }

        healthSlider.value = endValue;
        GetHealthTextComponent(healthSlider).text = $"{endValue.ToString()}/{healthSlider.maxValue.ToString()}";
    }
    void ActionButtonsInputAvailable(bool interactable)
    {
        rockButton.interactable = interactable;
        paperButton.interactable = interactable;
        scissorsButton.interactable = interactable;
    }
    private Text GetHealthTextComponent(Slider slider)
    {
        Text[] texts = slider.GetComponentsInChildren<Text>();
        foreach (Text text in texts)
        {
            if (text.gameObject.name == "valueText") // ��� ����������� ������ ���������� �������
            {
                return text;
            }
        }
        Debug.Log("��� ����������� ������ � ��������");
        return null;
    }
    void RestartGame()
    {
        StopAllCoroutines();
        // ���������� ��������
        playerCurrentHealth = playerStartHealth;
        enemyCurrentHealth = enemyStartHealth;

        // ��������� �������� ��������� ��������
        PlayerMainHealthSlider.value = playerCurrentHealth;
        PlayerDamageHealthSlider.value = playerCurrentHealth;

        EnemyMainHealthSlider.value = enemyCurrentHealth;
        EnemyDamageHealthSlider.value = enemyCurrentHealth;

        // ��������� ����� ��������
        GetHealthTextComponent(PlayerMainHealthSlider).text = $"{playerCurrentHealth}/{playerStartHealth}";
        GetHealthTextComponent(EnemyMainHealthSlider).text = $"{enemyCurrentHealth}/{enemyStartHealth}";

        // ���������� ��������� ������
        roundNumber = 0;
        roundStatusText.text = "������ ������ �����!";
        timerText.text = "";

        // ����� ��������� ���� �������
        Invoke("StartPreparation", delay);
    }

    void GameOver(bool playerWon)
    {

        if (playerWon)
        {
            roundStatusText.text = "�� �������� ����!";
        }
        else
        {
            roundStatusText.text = "�� ��������� ����!";
        }

        // ��������� ������
        ActionButtonsInputAvailable(false);

        // ������������� ������� � ��������

        // ����� ��������� ������ ������������� ����
        Invoke("RestartGame", 3f); // ���������� ����� 3 �������
    }


}