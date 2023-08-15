using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using DG.Tweening;
using Newtonsoft.Json;
using TMPro;

namespace SampleProject
{
    [System.Serializable]
    public class Client
    {
        public bool isManager;
        public int id;
        public string label;
    }

    [System.Serializable]
    public class ClientData
    {
        public List<Client> clients;
        public Dictionary<string, Data> data;
    }

    [System.Serializable]
    public class Data
    {
        public string address;
        public string name;
        public int points;
    }

    public class APIClient : MonoBehaviour
    {
        private const string apiUrl = "https://qa2.sunbasedata.com/sunbase/portal/api/assignment.jsp?cmd=client_data";
        public GameObject clientButtonPrefab;
        public Transform clientButtonContainer;
        public GameObject popupPanel;
        public Text popupNameText;
        public Text popupPointsText;
        public Text popupAddressText;
        private ClientData clientData;
        public GameObject border;
        public GameObject closeButton;
        public TMP_Dropdown filterDropdown;

        void Start()
        {
            StartCoroutine(FetchData());
        }

        private IEnumerator FetchData()
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(apiUrl))
            {
                yield return webRequest.SendWebRequest();

                if (webRequest.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("Error fetching data: " + webRequest.error);
                    yield break;
                }

                string jsonData = webRequest.downloadHandler.text;
                Debug.Log(jsonData);

                clientData = JsonConvert.DeserializeObject<ClientData>(jsonData);
                CreateClientButtons();
            }
        }

        private void CreateClientButtons()
        {
            foreach (Client client in clientData.clients)
            {
                GameObject clientButton = Instantiate(clientButtonPrefab, clientButtonContainer);
                TMP_Text buttonText = clientButton.GetComponentInChildren<TMP_Text>();

                if (clientData.data.TryGetValue(client.id.ToString(), out Data clientDataEntry))
                {
                    buttonText.text = $"{client.label} - Points: {clientDataEntry.points}";
                }
                else
                {
                    buttonText.text = $"{client.label} - Points: N/A";
                }

                Button buttonComponent = clientButton.GetComponent<Button>();
                buttonComponent.onClick.AddListener(() => ShowPopup(client.id));
            }
        }

        public void ShowPopup(int clientId)
        {
            if (clientData.data.TryGetValue(clientId.ToString(), out Data clientDataEntry))
            {
                popupNameText.text = $"Name: {clientDataEntry.name}";
                popupPointsText.text = $"Points: {clientDataEntry.points}";
                popupAddressText.text = $"Address: {clientDataEntry.address}";

                popupPanel.SetActive(true);
                border.SetActive(true);
                closeButton.SetActive(true);
            }
            else
            {
                Debug.LogWarning($"No data found for client ID {clientId}");
            }
        }

        public void ClosePopup()
        {
            popupPanel.GetComponent<CanvasGroup>().DOFade(0f, 0.5f).OnComplete(() =>
            {
                popupPanel.SetActive(false);
            });
        }

        public void OnDropdownValueChanged()
        {
            string selectedOption = filterDropdown.options[filterDropdown.value].text;

            // Clear the previous client buttons
            foreach (Transform child in clientButtonContainer)
            {
                Destroy(child.gameObject);
            }

            // Create new client buttons based on the selected option
            foreach (Client client in clientData.clients)
            {
                bool shouldDisplay = false;

                // Apply filtering based on the selected option
                if (selectedOption == "All clients")
                {
                    shouldDisplay = true;
                }
                else if (selectedOption == "Managers only")
                {
                    if (client.isManager)
                    {
                        shouldDisplay = true;
                    }
                }
                else if (selectedOption == "Non managers")
                {
                    if (!client.isManager)
                    {
                        shouldDisplay = true;
                    }
                }

                if (shouldDisplay)
                {
                    GameObject clientButton = Instantiate(clientButtonPrefab, clientButtonContainer);
                    TMP_Text buttonText = clientButton.GetComponentInChildren<TMP_Text>();

                    if (clientData.data.TryGetValue(client.id.ToString(), out Data clientDataEntry))
                    {
                        buttonText.text = $"{client.label} - Points: {clientDataEntry.points}";
                    }
                    else
                    {
                        buttonText.text = $"{client.label} - Points: N/A";
                    }

                    Button buttonComponent = clientButton.GetComponent<Button>();
                    buttonComponent.onClick.AddListener(() => ShowPopup(client.id));
                }
            }
        }

    }
}
