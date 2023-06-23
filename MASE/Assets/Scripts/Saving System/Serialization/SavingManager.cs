using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;
using UnityEngine;

public class SavingManager : MonoBehaviour
{
    #region Simulation Saving methods
    public static void SaveNewSim(SaveSimulationData saveData)
    {
        int save_no = -1;
        string[] linesread = ReadSimLines().ToArray();
        foreach (string line in linesread)
        {
            SaveSimulationData save = JsonUtility.FromJson<SaveSimulationData>(line);
            save_no = save.savenumber;
        }
        save_no += 1;
        saveData.savenumber = save_no;
        string json = JsonUtility.ToJson(saveData);
        json += "\n";
        try
        {
            File.AppendAllText(Application.dataPath + "/Saves/Simulation_Saves/SimSaves.txt", json);
        }
        catch (Exception)
        {

            throw;
        }
    }

    public static void UpdateSim(SaveSimulationData updateddata, int save_no)
    {
        string[] linesread = File.ReadAllLines(Application.dataPath + "/Saves/Simulation_Saves/SimSaves.txt");
        string json = JsonUtility.ToJson(updateddata);
        linesread[save_no] = json;
        try
        {
            File.WriteAllLines(Application.dataPath + "/Saves/Simulation_Saves/SimSaves.txt", linesread);
        }
        catch (Exception)
        {

            throw;
        }
    }

    public static SaveSimulationData LoadSim(int save_no)
    {
        SaveSimulationData foundsave = new SaveSimulationData();
        string[] linesread = ReadSimLines();
        for (int i = 0; i < linesread.Length; i++)
        {
            SaveSimulationData save = JsonUtility.FromJson<SaveSimulationData>(linesread[i]);
            if (save.savenumber == save_no)
            {
                foundsave = save;
                break;
            }
        }
        return foundsave;
    }

    public static string[] ReadSimLines()
    {
        string[] linesread = File.ReadLines(Application.dataPath + "/Saves/Simulation_Saves/SimSaves.txt").ToArray();
        return linesread;
    }

    public static SaveSimulationData[] LoadAllSimData()
    {
        string[] linesread = ReadSimLines();
        SaveSimulationData[] data = new SaveSimulationData[linesread.Length];
        for (int i = 0; i < linesread.Length; i++)
        {
            data[i] = JsonUtility.FromJson<SaveSimulationData>(linesread[i]);
        }

        return data;
    }

    public static void DeleteSimSave(int saveno)
    {
        string[] linesread = File.ReadAllLines(Application.dataPath + "/Saves/Simulation_Saves/SimSaves.txt");
        SaveSimulationData save;
        string[] updatedlines = new string[linesread.Length - 1];
        int count = 0;
        for (int i = 0; i < linesread.Length; i++)
        {
            save = JsonUtility.FromJson<SaveSimulationData>(linesread[i]);
            if (save.savenumber != saveno)
            {
                save.savenumber = count;
                linesread[i] = JsonUtility.ToJson(save);
                updatedlines[count] = linesread[i];
                count++;
            }
        }
        try
        {
            File.WriteAllLines(Application.dataPath + "/Saves/Simulation_Saves/SimSaves.txt", updatedlines);
        }
        catch (Exception)
        {

            throw;
        }
    }

    public static void SaveBestCreature(BestCreature creature)
    {
        string json = JsonUtility.ToJson(creature);
        json += "\n";
        try
        {
            File.AppendAllText(Application.dataPath + "/Saves/BestCreatures/best_creatures.txt", json);
        }
        catch (Exception)
        {

            throw;
        }
    }
    #endregion

    #region Best creature saving methods
    public static BestCreature GetBestCreature(int save_no)
    {
        BestCreature foundsave = new BestCreature();
        string[] linesread = ReadBestCreatureLines();
        for (int i = 0; i < linesread.Length; i++)
        {
            BestCreature save = JsonUtility.FromJson<BestCreature>(linesread[i]);
            if (save.save_no == save_no)
            {
                foundsave = save;
                return foundsave;
            }
        }
        return null;
    }

    public static void UpdateBestCreature(BestCreature updateddata, int save_no)
    {
        string[] linesread = File.ReadAllLines(Application.dataPath + "/Saves/BestCreatures/best_creatures.txt");
        string json = JsonUtility.ToJson(updateddata);

        for (int i = 0; i < linesread.Length; i++)
        {
            BestCreature save = JsonUtility.FromJson<BestCreature>(linesread[i]);
            if (save.save_no == save_no)
            {
                linesread[i] = json;
            }
        }
        try
        {
            File.WriteAllLines(Application.dataPath + "/Saves/BestCreatures/best_creatures.txt", linesread);
        }
        catch (Exception)
        {

            throw;
        }
    }

    public static string[] ReadBestCreatureLines()
    {
        string[] linesread = File.ReadLines(Application.dataPath + "/Saves/BestCreatures/best_creatures.txt").ToArray();
        return linesread;
    }
    #endregion
}
