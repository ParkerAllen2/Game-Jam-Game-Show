using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Stage1Controller : SceneController
{
    public DialogMessage startMessage;
    public DialogMessage[] endMessage;

    public DialogManager dialogManager;

    public GameObject enemy;
    public GameObject[] weapons;
    public Transform[] spawnLocations;

    public int enemyCounter;

    bool win;

    public void Start()
    {
        dialogManager.StartDialog(startMessage, "start");
        SoundController.sc.PlayMenuMusic();
        //StartSpawnEnemies(1);
    }

    public void StartSpawnEnemies(int num)
    {
        enemyCounter = num;
        SoundController.sc.PlayBattleMusic();
        SpawnWeapons(Random.Range(1, num + 2));
        StartCoroutine(SpawnEnemies(num));
    }

    public void KillEnemy()
    {
        enemyCounter--;
        if(enemyCounter <= 0)
        {
            EndStage(1);
        }
    }

    public void EndStage(int w)
    {
        win = w == 1;
        SoundController.sc.PlayMenuMusic();
        dialogManager.StartDialog(endMessage[w], "end");
    }

    public override void ReadAnswers(List<string> ans)
    {
        if (ans[0].Equals("start"))
        {
            int num = int.Parse(ans[ans.Count - 1]);
            StartSpawnEnemies(num);
        }
        else if(ans[ans.Count - 1].Equals("Yes"))
        {
            if (win)
            {
                SceneManager.LoadScene("Stage2");
            }
            else
            {
                SceneManager.LoadScene("Stage1.5");
            }
        }
        else
        {
            Application.Quit();
        }
    }

    void SpawnWeapons(int num)
    {
        for( int i = 0; i < num; i++)
        {
            int x = Random.Range(0, weapons.Length);
            Vector3 pos = Random.insideUnitCircle * 30 + (Vector2)transform.position;
            Instantiate(weapons[x], pos, Quaternion.identity);
        }
    }

    IEnumerator SpawnEnemies(int num)
    {
        for(int i = 0; i < num; i++)
        {
            int x = Random.Range(0, spawnLocations.Length);
            HealthController hc = Instantiate(enemy, spawnLocations[x].position, Quaternion.identity).GetComponent<HealthController>();
            hc.overlord = this;
            yield return new WaitForSeconds(Random.Range(.5f, 2));
        }
    }
}
