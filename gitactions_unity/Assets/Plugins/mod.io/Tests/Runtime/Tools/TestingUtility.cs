using ModIO;
using ModIO.Implementation;
using ModIO.Implementation.API.Objects;
using ModIO.Implementation.Platform;
using System.Threading.Tasks;

namespace ModIOTesting
{
    internal static class TestingUtility
    {
        //(NOTE): This is valid until August 16, 2022
        // TODO WEB Setup a permanent solution for test tokens on different servers
        public static readonly AccessTokenObject token = new AccessTokenObject() {
            code = 200, date_expires = 1660695215,
            access_token =
                "eyJlbmMiOiJBMTI4Q0JDLUhTMjU2IiwiYWxnIjoiUlNBLU9BRVAiLCJjdHkiOiJKV1QiLCJ6aXAiOiJERUYiLCJ4NXQiOiJ5OFFfSTlwNl9QRkJOd2ZiRDJVRkFhV1VoLU0ifQ.Q7oTqIm4H8k6vpYB6_9trLsodljyXAXnGa6ZYA78UUHBATOdwBEMqEmmMr9p94pZEo4zbJc87r0WB-m6qQjruIcLrX3vMMU3s6RG6nBCHJCBtj4-SPHLyB6RhIUWImUaapT0pXxFjoJJZUVWFTJeoE7l69bgfDHwigRWlFlaMi3oXk4FQp-SE2JW2NHGxiXFMi-5TfU-JBa9HgTIURiXVQPiMl__zLmm_DJpwJcr9e4VI73H-Q15KIO-P39G_eRSlpKDawG4eLWfIoP-UnOQ9oeGrd7-TTYCjNXcODNuXVWM4IKG0yzHRtQJGyel2kyr6a2IO80vc2SLA4kO1uyGyw.GS5nnPfhSfPOWtkWNJx-Ow.XF0_KzgozwgaEhlyJDIjDeXmm0zAgq4ItxbJ6ux95AU_4qrYXiPb9bEbEYwcI9p17JK9FzVPO5bCUGBitWkrLkI7qN8sn5-m2VK8FvDr5MduH98e1tHVLXrIM_u7HUW-4QC8FRBbDKfv6GD2ueCUPQFIKGF52DtS2wzE3XORuMzgwZfCUR8twlnBgNI0RL0RJ5qQpoGlCMOJW0x1LLvzvztHqmkI7Oj5cVytdi9EzgffxRGZAJJ5JuZm77EkWJrOpNbIDw4jphZ5K1Fcn98jGh_YzQ2rqrujlf-mowsiBIGo6ZOybCLyd0QufrRm1z7ZFnrD0GquiPK2bDwjSgTFQaeWmk9T3GsHObRTPA-rWvv2NNZutN0BSTNDodG0iIwE1uKF_SWX4U-9isTnOPAZveMIupjZdOU9FOjCKplkO4hbKssnC-8XRi8LadxZSkw6s8e6LwDGpJ0IkAIaI-I-8zYK3FLt8nPNC7E84iwLzgwHV5i-EFUJwxZ_GrgfHeX7Py-09rOr-VQdOicA77YS0v60S9nBIqERel-ipiiUbxvUuofdpk_bcI0EHHYFSZHnVAYN0Ep9qdYw73-rnppGLKvytnLzYlAvGe6Ip7lTA5dpOPizuRwC1lFULqsovWnNhuzsbIl3WrKAkrxjjqG0qWXDJ8SIr1Q94q-zpP1hZIjDmnCZEwJj7IpXZaiC7h-IdbPtOh5rbBjx3c6FMumCc2NwagI6nQndpyM-pc9nMaqGeVjzV7vAOvZdLNCKq7FBdLZ3Raym2F2vex-GSdHrp3XF0AGstgl_yASX2JIaFoD6wsV9WhpNE_r6Q5-hAl8JtmBL94evQfo5HYxlX8Ix9NVcDYf8aUzCaTFhxr42Prpq5uBMYST4X2UngwRJkzr22x7sEtdyu8N6AHzkFY104_nIEuFy1t4KsP9xiQrXBPpjpAhPLEL0VnduzAe5DsoME-YeACz98limnEckt_iGqoN57pNQZdtZRIvWs4hNgGBBKJX4YN9Iko7Ib6_lCmlYIzvUI9pSy2EvRne3gxZ5OThMjjggZua-L4-GqyrTs1k.DpEgsqE8eNQOkkH1Wo90Zg"
        };

        internal static void SetConfigManuallyToTestServerForGETRequests()
        {
            // TODO edit the config file manually so we can test with both initialize overloads
        }
        internal static void SetConfigManuallyToTestServerForPOSTRequests()
        {
            // TODO edit the config file manually so we can test with both initialize overloads
        }

        internal static void SetConfigManuallyToProductionServer()
        {
            // TODO edit the config file manually so we can test with both initialize overloads
        }

        internal static ServerSettings GetTestSettingsForWritingRequests()
        {
            ServerSettings serverSettings = new ServerSettings();

            // TODO Setup a dedicated test game profile for POST/PUT/DELETE Unit testing
            serverSettings.gameId = 8;
            serverSettings.gameKey = "7e5d288f838740e089b6d8076f68f8d6";
            serverSettings.serverURL = "https://api.test.mod.io/v1";
            serverSettings.languageCode = "en";

            return serverSettings;
        }

        internal static ServerSettings GetTestSettingsForReadingRequests()
        {
            ServerSettings serverSettings = new ServerSettings();

            // TODO Setup a dedicated test game profile for GET Unit testing
            serverSettings.gameId = 8;
            serverSettings.gameKey = "7e5d288f838740e089b6d8076f68f8d6";
            serverSettings.serverURL = "https://api.test.mod.io/v1";
            serverSettings.languageCode = "en";

            return serverSettings;
        }

        // (NOTE): TODO Ensure a method that can do this with as 1:1 use of the implementation as
        // possible
        internal static async Task SetupUserCredentials()
        {
            // TODO WEB Setup a permanent solution for test user on different servers/doesnt expire?
            await UserData.instance.SetOAuthToken(token);
            UserData.instance.userObject = new UserObject();
            UserData.instance.userObject.username = "stephenluc1624927318";
            UserData.instance.userObject.id = 66360;
            ModCollectionManager.AddUserToRegistry(UserData.instance.userObject);
        }

        /// <summary>
        /// use this to make the the current authenticated session invalid
        /// </summary>
        internal static void InvalidateCredentials()
        {
            UserData.instance.oAuthToken = "invalid";
            UserData.instance.oAuthTokenWasRejected = true;
        }
        /// <summary>
        /// use this to set the user as unauthenticated, a guest
        /// </summary>
        internal static void RemoveCredentials()
        {
            UserData.instance.oAuthToken = string.Empty;
            UserData.instance.oAuthTokenWasRejected = false;
        }

        internal static void ClearAllData()
        {
#if UNITY_EDITOR
            SystemIOWrapper.DeleteDirectory(EditorDataService.GlobalRootDirectory);
#elif UNITY_STANDALONE
            throw new System.NotImplementedException();
#elif UNITY_SWITCH
            throw new System.NotImplementedException();
#endif
        }
    }
}
