using System;
using UnityEngine;
using UnityEngine.UI;

public class ScreenManager : MonoBehaviour
{
    public GameObject lobbyScreen;    // Экран лобби
    public GameObject gameScreen;     // Экран игры
    public Button startButton;
    public Button endMatchButton;



    public Text matchEndTitleText; // Ссылка на текст заголовка попапа


    // Попапы
    public GameObject settingsPopup;  // Попап настроек
    public GameObject matchEndPopup;  // Попап завершения матча

    public static event Action OnStartButtonPressed;
    public static event Action OnEndMatchButtonPressed;



    void Start()
    {
        startButton.onClick.AddListener(StartButtonPressed);
        endMatchButton.onClick.AddListener(EndMatchButtonPressed);

        ShowLobby(); // По умолчанию показываем лобби
    }

    private void EndMatchButtonPressed()
    {
        matchEndPopup.SetActive(false);
        gameScreen.SetActive(false);
        lobbyScreen.SetActive(true);

        // Вызов события
        OnEndMatchButtonPressed?.Invoke();
    }

    // Метод, вызываемый при нажатии на кнопку "Старт"
    void StartButtonPressed()
    {
        // Переход на экран игры
        ShowBattleScreen();

        // Вызов события
        OnStartButtonPressed?.Invoke(); // Вызов всех подписчиков на это событие
    }

    public void ShowLobbyScreen()
    {
        lobbyScreen.SetActive(true);
        gameScreen.SetActive(false);
    }

    public void ShowBattleScreen()
    {
        lobbyScreen.SetActive(false);
        gameScreen.SetActive(true);
    }

    // Показать лобби
    public void ShowLobby()
    {
        lobbyScreen.SetActive(true);
        gameScreen.SetActive(false);
        settingsPopup.SetActive(false);
        matchEndPopup.SetActive(false);
    }

    // Показать экран игры


    // Показать попап завершения матча
    public void ShowMatchEndPopup(bool playerWon)
    {
        if (playerWon)
        {
            matchEndTitleText.text = "Победа!";
        }
        else
        {
            matchEndTitleText.text = "Поражение";
        }
        matchEndPopup.SetActive(true);
    }

    // Скрыть попап завершения матча


    // Показать попап настроек
    public void ShowSettingsPopup()
    {
        settingsPopup.SetActive(true);
    }

    // Скрыть попап настроек
    public void HideSettingsPopup()
    {
        settingsPopup.SetActive(false);
    }

}
