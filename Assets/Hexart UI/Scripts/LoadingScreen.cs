﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace VIC.Creator.UI
{
    public class LoadingScreen : MonoBehaviour
    {
        private static LoadingScreen instance = null;

        [Header("RESOURCES")]
        public CanvasGroup canvasAlpha;
        public Text status;
        public Text title;
        public Text titleDescription;
        public static string prefabName = "Stock_Style";

        [Header("RANDOM HINTS")]
        public Text hintsText;
        public List<string> HintList = new List<string>();
        public bool changeHintWithTimer;
        public float hintTimerValue = 5;
        // [Range(0.1f, 1)] public float hintFadingSpeed = 1;
        private float htvTimer;

        [Header("RANDOM IMAGES")]
        public Image imageObject;
        public Animator fadingAnimator;
        public List<Sprite> ImageList = new List<Sprite>();
        public bool changeImageWithTimer;
        public float imageTimerValue = 5;
        [Range(0.1f, 5)] public float imageFadingSpeed = 1;
        private float itvTimer;

        [Header("SETTINGS")]
        public bool enableTitleDesc = true;
        public bool enableRandomHints = true;
        public bool enableRandomImages = true;
        public string titleText;
        public string titleDescText;
        [Range(0.1f, 10)] public float fadingAnimationSpeed = 2.0f;

        private bool isHintAlphaZero;

        void Start()
        {
            // If this is disabled, then disable Title Description object
            if (enableTitleDesc == false)
            {
                titleDescription.enabled = false;
            }

            // If this is enabled, generate random hints
            if (enableRandomHints == true)
            {
                string hintChose = HintList[Random.Range(0, HintList.Count)];
                hintsText.text = hintChose;
            }

            // If this is enabled, generate random images
            if (enableRandomImages == true)
            {
                Sprite imageChose = ImageList[Random.Range(0, ImageList.Count)];
                imageObject.sprite = imageChose;
            }

            // Set up title text
            title.text = titleText;
            titleDescription.text = titleDescText;

            // Set up Random Image fading anim speed
            fadingAnimator.speed = imageFadingSpeed;
            // fadingAnimator.SetFloat("Fade Out", imageFadingSpeed);
        }
        // Scene loading process
        private AsyncOperation loadingProcess;

        // Load a new scene
        public static void LoadScene(string sceneName)
        {
            // If there isn't a LoadingScreen, then create a new one
            instance = Instantiate(Resources.Load<GameObject>(prefabName)).GetComponent<LoadingScreen>();
            // Don't destroy loading screen while it's loading
            DontDestroyOnLoad(instance.gameObject);

            // Enable loading screen
            instance.gameObject.SetActive(true);
            // Start loading between scenes
            instance.loadingProcess = SceneManager.LoadSceneAsync(sceneName);
            // Don't allow scene switching
            instance.loadingProcess.allowSceneActivation = false;
        }

        void Awake()
        {
            // Set loading screen invisible at first (panel alpha color)
            canvasAlpha.alpha = 0f;
        }

        void Update()
        {
            // Update loading status
            if (status != null)
            {
                status.text = Mathf.Round(loadingProcess.progress * 100f).ToString() + "%";
            }

            // If loading is complete
            if (loadingProcess.isDone)
            {
                // Fade out
                canvasAlpha.alpha -= fadingAnimationSpeed * Time.deltaTime;

                // If fade out is complete, then disable the object
                if (canvasAlpha.alpha <= 0)
                {
                    Destroy(gameObject);
                }
            }

            else // If loading proccess isn't completed
            {
                // Start Fade in
                canvasAlpha.alpha += fadingAnimationSpeed * Time.deltaTime;

                // If loading screen is visible
                if (canvasAlpha.alpha >= 1)
                {
                    // We're good to go. New scene is on! :)
                    loadingProcess.allowSceneActivation = true;
                }
            }


            // Check if random images are enabled
            if (enableRandomImages == true)
            {
                itvTimer += Time.deltaTime;

                if (itvTimer >= imageTimerValue && fadingAnimator.GetCurrentAnimatorStateInfo(0).IsName("Start"))
                {
                    fadingAnimator.Play("Fade In");
                }

                else if (itvTimer >= imageTimerValue && fadingAnimator.GetCurrentAnimatorStateInfo(0).IsName("Fade Out"))
                {
                    Sprite imageChose = ImageList[Random.Range(0, ImageList.Count)];
                    imageObject.sprite = imageChose;
                    itvTimer = 0.0f;
                }
            }

            // Check if random hints are enabled
            if (enableRandomHints == true)
            {
                htvTimer += Time.deltaTime;

                if (htvTimer >= hintTimerValue && isHintAlphaZero == false)
                {
                    string hintChose = HintList[Random.Range(0, HintList.Count)];
                    hintsText.text = hintChose;
                    htvTimer = 0.0f;
                }
            }
        }
    }
}