using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif


namespace LayerLab.SuperCasual
{
    public class PanelControlSuperCasual : MonoBehaviour
    {
        private int page = 0;
        private bool isReady = false;
        [SerializeField] private List<GameObject> panels = new List<GameObject>();
        private TextMeshProUGUI textTitle;
        [SerializeField] private Transform panelTransform;
        [SerializeField] private Button buttonPrev;
        [SerializeField] private Button buttonNext;

        private void Start()
        {
            textTitle = transform.GetComponentInChildren<TextMeshProUGUI>();
            buttonPrev.onClick.AddListener(Click_Prev);
            buttonNext.onClick.AddListener(Click_Next);

            foreach (Transform t in panelTransform)
            {
                panels.Add(t.gameObject);
                t.gameObject.SetActive(false);
            }

            panels[page].SetActive(true);
            isReady = true;

            CheckControl();
        }

        void Update()
        {
            if (panels.Count <= 0 || !isReady) return;

            
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
            {
                Click_Prev();
            }
            else if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
            {
                Click_Next();
            }
#else
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                Click_Prev();
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                Click_Next();
            }
#endif
        }

        //Click_Prev
        public void Click_Prev()
        {

            if (page <= 0 || !isReady) return;

            panels[page].SetActive(false);
            panels[page -= 1].SetActive(true);
            textTitle.text = panels[page].name;
            CheckControl();
        }

        //Click_Next
        public void Click_Next()
        {
            if (page >= panels.Count - 1) return;

            panels[page].SetActive(false);
            panels[page += 1].SetActive(true);
            CheckControl();
        }

        void SetArrowActive()
        {
            buttonPrev.gameObject.SetActive(page > 0);
            buttonNext.gameObject.SetActive(page < panels.Count - 1);
        }

        //SetTitle, SetArrow Active
        private void CheckControl()
        {
            textTitle.text = panels[page].name.Replace("_", " ");
            SetArrowActive();
        }
    }
}
