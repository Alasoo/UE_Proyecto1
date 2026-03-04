

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SaveSystem.Player;
using UnityEngine;

namespace SaveSystem
{
    public static class SaveLoadManager<T>
    {
        public static void SaveData(string key, T data)
        {
            string dataString = JsonConvert.SerializeObject(data);
            PlayerPrefs.SetString(key, dataString);
            PlayerPrefs.Save();
        }

        public static (T data, bool success) LoadData(string key)
        {
            T data = default(T);
            if (!PlayerPrefs.HasKey(key)) return (data, false);
            try
            {
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                string dataText = PlayerPrefs.GetString(key, "");
                data = JsonConvert.DeserializeObject<T>(dataText, settings);
                return (data, true);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error: {e}");
            }
            return (data, false);
        }
    }
}