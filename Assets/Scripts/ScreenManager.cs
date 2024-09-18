using System;
using UnityEngine;
using UnityEngine.UI;

public class ScreenManager : MonoBehaviour
{
    public GameObject lobbyScreen;    // Экран лобби
    public GameObject gameScreen;     // Экран игры
    public Button startButton;


    public Text matchEndTitleText; // Ссылка на текст заголовка попапа


    // Попапы
    public GameObject settingsPopup;  // Попап настроек
    public GameObject matchEndPopup;  // Попап завершения матча

    public static event Action OnStartButtonPressed;


    void Start()
    {
        startButton.onClick.AddListener(StartButtonPressed);

        ShowLobby(); // По умолчанию показываем лобби
    }
    // Метод, вызываемый при нажатии на кнопку "Старт"
    void StartButtonPressed()
    {
        // Переход на экран игры
        lobbyScreen.SetActive(false);
        gameScreen.SetActive(true);

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
    public void StartGame()
    {
        lobbyScreen.SetActive(false);
        gameScreen.SetActive(true);


}

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
public void HideMatchEndPopup()
{
    matchEndPopup.SetActive(false);
}

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

// Выйти в лобби (используется в попапе завершения матча)
public void ReturnToLobby()
{
    ShowLobby();
}
}
