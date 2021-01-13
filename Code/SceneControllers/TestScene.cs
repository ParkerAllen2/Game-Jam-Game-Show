using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScene : MonoBehaviour
{
    public DialogMessage dialogMessage;

    public DialogManager dialogManager;

    public void Start()
    {
        dialogManager.StartDialog(dialogMessage, "");
    }
}
