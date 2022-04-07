using System;
using System.Collections;
using ModIO;
using ModIO.Implementation;
using ModIOBrowser.Implementation;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ModIOBrowser
{
    /// <summary>
    ///the main interface for interacting with the Mod Browser UI
    /// </summary>
    public partial class Browser
    {
        [Header("Mod Details Panel")]
        [SerializeField] GameObject ModDetailsPanel;
        [SerializeField] RectTransform ModDetailsContentRect;
        [SerializeField] GameObject ModDetailsGalleryLoadingAnimation;
        [SerializeField] Image ModDetailsGalleryFailedToLoadIcon;
        [SerializeField] Image[] ModDetailsGalleryImage;
        [SerializeField] TMP_Text ModDetailsSubscribeButtonText;
        [SerializeField] TMP_Text ModDetailsName;
        [SerializeField] TMP_Text ModDetailsSummary;
        [SerializeField] TMP_Text ModDetailsDescription;
        [SerializeField] TMP_Text ModDetailsFileSize;
        [SerializeField] TMP_Text ModDetailsLastUpdated;
        [SerializeField] TMP_Text ModDetailsReleaseDate;
        [SerializeField] TMP_Text ModDetailsSubscribers;
        [SerializeField] TMP_Text ModDetailsCreatedBy;
        [SerializeField] TMP_Text ModDetailsUpVotes;
        [SerializeField] TMP_Text ModDetailsDownVotes;
        [SerializeField] GameObject ModDetailsGalleryNavBar;
        [SerializeField] Transform ModDetailsGalleryNavButtonParent;
        [SerializeField] GameObject ModDetailsGalleryNavButtonPrefab;
        [SerializeField] GameObject ModDetailsDownloadProgressDisplay;
        [SerializeField] Image ModDetailsDownloadProgressFill;
        [SerializeField] TMP_Text ModDetailsDownloadProgressRemaining;
        [SerializeField] TMP_Text ModDetailsDownloadProgressSpeed;
        [SerializeField] TMP_Text ModDetailsDownloadProgressCompleted;
        bool galleryImageInUse;
        Sprite[] ModDetailsGalleryImages;
        bool[] ModDetailsGalleryImagesFailedToLoad;
        int galleryPosition;
        float galleryTransitionTime = 0.3f;
        IEnumerator galleryTransition;
        ModProfile currentModProfileBeingViewed;
        IEnumerator downloadProgressUpdater;
        Action modDetailsOnCloseAction;
        
        // measuring the progress bar
        ModId detailsModIdOfLastProgressUpdate = new ModId(-1);
        float detailsProgressTimePassed = 0f;

#region Mod Details Panel

        internal void OpenModDetailsPanel(ModProfile profile, Action actionToInvokeWhenClosed)
        {
            modDetailsOnCloseAction = actionToInvokeWhenClosed;
            GoToPanel(ModDetailsPanel);
            defaultModDetailsSelection.Select();
            HydrateModDetailsPanel(profile);
        }

        public void CloseModDetailsPanel()
        {
            ModDetailsPanel.SetActive(false);
            modDetailsOnCloseAction?.Invoke();
        }
        
        internal void HydrateModDetailsPanel(ModProfile profile)
        {
            currentModProfileBeingViewed = profile;
            UpdateSubscribeButtonText();
            ModDetailsGalleryLoadingAnimation.SetActive(true);
            ModDetailsGalleryImage[0].color = Color.clear;
            ModDetailsGalleryImage[1].color = Color.clear;
            ModDetailsName.text = profile.name;
            ModDetailsDescription.text = profile.description;
            ModDetailsSummary.text = profile.summary;
            //ModDetailsFileSize.text = profile.stats.filesize; // TODO
            //ModDetailsLastUpdated.text = profile.stats.updated; // TODO
            //ModDetailsReleaseDate.text = profile.stats.released; // TODO
            ModDetailsCreatedBy.text = $"{profile.creatorUsername}";
            ModDetailsUpVotes.text = "+" + Utility.GenerateHumanReadableNumber(profile.stats.ratingsPositive);
            ModDetailsDownVotes.text = "-" + Utility.GenerateHumanReadableNumber(profile.stats.ratingsNegative);
            ModDetailsSubscribers.text = Utility.GenerateHumanReadableNumber(profile.stats.subscriberTotal);

            int position = 0;
            galleryPosition = 0;
            ModDetailsGalleryImages = new Sprite[profile.galleryImages_320x180.Length];
            ModDetailsGalleryImagesFailedToLoad = new bool[ModDetailsGalleryImages.Length];

            // If there are no gallery images, use the logo instead and hide navbar for gallery images
            if(ModDetailsGalleryImages.Length == 0)
            {
                // If no gallery images, hide gallery navbar and replace image with logo instead
                ModDetailsGalleryNavBar.SetActive(false);

                Action<ResultAnd<Texture2D>> onLogoDownloadcomplete = delegate(ResultAnd<Texture2D> r)
                {
                    if(r.result.Succeeded())
                    {
                        Sprite sp = Sprite.Create(r.value, new Rect(Vector2.zero, new Vector2(r.value.width, r.value.height)), Vector2.zero);
                        Image image = GetCurrentGalleryImageComponent();
                        image.sprite = sp;
                        image.color = Color.white;
                    }
                    else
                    {
                        Image image = GetCurrentGalleryImageComponent();
                        image.sprite = null;
                        image.color = colorScheme.GetSchemeColor(ColorSetterType.LightGrey3);
                    }
                };

                ModIOUnity.DownloadTexture(profile.logoImage_640x360, onLogoDownloadcomplete);
            }
            else
            {
                // download gallery images and setup navbar buttons
                
                if(profile.galleryImages_320x180.Length == 1)
                {
                    ModDetailsGalleryNavBar.SetActive(false);
                }
                else
                {
                    ModDetailsGalleryNavBar.SetActive(true);
                }

                ListItem.HideListItems<GalleryImageButtonListItem>();

                foreach(var downloadReference in profile.galleryImages_320x180)
                {
                    int thisPosition = position;
                    position++;

                    ListItem li = ListItem.GetListItem<GalleryImageButtonListItem>(ModDetailsGalleryNavButtonPrefab, ModDetailsGalleryNavButtonParent, colorScheme);

                    // setup the delegate for the button click
                    Action transitionGalleryImage = delegate { TransitionToDifferentGalleryImage(thisPosition); };

                    li.Setup(transitionGalleryImage);

                    // REVIEW @Jackson the most sensible use for this is to use a lambda, in which case
                    // we may want to re-think the design for how it is used
                    Action<ResultAnd<Texture2D>> imageDownloaded = r =>
                    {
                        if(r.result.Succeeded())
                        {
                            ModDetailsGalleryImages[thisPosition] = Sprite.Create(r.value, 
                                new Rect(Vector2.zero, new Vector2(r.value.width, r.value.height)), Vector2.zero);

                            if(thisPosition == galleryPosition)
                            {
                                ModDetailsGalleryFailedToLoadIcon.gameObject.SetActive(false);
                                ModDetailsGalleryLoadingAnimation.SetActive(false);
                                Image image = GetCurrentGalleryImageComponent();
                                image.sprite = ModDetailsGalleryImages[thisPosition];
                                image.color = Color.white;
                            }
                        }
                        else
                        {
                            ModDetailsGalleryImages[thisPosition] = null;
                            ModDetailsGalleryImagesFailedToLoad[thisPosition] = true;
                            
                            if(thisPosition == galleryPosition)
                            {
                                ModDetailsGalleryLoadingAnimation.SetActive(false);
                                ModDetailsGalleryFailedToLoadIcon.gameObject.SetActive(true);
                                Image image = GetCurrentGalleryImageComponent();
                                image.sprite = null;
                                image.color = colorScheme.GetSchemeColor(ColorSetterType.LightGrey3);
                            }
                        }
                    };

                    ModIOUnity.DownloadTexture(downloadReference, imageDownloaded);
                }

                LayoutRebuilder.ForceRebuildLayoutImmediate(ModDetailsGalleryNavButtonParent as RectTransform);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(ModDetailsName.transform.parent as RectTransform);
        }

        public void ModDetailsSubscribeButtonPress()
        {
            if(IsSubscribed(currentModProfileBeingViewed.id))
            {
                // This isnt actually subscribed to 'yet' but we make the UI toggle straight away
                ModDetailsSubscribeButtonText.text = "Subscribe";
                UnsubscribeFromModEvent(currentModProfileBeingViewed, UpdateSubscribeButtonText);
            }
            else
            {
                // This isnt actually unsubscribed 'yet' but we make the UI toggle straight away
                ModDetailsSubscribeButtonText.text = "Unsubscribe";
                SubscribeToModEvent(currentModProfileBeingViewed, UpdateSubscribeButtonText);
            }
        }

        public void ModDetailsRatePositiveButtonPress()
        {
            RateModEvent(currentModProfileBeingViewed.id, ModRating.Positive);
        }

        public void ModDetailsRateNegativeButtonPress()
        {
            RateModEvent(currentModProfileBeingViewed.id, ModRating.Negative);
        }

        public void ModDetailsReportButtonPress()
        {
            Selectable selectionOnClose = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();
            if (selectionOnClose == null)
            {
                selectionOnClose = defaultModDetailsSelection;
            }
            Instance.OpenReportPanel(currentModProfileBeingViewed, selectionOnClose);
        }

        void UpdateSubscribeButtonText()
        {
            if(IsSubscribed(currentModProfileBeingViewed.id))
            {
                ModDetailsSubscribeButtonText.text = "Unsubscribe";
            }
            ModDetailsSubscribeButtonText.text = "Subscribe";

            ModIOUnity.IsAuthenticated((r) =>
            {
                if(!r.Succeeded())
                {
                    ModDetailsSubscribeButtonText.text = "Log in to Subscribe";
                }
            });
        }

        /// <summary>
        /// This should get called frame by frame for an accurate progress estimate
        /// </summary>
        /// <param name="handle"></param>
        void UpdateModDetailsDownloadProgress(ProgressHandle handle)
        {
            if( handle == null || handle.modId != currentModProfileBeingViewed.id || handle.Completed)
            {
                ModDetailsDownloadProgressDisplay.SetActive(false);
                return;
            }

            if (!ModDetailsDownloadProgressDisplay.activeSelf)
            {
                ModDetailsDownloadProgressDisplay.SetActive(true);
            }
            if(detailsModIdOfLastProgressUpdate != handle.modId)
            {
                detailsModIdOfLastProgressUpdate = handle.modId;
            }
            
            float timeRemainingInSeconds = (detailsProgressTimePassed / handle.Progress) - detailsProgressTimePassed;
            ModDetailsDownloadProgressFill.fillAmount = handle.Progress;
            ModDetailsDownloadProgressRemaining.text = $"{Utility.GenerateHumanReadableTimeStringFromSeconds((int)timeRemainingInSeconds)} remaining";
        
            detailsProgressTimePassed += Time.deltaTime;
            
            // TODO total completed
            // TODO download speed
        }

        internal void ShowNextGalleryImage()
        {
            int index = Utility.GetNextIndex(galleryPosition, ModDetailsGalleryImages.Length);
            TransitionToDifferentGalleryImage(index);
        }

        internal void ShowPreviousGalleryImage()
        {
            int index = Utility.GetPreviousIndex(galleryPosition, ModDetailsGalleryImages.Length);
            TransitionToDifferentGalleryImage(index);
        }

        void TransitionToDifferentGalleryImage(int index)
        {
            if(galleryTransition != null)
            {
                StopCoroutine(galleryTransition);
            }
            galleryTransition = TransitionGalleryImage(index);
            StartCoroutine(galleryTransition);
        }

        IEnumerator TransitionGalleryImage(int index)
        {
            galleryPosition = index;
            if(ModDetailsGalleryImages.Length >= index)
            {
                // It's likely we haven't loaded the gallery images yet
                yield break;
            }

            Image next = GetNextGalleryImageComponent();
            Image current = GetCurrentGalleryImageComponent();

            if(current.sprite == ModDetailsGalleryImages[index])
            {
                // Stop the transition, we are already showing the gallery image we want to transition to
                yield break;
            }

            galleryImageInUse = !galleryImageInUse;

            next.sprite = ModDetailsGalleryImages[index];
            if(next.sprite == null)
            {
                ModDetailsGalleryFailedToLoadIcon.gameObject.SetActive(true);
                next.color = colorScheme.GetSchemeColor(ColorSetterType.LightGrey3);
            }
            else
            {
                ModDetailsGalleryFailedToLoadIcon.gameObject.SetActive(false);
                next.color = Color.white;
            }

            float time;
            float timePassed = 0f;
            Color colIn = next.color;
            Color colFailedIcon = ModDetailsGalleryFailedToLoadIcon.color;
            Color colOut = current.color;
            colIn.a = 0f;
            colFailedIcon.a = 0f;

            while(timePassed <= galleryTransitionTime)
            {
                time = timePassed / galleryTransitionTime;

                colIn.a = time;
                colFailedIcon.a = time;
                colOut.a = 1f - time;

                next.color = colIn;
                ModDetailsGalleryFailedToLoadIcon.color = colFailedIcon;
                current.color = colOut;

                yield return null;
                timePassed += Time.deltaTime;
            }
        }

        Image GetCurrentGalleryImageComponent()
        {
            int current = galleryImageInUse ? 0 : 1;
            return ModDetailsGalleryImage[current];
        }

        Image GetNextGalleryImageComponent()
        {
            int next = galleryImageInUse ? 1 : 0;
            return ModDetailsGalleryImage[next];
        }

#endregion

    }
}
