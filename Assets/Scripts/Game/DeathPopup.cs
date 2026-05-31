using AudioController;
using GameSystem;
using UnityEngine;
using UnityEngine.UI;

public class DeathPopup : Singleton<DeathPopup>
{
    [SerializeField] private CanvasGroup cg;
    [SerializeField] private Button menuButton;
    [SerializeField] private AudioClip deathSound;


    protected override void Awake()
    {
        base.Awake();
        Init();
    }

    public void Init()
    {
        cg.alpha = 0;
        cg.interactable = false;
        cg.blocksRaycasts = false;

        menuButton.onClick.AddListener(GoMenu);
    }

    public void Open()
    {
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        Audios.Instance.PlayEffect(deathSound);

        cg.alpha = 1;
        cg.interactable = true;
        cg.blocksRaycasts = true;
    }

    private void GoMenu()
    {
        //Time.timeScale = 1;
        menuButton.interactable = false;
        _ = SceneLoader.Instance.LoadMenu();
    }

    void OnDestroy()
    {
        Time.timeScale = 1;
    }


}
