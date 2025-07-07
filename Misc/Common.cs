using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

namespace VirtueSky.Misc {
    public static partial class Common {
        public static string Format(this string fmt, params object[] args) =>
            string.Format(System.Globalization.CultureInfo.InvariantCulture.NumberFormat, fmt, args);

        public static bool IsInteger(this float value) {
            return (value == (int)value);
        }

        public static int GetNumberInAString(this string str) {
            try {
                var getNumb = Regex.Match(str, @"\d+").Value;
                return Int32.Parse(getNumb);
            }
            catch (Exception e) {
                return -1;
            }

            return -1;
        }

        public static float GetScreenRatio() {
            return (1920f / 1080f) / (Screen.height / (float)Screen.width);
        }

        public static void CallActionAndClean(ref Action action) {
            if (action == null) return;
            var a = action;
            a();
            action = null;
        }

        #region Internet Connection

        private static IEnumerator internetConnectionCoroutine;

        public static void StopCheckInternetConnection(this MonoBehaviour mono) {
            mono.StopCoroutine(internetConnectionCoroutine);
        }

        public static void CheckInternetConnection(this MonoBehaviour mono, Action actionConnected,
            Action actionDisconnected) {
            if (internetConnectionCoroutine != null) mono.StopCoroutine(internetConnectionCoroutine);
            internetConnectionCoroutine = InternetConnection((isConnected) => {
                if (isConnected) {
                    actionConnected?.Invoke();
                }
                else {
                    actionDisconnected?.Invoke();
                }
            });
            mono.StartCoroutine(internetConnectionCoroutine);
        }

        public static IEnumerator InternetConnection(Action<bool> action) {
            bool result;
            string url = "http://google.com";
#if UNITY_ANDROID
            url = "http://google.com";
#elif UNITY_IOS
            url = "https://captive.apple.com/hotspot-detect.html";
#endif

            using (UnityWebRequest request = UnityWebRequest.Head(url)) {
                yield return request.SendWebRequest();
                result = !request.isNetworkError && !request.isHttpError && request.responseCode == 200 &&
                         request.error == null;
            }

            action(result);
            internetConnectionCoroutine = null;
        }

        #endregion
    }
}