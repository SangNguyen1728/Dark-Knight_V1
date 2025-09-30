using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PlayerUIPopUpManager : MonoBehaviour
{
    [Header("You Dead Pop Up")]
    [SerializeField] GameObject youDiePopUpGameObject;
    [SerializeField] TextMeshProUGUI youDiePopUpBackgroundText;
    [SerializeField] TextMeshProUGUI youDiePopUpText;
    [SerializeField] CanvasGroup youDiePopUpCanvasGroup;// allow to set alpha to fade over time

    [Header("Message Pop Up")]
    [SerializeField] TextMeshProUGUI popUpMessageText;
    [SerializeField] GameObject popUpMessageGameObject;

    [Header("Boss Defeated Pop Up")]
    [SerializeField] GameObject bossDefeatedPopUpGameObject;
    [SerializeField] TextMeshProUGUI bossDefeatedPopUpBackgroundText;
    [SerializeField] TextMeshProUGUI bossDefeatedPopUpText;
    [SerializeField] CanvasGroup bossDefeatedPopUpCanvasGroup;
    
    [Header("Grace Restored Pop Up")]
    [SerializeField] GameObject graceRestoredPopUpGameObject;
    [SerializeField] TextMeshProUGUI graceRestoredPopUpBackgroundText;
    [SerializeField] TextMeshProUGUI graceRestoredPopUpText;
    [SerializeField] CanvasGroup graceRestoredPopUpCanvasGroup;

    [Header("Item Pop Up")]
    [SerializeField] GameObject itemPopUpGamObject;
    [SerializeField] Image itemIcon;
    [SerializeField] TextMeshProUGUI itemName;
    [SerializeField] TextMeshProUGUI itemAmonut;


    public void CloseAllPopUpWindows()
    {
        popUpMessageGameObject.SetActive(false);
        itemPopUpGamObject.SetActive(false);

        PlayerUIManager.instance.popUpWindowIsOpen = false;
    }
    public void SendPlayerMessagePopUp(string messageText)
    {
        PlayerUIManager.instance.popUpWindowIsOpen = true;
        popUpMessageText.text = messageText;
        popUpMessageGameObject.SetActive(true);
    }

    public void SendItemPopUp(Item item, int amount)
    {
        itemAmonut.enableAutoSizing = false;
        itemIcon.sprite = item.itemIcon;
        itemName.text = item.itemName;

        if(amount > 0)
        {
            itemAmonut.enabled = true;
            itemAmonut.text = "x" + amount.ToString();
        }

        itemPopUpGamObject.SetActive(true);
        PlayerUIManager.instance.popUpWindowIsOpen = true;
    }
    public void SendYouDiedPopUp()
    {
        
        youDiePopUpGameObject.SetActive(true);
        
        youDiePopUpBackgroundText.characterSpacing = 0;

        // stretch pop up
        StartCoroutine(StretchPopUpTextOverTime(youDiePopUpBackgroundText, 8, 19));

        // fade in pop up
        StartCoroutine(FadeInPopUpOverTime(youDiePopUpCanvasGroup,5));

        // wait then fade out pop up
        StartCoroutine(WaitThenFadeOut(youDiePopUpCanvasGroup, 2, 5));
    } 
    public void SendBossDefeatedPopUp(string bossDefeatedMessager)
    {
        bossDefeatedPopUpText.text = bossDefeatedMessager;
        bossDefeatedPopUpBackgroundText.text = bossDefeatedMessager;

        bossDefeatedPopUpGameObject.SetActive(true);

        bossDefeatedPopUpBackgroundText.characterSpacing = 0;

        // stretch pop up
        StartCoroutine(StretchPopUpTextOverTime(bossDefeatedPopUpBackgroundText, 8, 19));

        // fade in pop up
        StartCoroutine(FadeInPopUpOverTime(bossDefeatedPopUpCanvasGroup, 5));

        // wait then fade out pop up
        StartCoroutine(WaitThenFadeOut(bossDefeatedPopUpCanvasGroup, 2, 5));
    }

    public void SendGraceRestoredPopUp(string graceRestoredMessage)
    {
        graceRestoredPopUpText.text = graceRestoredMessage;
        graceRestoredPopUpBackgroundText.text = graceRestoredMessage;

        graceRestoredPopUpGameObject.SetActive(true);

        graceRestoredPopUpBackgroundText.characterSpacing = 0;

        // stretch pop up
        StartCoroutine(StretchPopUpTextOverTime(graceRestoredPopUpBackgroundText, 8, 19));

        // fade in pop up
        StartCoroutine(FadeInPopUpOverTime(graceRestoredPopUpCanvasGroup, 5));

        // wait then fade out pop up
        StartCoroutine(WaitThenFadeOut(graceRestoredPopUpCanvasGroup, 2, 5));
    }

    private IEnumerator StretchPopUpTextOverTime(TextMeshProUGUI text, float duration, float stretchAmout)
    {
        if(duration >0)
        {
            text.characterSpacing = 0; // reset character spacing
            float timer = 0;

            //yield return null;

            while(timer < duration)
            {
                timer = timer * Time.deltaTime;
                text.characterSpacing = Mathf.Lerp(text.characterSpacing, stretchAmout, duration * (Time.deltaTime / 20));
                yield return null;
            }
        }
    }
    private IEnumerator FadeInPopUpOverTime(CanvasGroup canvas, float duration)
    {
        if(duration > 0)
        {

            canvas.alpha = 0;
            float timer = 0;

            //yield return null;

            while (timer < duration)
            {
                timer = timer + Time.deltaTime;
                canvas.alpha = Mathf.Lerp(canvas.alpha, 1, timer/duration);
                yield return null;
            }
        }
        canvas.alpha = 1;

        //yield return null;
    }
    private IEnumerator WaitThenFadeOut(CanvasGroup canvas, float duration, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (duration > 0)
        {
            float timer = 0;

            while (delay > 0)
            {
                delay = delay - Time.deltaTime;
                yield return null;
            }

            canvas.alpha = 1;
            

            yield return null;

            while (timer < duration)
            {
                timer = timer * Time.deltaTime;
                canvas.alpha = Mathf.Lerp(canvas.alpha, 0, duration * Time.deltaTime);
                yield return null;
            }
        }
        canvas.alpha = 0;

        youDiePopUpGameObject.SetActive(false);
        yield return null;
    }
}
