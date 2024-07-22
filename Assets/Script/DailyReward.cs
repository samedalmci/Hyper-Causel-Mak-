using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DailyReward : MonoBehaviour
{
    public bool initialized;
    public long rewardGivingTimeTicks;
    public GameObject rewardMenu;
    public Text remaingingTimeText;


    public void InitializeDailyReward()
    {

        if (PlayerPrefs.HasKey("lastDailyReard"))
        {
            rewardGivingTimeTicks = long.Parse(PlayerPrefs.GetString("lastDailyReard")) + 864000000000;
            long currentTime = System.DateTime.Now.Ticks;
            if (currentTime >= rewardGivingTimeTicks)
            {
                GiveReward();   
            }

        }
        else 
        {
            GiveReward();
        
        }
        initialized = true;
    }

    public void GiveReward() 
    {
        LevelController.Current.GiveMoneyToPlayer(100);
        rewardMenu.SetActive(true);
        PlayerPrefs.SetString("lastDailyReard", System.DateTime.Now.Ticks.ToString());
        rewardGivingTimeTicks = long.Parse(PlayerPrefs.GetString("lastDailyReard")) + 864000000000;

    }
    // Update is called once per frame
    void Update()
    {
        if (initialized)
        {
            if (LevelController.Current.startMenu.activeInHierarchy)
            {
                long currentTime = System.DateTime.Now.Ticks;
                long remaingTime = rewardGivingTimeTicks - currentTime;
                if (remaingTime <= 0)
                {
                    GiveReward();
                }
                else 
                {
                    System.TimeSpan timeSpan = System.TimeSpan.FromTicks(remaingTime);
                    remaingingTimeText.text = string.Format("{0}:{1}:{2}",timeSpan.Hours.ToString("D2") , timeSpan.Minutes.ToString("D2"), timeSpan.Seconds.ToString("D2"));
                
                }

            }

        }

       
        
    }
    public void TapToReturnButton()
    {
        rewardMenu.SetActive(false);
    }
}
