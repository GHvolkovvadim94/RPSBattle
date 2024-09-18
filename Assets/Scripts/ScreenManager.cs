using System;
using UnityEngine;
using UnityEngine.UI;

public class ScreenManager : MonoBehaviour
{
    public GameObject lobbyScreen;    // ����� �����
    public GameObject gameScreen;     // ����� ����
    public Button startButton;
    public Button endMatchButton;



    public Text matchEndTitleText; // ������ �� ����� ��������� ������


    // ������
    public GameObject settingsPopup;  // ����� ��������
    public GameObject matchEndPopup;  // ����� ���������� �����

    public static event Action OnStartButtonPressed;
    public static event Action OnEndMatchButtonPressed;



    void Start()
    {
        startButton.onClick.AddListener(StartButtonPressed);
        endMatchButton.onClick.AddListener(EndMatchButtonPressed);

        ShowLobby(); // �� ��������� ���������� �����
    }

    private void EndMatchButtonPressed()
    {
        matchEndPopup.SetActive(false);
        gameScreen.SetActive(false);
        lobbyScreen.SetActive(true);

        // ����� �������
        OnEndMatchButtonPressed?.Invoke();
    }

    // �����, ���������� ��� ������� �� ������ "�����"
    void StartButtonPressed()
    {
        // ������� �� ����� ����
        ShowBattleScreen();

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

}
