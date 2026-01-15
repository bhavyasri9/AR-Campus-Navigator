using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FirebaseAuthUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField emailInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private TMP_InputField displayNameInput;
    [SerializeField] private Button loginButton;
    [SerializeField] private Button registerButton;
    [SerializeField] private Button logoutButton;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private GameObject authPanel;

    void Start()
    {
        if (loginButton != null) loginButton.onClick.AddListener(OnLoginClick);
        if (registerButton != null) registerButton.onClick.AddListener(OnRegisterClick);
        if (logoutButton != null) logoutButton.onClick.AddListener(OnLogoutClick);

        UpdateAuthUI();
    }

    private async void OnLoginClick()
    {
        if (string.IsNullOrEmpty(emailInput.text) || string.IsNullOrEmpty(passwordInput.text))
        {
            ShowStatus("Please enter email and password", Color.red);
            return;
        }

        ShowStatus("Logging in...", Color.yellow);
        await FirebaseManager.Instance.LoginAsync(emailInput.text, passwordInput.text);
        
        if (FirebaseManager.Instance.GetCurrentUser() != null)
        {
            ShowStatus("Login successful!", Color.green);
            UpdateAuthUI();
        }
        else
        {
            ShowStatus("Login failed", Color.red);
        }
    }

    private async void OnRegisterClick()
    {
        if (string.IsNullOrEmpty(emailInput.text) || string.IsNullOrEmpty(passwordInput.text) || string.IsNullOrEmpty(displayNameInput.text))
        {
            ShowStatus("Please fill all fields", Color.red);
            return;
        }

        ShowStatus("Registering...", Color.yellow);
        await FirebaseManager.Instance.RegisterAsync(emailInput.text, passwordInput.text, displayNameInput.text);
        
        if (FirebaseManager.Instance.GetCurrentUser() != null)
        {
            ShowStatus("Registration successful!", Color.green);
            UpdateAuthUI();
        }
        else
        {
            ShowStatus("Registration failed", Color.red);
        }
    }

    private void OnLogoutClick()
    {
        FirebaseManager.Instance.Logout();
        ShowStatus("Logged out", Color.yellow);
        UpdateAuthUI();
    }

    private void UpdateAuthUI()
    {
        bool isLoggedIn = FirebaseManager.Instance.GetCurrentUser() != null;
        
        if (authPanel != null)
            authPanel.SetActive(!isLoggedIn);

        if (logoutButton != null)
            logoutButton.gameObject.SetActive(isLoggedIn);

        if (isLoggedIn)
        {
            ShowStatus("Logged in as: " + FirebaseManager.Instance.GetCurrentUser().Email, Color.green);
        }
    }

    private void ShowStatus(string message, Color color)
    {
        if (statusText != null)
        {
            statusText.text = message;
            statusText.color = color;
        }
        Debug.Log(message);
    }
}
