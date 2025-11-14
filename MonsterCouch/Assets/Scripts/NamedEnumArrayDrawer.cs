using UnityEditor;
using UnityEngine;
using System;
public class NamedEnumArrayAttribute : PropertyAttribute
{
    public Type EnumType;

    public NamedEnumArrayAttribute(Type enumType)
    {
        EnumType = enumType;
    }
}

[CustomPropertyDrawer(typeof(NamedEnumArrayAttribute))]
public class NamedEnumArrayDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var attr = attribute as NamedEnumArrayAttribute;
        string[] names = Enum.GetNames(attr.EnumType);

        return (names.Length + 1) * (EditorGUIUtility.singleLineHeight + 2);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var attr = attribute as NamedEnumArrayAttribute;
        string[] names = Enum.GetNames(attr.EnumType);

        if (property.arraySize != names.Length)
            property.arraySize = names.Length;

        position.height = EditorGUIUtility.singleLineHeight;
        EditorGUI.LabelField(position, label);

        position.y += EditorGUIUtility.singleLineHeight + 2;

        for (int i = 0; i < names.Length; i++)
        {
            SerializedProperty element = property.GetArrayElementAtIndex(i);

            EditorGUI.PropertyField(
                new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
                element,
                new GUIContent(names[i]),
                true
            );

            position.y += EditorGUIUtility.singleLineHeight + 2;
        }

        EditorGUI.EndProperty();
    }
}
