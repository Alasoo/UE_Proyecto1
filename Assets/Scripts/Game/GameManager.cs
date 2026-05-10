using GameSystem;
using SaveSystem;
using UnityEngine;

public class GameManager : SingletonDontDestroy<GameManager>
{
    protected override void Awake()
    {
        base.Awake();
        //SaveLoadManager.LoadData();
    }

#if UNITY_EDITOR
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (GameInfo.playerSelected == null) return;
            //GameInfo.playerSelected.playerData.AddExperience(Random.Range(0, 50));
            //Debug.Log(GameInfo.playerSelected.playerData.experience);
            //SaveLoadManager<PlayerData>.SaveData(GameInfo.playerSelected.id, GameInfo.playerSelected.playerData);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            PlayerPrefs.DeleteAll();
            Debug.Log($"Borrado playerprefs");
        }
    }
#endif
}
