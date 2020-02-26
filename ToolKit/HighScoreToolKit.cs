using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// High Score Tool Kit: Tools for Adding/Getting/Removing/Serializing high scores.
/// Meant to be used as static methods. If used as a GameObject script the debugging and examples will be run in the Start method.
/// </summary>
public class HighScoreToolKit: MonoBehaviour
{
    /// <summary>
    /// HighScore struct that carries all the data for serialization
    /// </summary>
    private struct HighScores<T> where T : BasePlayer
    {
        [SerializeReference]
        public List<T> players;
    }

    /// <summary>
    /// BasePlayer: All player highscores must inherit from this class.
    /// Extra data can be then added. Sorting requires "name" and "score".
    /// </summary>
    [Serializable]
    public abstract class BasePlayer
    {
        public string name;
        public int score;
    }

    /// <summary>
    /// Example of extending the BasePlayer to have more saved stats.
    /// </summary>
    [Serializable]
    private class Player : BasePlayer
    {
        public string extra = "yes";
    }

    /// <summary>
    /// Debugging / Testing / Example Usage
    /// </summary>
    private void Start()
    {
        // Resets all scores for a given level name
        ResetHighScores("Test");

        // Adds a players score to a level name and gets the returned rank
        int rank = AddHighScore<Player>("Test", new Player { name = "Test-Kun", score = 10, extra = "Test-Stk" });

        // Sorts the scores decending and adds a new player's score
        AddHighScore<Player>("Test", new Player { name = "aest-Kun", score = 1 }, 5, true);
        
        // Gets all scores for level and returns them in a list
        List<Player> players = GetHighScores<Player>("Test");

        // Serializes scores to be viewed in the debugger
        HighScores<Player> hs = new HighScores<Player>() { players = players };
        Debug.Log(JsonUtility.ToJson(hs, true));
    }

    /// <summary>
    /// Added a prefix to the level name to avoid overwriting other player prefs
    /// </summary>
    /// <param name="levelName">Level Name</param>
    /// <returns>Level Name with High Score prefix</returns>
    public static string GetIdentifier(string levelName)
    {
         return $"Highscores-{levelName}";
    }

    /// <summary>
    /// Removes all high score for given level
    /// </summary>
    /// <param name="levelName">Level Name</param>
    public static void ResetHighScores(string levelName)
    {
        PlayerPrefs.DeleteKey(GetIdentifier(levelName));
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Adds a Player object to the highscore list
    /// </summary>
    /// <param name="levelName">Name of the level to suffix the Highscore string for original save locations</param>
    /// <param name="player">Player data struct object</param>
    /// <param name="limit">Total highscores to save</param>
    /// <param name="asc">Set true to sort players descending</param>
    /// <returns>Returns index if added player in final list else 0</returns>
    public static int AddHighScore<T>(string levelName, T player, int limit=100, bool asc=false) where T: BasePlayer
    {
        HighScores<T> highScores = new HighScores<T>() { players = GetHighScores<T>(levelName) ?? new List<T>() };

        highScores.players.Add(player);

        highScores.players = (!asc ? highScores.players.OrderByDescending(a => a.score) : highScores.players.OrderBy(a => a.score))
            .ThenBy(b => b.name)
            .Take(limit)
            .ToList();

        string json = JsonUtility.ToJson(highScores);

        PlayerPrefs.SetString(GetIdentifier(levelName), json);
        PlayerPrefs.Save();

        return highScores.players.IndexOf(player) + 1;
    }

    /// <summary>
    /// Get all high scores for given level name
    /// </summary>
    /// <param name="levelName">Level Name</param>
    /// <returns>List of Generic type deriving from BasePlayer</returns>
    public static List<T> GetHighScores<T>(string levelName) where T : BasePlayer
    {
        HighScores<T> highScores = JsonUtility.FromJson<HighScores<T>>(PlayerPrefs.GetString(GetIdentifier(levelName), "{}"));
        return highScores.players;
    }
}
