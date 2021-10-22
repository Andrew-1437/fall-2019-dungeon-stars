using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelSelect : MonoBehaviour
{
    public Button[] buttons;
    public RectTransform content;

    int highestLevelCompleted; // Get from PlayerPrefs

    private void Start()
    {
        highestLevelCompleted = PlayerPrefs.GetInt("highestLevelCompleted", 0);

        for (int i = 0; i < buttons.Length; i++)
        {
            if(i > highestLevelCompleted)
            {
                buttons[i].interactable = false;
                buttons[i].GetComponent<LineRenderer>().enabled = false;
                buttons[i].GetComponentInChildren<TextMeshProUGUI>().text = "???";
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            SceneLoader.Instance.LoadScene("MainMenu");

        float scroll = Input.mouseScrollDelta.y * Time.deltaTime * 10f;
        content.localScale = new Vector3(
            Mathf.Clamp(content.localScale.x + scroll, .2f, 2f),
            Mathf.Clamp(content.localScale.y + scroll, .2f, 2f), 1);
    }

    public void LoadLevel(string levelName)
    {
        SceneLoader.Instance.LoadScene(levelName);
    }
}
