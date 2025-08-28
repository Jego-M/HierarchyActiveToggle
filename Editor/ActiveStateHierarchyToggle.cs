using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace HierarchyActiveToggle
{

    [InitializeOnLoad]
    public static class ActiveStateHierarchyToggle
    {
        private static readonly GUIContent _inactiveSelfIcon;
        private static readonly GUIContent _activeSelfActiveHierarchyIcon;
        private static readonly GUIContent _activeSelfInactiveHierarchyIcon;

        private const string ICON_INACTIVE__SELF_NAME = "sv_icon_dot0_pix16_gizmo";
        private const string ICON_ACTIVE_SELF_ACTIVE_HIERARCHY_NAME = "sv_icon_dot3_pix16_gizmo";
        private const string ICON_ACTIVE_SELF_INACTIVE_HIERARCHY_NAME = "sv_icon_dot1_pix16_gizmo";

        private const float ICON_ALPHA = .7f;
        private const float ICON_X_OFFSET = 34;
        private const float ICON_Y_OFFSET = 1.9f;
        private const float ICON_SIZE = 12;

        static ActiveStateHierarchyToggle()
        {
            EditorApplication.hierarchyWindowItemOnGUI += OnItemGUI;

            _inactiveSelfIcon = EditorGUIUtility.IconContent(ICON_INACTIVE__SELF_NAME);
            _activeSelfActiveHierarchyIcon = EditorGUIUtility.IconContent(ICON_ACTIVE_SELF_ACTIVE_HIERARCHY_NAME);
            _activeSelfInactiveHierarchyIcon = EditorGUIUtility.IconContent(ICON_ACTIVE_SELF_INACTIVE_HIERARCHY_NAME);
        }

        static void OnItemGUI(int instanceID, Rect selectionRect)
        {
            if (!(EditorUtility.InstanceIDToObject(instanceID) is GameObject gameObject))
                return;

            // By default x > 0, to capture mouse across the entire row we change it to 0
            selectionRect.width += selectionRect.x;
            selectionRect.x = 0;
            if (!selectionRect.Contains(Event.current.mousePosition)) return;

            GUIContent icon = !gameObject.activeSelf       ? _inactiveSelfIcon :
                              gameObject.activeInHierarchy ? _activeSelfActiveHierarchyIcon   :
                                                             _activeSelfInactiveHierarchyIcon;
            Rect iconRect = new Rect(ICON_X_OFFSET, selectionRect.y + ICON_Y_OFFSET, ICON_SIZE, ICON_SIZE);

            Color oldColor = GUI.color;
            GUI.color = new Color(1, 1, 1, ICON_ALPHA);

            if (GUI.Button(iconRect, icon, GUIStyle.none))
            {
                Undo.RecordObject(gameObject, (gameObject.activeSelf ? "Deactivate " : "Activate ") + gameObject.name);
                gameObject.SetActive(!gameObject.activeSelf);
                if (!Application.isPlaying)
                    EditorSceneManager.MarkSceneDirty(gameObject.scene);
            }

            GUI.color = oldColor;
        }
    }
}