using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InfiniteRectItem<T> : MonoBehaviour
{
    public abstract void UpdateItem(T value);
}
