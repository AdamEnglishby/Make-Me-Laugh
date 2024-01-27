﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(SphereCollider), typeof(Rigidbody))]
public class HotspotDetector : MonoBehaviour
{
    [SerializeField] private Color edgeHighlightColour;

    private readonly List<Hotspot> _hotspots = new();

    private static readonly int OutlineColor = Shader.PropertyToID("_OutlineColor");

    public void Interact()
    {
        _hotspots.Sort((h1, h2) =>
        {
            var ourPosition = transform.position;
            var distance1 = Vector3.Distance(h1.transform.position, ourPosition);
            var distance2 = Vector3.Distance(h2.transform.position, ourPosition);
            return Mathf.RoundToInt(distance1 * 100 - distance2 * 100);
        });
        _hotspots.First().Interact();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out Hotspot hotspot)) return;
        foreach (var r in hotspot.GetComponentsInChildren<Renderer>())
        {
            foreach (var mat in r.materials)
            {
                mat.SetColor(OutlineColor, edgeHighlightColour);
            }
        }

        _hotspots.Add(hotspot);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent(out Hotspot hotspot)) return;
        foreach (var r in hotspot.GetComponentsInChildren<Renderer>())
        {
            foreach (var mat in r.materials)
            {
                mat.SetColor(OutlineColor, Color.black);
            }
        }

        _hotspots.Remove(hotspot);
    }
}