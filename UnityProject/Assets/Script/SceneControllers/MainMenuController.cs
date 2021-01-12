using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : SceneController
{
    public DialogMessage startMessage;
    public DialogMessage questionMessage;

    public DialogManager dialogManager;

    public GameObject creditsPanel;

    public void Start()
    {
        dialogManager.StartDialog(startMessage);
    }

    public void Restart()
    {
        dialogManager.StartDialog(questionMessage);
    }

    public override void ReadAnswers(List<string> ans)
    {
        if (ans[0].Equals("Start"))
        {
            SceneManager.LoadScene("Stage1");
        }
        else if (ans[0].Equals("Credits"))
        {
            creditsPanel.SetActive(true);
        }
        else
        {
            Application.Quit();
        }
    }
}
