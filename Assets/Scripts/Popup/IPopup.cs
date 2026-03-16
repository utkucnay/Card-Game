using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class APopup : MonoBehaviour
{
    public Action OnClose { get; set; }
    public abstract void Open();
    public abstract void Close();
}
