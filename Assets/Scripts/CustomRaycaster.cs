using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomRaycaster : UnityEngine.EventSystems.BaseRaycaster
{
    public override Camera eventCamera
    {
        get
        {
            return canvas.worldCamera != null ? canvas.worldCamera : Camera.main;
        }
    }

    Canvas canvas;
    void Start()
    {
        canvas = GetComponent<Canvas>();
    }

    public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
    {
        var pos = new Vector2(eventCamera.pixelWidth * 0.5f, eventCamera.pixelHeight * 0.5f);

        var hitGraphics = new List<Graphic>();
        var graphics = GraphicRegistry.GetGraphicsForCanvas(canvas);
        for( int i = 0; i < graphics.Count; ++i )
        {
            var graphic = graphics[i];
            if (!graphic.raycastTarget) continue;
            if (graphic.depth == -1) continue;
            if (!RectTransformUtility.RectangleContainsScreenPoint(graphic.rectTransform, pos, eventCamera))
            {
                continue;
            }
            if (graphic.Raycast(pos, eventCamera))
            {
                hitGraphics.Add(graphic);
            }
        }

        hitGraphics.Sort((g1, g2) => g2.depth.CompareTo(g1.depth));
        for( int i = 0; i < hitGraphics.Count; ++i )
        {
            var graphic = hitGraphics[i];
            resultAppendList.Add(new RaycastResult
            {
                gameObject = graphic.gameObject,
                module = this,
                distance = Vector3.Distance(eventCamera.transform.position, canvas.transform.position),
                index = resultAppendList.Count
            });
        }
    }
}
