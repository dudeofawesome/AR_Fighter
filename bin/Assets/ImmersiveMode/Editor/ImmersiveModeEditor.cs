using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[InitializeOnLoad]
public static class ImmersiveModeEditor
{
    private const string package = "com.ruudlenders.immersivemode.";
    private const string path = "Assets/Plugins/Android/AndroidManifest.xml";

    static ImmersiveModeEditor()
    {
        if (!System.IO.File.Exists(path))
        {
            Debug.LogError("ImmersiveMode could not find 'AndroidManifest.xml'. Make sure that 'AndroidManifest.xml' and 'ImmersiveMode.jar' are placed in the following folder: '/Plugins/Android/'.");
        }

        EditorApplication.update += Start;
        EditorApplication.update += Update;
    }

    static void Start()
    {
        EditorApplication.update -= Start;
    }

    static void Update()
    {
        UpdateAndroidManifest();
    }

    /// <summary>
    /// Updates AndroidManifest.xml with Unity's player settings.
    /// </summary>
    static void UpdateAndroidManifest()
    {
        // Check if AndroidManifest.xml exists in the expected folder.
        if (System.IO.File.Exists(path))
        {
            // Check the last write time to determine if AndroidManifest.xml was updated externally.
            var lastCheckTime = new System.DateTime(long.Parse(EditorPrefs.GetString(package + "lastchecktime", "0")));
            var lastWriteTime = System.IO.File.GetLastWriteTime(path);
            var updatedExternally = lastWriteTime > lastCheckTime;

            // Obtain relevant player settings and store them in a list paired with keys.
            var playerSettings = new List<string[]>();
            playerSettings.Add(new string[] { "package", PlayerSettings.bundleIdentifier.ToLower() });

            // Compare stored player settings with EditorPrefs, remove settings that do not require an update.
            for (int i = 0; i < playerSettings.Count; i++)
            {
                var key = package + playerSettings[i][0].ToLower();
                var value = playerSettings[i][1];

                if (EditorPrefs.GetString(key, string.Empty) != value)
                {
                    EditorPrefs.SetString(key, value);
                }
                else if (!updatedExternally)
                {
                    playerSettings.RemoveAt(i--);
                }
            }

            // Check if any of the player settings remain in the list.
            if (playerSettings.Count > 0)
            {
                // Read all xml from AndroidManifest.xml and store it in a string.
                string xml = string.Empty;
                using (var sr = new System.IO.StreamReader(path))
                {
                    xml = sr.ReadToEnd();
                }

                // Insert all player settings into the xml string, remove unchanged settings from the list.
                for (int i = 0; i < playerSettings.Count; i++)
                {
                    var key = playerSettings[i][0] + @"=""";
                    var newValue = playerSettings[i][1];

                    int count = 0;
                    bool xmlUpdated = false;
                    for (int index = 0; index < xml.Length; index += newValue.Length + 1)
                    {
                        index = xml.IndexOf(key, index);
                        if (index == -1) break;
                        index += key.Length;
                        count++;

                        var oldValue = xml.Substring(index, xml.IndexOf(@"""", index) - index);
                        if (oldValue != newValue)
                        {
                            xml = xml.Substring(0, index) + newValue + xml.Substring(index + oldValue.Length);
                            xmlUpdated = true;
                        }
                    }

                    if (count == 0)
                    {
                        Debug.LogError("ImmersiveMode could not find '" + key + "' in 'AndroidManifest.xml'." +
                                "\n" + "Make sure there are no whitespaces next to the equals sign (=).");
                    }
                    else if (!xmlUpdated)
                    {
                        playerSettings.RemoveAt(i--);
                    }
                }

                if (playerSettings.Count > 0)
                {
                    // Write the updated xml string back to AndroidManifest.xml.
                    using (var sw = new System.IO.StreamWriter(path))
                    {
                        sw.Write(xml);
                    }
                }

                // Save the current time to EditorPrefs, so that it can be used to check for external updates.
                EditorPrefs.SetString(package + "lastchecktime", System.DateTime.Now.Ticks.ToString());
            }
        }
    }
}