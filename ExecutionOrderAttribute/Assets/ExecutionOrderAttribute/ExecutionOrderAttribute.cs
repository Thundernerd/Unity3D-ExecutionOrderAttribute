using System;
#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif
using UnityEngine;

[AttributeUsage( AttributeTargets.Class, Inherited = false, AllowMultiple = false )]
sealed class ExecutionOrderAttribute : Attribute {

    public readonly int ExecutionOrder = 0;

    public ExecutionOrderAttribute( int executionOrder ) {
        ExecutionOrder = executionOrder;
    }

#if UNITY_EDITOR
    private const string PREFS_KEY = "_executionOrderUpdated";
    private const string PB_TITLE = "Updating Execution Order";
    private const string PB_MESSAGE = "Hold on to your butt, Cap'n!";
    private const string ERR_MESSAGE = "Unable to locate and set execution order for {0}";

    [InitializeOnLoadMethod]
    private static void Execute() {
        if ( EditorPrefs.GetBool( PREFS_KEY, false ) ) {
            EditorPrefs.DeleteKey( PREFS_KEY );
            return;
        }

        var type = typeof( ExecutionOrderAttribute );
        var assembly = type.Assembly;
        var types = assembly.GetTypes();

        var progress = 0f;
        var step = 1f / types.Length;

        foreach ( var item in types ) {
            var cancelled = EditorUtility.DisplayCancelableProgressBar( PB_TITLE, PB_MESSAGE, progress );
            progress += step;

            if ( cancelled ) {
                break;
            }

            var attributes = item.GetCustomAttributes( type, false );
            if ( attributes.Length == 0 ) continue;
            var attribute = attributes[0] as ExecutionOrderAttribute;

            var asset = "";
            var guids = AssetDatabase.FindAssets( string.Format( "{0} t:script", item.Name ) );

            if ( guids.Length > 1 ) {
                foreach ( var guid in guids ) {
                    var assetPath = AssetDatabase.GUIDToAssetPath( guid );
                    var filename = Path.GetFileNameWithoutExtension( assetPath );
                    if ( filename == item.Name ) {
                        asset = guid;
                        break;
                    }
                }
            } else if ( guids.Length == 1 ) {
                asset = guids[0];
            } else {
                Debug.LogErrorFormat( ERR_MESSAGE, item.Name );
                return;
            }

            var script = AssetDatabase.LoadAssetAtPath<MonoScript>( AssetDatabase.GUIDToAssetPath( asset ) );
            if ( MonoImporter.GetExecutionOrder( script ) != attribute.ExecutionOrder ) {
                MonoImporter.SetExecutionOrder( script, attribute.ExecutionOrder );
                EditorPrefs.SetBool( PREFS_KEY, true );
            }
        }

        EditorUtility.ClearProgressBar();
    }
#endif
}
