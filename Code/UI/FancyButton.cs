using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FancyButton : MonoBehaviour
{
    public GameObject leftHighlight, rightHighlight;
    public int incTextSize;
    public bool bold;

    string originalText;
    Text t;

    public void Start()
    {
        t = GetComponentInChildren<Text>();
    }

    public void Highlight()
    {
        if (leftHighlight != null)
            leftHighlight.SetActive(true);

        if (rightHighlight != null)
            rightHighlight.SetActive(true);

        t.fontSize += incTextSize;

        if(bold)
        {
            originalText = t.text;
            t.text = "<b>" + t.text + "</b>";
        }
    }

    public void UnHighlight()
    {
        if (leftHighlight != null)
            leftHighlight.SetActive(false);

        if (rightHighlight != null)
            rightHighlight.SetActive(false);

        t.fontSize -= incTextSize;

        if (bold)
        {
            t.text = originalText;
        }
    }
}
