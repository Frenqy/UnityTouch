using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VIC.Creator.UI
{
    public class ListTabsManager : MonoBehaviour
    {
        [Header("PANEL LIST")]
        public List<GameObject> panels = new List<GameObject>();

        [Header("BUTTON LIST")]
        public List<GameObject> buttons = new List<GameObject>();

        [Header("Exit Button")]
        public GameObject exitBtn;

        public static ListTabsManager Instance;

        // [Header("PANEL ANIMS")]
        private string panelFadeIn = "MP Fade-in";
        private string panelFadeInHex = "MP Fade-in Hexagon";
        private string panelFadeOut = "MP Fade-out";

        // [Header("BUTTON ANIMS")]
        private string buttonFadeIn = "TPB Pressed";
        private string buttonFadeOut = "TPB Dissolve";

        private GameObject currentPanel;
        private GameObject nextPanel;

        private GameObject currentButton;

        [Header("SETTINGS")]
        public int currentPanelIndex = 0;
        private int currentButtonIndex = 0;


        private bool isHexagonAnimEnabled;

        private Animator currentPanelAnimator;
        private Animator nextPanelAnimator;

        private Animator currentButtonAnimator;

        private void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            currentPanel = panels[currentPanelIndex];
            currentPanelAnimator = currentPanel.GetComponent<Animator>();
            currentPanelAnimator.Play(panelFadeIn);

            currentButton = buttons[currentPanelIndex];
            currentButtonAnimator = currentButton.GetComponent<Animator>();
            currentButtonAnimator.Play(buttonFadeIn);
        }

        /// <summary>
        /// 输入值为新的页面编号
        /// </summary>
        /// <param name="newPanel"></param>
        public void PanelAnim(int newPanel)
        {
            if (isHexagonAnimEnabled == true)
            {
                if (newPanel != currentPanelIndex)
                {
                    currentPanel = panels[currentPanelIndex];

                    currentPanelIndex = newPanel;
                    nextPanel = panels[currentPanelIndex];

                    currentPanelAnimator = currentPanel.GetComponent<Animator>();
                    nextPanelAnimator = nextPanel.GetComponent<Animator>();

                    currentPanelAnimator.Play(panelFadeOut);
                    nextPanelAnimator.Play(panelFadeInHex);
                }
            }

            else
            {
                if (newPanel != currentPanelIndex)
                {
                    currentPanel = panels[currentPanelIndex];

                    currentPanelIndex = newPanel;
                    nextPanel = panels[currentPanelIndex];

                    currentPanelAnimator = currentPanel.GetComponent<Animator>();
                    nextPanelAnimator = nextPanel.GetComponent<Animator>();

                    currentPanelAnimator.Play(panelFadeOut);
                    nextPanelAnimator.Play(panelFadeIn);
                }
            }

            // 只有首页出现关闭按钮
            if (newPanel != 0)
                exitBtn.SetActive(false);
            else
                exitBtn.SetActive(true);

        }

        /// <summary>
        /// 新打开的页面是否播放六边形动画
        /// </summary>
        /// <param name="hexAnimEnabled"></param>
        public void HexagonAnim(bool hexAnimEnabled)
        {
            if (hexAnimEnabled == true)
            {
                isHexagonAnimEnabled = true;
            }

            else
            {
                isHexagonAnimEnabled = false;
            }
        }

        public void ButtonAnimIn(int newButton)
        {
            currentButton = buttons[newButton];
            currentButtonAnimator = currentButton.GetComponent<Animator>();
            currentButtonAnimator.Play(buttonFadeIn);
        }

        public void ButtonAnimOut(int newButton)
        {
            currentButton = buttons[newButton];
            currentButtonAnimator = currentButton.GetComponent<Animator>();
            currentButtonAnimator.Play(buttonFadeOut);
        }

        public void ModalAnim(bool isOn)
        {
            currentPanel = panels[currentPanelIndex];

            if (isOn == true)
            {
                currentPanelAnimator = currentPanel.GetComponent<Animator>();
                currentPanelAnimator.Play("MP Modal Out");
            }

            else
            {
                currentPanelAnimator = currentPanel.GetComponent<Animator>();
                currentPanelAnimator.Play("MP Modal In");
            }
        }
    }
}
