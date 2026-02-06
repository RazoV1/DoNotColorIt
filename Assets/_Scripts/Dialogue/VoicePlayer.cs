using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NpcNameMapping
{
    public string dialogueName; 
    public string folderName;
}

[RequireComponent(typeof(AudioSource))]
public class VoicePlayer : MonoBehaviour
{
    [SerializeField] private string currentLocate = "RU";
    private AudioSource audioSource;

    private Dictionary<string, AudioClip[]> npcvoices = new Dictionary<string, AudioClip[]>();
    private Dictionary<string, int> npcVoiceIndex = new Dictionary<string, int>();
    [SerializeField] private List<NpcNameMapping> npcNameMappings;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f;
    }

    public void PlayNext(string npcName)
    {
        if (string.IsNullOrEmpty(npcName))
        {
            return;
        }

        if (!npcvoices.ContainsKey(npcName))
        {
            string folderName = GetFolderName(npcName);

            AudioClip[] clips = Resources.LoadAll<AudioClip>(
                $"DialogueAudio/{currentLocate}/{folderName}"
            );
            if (clips == null || clips.Length == 0)
            {
                return;
            }

            npcvoices[npcName] = clips;
            npcVoiceIndex[npcName] = 0;
        }

        AudioClip[] npcClips = npcvoices[npcName];
        int index = npcVoiceIndex[npcName];
        audioSource.Stop();
        audioSource.clip = npcClips[index];
        audioSource.Play();
        index++;
        if(index>=npcClips.Length)
        {
            index = 0;
        }
        npcVoiceIndex[npcName] = index;
    }

    public void SetLocale(string locale)
    {
        if (currentLocate == locale)
            return;

        currentLocate = locale;

        npcvoices.Clear();
        npcVoiceIndex.Clear();
    }

    public void Stop()
    {
        audioSource.Stop();
    }

    private string GetFolderName(string dialogueName)
    {
        foreach (var mapping in npcNameMappings)
        {
            if (mapping.dialogueName == dialogueName)
                return mapping.folderName;
        }
        return dialogueName;
    }
}
