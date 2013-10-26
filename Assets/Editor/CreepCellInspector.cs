using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CreepCell))]
public class CreepCellInspector : Editor
{
    private float _editableFillingLevel = 1.0f;//((CreepCell)target).FillingLevel;
    public float CellFilling
    {
        get
        {
            return _editableFillingLevel;
        }
        set
        {
            if (_editableFillingLevel == value)
                return;
            
            _editableFillingLevel = value;
            CreepCell cc = (CreepCell)target;
            cc.FillingLevel = _editableFillingLevel;
            Debug.Log("Set CellFilling to: " + _editableFillingLevel);
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        DrawDefaultInspector();

        var cc = ((CreepCell) target);
        _editableFillingLevel = cc.FillingLevel;
        EditorGUILayout.BeginVertical();
        CellFilling = EditorGUILayout.Slider(_editableFillingLevel, 0, 1);
        EditorGUILayout.EndVertical();

        if (GUILayout.Button("Regenerate"))
        {
            cc.UpdateMesh();
        }
    }
}
