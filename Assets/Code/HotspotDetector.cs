using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(SphereCollider), typeof(Rigidbody))]
public class HotspotDetector : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private Color edgeHighlightColour;

    private readonly List<Hotspot> _hotspots = new();
    private Hotspot _activeHotspot;

    private static readonly int OutlineColor = Shader.PropertyToID("_OutlineColor");

    public async Task Interact(Player p)
    {
        if (!_activeHotspot) return;
        p.InteractionEnabled = false;
        await _activeHotspot.Interact(p);
        p.InteractionEnabled = true;
    }

    private void Update()
    {
        _hotspots.Sort((h1, h2) =>
        {
            var ourPosition = transform.position;
            var distance1 = Vector3.Distance(h1.transform.position, ourPosition);
            var distance2 = Vector3.Distance(h2.transform.position, ourPosition);
            return Mathf.RoundToInt(distance1 * 100 - distance2 * 100);
        });
        
        foreach (var hotspot in _hotspots)
        {
            SetEdgeColour(new Color(0, 0, 0, 0), hotspot);
        }

        _activeHotspot = _hotspots.FirstOrDefault();
        if (_activeHotspot)
        {
            SetEdgeColour(edgeHighlightColour, _activeHotspot);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!player.InteractionEnabled) return;
        if (!other.TryGetComponent(out Hotspot hotspot)) return;
        _hotspots.Add(hotspot);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!player.InteractionEnabled) return;
        if (!other.TryGetComponent(out Hotspot hotspot)) return;
        SetEdgeColour(new Color(0, 0, 0, 0), hotspot);
        _hotspots.Remove(hotspot);
    }

    private static void SetEdgeColour(Color color, Component hotspot)
    {
        foreach (var r in hotspot.GetComponentsInChildren<Renderer>())
        {
            foreach (var mat in r.materials)
            {
                mat.SetColor(OutlineColor, color);
            }
        }
    }
    
}