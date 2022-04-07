using System;
using UnityEngine;

namespace ModIO.Implementation.API.Requests
{
    internal static class AddModMedia
    {
        // public struct ResponseSchema
        // {
        //     // (NOTE): mod.io returns a MessageObject as the schema.
        //     // This schema will only be used if the server schema changes or gets expanded on
        // }

        public static readonly RequestTemplate Template =
            new RequestTemplate { requireAuthToken = true, canCacheResponse = false,
                                  requestResponseType = WebRequestResponseType.Text,
                                  requestMethodType = WebRequestMethodType.POST };

        public static string URL(ModProfileDetails details, out WWWForm form)
        {
            form = new WWWForm();

            if(details.logo != null)
            {
                form.AddBinaryData("logo", details.logo.EncodeToPNG(), "logo.png", null);
            }
            // TODO @Steve add and check for gallery images as well

            return $"{Settings.server.serverURL}{@"/games/"}"
                   + $"{Settings.server.gameId}{@"/mods/"}{details.modId}?";
        }
    }
}
