using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//[CustomEditor(typeof(VoxelGrid))]
public class VoxelGridEditor : Editor {

    private float BrushScale = 1;
    private bool ResizingBrush = false;
    private bool Painting = false;
    private Vector3 BrushPosition;
    private Vector2 BrushResizeBasis;

    void OnSceneGUI()
    {
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        Event e = Event.current;
        Vector2 mousePosition = e.mousePosition;
        switch (e.type)
        {
            case EventType.Repaint:
                Handles.color = new Color(1, 1, 1, 0.2f);
                Handles.DrawSolidArc(BrushPosition, Vector3.forward, Vector3.right, 360, BrushScale * BrushScale);
                HandleUtility.Repaint();
                break;

            case EventType.MouseMove:
                if (ResizingBrush)
                {
                    BrushScale += (mousePosition.x - BrushResizeBasis.x) / 100f;
                    BrushScale = Mathf.Max(BrushScale, 0f);
                }
                else
                {
                    Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);
                    float scalar = -ray.origin.z / ray.direction.z;
                    BrushPosition = ray.origin + scalar * ray.direction;

                    if (Painting)
                    {
                        float scale = BrushScale * BrushScale;
                        for (int x = (int)(mousePosition.x - scale),
                            xMax = x + (int)(scale + 1);
                            x < xMax;
                            x++)
                        {
                            for (int y = (int)(mousePosition.y - scale),
                                yMax = y + (int)(scale + 1);
                                y < yMax;
                                y++)
                            {
                                //VoxelGrid grid = target as VoxelGrid;
                                //grid.Cells[x * grid.XResolution + y] = true;
                                //grid.RequestRedraw();
                                //Debug.Log("Painted");
                            }
                        }
                    }
                }

                BrushResizeBasis = mousePosition;
                break;

            case EventType.MouseDown:
                Painting = true;
                break;

            case EventType.MouseUp:
                Painting = false;
                break;

            case EventType.KeyDown:
                switch (e.keyCode)
                {
                    case KeyCode.LeftControl:
                        ResizingBrush = true;
                        break;
                }
                break;

            case EventType.KeyUp:
                ResizingBrush = false;
                break;
        }
    }

}
