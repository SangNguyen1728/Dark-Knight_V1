using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_StatBar : MonoBehaviour
{
    protected Slider slider;
    protected RectTransform rectTransform;

    [Header("Bar Options")]
    [SerializeField] protected bool scaleBarLengthWithStats = true;
    [SerializeField] protected float widthScaleMultiplier = 1;
    // variable to scale bar size depending on stat (highest stat = longer bar across screen)
    protected virtual void Awake()
    {
        slider = GetComponent<Slider>();
        rectTransform = GetComponent<RectTransform>();
    }
    protected virtual void Start()
    {

    }
    public virtual void SetStat(int newValue)
    {
        slider.value = newValue;
        Debug.Log("Current Health" + newValue);
        //slider.value = Mathf.Clamp(newValue, 0, slider.maxValue);
    }
    public virtual void SetMaxStat(int maxValue)
    {
        slider.maxValue = maxValue;
        slider.value = maxValue;

        if (scaleBarLengthWithStats)
        {
            rectTransform.sizeDelta = new Vector2(maxValue * widthScaleMultiplier, rectTransform.sizeDelta.y);

            // reset the position of the bars based on their layout grout's setting
            PlayerUIManager.instance.playerHudManager.RefeshHUD();
        }
       
       
    }
 }

