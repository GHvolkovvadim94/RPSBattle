using UnityEngine;
using UnityEngine.UI;

public class ScreenManager : MonoBehaviour
{
    public GameObject lobbyScreen;    // ����� �����
    public GameObject gameScreen;     // ����� ����

    public Text matchEndTitleText; // ������ �� ����� ��������� ������


    // ������
    public GameObject settingsPopup;  // ����� ��������
    public GameObject matchEndPopup;  // ����� ���������� �����

    void Start()
    {
        ShowLobby(); // �� ��������� ���������� �����
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
