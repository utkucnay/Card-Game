using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class InfiniteScrollRect<TVisual, TValue> : MonoBehaviour where TVisual : InfiniteRectItem<TValue>
{
    [SerializeField] protected List<TVisual> poolOfVisualElements;
    [SerializeField] protected ScrollRect scrollRect;
    [SerializeField] protected float itemSize;
    [SerializeField] protected float itemSpacing;

    private List<TValue> values;

    private void OnEnable()
    {
        scrollRect.onValueChanged.AddListener(OnScrollValueChanged);   
    }

    private void OnDisable()
    {
        scrollRect.onValueChanged.RemoveListener(OnScrollValueChanged);
    }

    private void OnScrollValueChanged(Vector2 scrollPosition)
    {
        if (values == null || values.Count == 0) return;

        int firstVisibleIndex = GetFirstVisibleIndex();
        int visibleItemCount = GetVisibleItemCount();
        UpdateVisibleItems(firstVisibleIndex, visibleItemCount);
    }

    private int GetFirstVisibleIndex()
    {
        if (scrollRect.vertical)
        {
            float contentTop = scrollRect.content.anchoredPosition.y;
            return Mathf.FloorToInt(contentTop / (itemSize + itemSpacing));
        }
        else if (scrollRect.horizontal)
        {
            float contentLeft = -scrollRect.content.anchoredPosition.x;
            return Mathf.FloorToInt(contentLeft / (itemSize + itemSpacing));
        }
        return 0;
    }

    private void UpdateVisibleItems(int firstVisibleIndex, int visibleItemCount)
    {
        for (int i = 0; i < poolOfVisualElements.Count; i++)
        {
            int itemIndex = firstVisibleIndex + i;
            if (itemIndex >= 0 && itemIndex < values.Count)
            {
                poolOfVisualElements[i].gameObject.SetActive(true);
                if (scrollRect.vertical)
                {
                    poolOfVisualElements[i].transform.localPosition = new Vector3(0, -(itemIndex * (itemSize + itemSpacing)), 0);
                }
                else if (scrollRect.horizontal)
                {
                    poolOfVisualElements[i].transform.localPosition = new Vector3(itemIndex * (itemSize + itemSpacing), 0, 0);
                }
                poolOfVisualElements[i].UpdateItem(values[itemIndex]);
            }
            else
            {
                poolOfVisualElements[i].gameObject.SetActive(false);
            }
        }
    }

    private int GetVisibleItemCount()
    {
        if (scrollRect.vertical)
        {
            return Mathf.CeilToInt(scrollRect.viewport.rect.height / (itemSize + itemSpacing)) + 1;
        }
        else if (scrollRect.horizontal)
        {
            return Mathf.CeilToInt(scrollRect.viewport.rect.width / (itemSize + itemSpacing)) + 1;
        }
        return 0;
    }

    public void UpdateElements()
    {
        int firstVisibleIndex = GetFirstVisibleIndex();
        int visibleItemCount = GetVisibleItemCount();
        UpdateVisibleItems(firstVisibleIndex, visibleItemCount);
    }

    public virtual void SetItems(List<TValue> values)
    {
        this.values = values;
        UpdateContentSize();
        OnScrollValueChanged(scrollRect.normalizedPosition);
    }
    public virtual void AddItem(TValue value)
    {
        if (values == null)
        {
            values = new List<TValue> { value };
        }
        else
        {
            values.Add(value);
        }

        UpdateContentSize();
        OnScrollValueChanged(scrollRect.normalizedPosition);
    }

    public virtual bool TryGetItem(Predicate<TValue> predicate, out TValue item, out int index)
    {
        item = default;
        index = -1;
        if (values == null) return false;
        index = values.FindIndex(predicate);
        if (index == -1) return false;
        item = values[index];
        return true;
    }

    public virtual TValue GetItem(Predicate<TValue> predicate)
    {
        if (values == null) return default;
        int index = values.FindIndex(predicate);
        if (index == -1)
        {
            Debug.LogError("Item not found with the given predicate.");
            return default;
        }
        return values[index];
    }

    public virtual TValue GetItem(int index)
    {
        if (values == null || index < 0 || index >= values.Count)
        {
            Debug.LogError("Index out of range: " + index);
            return default;
        }
        return values[index];
    }

    public virtual TVisual GetVisualItem(Predicate<TVisual> predicate)
    {
        if (poolOfVisualElements == null) return default;
        int index = poolOfVisualElements.FindIndex(predicate);
        if (index == -1)
        {
            Debug.LogError("Item not found with the given predicate.");
            return default;
        }
        return poolOfVisualElements[index];
    }
    
    public virtual TVisual GetVisualItem(int index)
    {
        if (poolOfVisualElements == null || index < 0 || index >= poolOfVisualElements.Count)
        {
            Debug.LogError("Index out of range: " + index);
            return default;
        }
        return poolOfVisualElements[index];
    }

    public virtual void ClearItems()
    {
        values?.Clear();
        UpdateContentSize();
        OnScrollValueChanged(Vector2.zero);
    }

    public Tween? GoToLastItem(bool animate = true)
    {
        if (values == null || values.Count == 0) return null;
        return GoToItem(values.Count - 1, animate);
    }

    public Tween? GoToItem(int index, bool animate = true)
    {
        if (index < 0 || index >= values.Count)
        {
            Debug.LogError("Index out of range: " + index);
            return null;
        }

        if (IsItemVisible(index, 2))
        {
            return null; // No need to scroll if the item is already visible
        }

        if (animate)
        {
            if (scrollRect.vertical)
            {
                float targetY = index * (itemSize + itemSpacing) - scrollRect.viewport.rect.height / 2;
                return scrollRect.content.DOAnchorPosY(targetY, 0.5f).SetEase(Ease.OutCubic);
            }
            else if (scrollRect.horizontal)
            {
                float targetX = -index * (itemSize + itemSpacing) + scrollRect.viewport.rect.width / 2;
                return scrollRect.content.DOAnchorPosX(targetX, 0.5f).SetEase(Ease.OutCubic);
            }
        }
        else
        {
            if (scrollRect.vertical)
            {
                float targetY = index * (itemSize + itemSpacing) - scrollRect.viewport.rect.height / 2;
                scrollRect.content.anchoredPosition = new Vector2(scrollRect.content.anchoredPosition.x, targetY);
            }
            else if (scrollRect.horizontal)
            {
                float targetX = -index * (itemSize + itemSpacing) + scrollRect.viewport.rect.width / 2;
                scrollRect.content.anchoredPosition = new Vector2(targetX, scrollRect.content.anchoredPosition.y);
            }
        }
        return null;
    }

    public bool IsItemVisible(int index, int offset = 0)
    {
        if (index < 0 || index >= values.Count)
        {
            Debug.LogError("Index out of range: " + index);
            return false;
        }

        int firstVisibleIndex = GetFirstVisibleIndex();
        int visibleItemCount = GetVisibleItemCount();

        return index >= firstVisibleIndex + offset && index < firstVisibleIndex + visibleItemCount - offset;
    }

    private void UpdateContentSize()
    {
        if (scrollRect.vertical && scrollRect.horizontal)
        {
            Debug.LogError("InfiniteScrollRect does not support both vertical and horizontal scrolling at the same time.");
            return;
        }

        if (scrollRect.vertical)
            scrollRect.content.sizeDelta = new Vector2(scrollRect.content.sizeDelta.x, values.Count * (itemSize + itemSpacing));
        else if (scrollRect.horizontal)
            scrollRect.content.sizeDelta = new Vector2(values.Count * (itemSize + itemSpacing), scrollRect.content.sizeDelta.y);
    }
}