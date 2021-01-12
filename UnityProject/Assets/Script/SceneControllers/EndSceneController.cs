using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndSceneController : SceneController
{
    public DialogMessage startMessage;

    public DialogManager dialogManager; 

    public void Start()
    {
        dialogManager.StartDialog(startMessage);
    }

    public override void ReadAnswers(List<string> ans)
    {
        if (ans[0].Equals("Main"))
        {
            SceneManager.LoadScene("MainMenu");
        }
        else
        {
            Application.Quit();
        }
    }
}
