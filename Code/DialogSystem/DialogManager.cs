using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    //Storing UI stuff
    public Image nameTag;
    public Text dialogBox;
    public GameObject dialogPanel;
    public SceneController controller;

    DialogMessage dialogMessage;    //Current dialogue message
    Queue<string> sentences;        //queue of sentences from dialogue messages

    public Button prefabAnsBut;     //prefab of buttons
    public GameObject panelButtons; //panel to put buttons
    Button[] buttons;               //array of buttons

    List<string> answers;       //todo

    public float typingSpeed = .02f;    //typing speed
    bool isTyping;                      //flag if still typing
    string currentSentence;             //the current sentence

    public void Awake()
    {
        sentences = new Queue<string>();
        answers = new List<string>();
        buttons = new Button[0];
    }

    public void StartDialog(DialogMessage dialog, string ans)
    {
        answers.Add(ans);
        StartDialog(dialog);
    }

    //Starts a new Dialogue with given dialogue message
    public void StartDialog(DialogMessage dialog)
    {
        ClearButtons();
        sentences.Clear();
        dialogPanel.SetActive(true);

        dialogMessage = dialog;
        nameTag.sprite = dialog.nameTag;

        foreach(string sentence in dialog.sentences)
        {
            sentences.Enqueue(sentence);
        }
        DisplayNextSentence();
    }

    //displays next sentence from dialogue messages
    public void DisplayNextSentence()
    {
        StopAllCoroutines();    //stop typing
        if (isTyping)
        {
            dialogBox.text = currentSentence;   //if it was typing set sentence to current sentence
            isTyping = false;
        }
        else if (sentences.Count == 0)
        {
            if (!panelButtons.activeInHierarchy)
            {
                if (dialogMessage.endDialog)
                {
                    controller.ReadAnswers(answers);
                    answers.Clear();
                    dialogPanel.SetActive(false);   //if dialog was end of converstion hide dialogue panel
                }
                else if (dialogMessage.dialogueOptions.Length == 1)
                {
                    StartDialog(dialogMessage.dialogueOptions[0].nextDialogue);
                }
                else
                {
                    CreateButtons();    //else display dialogue options
                }
            }
            return;
        }
        else
            StartCoroutine(TypeSentence(sentences.Dequeue()));  //else start typing next sentence
    }

    //types givien sentence
    IEnumerator TypeSentence(string sentence)
    {
        currentSentence = sentence;
        isTyping = true;
        dialogBox.text = "";

        foreach (char letter in sentence.ToCharArray())
        {
            dialogBox.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;
    }

    //creates buttons for dialogue options
    public void CreateButtons()
    {
        int size = dialogMessage.dialogueOptions.Length;
        buttons = new Button[size];
        panelButtons.SetActive(true);

        for(int i = 0; i < size; i++)
        {
            string s = dialogMessage.dialogueOptions[i].text;

            //Instatiant buttons as children of dialogue panel
            buttons[i] = Instantiate(prefabAnsBut, panelButtons.transform);
            buttons[i].GetComponentInChildren<Text>().text = s;

            //add onclick lisenter to start next dialogue
            DialogMessage next = dialogMessage.dialogueOptions[i].nextDialogue;
            buttons[i].onClick.AddListener(delegate { StartDialog(next, s); });
        }
    }

    //destroy de buttons
    public void ClearButtons()
    {
        panelButtons.SetActive(false);
        foreach(Button b in buttons)
        {
            Destroy(b.gameObject);
        }
        buttons = new Button[0];
    }
}
