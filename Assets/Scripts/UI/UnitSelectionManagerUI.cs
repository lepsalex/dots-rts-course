using System;
using MonoBehaviours;
using UnityEngine;

namespace UI
{
    public class UnitSelectionManagerUI : MonoBehaviour
    {
        [SerializeField]
        private RectTransform selectionAreaRectTransform;

        [SerializeField]
        private Canvas canvas;

        private void Start()
        {
            // register event handlers
            UnitSelectionManager.Instance.OnSelectionAreaStart += UnitSelectionManager_OnSelectionAreaStart;
            UnitSelectionManager.Instance.OnSelectionAreaEnd += UnitSelectionManager_OnSelectionAreaEnd;

            // hide on start
            selectionAreaRectTransform.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (selectionAreaRectTransform.gameObject.activeSelf)
            {
                UpdateVisual();
            }
        }

        private void UnitSelectionManager_OnSelectionAreaStart(object sender, System.EventArgs e)
        {
            selectionAreaRectTransform.gameObject.SetActive(true);

            UpdateVisual();
        }

        private void UnitSelectionManager_OnSelectionAreaEnd(object sender, System.EventArgs e)
        {
            selectionAreaRectTransform.gameObject.SetActive(false);
        }

        private void UpdateVisual()
        {
            var selectionAreaRect = UnitSelectionManager.Instance.GetSelectionAreaRect();

            // all three dimension scale equally so just grabbing x is fine
            var scale = canvas.transform.localScale.x;

            selectionAreaRectTransform.anchoredPosition = new Vector2(selectionAreaRect.x, selectionAreaRect.y) / scale;
            selectionAreaRectTransform.sizeDelta = new Vector2(selectionAreaRect.width, selectionAreaRect.height) / scale;
        }
    }
}
