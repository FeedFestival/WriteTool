﻿using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

namespace Assets.Scripts.Utils
{
    public static class DataUtils
    {
        public static string GetDataValue(string data, string index)
        {
            string value = data.Substring(data.IndexOf(index, System.StringComparison.Ordinal) + index.Length);
            if (value.Contains("|"))
                value = value.Remove(value.IndexOf('|'));
            return value;
        }
        public static int GetIntDataValue(string data, string index)
        {
            int numb;
            var success = int.TryParse(GetDataValue(data, index), out numb);

            return success ? numb : 0;
        }
        public static bool GetBoolDataValue(string data, string index)
        {
            var value = GetDataValue(data, index);
            if (string.IsNullOrEmpty(value) || value.Equals("0"))
                return false;
            return true;
        }
        public static long GetLongDataValue(string data, string index)
        {
            long numb;
            var success = long.TryParse(GetDataValue(data, index), out numb);

            return success ? numb : 0;
        }
        public static long GetLongDataValue(string data)
        {
            long numb;
            var success = long.TryParse(data, out numb);

            return success ? numb : 0;
        }
        public static string EncodeTextInBytes(string text)
        {
            byte[] bytes = System.Text.Encoding.Unicode.GetBytes(text);
            var textBytes = string.Empty;
            for (var i = 0; i < bytes.Length; i++)
            {
                textBytes += bytes[i].ToString() + ',';
            }
            return textBytes;
        }

        public static string AddPathsToText(string[] paths)
        {
            var fullText = string.Empty;
            int count = 0;
            foreach (string path in paths)
            {
                fullText += path;
                count++;
                if (count != paths.Length)
                {
                    fullText += ";";
                }
            }
            return fullText;
        }

        public static string[] GetPathsFromText(string fullText)
        {
            var paths = new string[2];
            var splitString = fullText.Split(';');
            paths[0] = splitString[0];
            paths[1] = splitString[1];
            return paths;
        }

        public static string DecodeTextFromBytes(string text)
        {
            int count = text.Split(',').Length - 1;
            byte[] bytes = new byte[count];

            for (var i = 0; i < count; i++)
            {
                var index = text.IndexOf(',');
                var numberString = text.Substring(0, index);

                var number = Convert.ToInt32(numberString);
                byte bit = Convert.ToByte(number);

                bytes[i] = bit;

                int toRemove = (index + 1);
                text = text.Substring(toRemove, text.Length - toRemove);
            }

            return System.Text.Encoding.Unicode.GetString(bytes);
        }
    }

    public static class UIX
    {
        /// <summary>
        /// Forces the layout of a UI GameObject and all of it's children to update
        /// their positions and sizes.
        /// </summary>
        /// <param name="xform">
        /// The parent transform of the UI GameObject to update the layout of.
        /// </param>
        public static void UpdateLayout(Transform xform)
        {
            Canvas.ForceUpdateCanvases();
            UpdateLayout_Internal(xform);
        }

        private static void UpdateLayout_Internal(Transform xform)
        {
            if (xform == null || xform.Equals(null))
            {
                return;
            }

            // Update children first
            for (int x = 0; x < xform.childCount; ++x)
            {
                UpdateLayout_Internal(xform.GetChild(x));
            }

            // Update any components that might resize UI elements
            foreach (var layout in xform.GetComponents<LayoutGroup>())
            {
                layout.CalculateLayoutInputVertical();
                layout.CalculateLayoutInputHorizontal();
            }
            foreach (var fitter in xform.GetComponents<ContentSizeFitter>())
            {
                fitter.SetLayoutVertical();
                fitter.SetLayoutHorizontal();
            }
        }
    }
}