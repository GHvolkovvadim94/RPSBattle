using System;
using UnityEngine;
using UnityEngine.UI;

public class ScreenManager : MonoBehaviour
{
    public GameObject lobbyScreen;    // ����� �����
    public GameObject gameScreen;     // ����� ����
    public Button startButton;


    public Text matchEndTitleText; // ������ �� ����� ��������� ������


    // ������
    public GameObject settingsPopup;  // ����� ��������
    public GameObject matchEndPopup;  // ����� ���������� �����

    public static event Action OnStartButtonPressed;


    void Start()
    {
        startButton.onClick.AddListener(StartButtonPressed);

        ShowLobby(); // �� ��������� ���������� �����
    }
    // �����, ���������� ��� ������� �� ������ "�����"
    void StartButtonPressed()
    {
        // ������� �� ����� ����
        lobbyScreen.SetActive(false);
        gameScreen.SetActive(true);

        // ����� �������
        OnStartButtonPressed?.Invoke(); // ����� ���� ����������� �� ��� �������
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

    // �������� �����
    public void ShowLobby()
    {
        lobbyScreen.SetActive(true);
        gameScreen.SetActive(false);
        settingsPopup.SetActive(false);
        matchEndPopup.SetActive(false);
    }

    // �������� ����� ����
    public void StartGame()
    {
        lobbyScreen.SetActive(false);
        gameScreen.SetActive(true);


}

// �������� ����� ���������� �����
public void ShowMatchEndPopup(bool playerWon)
{
    if (playerWon)
    {
        matchEndTitleText.text = "������!";
    }
    else
    {
        matchEndTitleText.text = "���������";
    }
    matchEndPopup.SetActive(true);
}

// ������ ����� ���������� �����
public void HideMatchEndPopup()
{
    matchEndPopup.SetActive(false);
}

// �������� ����� ��������
public void ShowSettingsPopup()
{
    settingsPopup.SetActive(true);
}

// ������ ����� ��������
public void HideSettingsPopup()
{
    settingsPopup.SetActive(false);
}

// ����� � ����� (������������ � ������ ���������� �����)
public void ReturnToLobby()
{
    ShowLobby();
}
}
