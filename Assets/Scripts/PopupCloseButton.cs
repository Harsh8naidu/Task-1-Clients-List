using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupCloseButton : MonoBehaviour
{
    public GameObject popupPanel; // Reference to the popup panel

    private void Start()
    {
        Button closeButton = GetComponent<Button>();

        // Add a click event listener
        closeButton.onClick.AddListener(ClosePopup);
    }

    private void ClosePopup()
    {
        // Deactivate the popup panel
        popupPanel.SetActive(false);
    }
}
