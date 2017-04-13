using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Calls another script to parse a CSV in a specific format. Parses out different
/// languages from that file, and key/value pairs for a single language. Also provides
/// interface for retrieving those values based on a key, as well as changing the
/// the current langage.
/// </summary>
/// <remarks>
/// Revision History:
/// <Date>      <Name> <Description>
/// 2015/03/23  Dylan  Initial verison
/// 2015/03/24  Taro   Refactoring Documentation,
///                    replacing methods with properties
///                    And turning the script to a Singleton
/// 2015/03/25  Taro   Adding variables for debugging
/// </remarks>
public class CSVLanguageParser : ISingletonScript
{
    [Header("CSV File")]
    [Tooltip("Set this variable if the CSV file is a text asset that isn't in the Resources folder.")]
    [SerializeField]
    TextAsset loadFileAsset = null;
    [Tooltip("Set this variable if the CSV file is going to be loaded from the Resources folder.")]
    [SerializeField]
    string loadFileName = "";
    [Header("Content")]
    [Tooltip("The header containing the keys, referencing each string.")]
    [SerializeField]
    string keyHeader = "Keys";
    [Tooltip("The language to test on game loading. Leave blank to use the default language.")]
    [SerializeField]
    string testLanguage = "";
    /// <summary>
    /// The loaded file.
    /// </summary>
    TextAsset inputFile = null;
    /// <summary>
    /// List of supported languages available after the file has been parsed.
    /// </summary>
    List<string> supportedLanguages = new List<string>();
    /// <summary>
    /// A dictionary of the keys, current-language-values.
    /// </summary>
    Dictionary<string, string> dictionary = new Dictionary<string, string>();
    /// <summary>
    /// Default Language in case needed for whatever reason. Defaults to first language encountered in the file.
    /// </summary>
    string defaultLanguage = "";
    /// <summary>
    /// Currently selected langauge. Defaults to the first language encountered in the file.
    /// </summary>
    string currentLanguage = "";

    /// <summary>
    /// Called when the first scene is loaded.
    /// </summary>
    public override void SingletonStart(Singleton globalGameObject)
    {
        if(string.IsNullOrEmpty(testLanguage) == false)
        {
            defaultLanguage = testLanguage;
            currentLanguage = testLanguage;
        }
        if(loadFileAsset != null)
        {
            ParseFile(loadFileAsset);
        }
        else if(string.IsNullOrEmpty(loadFileName) == false)
        {
            ParseFile(loadFileName);
        }
        else
        {
            Debug.LogWarning("No file found for CSVLanguageParser");
        }
    }
    
    /// <summary>
    /// Called when any scene after the first one is loaded.
    /// </summary>
    public override void SceneStart(Singleton globalGameObject)
    {
        // Do nothing
    }

    #region Properties
    public string this[string key]
    {
        get
        {
            return dictionary [key].ToString();
        }
    }

    /// <summary>
    /// Gets the list of langauges identified in the most recent parse.
    /// </summary>
    /// <returns>The supported languages.</returns>
    public List<string> SupportedLanguages
    {
        get
        {
            return supportedLanguages;
        }
    }

    /// <summary>
    /// Gets the default language.
    /// </summary>
    /// <value>The default language.</value>
    public string DefaultLanguage
    {
        get
        {
            return defaultLanguage;
        }
    }

    /// <summary>
    /// Gets or sets the current language.
    /// </summary>
    /// <value>The current language.</value>
    /// <remarks>
    /// When setting a supported language, this property will
    /// reparse the file to reload the dictionary.
    /// If a language is not supported, an error will be raised.
    /// </remarks>
    public string CurrentLanguage
    {
        get
        {
            return currentLanguage;
        }
        set
        {
            if (value != currentLanguage)
            {
                if (supportedLanguages.Contains(value) == true)
                {
                    currentLanguage = value;
                    ParseFile ();
                }
                else
                {
                    Debug.LogError(value + " is not a supported langague.");
                }
            }
        }
    }
    #endregion

    public void ParseFile(string csvFilePath)
    {
        inputFile = Resources.Load<TextAsset>(csvFilePath);
        ParseFile ();
    }
    
    public void ParseFile(TextAsset textAsset)
    {
        inputFile = textAsset;
        ParseFile ();
    }

    public void UnloadFile()
    {
        if(inputFile != null)
        {
            Resources.UnloadAsset(inputFile);
        }
    }

    public bool ContainsKey(string key)
    {
        return dictionary.ContainsKey(key);
    }

    public List<string> GetAllKeys()
    {
        List <string> keys = new List<string>();
        foreach (KeyValuePair<string, string> pair in dictionary)
        {
            keys.Add(pair.Key);
        }
        return keys;
    }

    public void ResetToDefaultLanguage()
    {
        currentLanguage = defaultLanguage;
        ParseFile ();
    }

    #region Helper Methods
    /// <summary>
    /// Helper method to parse file based on inputFile.
    /// </summary>
    void ParseFile()
    {
        /* Read the input file. Data is expected to be returned in a list of
         * Dictionary objects in which the position in the list corolates to the
         * row-number from the CSV file (element 0 is the second row where the
         * first row is headers), the keys in the dictionary reflect the column
         * names from the header, and the values in the dictionary reflect the
         * values for a given row/column.
         */
        List<Dictionary<string, string>> data = CSVReader.Read (inputFile);
        
        /* Read the first row pulled back from the csv file. Parse the
         * dictionary to get a list of all the keys that were found. This
         * will be the list of languages in the file. When building the list,
         * ignore the column that has the language-independent keys. Also
         * while building, if a defualt or current language has not yet been
         * set, then set them to the first language encountered.
         */
        supportedLanguages.Clear ();
        foreach (string key in data[0].Keys)
        {
            if (key != keyHeader)
            {
                supportedLanguages.Add(key);
                if(string.IsNullOrEmpty(defaultLanguage) == true)
                {
                    defaultLanguage = key;
                }
                if(string.IsNullOrEmpty(currentLanguage) == true)
                {
                    currentLanguage = key;
                }
            }
        }
        
        /* Loop through each row in the file. Grab the langauge-independet, and
         * the value based on the current langage. Put them in the dictionary.
         */
        dictionary.Clear ();
        for (int i = 0; i < data.Count; i++)
        {
            string key = data [i][keyHeader];
            string val =  data [i][currentLanguage];
            dictionary.Add (key, val);
        }

        /* Update any Text labels */
        foreach(TranslatedText label in TranslatedText.AllTranslationScripts)
        {
            label.UpdateLabel();
        }
        foreach(TranslatedTextMesh label in TranslatedTextMesh.AllTranslationScripts)
        {
            label.UpdateLabel();
        }
    }
    #endregion
}