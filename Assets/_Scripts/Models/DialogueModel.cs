using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DialogueModel
{
    public string characterA {  get; set; }
    public string characterB { get; set; }
    public List<string> replics { get; set; }
}
