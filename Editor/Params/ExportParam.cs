
using UnityEngine;
using UnityEditor;

namespace CubemapConverter
{
	public enum ExportFileFormat
	{
		kPng,
		kExr16,
		kExr16Zip,
		kExr16Rle,
		kExr32,
		kExr32Zip,
		kExr32Rle,
	}
	[System.Serializable]
	public class ExportParam : BaseParam
	{
		public ExportParam() : base( true)
		{
		}
		public override void OnEnable( Window window)
		{
			base.OnEnable( window);
			
			if( faceSuffixes == null)
			{
				faceSuffixes = new string[ kFaceNames.Length];
				
				for( int i0 = 0; i0 < faceSuffixes.Length; ++i0)
				{
					faceSuffixes[ i0] = kFaceNames[ i0];
				}
			}
		}
		public override void OnGUI( ConvertType convertType)
		{
			OnPUI( "Export Settings", () =>
			{
				EditorGUI.BeginChangeCheck();
				int newResolution = EditorGUILayout.IntPopup( "Resolution", 
					resolution, kResolutionNames, kResolutions);
				if( EditorGUI.EndChangeCheck() != false)
				{
					Record( "Change Resolution Size");
					resolution = newResolution;
				}
				
				EditorGUI.BeginChangeCheck();
				var newFileFormat = (ExportFileFormat)EditorGUILayout.EnumPopup( "File Format", fileFormat);
				if( EditorGUI.EndChangeCheck() != false)
				{
					Record( "Change File Format");
					fileFormat = newFileFormat;
				}
				
				if( convertType == ConvertType.kFromPanoramaTo6Sided)
				{
					bool bErrorFaceSuffixes = false;
					
					for( int i0 = 0; i0 < faceSuffixes.Length; ++i0)
					{
						EditorGUI.BeginChangeCheck();
						var newSuffix = EditorGUILayout.TextField( kFaceNames[ i0] + " Suffix", faceSuffixes[ i0]);
						if( EditorGUI.EndChangeCheck() != false)
						{
							Record( "Change File Format");
							faceSuffixes[ i0] = newSuffix;
						}
						if( string.IsNullOrEmpty( faceSuffixes[ i0]) != false)
						{
							bErrorFaceSuffixes = true;
						}
					}
					if( bErrorFaceSuffixes != false)
					{
						EditorGUILayout.HelpBox( "Export するにはすべての Suffix を入力して下さい", MessageType.Error);
					}
				}
			});
		}
		internal bool VerifyExport( ConvertType convertType)
		{
			if( convertType == ConvertType.kFromPanoramaTo6Sided)
			{
				for( int i0 = 0; i0 < faceSuffixes.Length; ++i0)
				{
					if( string.IsNullOrEmpty( faceSuffixes[ i0]) != false)
					{
						return false;
					}
				}
			}
			return true;
		}
		
		static readonly string[] kResolutionNames = new string[]
		{
			"16", "32", "64", "128", "256", "512", "1024", "2048"
		};
		static readonly int[] kResolutions = new int[]
		{
			16, 32, 64, 128, 256, 512, 1024, 2048
		};
		static readonly string[] kFaceNames = new string[]
		{
			"Left",
			"Right",
			"Top",
			"Bottom",
			"Front",
			"Back"
		};
		
		[SerializeField]
		internal int resolution = 512;
		[SerializeField]
		internal ExportFileFormat fileFormat = ExportFileFormat.kPng;
		[SerializeField]
		internal string[] faceSuffixes = null;
	}
}
