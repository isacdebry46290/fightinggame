using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerContainerUI : MonoBehaviour
{

    public TextMeshProUGUI scoreText;
    public Image healthbarfill;
    public Image chargeBarFill;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void updateScoreText(int Score)
    {
        scoreText.text = Score.ToString();
    }

    public void updateHealthBar(int curHp, int maxHp)
    {
        healthbarfill.fillAmount = (float)curHp / (float)maxHp;
    }
    public void updateChargeBar(float cahrgedmg, float maxChargeDmg)
    {
        chargeBarFill.fillAmount = maxChargeDmg / maxChargeDmg;
    }

    public void initialize(Color color)
    {
        scoreText.color = color;
        healthbarfill.color = color;
        scoreText.text = "0";
        healthbarfill.fillAmount = 1;
        chargeBarFill.fillAmount = 0;
    }

}
