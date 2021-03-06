using System.Collections.Generic;
using ModIO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ModIOBrowser.Implementation
{
    internal class SearchResultListItem_Overlay : MonoBehaviour
    {
        [SerializeField] Animator animator;
        [SerializeField] Image image;
        [SerializeField] GameObject failedToLoadIcon;
        [SerializeField] GameObject loadingIcon;
        [SerializeField] TMP_Text title;
        [SerializeField] TMP_Text downloads;
        [SerializeField] TMP_Text subscribeButtonText;
        [SerializeField] Transform contextMenuPosition;

        public SearchResultListItem listItemToReplicate;
        public SearchResultListItem lastListItemToReplicate;

        // Mimics the look of a SearchResultListItem
        public void Setup(SearchResultListItem listItem)
        {
            // If we are already displaying this list item in the overlay, bail
            if(listItem == listItemToReplicate && gameObject.activeSelf)
            {
                return;
            }
            lastListItemToReplicate = listItemToReplicate;
            listItemToReplicate = listItem;

            Transform t = transform;
            t.SetParent(listItem.transform.parent);
            t.SetAsLastSibling();
            t.position = listItem.transform.position;
            gameObject.SetActive(true);
            failedToLoadIcon.SetActive(listItemToReplicate.failedToLoadIcon.activeSelf);
            loadingIcon.SetActive(listItemToReplicate.loadingIcon.activeSelf);
            animator.Play("Inflate");
            
            // Set if the list item is still waiting for the image to download. The action will
            // get invoked when the download finishes.
            listItemToReplicate.imageLoaded = ReloadImage;
            
            image.sprite = listItemToReplicate.image.sprite;
            title.text = listItemToReplicate.title.text;
            // downloads.text = listItemToReplicate.downloads.text; // TODO implement
            // TODO subscribed or not check to update button text (Subscribe/Unsubscribe)
        }

        public void SubscribeButton()
        {
            if(Browser.IsSubscribed(listItemToReplicate.profile.id))
            {
                // We are pre-emptively changing the text here to make the UI feel more responsive
                subscribeButtonText.text = "Subscribe";
                Browser.UnsubscribeFromModEvent(listItemToReplicate.profile, UpdateSubscribeButton);
            }
            else
            {
                // We are pre-emptively changing the text here to make the UI feel more responsive
                subscribeButtonText.text = "Unsubscribe";
                Browser.SubscribeToModEvent(listItemToReplicate.profile, UpdateSubscribeButton);
            }
        }

        public void OpenModDetailsForThisModProfile()
        {
            listItemToReplicate.OpenModDetailsForThisProfile();
        }
        
        public void ShowMoreOptions()
        {
            List<ContextMenuOption> options = new List<ContextMenuOption>();

            //TODO If not subscribed add force uninstall and subscribe options 

            // Add Vote up option to context menu
            options.Add(new ContextMenuOption
            {
                name = "Vote up",
                action = delegate
                {
                    ModIOUnity.RateMod(listItemToReplicate.profile.id, ModRating.Positive, delegate { });
                    Browser.Instance.CloseContextMenu();
                }
            });

            // Add Vote up option to context menu
            options.Add(new ContextMenuOption
            {
                name = "Vote down",
                action = delegate
                {
                    ModIOUnity.RateMod(listItemToReplicate.profile.id, ModRating.Negative, delegate { });
                    Browser.Instance.CloseContextMenu();
                }
            });

            // Add Report option to context menu
            options.Add(new ContextMenuOption
            {
                name = "Report",
                action = delegate
                {
                    // TODO open report menu
                    Browser.Instance.CloseContextMenu();
                    Browser.Instance.OpenReportPanel(listItemToReplicate.profile, listItemToReplicate.selectable);
                }
            });

            // Open context menu
            Browser.Instance.OpenContextMenu(contextMenuPosition, options, listItemToReplicate.selectable);
        }

        public void UpdateSubscribeButton()
        {
            SetSubscribeButtonText();
        }

        public void SetSubscribeButtonText()
        {
            if(Browser.IsSubscribed(listItemToReplicate.profile.id))
            {
                subscribeButtonText.text = "Unsubscribe";
            } 
            else 
            {
                subscribeButtonText.text = "Subscribe";
            }
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        void ReloadImage()
        {
            image.sprite = listItemToReplicate.image.sprite;
            failedToLoadIcon.SetActive(listItemToReplicate.failedToLoadIcon.activeSelf);
            loadingIcon.SetActive(listItemToReplicate.loadingIcon.activeSelf);
        }
    }
}
