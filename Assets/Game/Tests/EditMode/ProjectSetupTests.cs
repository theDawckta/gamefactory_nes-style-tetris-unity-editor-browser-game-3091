using NUnit.Framework;
using UnityEngine;
using System.IO;

public class ProjectSetupTests
{
    [Test]
    public void RequiredFolders_AllExist()
    {
        string[] folders =
        {
            "Assets/Game/Scripts",
            "Assets/Game/Scripts/UI",
            "Assets/Game/Audio/Music",
            "Assets/Game/Audio/SFX",
            "Assets/Game/Scenes",
            "Assets/Game/Sprites",
            "Assets/Game/ScriptableObjects"
        };

        foreach (string folder in folders)
            Assert.IsTrue(Directory.Exists(folder), $"Folder missing: {folder}");
    }

    [Test]
    public void SampleScene_Exists()
    {
        Assert.IsTrue(File.Exists("Assets/Game/Scenes/SampleScene.unity"), "SampleScene.unity not found");
    }
}
