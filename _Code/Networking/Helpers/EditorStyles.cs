// HTEditorTool v1.0 (September 2012)
// HTEditorTool library is copyright (c) of Hedgehog Team
// Please send feedback or bug reports to the.hedgehog.team@gmail.com
using UnityEngine;
using System.Collections;

#if UNITY_EDITOR

using UnityEditor;

namespace DFNetwork.Helpers {
	public class EditorStyling{
		
		public static  Texture2D gradientTexture;
		
		private static GUIStyle defaultStyle;
		private static GUIStyle paddingStyle;
		private static GUIStyle headerStyle;
		private static GUIStyle labelCenter;

		public static GUIStyle GetHeaderStyle {
			
			get {
				
				if (headerStyle == null) {
					
					headerStyle = new GUIStyle();
					headerStyle.padding = new RectOffset(0,0,0,0);
					headerStyle.fontSize = 20;					
					headerStyle.normal.textColor = Color.white;
					
				}
				
				return headerStyle;
				
			}
			
		}
		
		public static GUIStyle GetDefaultStyle {
			get {
				
				if (defaultStyle == null) {
		
					defaultStyle = new GUIStyle();
					defaultStyle.padding = new RectOffset(10,10,0,10);				
					
				}
				
				return defaultStyle;
				
			}
		}
		
		public static GUIStyle GetPaddingStyle {
			get {
				if (paddingStyle == null) {
		
					paddingStyle = new GUIStyle();
					paddingStyle.padding = new RectOffset(30,0,0,0);				
					
				}
				
				return paddingStyle;
			}
		}

		public static GUIStyle GetEditorCenterLable {
			get {

				if (labelCenter == null) {

					labelCenter = new GUIStyle();
					labelCenter.alignment = TextAnchor.LowerCenter;
					labelCenter.contentOffset = new Vector2(-14f, 0f);
					labelCenter.normal.textColor = Color.white;

				}

				return labelCenter;

			}
		}
		

		public static void DrawHeader(string title) {
			
			EditorGUILayout.LabelField(title, GetHeaderStyle);
			GUILayout.Space(10);
			
		}

		#region Helper
		public static bool DrawTitleFoldOut( bool foldOut,string text){
			
			GUIStyle foldOutStyle =  new GUIStyle( EditorStyles.foldout);
			foldOutStyle.fontStyle = FontStyle.Bold;
			
			Color foldTextColor=Color.black;
			if (EditorGUIUtility.isProSkin){
				foldTextColor = new Color( 242f/255f,152f/255f,13f/255f);	
			}
			
			foldOutStyle.onActive.textColor = foldTextColor;
			foldOutStyle.onFocused.textColor = foldTextColor;
			foldOutStyle.onHover.textColor = foldTextColor;
			foldOutStyle.onNormal.textColor = foldTextColor;
			foldOutStyle.active.textColor = foldTextColor;
			foldOutStyle.focused.textColor = foldTextColor;
			foldOutStyle.hover.textColor = foldTextColor;
			foldOutStyle.normal.textColor = foldTextColor;
			
			Rect lastRect = DrawTitleGradient();
			GUI.color = Color.white;
			bool value = EditorGUI.Foldout(new Rect(lastRect.x + 3, lastRect.y+1, lastRect.width - 5, lastRect.height),foldOut,text,foldOutStyle);
			GUI.color = Color.white;
			
			return value;
		}
			
		public static void DrawSeparatorLine()
		{
			
		    GUILayout.Space(10);
	        Rect lastRect = GUILayoutUtility.GetLastRect();
			
			GUI.color = Color.gray;
		    GUI.DrawTexture(new Rect(0, lastRect.yMax-0, Screen.width, 1f), EditorGUIUtility.whiteTexture);
			GUI.color = Color.white;
		}
		
		public static Rect DrawTitleGradient()
		{
		    GUILayout.Space(30);
			Rect lastRect = GUILayoutUtility.GetLastRect();
		    lastRect.yMin = lastRect.yMin + 5;
		    lastRect.yMax = lastRect.yMax - 5;
		    lastRect.width =  Screen.width - 30;
			
			
			GUI.DrawTexture(new Rect(15, lastRect.yMin, Screen.width - 30, lastRect.yMax- lastRect.yMin), GetGradientTexture());
			GUI.color = new Color(0.54f, 0.54f, 0.54f);
			GUI.DrawTexture(new Rect(15, lastRect.yMin, Screen.width - 30, 1f),  EditorGUIUtility.whiteTexture);
			GUI.DrawTexture(new Rect(15, lastRect.yMax- 1f, Screen.width - 30, 1f),  EditorGUIUtility.whiteTexture);
			
			return lastRect;
		}
		
		private static Texture2D GetGradientTexture(){
			
			if (EditorStyling.gradientTexture==null){
				EditorStyling.gradientTexture = CreateGradientTexture(/*new Color(24, 168, 245), new Color(24, 98, 245)*/);
			}
			return gradientTexture;
		}
			
		private static Texture2D CreateGradientTexture(/*Color colFrom, Color colTo*/)
		{
			Texture2D myTexture = new Texture2D(1, 16);
			//myTexture.set_name("Gradient Texture by Hedgehog Team");
			myTexture.hideFlags = HideFlags.HideInInspector;
			myTexture.filterMode = FilterMode.Bilinear;
			myTexture.hideFlags = HideFlags.DontSave;
			float start= 0.4f;
			float end= 0.8f;
			float step = (end-start)/16;
			Color color = new Color(start,start,start);
			
			Color pixColor = color;
			for (int i = 0; i < 16; i++)
			{
				pixColor = new Color (pixColor.r+step, pixColor.b+step, pixColor.b+step,0.5f);
				myTexture.SetPixel(0, i, pixColor);
			}
			myTexture.Apply();
	
			return myTexture;
		}
		#endregion
		
	}
}

#endif