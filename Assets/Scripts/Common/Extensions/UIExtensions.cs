﻿using UnityEngine.Events;
using UnityEngine.UI;

namespace Common.Extensions
{
    public static class UIExtensions
    {
        public static void SetClickListener(this Button button, UnityAction callback)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(callback);
        }
    }
}