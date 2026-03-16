using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class FlightManager : MonoSingleton<FlightManager>
{
    private Queue<Flight> flights;
    [SerializeField] private List<Flight> serializeFlights;
    [SerializeField] private Vector3 bezierUpsideIn = new Vector3(2, 2, 0);
    [SerializeField] private Vector3 bezierUpsideOut = new Vector3(-2, 4, 0);
    [SerializeField] private Vector3 bezierDownsideIn = new Vector3(0.75f, -0.75f, 0);
    [SerializeField] private Vector3 bezierDownsideOut = new Vector3(-0.75f, -1, 0);

    private void Awake()
    {
        flights = new Queue<Flight>(serializeFlights);
    }

    public Tween Flight(Vector3 start, Vector3 end, Vector2 size, float duration, int count, Vector2 spawnArea, Sprite sprite)
    {
        if (flights.Count == 0)
        {
            Debug.LogError("No more flights available in the queue.");
            return null;
        }

        Sequence sequence = DOTween.Sequence();

        for (int i = 0; i < count; i++)
        {
            var flight = flights.Dequeue();
            flight.gameObject.SetActive(true);
            flight.SetSprite(sprite);
            var spawnOffset = new Vector3(Random.Range(-spawnArea.x / 2, spawnArea.x / 2), Random.Range(-spawnArea.y / 2, spawnArea.y / 2), 0);
            ((RectTransform)flight.transform).position = start + spawnOffset;
            ((RectTransform)flight.transform).sizeDelta = size;
            sequence.Join(flight.transform.DOPath(
                spawnOffset.y > 0 ? new Vector3[] { end, bezierUpsideIn, bezierUpsideOut } : new Vector3[] { end, bezierDownsideIn, bezierDownsideOut }, duration, 
                PathType.CubicBezier , PathMode.Sidescroller2D, 10, Color.red).SetEase(Ease.OutCubic).OnComplete(() =>
                {
                    flight.gameObject.SetActive(false);
                    flights.Enqueue(flight);
                }));
        }

        return sequence;
    }
}
