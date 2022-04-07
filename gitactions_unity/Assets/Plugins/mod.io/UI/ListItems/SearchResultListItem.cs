﻿using System;
using ModIO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using ModIO.Implementation;

namespace ModIOBrowser.Implementation
{
    /// <summary>
    /// This is used for the SearchResultListItem prefab for the home view rows, such as
    /// recently added, highest rated, etc
    /// </summary>
    /// <remarks>
    /// This is nearly identical to BrowserModListItem.cs due to the potential of any future
    /// design changes if we ever want them to be more distinguished.
    /// </remarks>
    internal class SearchResultListItem : ListItem, IDeselectHandler, ISelectHandler, IPointerEnterHandler
    {
        public Image image;
        public TMP_Text title;
        public GameObject loadingIcon;
        public GameObject failedToLoadIcon;
        public Action imageLoaded;
        public ModProfile profile;

        // TODO This may need to be implemented with mouse & keyboard support
        public void OpenModDetailsForThisProfile()
        {
            if(isPlaceholder)
            {
                return;
            }
            Browser.Instance.OpenModDetailsPanel(profile, Browser.Instance.OpenSearchResultsWithoutRefreshing);
        }

#region MonoBehaviour
        public void OnSelect(BaseEventData eventData)
        {
            SelectionOverlayHandler.Instance.MoveSelection(this);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            SelectionOverlayHandler.Instance.Deselect(this);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            // When using mouse we want to disable the viewport restraint from moving the screen
            ViewportRestraint.temporarilyDisableViewportRestraint = true;
            selectable.Select();
        }
#endregion // MonoBehaviour

#region Overrides
        public override void PlaceholderSetup()
        {
            base.PlaceholderSetup();
            image.color = Color.clear;
            loadingIcon.SetActive(true);
            failedToLoadIcon.SetActive(false);
            title.text = string.Empty;
            //downloads.text = string.Empty;
        }

        public override void Setup(ModProfile profile)
        {
            base.Setup();
            this.profile = profile;
            image.color = Color.clear;
            loadingIcon.SetActive(true);
            failedToLoadIcon.SetActive(false);
            title.text = profile.name;
            //downloads.text = GenerateHumanReadableString(profile.stats.downloadsTotal);
            ModIOUnity.DownloadTexture(profile.logoImage_320x180, SetIcon);
            gameObject.SetActive(true);
        }

        public override void SetViewportRestraint(RectTransform content, RectTransform viewport)
        {
            base.SetViewportRestraint(content, viewport);
            viewportRestraint.Top = 376;
            viewportRestraint.Bottom = 124;
        }
#endregion // Overrides
        public void SetAsLastRowItem()
        {
            viewportRestraint.Top = 100;
            viewportRestraint.Bottom = 540;
        }

        void SetIcon(ResultAnd<Texture2D> textureAnd)
        {
            if(textureAnd.result.Succeeded() && textureAnd != null)
            {
                image.sprite = Sprite.Create(textureAnd.value, 
                    new Rect(Vector2.zero, new Vector2(textureAnd.value.width, textureAnd.value.height)), Vector2.zero);
                image.color = Color.white;
                loadingIcon.SetActive(false);
            }
            else
            {
                failedToLoadIcon.SetActive(true);
                loadingIcon.SetActive(false);
            }
            imageLoaded?.Invoke();
        }
    }
}
