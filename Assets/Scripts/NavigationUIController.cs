using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationUIController : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject navigationPanel;

    public void StartNavigation()
    {
        mainMenu.SetActive(false);
        navigationPanel.SetActive(true);
    }

    public void ResetNavigation()
    {
        navigationPanel.SetActive(false);
        mainMenu.SetActive(true);
    }
}
