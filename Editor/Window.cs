
using System.IO;
using UnityEngine;
using UnityEditor;

namespace CubemapConverter
{
	public enum ConvertType
	{
		kFrom6SidedToCubemap,
		kFromPanoramaToCubemap,
		kFromPanoramaTo6Sided,
	}
	public class Window : EditorWindow
	{
		[MenuItem ("Tools/Cubemap Converter &C")]	
		static void ShowWindow() 
		{
			GetWindow<Window>().Show();	
		}
		void OnEnable()
		{
			if( importParam == null)
			{
				importParam = new ImportParam();
			}
			importParam.OnEnable( this);
			
			if( exportParam == null)
			{
				exportParam = new ExportParam();
			}
			exportParam.OnEnable( this);
		}
		void OnDisable()
		{
			if( importParam != null)
			{
				importParam.OnDisable();
			}
			if( exportParam != null)
			{
				exportParam.OnDisable();
			}
		}
		void OnGUI()
		{
			EditorGUI.BeginChangeCheck();
			
		//	var newConvertType = (ConvertType)EditorGUILayout.EnumPopup( "Convert Type", convertType);
			var newConvertType = (ConvertType)EditorGUILayout.EnumPopup( convertType);
			
			if( EditorGUI.EndChangeCheck() != false)
			{
				Record( "Change Convert Type");
				convertType = newConvertType;
			}
			importParam?.OnGUI( convertType);
			exportParam?.OnGUI( convertType);
			
			EditorGUILayout.BeginHorizontal();
			{
				GUILayout.FlexibleSpace();
				
				EditorGUI.BeginDisabledGroup( !exportParam.VerifyExport( convertType));
				bool export = GUILayout.Button( "    Export    ");
				EditorGUI.EndDisabledGroup();
				if( export != false)
				{
					System.Func<int, bool, Color[][]> importMethod = null;
					
					switch( convertType)
					{
						case ConvertType.kFrom6SidedToCubemap:
						{
							importMethod = importParam.GetTexturesFrom6Sided;
							break;
						}
						case ConvertType.kFromPanoramaToCubemap:
						case ConvertType.kFromPanoramaTo6Sided:
						{
							importMethod = importParam.GetTexturesFromPanorama;
							break;
						}
					}
					if( importMethod != null)
					{
						string extension;
						TextureFormat textureFormat;
						bool bExportEXR = true;
						Texture2D.EXRFlags exrFlags = Texture2D.EXRFlags.None;
						System.Func<Texture2D, Texture2D.EXRFlags, byte[]> encodeMethod;
						
						switch( exportParam.fileFormat)
						{
							case ExportFileFormat.kExr16:
							{
								extension = "exr";
								textureFormat = TextureFormat.RGBAHalf;
								encodeMethod = EncodeToEXR;
								break;
							}
							case ExportFileFormat.kExr16Zip:
							{
								extension = "exr";
								textureFormat = TextureFormat.RGBAHalf;
								exrFlags |= Texture2D.EXRFlags.CompressZIP;
								encodeMethod = EncodeToEXR;
								break;
							}
							case ExportFileFormat.kExr16Rle:
							{
								extension = "exr";
								textureFormat = TextureFormat.RGBAHalf;
								exrFlags |= Texture2D.EXRFlags.CompressRLE;
								encodeMethod = EncodeToEXR;
								break;
							}
							case ExportFileFormat.kExr32:
							{
								extension = "exr";
								textureFormat = TextureFormat.RGBAFloat;
								exrFlags |= Texture2D.EXRFlags.OutputAsFloat;
								encodeMethod = EncodeToEXR;
								break;
							}
							case ExportFileFormat.kExr32Zip:
							{
								extension = "exr";
								textureFormat = TextureFormat.RGBAFloat;
								exrFlags |= Texture2D.EXRFlags.OutputAsFloat;
								exrFlags |= Texture2D.EXRFlags.CompressZIP;
								encodeMethod = EncodeToEXR;
								break;
							}
							case ExportFileFormat.kExr32Rle:
							{
								extension = "exr";
								textureFormat = TextureFormat.RGBAFloat;
								exrFlags |= Texture2D.EXRFlags.OutputAsFloat;
								exrFlags |= Texture2D.EXRFlags.CompressRLE;
								encodeMethod = EncodeToEXR;
								break;
							}
							default: /* case ExportFileFormat.kPng: */
							{
								extension = "png";
								bExportEXR = false;
								textureFormat = TextureFormat.RGBA32;
								encodeMethod = EncodeToPNG;
								break;
							}
						}
						string savePath = EditorUtility.SaveFilePanelInProject( 
							"Cubemap Converter", string.Empty, extension, string.Empty);
						if( string.IsNullOrEmpty( savePath) == false)
						{
							Color[][] colors = importMethod( exportParam.resolution, bExportEXR);
							if( colors != null)
							{
								switch( convertType)
								{
									case ConvertType.kFrom6SidedToCubemap:
									case ConvertType.kFromPanoramaToCubemap:
									{
										Texture2D cubemap = CreateCubeTexture2D( colors, exportParam.resolution, textureFormat);
										if( cubemap != null)
										{
											byte[] bytes = encodeMethod( cubemap, exrFlags);
											DestroyImmediate( cubemap);
											File.WriteAllBytes( savePath, bytes);
											AssetDatabase.Refresh();
											
											var importer = TextureImporter.GetAtPath( savePath) as TextureImporter;
											if( importer != null)
											{
												importer.textureShape = TextureImporterShape.TextureCube;
												importer.wrapMode = TextureWrapMode.Clamp;
												AssetDatabase.ImportAsset( savePath);
											}
										}
										break;
									}
									case ConvertType.kFromPanoramaTo6Sided:
									{
										string directory = Path.GetDirectoryName( savePath);
										string fileName = Path.GetFileNameWithoutExtension( savePath);
										
										for( int i0 = 0; i0 < colors.Length; ++i0)
										{
											Texture2D texture = CreateTexture2D( colors[ i0], exportParam.resolution, textureFormat);
											if( texture != null)
											{
												savePath = Path.ChangeExtension( 
													string.Format( "{0}/{1}-{2}", 
														directory, fileName, exportParam.faceSuffixes[ i0]), extension);
												byte[] bytes = encodeMethod( texture, exrFlags);
												DestroyImmediate( texture);
												File.WriteAllBytes( savePath, bytes);
											}
										}
										break;
									}
								}
							}
						}
					}
					AssetDatabase.Refresh();
				}
				GUILayout.FlexibleSpace();
			}
			EditorGUILayout.EndHorizontal();
		}
		internal void Record( string label)
		{
			Undo.RecordObject( this, label);
		}
		static byte[] EncodeToPNG( Texture2D texture, Texture2D.EXRFlags exrFlags)
		{
			return texture.EncodeToPNG();
		}
		static byte[] EncodeToEXR( Texture2D texture, Texture2D.EXRFlags exrFlags)
		{
			return ImageConversion.EncodeToEXR( texture, exrFlags);
		}
		static Texture2D CreateTexture2D( Color[] colors, int resolution, TextureFormat format)
		{
			Texture2D texture = null;
			try
			{
				texture = new Texture2D( resolution, resolution, format, false, true);
				
				for( int y = 0; y < resolution; ++y)
				{
					int yOffset = y * resolution;
					
					for( int x = 0; x < resolution; ++x)
					{
						texture.SetPixel( x, y, colors[ x + yOffset]);
					}
				}
			}
			catch( System.Exception e)
			{
				if( texture != null)
				{
					DestroyImmediate( texture);
					texture = null;
				}
				Debug.LogError( e);
			}
			return texture;
		}
		static Texture2D CreateCubeTexture2D( Color[][] faceColors, int resolution, TextureFormat format)
		{
			Texture2D cubemap = null;
			try
			{
				cubemap = new Texture2D( resolution * faceColors.Length, resolution, format, false, true);
				
				for( int i0 = 0; i0 < faceColors.Length; ++i0)
				{
					int xOffset = i0 * resolution;
					Color[] colors = faceColors[ i0];
					
					for( int y = 0; y < resolution; ++y)
					{
						int yOffset = y * resolution;
						
						for( int x = 0; x < resolution; ++x)
						{
							cubemap.SetPixel( x + xOffset, y, colors[ x + yOffset]);
						}
						
						float progress = (float)y / (float)resolution / faceColors.Length;
						progress += 1.0f / faceColors.Length * i0;
						EditorUtility.DisplayProgressBar( "Cubemap Converter", "Export...", progress);
					}
				}
				cubemap.wrapMode = TextureWrapMode.Clamp;
				cubemap.Apply();
			}
			catch( System.Exception e)
			{
				if( cubemap != null)
				{
					DestroyImmediate( cubemap);
					cubemap = null;
				}
				Debug.LogError( e);
			}
			EditorUtility.ClearProgressBar();
			
			return cubemap;
		}
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
		ConvertType convertType = ConvertType.kFromPanoramaToCubemap;
		[SerializeField]
		ImportParam importParam = default;
		[SerializeField]
		ExportParam exportParam = default;
	}
}
