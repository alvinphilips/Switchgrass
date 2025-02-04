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
            if (Event.current.keyCode == KeyCode.E && trackNodes.Count == 1 && trackNodes[0].next is null)
            {
                var current = trackNodes[0];

                var parent = current.transform.parent;

                var position = current.transform.position + current.transform.forward;

                var next = Instantiate(current, position, current.transform.rotation, parent);
                Undo.RegisterCreatedObjectUndo (next.gameObject, "Created TrackNode");
                next.name = "TN_" + namingNumber;
                namingNumber++;
                next.width = current.width;
                next.racingLine = current.racingLine;
                current.JoinNode(next);

                Selection.activeObject = next;
            }
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
