using System;
using System.Net.Security;
using UnityEngine;

namespace WallDefense
{
    public class SaveManager : MonoBehaviour
    {
        public WallManager wallManager;
        public DayNightManager dayNightManager;
        public SaveData saveData;
        public void Start()
        {
            saveData = new();
        }
        public void Save()
        {
            SaveWalls();
            SaveDayNightData();
            string saveJson = JsonUtility.ToJson(saveData);
            System.IO.File.WriteAllText(Application.persistentDataPath + "/SaveData.json", saveJson);
        }
        public void Load()
        {
            string filePath = System.IO.Path.Combine(Application.persistentDataPath, "SaveData.json");
            string data = System.IO.File.ReadAllText(filePath);
            saveData = JsonUtility.FromJson<SaveData>(data);
            LoadWalls();
            LoadDayNightData();
        }
        public void SaveWalls()
        {
            SaveMaria();
            SaveSina();
            SaveRose();
        }
        public void LoadWalls()
        {
            LoadMaria();
            LoadSina();
            LoadRose();
        }
        public void SaveMaria()
        {
            saveData.maria = new()
            {
                ownerSettlement = wallManager.maria.ownerSettlement,
                top = wallManager.maria.top,
                middle = wallManager.maria.middle,
                bottom = wallManager.maria.bottom
            };

        }
        public void SaveSina()
        {
            saveData.sina = new()
            {
                ownerSettlement = wallManager.sina.ownerSettlement,
                top = wallManager.sina.top,
                middle = wallManager.sina.middle,
                bottom = wallManager.sina.bottom
            };
        }
        public void SaveRose()
        {
            saveData.rose = new()
            {
                ownerSettlement = wallManager.rose.ownerSettlement,
                top = wallManager.rose.top,
                middle = wallManager.rose.middle,
                bottom = wallManager.rose.bottom
            };
        }
        public void LoadMaria() => wallManager.maria.LoadWallData(saveData.maria);
        public void LoadSina() => wallManager.sina.LoadWallData(saveData.sina);
        public void LoadRose() => wallManager.rose.LoadWallData(saveData.rose);
        public void SaveDayNightData()
        {
            saveData.dayNightData = new()
            {
                currentDay = dayNightManager.currentDay,
                currentHour = dayNightManager.currentHour
            };
        }
        public void LoadDayNightData()
        {
            dayNightManager.LoadData(saveData.dayNightData);
        }
    }
    [Serializable]
    public class SaveData
    {
        public WallData maria;
        public WallData sina;
        public WallData rose;
        public DayNightData dayNightData;
    }
    [Serializable]
    public class WallData
    {
        public int ownerSettlement;
        public WallSegment top;
        public WallSegment middle;
        public WallSegment bottom;
    }
    [Serializable]
    public class DayNightData
    {
        public int currentDay, currentHour;
    }
}
