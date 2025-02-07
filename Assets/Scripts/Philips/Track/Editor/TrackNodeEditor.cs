using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEditor.ShortcutManagement;
using UnityEngine;
using UnityEngine.Serialization;

namespace Switchgrass.Track.Editor
{
    public enum TrackNodeToolModes
    {
        Default,
        Insert,
        Extend,
        Move,
    }
    
    [EditorTool("Track Node Tool", typeof(TrackNode))]
    public class TrackNodeEditor : EditorTool, IDrawSelectedHandles
    {

        public TrackNodeToolModes mode = TrackNodeToolModes.Default;
    
        private int namingNumber = 0;

        private void OnEnable()
        {
            namingNumber = FindObjectsOfType<TrackNode>().Length;
        }

        [Shortcut("Activate Track Node Tool", typeof(SceneView), KeyCode.T)]
        private static void TrackNodeToolShortcut()
        {
            if (Selection.GetFiltered<TrackNode>(SelectionMode.TopLevel).Length > 0)
            {
                ToolManager.SetActiveTool<TrackNodeEditor>();
            }
        }
        
        public override void OnToolGUI(EditorWindow window)
        {
            if (window is not SceneView)
            {
                return;
            }

            List<TrackNode> trackNodes = new();
            foreach (var obj in targets)
            {
                if (obj is not TrackNode trackNode) continue;
                trackNodes.Add(trackNode);
            }

            if (!Event.current.isKey) return;

            EditorGUI.BeginChangeCheck();
            
            // Big, messy, eventually clean up
            if (Event.current.keyCode == KeyCode.E && trackNodes.Count == 1 && trackNodes[0].next is null)
            {
                var current = trackNodes[0];
                var position = current.transform.position + current.transform.forward;

                var next = CreateNode(current, position, current.transform.rotation);
                
                next.width = current.width;
                next.racingLine = current.racingLine;
                
                current.JoinNode(next);

                Selection.activeObject = next;
            }

            if (Event.current.keyCode == KeyCode.I)
            {
                Event.current.Use();
                
                if (trackNodes.Count != 2)
                {
                    Debug.LogWarning("Two (2) adjacent nodes must be selected to [I]nsert a new node between them!");
                    return;
                }
                
                var first = trackNodes[0];
                var second = trackNodes[1];
                
                // Make sure they're adjacent nodes
                if (second.next == first)
                {
                    (first, second) = (second, first);
                }
                
                if (first.next != second)
                {
                    Debug.LogWarning("Two adjacent nodes must be selected to [I]nsert a new node between them!");
                    return;
                }
                
                var position = Vector3.Lerp(first.transform.position, second.transform.position, 0.5f);
                var rotation = Quaternion.Slerp(first.transform.rotation, second.transform.rotation, 0.5f);

                var middle = CreateNode(first, position, rotation);
                
                middle.width = first.width;
                middle.racingLine = Mathf.Lerp(first.racingLine, second.racingLine, 0.5f);
                
                first.JoinNode(middle);
                middle.JoinNode(second);

                Selection.activeObject = middle;
            }
            
            EditorGUI.EndChangeCheck();
        }

        private TrackNode CreateNode(TrackNode original, Vector3 position, Quaternion rotation)
        {
            var parent = original.transform.parent;
                
            var newNode = Instantiate(original, position, rotation, parent);
            Undo.RegisterCreatedObjectUndo (newNode.gameObject, "Created TrackNode");
                
            newNode.name = "TN_" + namingNumber;
            namingNumber++;

            return newNode;
        }
        
        public void OnDrawHandles()
        {
            // 'Active' TrackNode
            Handles.color = Color.cyan;
            foreach (var obj in targets)
            {
                if (obj is not TrackNode trackNode) continue;
                
                var offset = trackNode.transform.right * trackNode.width / 2;
                var position = trackNode.transform.position + Vector3.up * TrackNode.DisplayOffset;
                Handles.DrawLine(position + offset, position - offset);
                Handles.color = Color.yellow;
            }
        }
    }
}
