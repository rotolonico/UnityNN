using System.IO;
using UnityEngine;
using Utilities;

public static class NetworkStorage
{
    public static void SaveNetwork(NetworkSave save, string path)
    {
        var json = StringSerializationAPI.Serialize(typeof(NetworkSave), save);

        var file = new FileInfo($"{Application.persistentDataPath}/networkSaves/{path}");
        file.Directory?.Create();
        File.WriteAllText(file.FullName, json);
    }

    public static NetworkSave LoadNetwork(string path)
    {
        var json = !File.Exists($"{Application.persistentDataPath}/networkSaves/{path}")
            ? ""
            : File.ReadAllText($"{Application.persistentDataPath}/networkSaves/{path}");

        if (json == "") return null;
        return LoadNetworkFromJSON(json);
    }

    public static NetworkSave LoadNetworkFromJSON(string json) =>
        StringSerializationAPI.Deserialize(typeof(NetworkSave), json) as NetworkSave;
}