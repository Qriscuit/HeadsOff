using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MoreMountains.Feedbacks;
using UnityEngine.Events;

public class LogInMainMenu : MonoBehaviour
{
    public NM_GC _NMGC;

    string NetAddress = "localhost";
    
    // 1 : Ocean Factory
    // 2 : Sky Scraper
    // 3 : Junk Yard
    public string LevelName = "Ocean Factory";
    public bool LevelChosen = false;

    [Header("UI GameObjects")]
    public GameObject MainMenu;
    public GameObject LevelEnter;
    public GameObject SelectLevel;
    public GameObject LocalOrGC;

    [Header("ImageSets")]
    public GameObject GeneralImage;
    public GameObject OceanFactoryImages;
    public GameObject SkyscraperImages;
    public GameObject JunkYardImages;
    public GameObject LevelsButtons;
    public GameObject LanOrGCButtons;

    [Header("Feedbacks")]
    public MMFeedbacks UIFadeIn;
    public MMFeedbacks GameImagesPan;
    public MMFeedbacks OceanFactoryImagesPan;
    public MMFeedbacks SkyScraperImagesPan;
    public MMFeedbacks JunkYardImagesPan;
    public MMFeedbacks PlaySlide;
    public MMFeedbacks LevelSelection;
    public MMFeedbacks ServerSelection;

    public static LogInMainMenu Inst;
    public void Awake()
    {
        Inst = this;
    }

    private void Start()
    {
        StartCoroutine(PlayUIFadeIn());
    }

    IEnumerator PlayUIFadeIn()
    {
        yield return new WaitForSeconds(2);
        UIFadeIn.gameObject.SetActive(true);
        UIFadeIn?.PlayFeedbacks();
    }

    public void OnClick_PlayButton()
    {
        PlaySlide.PlayFeedbacks();
        //if (PlaySlide.Direction == MMFeedbacks.Directions.TopToBottom) 
        //else
        //{
        //    PlaySlide.Direction = MMFeedbacks.Directions.TopToBottom;
        //    PlaySlide.PlayFeedbacks();
        //}
    }
    
    public void OnClick_CreateServerButton()
    {
        //LevelsButtons.SetActive(true);
        //LanOrGCButtons.SetActive(false);
        LevelSelection.PlayFeedbacks();

        //if(LevelSelection.Direction == MMFeedbacks.Directions.)
    }

    public void OnClick_JoinServerButton()
    {
        //LanOrGCButtons.SetActive(true);
        //LevelsButtons.SetActive(false);
        ServerSelection.PlayFeedbacks();
    }

    public void OnClick_TurnOffLevelsGameObject()
    {
        if (LevelSelection.Direction == MMFeedbacks.Directions.BottomToTop) LevelsButtons.SetActive(false);
    }

    public void OnClick_TurnOffLanOrGCGameObject()
    {
        if (ServerSelection.Direction == MMFeedbacks.Directions.BottomToTop) LanOrGCButtons.SetActive(false);
    }

    public void OceanFactorySelected()
    {
        LevelName = "Ocean Factory";
        LevelChosen = true;
        StopPans();

        LevelSelection.PlayFeedbacks();
        ServerSelection.PlayFeedbacks();

        OceanFactoryImages.SetActive(true);
        OceanFactoryImagesPan.PlayFeedbacks();
    }

    public void SkyScraperSelected()
    {
        LevelName = "Skyscraper";
        LevelChosen = true;
        StopPans();

        LevelSelection.PlayFeedbacks();
        ServerSelection.PlayFeedbacks();

        SkyscraperImages.SetActive(true);
        SkyScraperImagesPan.PlayFeedbacks();
    }

    public void JunkyardSelected()
    {
        //LevelName = "Junk Yard";
        LevelName = "FINAL_EVENING"; //TODO
        LevelChosen = true;
        StopPans();

        LevelSelection.PlayFeedbacks();
        ServerSelection.PlayFeedbacks();

        JunkYardImages.SetActive(true);
        JunkYardImagesPan.PlayFeedbacks();
    }

    public void ConnectPlayerToLocalGame()
    {
        NetAddress = "localhost";
        _NMGC.LoadingScreenFeedBack.PlayFeedbacks();
        StartCoroutine(StartClientAsLoadingScreensAnimationEnds());
    }
    
    public void ConnectPlayerToCloudGame()
    {
        NetAddress = _NMGC.networkAddress;
        _NMGC.LoadingScreenFeedBack.PlayFeedbacks();
        StartCoroutine(StartClientAsLoadingScreensAnimationEnds());
    }

    IEnumerator StartClientAsLoadingScreensAnimationEnds()
    {
        yield return new WaitForSeconds(2f);
        _NMGC.StartClient(NetAddress);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    void StopPans()
    {
        GeneralImage.SetActive(false);
        OceanFactoryImages.SetActive(false);
        SkyscraperImages.SetActive(false);
        JunkYardImages.SetActive(false);

        GameImagesPan.StopFeedbacks();
        OceanFactoryImagesPan.StopFeedbacks();
        SkyScraperImagesPan.StopFeedbacks();
        JunkYardImagesPan.StopFeedbacks();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            MainMenu.SetActive(true);
            LevelEnter.SetActive(false);
            SelectLevel.SetActive(false);
            LocalOrGC.SetActive(false);

            PlaySlide.PlayFeedbacks();
        }
    }
}