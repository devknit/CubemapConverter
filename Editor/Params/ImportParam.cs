
using UnityEngine;
using UnityEditor;

namespace CubemapConverter
{
	[System.Serializable]
	public class ImportParam : BaseParam
	{
		public ImportParam() : base( true)
		{
		}
		public override void OnGUI( ConvertType convertType)
		{
			OnPUI( "Import Settings", () =>
			{
				switch( convertType)
				{
					case ConvertType.kFrom6SidedToCubemap:
					{
						OnSixSidedGUI();
						break;
					}
					case ConvertType.kFromPanoramaToCubemap:
					case ConvertType.kFromPanoramaTo6Sided:
					{
						OnPanoramaGUI();
						break;
					}
				}
				EditorGUILayout.Space();
				EditorGUI.BeginChangeCheck();
				float newDirection = EditorGUILayout.Slider( "Direction", direction, -180.0f, 180.0f);
				if( EditorGUI.EndChangeCheck() != false)
				{
					Record( "Change Direction");
					direction = newDirection;
				}
				EditorGUI.BeginChangeCheck();
				bool newExrToLinear = EditorGUILayout.Toggle( "EXR to Linear Space", exrToLinear);
				if( EditorGUI.EndChangeCheck() != false)
				{
					Record( "Change XR to Linear Space");
					exrToLinear = newExrToLinear;
				}
			});
		}
		void OnSixSidedGUI()
		{
			float width = 64 + EditorGUI.indentLevel * 15;
			const float height = 64;
			
			EditorGUILayout.HelpBox( "Resolution の値と一致する解像度のテクスチャを設定してください", MessageType.Info);
			
			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.BeginVertical();
				EditorGUILayout.LabelField( "Left (+X)");
				EditorGUI.BeginChangeCheck();
				var newLeftTexture = EditorGUILayout.ObjectField( leftTexture, typeof( Texture2D), false, 
					GUILayout.MinWidth( width), GUILayout.MaxWidth( width), GUILayout.MinHeight( height), GUILayout.MaxHeight( height)) as Texture2D;
				if( EditorGUI.EndChangeCheck() != false)
				{
					Record( "Change Left Texture");
					leftTexture = newLeftTexture;
				}
				EditorGUILayout.EndVertical();
			}
			{
				EditorGUILayout.BeginVertical();
				EditorGUILayout.LabelField( "Right (-X)");
				EditorGUI.BeginChangeCheck();
				var newRightTexture = EditorGUILayout.ObjectField( rightTexture, typeof( Texture2D), false, 
					GUILayout.MinWidth( width), GUILayout.MaxWidth( width), GUILayout.MinHeight( height), GUILayout.MaxHeight( height)) as Texture2D;
				if( EditorGUI.EndChangeCheck() != false)
				{
					Record( "Change Right Texture");
					rightTexture = newRightTexture;
				}
				EditorGUILayout.EndVertical();
			}
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();
			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.BeginVertical();
				EditorGUILayout.LabelField( "Top (+Y)");
				EditorGUI.BeginChangeCheck();
				var newTopTexture = EditorGUILayout.ObjectField( topTexture, typeof( Texture2D), false, 
					GUILayout.MinWidth( width), GUILayout.MaxWidth( width), GUILayout.MinHeight( height), GUILayout.MaxHeight( height)) as Texture2D;
				if( EditorGUI.EndChangeCheck() != false)
				{
					Record( "Change Top Texture");
					topTexture = newTopTexture;
				}
				EditorGUILayout.EndVertical();
			}
			{
				EditorGUILayout.BeginVertical();
				EditorGUILayout.LabelField( "Bottom (-Y)");
				EditorGUI.BeginChangeCheck();
				var newBottomTexture = EditorGUILayout.ObjectField( bottomTexture, typeof( Texture2D), false, 
					GUILayout.MinWidth( width), GUILayout.MaxWidth( width), GUILayout.MinHeight( height), GUILayout.MaxHeight( height)) as Texture2D;
				if( EditorGUI.EndChangeCheck() != false)
				{
					Record( "Change Bottom Texture");
					bottomTexture = newBottomTexture;
				}
				EditorGUILayout.EndVertical();
			}
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();
			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.BeginVertical();
				EditorGUILayout.LabelField( "Front (+Z)");
				EditorGUI.BeginChangeCheck();
				var newFrontTexture = EditorGUILayout.ObjectField( frontTexture, typeof( Texture2D), false, 
					GUILayout.MinWidth( width), GUILayout.MaxWidth( width), GUILayout.MinHeight( height), GUILayout.MaxHeight( height)) as Texture2D;
				if( EditorGUI.EndChangeCheck() != false)
				{
					Record( "Change Front Texture");
					frontTexture = newFrontTexture;
				}
				EditorGUILayout.EndVertical();
			}
			{
				EditorGUILayout.BeginVertical();
				EditorGUILayout.LabelField( "Back (-Z)");
				EditorGUI.BeginChangeCheck();
				var newBackTexture = EditorGUILayout.ObjectField( backTexture, typeof( Texture2D), false, 
					GUILayout.MinWidth( 64), GUILayout.MaxWidth( width), GUILayout.MinHeight( height), GUILayout.MaxHeight( height)) as Texture2D;
				if( EditorGUI.EndChangeCheck() != false)
				{
					Record( "Change Back Texture");
					backTexture = newBackTexture;
				}
				EditorGUILayout.EndVertical();
			}
			EditorGUILayout.EndHorizontal();
		}
		void OnPanoramaGUI()
		{
			const int width = 256;
			const int height = 128;
			
			EditorGUILayout.BeginHorizontal();
			{
				GUILayout.FlexibleSpace();
				EditorGUI.BeginChangeCheck();
				var newTexture = EditorGUILayout.ObjectField( panoramaTexture, typeof( Texture2D), false, 
					GUILayout.MinWidth( width), GUILayout.MaxWidth( width), GUILayout.MinHeight( height), GUILayout.MaxHeight( height)) as Texture2D;
				if( EditorGUI.EndChangeCheck() != false)
				{
					Record( "Change Panorama Texture");
					panoramaTexture = newTexture;
				}
				GUILayout.FlexibleSpace();
			}
			EditorGUILayout.EndHorizontal();
		}
		internal Color[][] GetTexturesFrom6Sided( int resolution, bool toLinear)
		{
			var textures = new Texture2D[]
			{
				leftTexture,
				rightTexture,
				topTexture,
				bottomTexture,
				frontTexture,
				backTexture
			};
			for( int i0 = 0; i0 < textures.Length; ++i0)
			{
				if( resolution != VerifyTexture( textures[ i0]))
				{
					Debug.LogError( "Resolution と一致しない解像度のテクスチャが設定されています");
					return null;
				}
			}
			var colors = new Color[ textures.Length][];
			
			for( int i0 = 0; i0 < textures.Length; ++i0)
			{
				colors[ i0] = GetPixels( textures[ i0], toLinear);
			}
			return colors;
		}
		internal Color[][] GetTexturesFromPanorama( int resolution, bool toLinear)
		{
			if( panoramaTexture == null)
			{
				return null;
			}
			
			Color[] panoramaColors = GetPixels( panoramaTexture, toLinear);
			int width = panoramaTexture.width;
			int height = panoramaTexture.height;
			float directionOffset = direction;
			float totalProgress = 0.0f;
			
			var colors = new Color[ 6][];
			colors[ 0] = CreateCubemapSidedPixels( panoramaColors, width, height, 
				CubemapFace.NegativeX, resolution, directionOffset, (progress) =>
				{
					EditorUtility.DisplayProgressBar( "Cubemap Converter", "Import...", progress / colors.Length + totalProgress);
				});
			totalProgress += 1.0f / colors.Length;
			
			colors[ 1] = CreateCubemapSidedPixels( panoramaColors, width, height, 
				CubemapFace.PositiveX, resolution, directionOffset, (progress) =>
				{
					EditorUtility.DisplayProgressBar( "Cubemap Converter", "Import...", progress / colors.Length + totalProgress);
				});
			totalProgress += 1.0f / colors.Length;
			
			colors[ 2] = CreateCubemapSidedPixels( panoramaColors, width, height, 
				CubemapFace.PositiveY, resolution, directionOffset, (progress) =>
				{
					EditorUtility.DisplayProgressBar( "Cubemap Converter", "Import...", progress / colors.Length + totalProgress);
				});
			totalProgress += 1.0f / colors.Length;
			
			colors[ 3] = CreateCubemapSidedPixels( panoramaColors, width, height, 
				CubemapFace.NegativeY, resolution, directionOffset, (progress) =>
				{
					EditorUtility.DisplayProgressBar( "Cubemap Converter", "Import...", progress / colors.Length + totalProgress);
				});
			totalProgress += 1.0f / colors.Length;
			
			colors[ 4] = CreateCubemapSidedPixels( panoramaColors, width, height, 
				CubemapFace.PositiveZ, resolution, directionOffset, (progress) =>
				{
					EditorUtility.DisplayProgressBar( "Cubemap Converter", "Import...", progress / colors.Length + totalProgress);
				});
			totalProgress += 1.0f / colors.Length;
			
			colors[ 5] = CreateCubemapSidedPixels( panoramaColors, width, height, 
				CubemapFace.NegativeZ, resolution, directionOffset, (progress) =>
				{
					EditorUtility.DisplayProgressBar( "Cubemap Converter", "Import...", progress / colors.Length + totalProgress);
				});
			
			EditorUtility.ClearProgressBar();
			return colors;
		}
		Color[] GetPixels( Texture2D texture, bool toLinear)
		{
			Color[] ret = null;
			
			string assetPath = AssetDatabase.GetAssetPath( texture);
			if( string.IsNullOrEmpty( assetPath) == false)
			{
				TextureImporter importer = TextureImporter.GetAtPath( assetPath) as TextureImporter;
				string extension = System.IO.Path.GetExtension( assetPath).ToLower();
				TextureImporterFormat importFormat;
				
				if( extension == ".exr" || extension == ".hdr")
				{
					importFormat = TextureImporterFormat.RGBAHalf;
				}
				else
				{
					importFormat = TextureImporterFormat.RGBA32;
				}
				
				var overrideTextureImporterSettings = new TextureImporterSettings();
				var textureImporterSettings = new TextureImporterSettings();
				importer.ReadTextureSettings( overrideTextureImporterSettings);
				importer.ReadTextureSettings( textureImporterSettings);
				
				var defaultPlatformSettings = importer.GetDefaultPlatformTextureSettings();
				var overrideDefaultPlatformSettings = new TextureImporterPlatformSettings();
				defaultPlatformSettings.CopyTo( overrideDefaultPlatformSettings);
				
				var targetPlatformSettings = importer.GetPlatformTextureSettings( GetBuildTargetPlatformName());
				var overrideTargetPlatformSettings = new TextureImporterPlatformSettings();
				targetPlatformSettings.CopyTo( overrideTargetPlatformSettings);
				
				overrideTextureImporterSettings.readable = true;
				overrideTextureImporterSettings.sRGBTexture = false; /* GetPixels には関係ない？ */
				
				overrideDefaultPlatformSettings.format = TextureImporterFormat.Automatic;
				overrideDefaultPlatformSettings.textureCompression = TextureImporterCompression.Uncompressed;
				overrideDefaultPlatformSettings.crunchedCompression = false;
				
				overrideTargetPlatformSettings.overridden = true;
				overrideTargetPlatformSettings.format = importFormat;
				overrideTargetPlatformSettings.textureCompression = TextureImporterCompression.Uncompressed;
				overrideTargetPlatformSettings.crunchedCompression = false;
				
				importer.SetTextureSettings( overrideTextureImporterSettings);
				importer.SetPlatformTextureSettings( overrideDefaultPlatformSettings);
				importer.SetPlatformTextureSettings( overrideTargetPlatformSettings);
				AssetDatabase.ImportAsset( assetPath, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
				
				ret = texture.GetPixels( 0);
				
				if( toLinear != false && exrToLinear != false)
				{
					for( int i0 = 0; i0 < ret.Length; ++i0)
					{
					#if true
						ret[ i0].r = Mathf.Pow( ret[ i0].r, 2.2f);
						ret[ i0].g = Mathf.Pow( ret[ i0].g, 2.2f);
						ret[ i0].b = Mathf.Pow( ret[ i0].b, 2.2f);
					#else
						ret[ i0].r = (float)((int)(Mathf.Pow( ret[ i0].r, 2.2f) * 255.0f) / 255.0f);
						ret[ i0].g = (float)((int)(Mathf.Pow( ret[ i0].g, 2.2f) * 255.0f) / 255.0f);
						ret[ i0].b = (float)((int)(Mathf.Pow( ret[ i0].b, 2.2f) * 255.0f) / 255.0f);
					#endif
					}
				}
				importer.SetTextureSettings( textureImporterSettings);
				importer.SetPlatformTextureSettings( defaultPlatformSettings);
				importer.SetPlatformTextureSettings( targetPlatformSettings);
				AssetDatabase.ImportAsset( assetPath, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
			}
			return ret;
		}
		static Color[] CreateCubemapSidedPixels( Color[] panoramaColors, int width, int height, 
			CubemapFace face, int resolution, float directionOffset, System.Action<float> progress)
		{
			Color[] colors = null;
			
			try
			{
				Vector3[] directions = GetCubemapDirections( face);
				
				Vector3 rotDx1 = (directions[ 1] - directions[ 0]) / (float)resolution;
				Vector3 rotDx2 = (directions[ 3] - directions[ 2]) / (float)resolution;
				
				float dy = 1.0f / (float)resolution;
				float fy = 0.0f;
				
				colors = new Color[ resolution * resolution];
				
				for( int y = 0; y < resolution; ++y)
				{
					Vector3 xv1 = directions[ 0];
					Vector3 xv2 = directions[ 2];
					int yOffset = y * resolution;
					
					for( int x = 0; x < resolution; ++x)
					{
						Vector3 direction = ((xv2 - xv1) * fy) + xv1;
						direction.Normalize();
						colors[ x + yOffset] = GetPixelProjectionSpherical( panoramaColors, width, height, direction, directionOffset);
						xv1 += rotDx1;
						xv2 += rotDx2;
					}
					fy += dy;
					progress?.Invoke( (float)y / (float)resolution);
				}
			}
			catch( System.Exception e)
			{
				Debug.LogError( e);
			}
			return colors;
		}
		static Color GetPixelProjectionSpherical( 
			Color[] panoramaColors, int width, int height, 
			Vector3 direction, float directionOffset)
		{
			float theta = Mathf.Atan2( direction.z, direction.x);	/* -π ～ +π（水平方向の円周上の回転）*/
			float phi = Mathf.Acos( direction.y);					/*  0  ～ +π（垂直方向の回転）*/
			
			theta += directionOffset * Mathf.PI / 180.0f;
			
			while( theta < -Mathf.PI)
			{
				theta += Mathf.PI + Mathf.PI;
			}
			while( theta > Mathf.PI)
			{
				theta -= Mathf.PI + Mathf.PI;
			}
			
			float dx = theta / Mathf.PI;	/* -1.0 ～ +1.0 */
			float dy = phi / Mathf.PI;		/*  0.0 ～ +1.0 */
			
			dx = dx * 0.5f + 0.5f;
			
			int px = (int)(dx * (float)width);
			if( px < 0)
			{
				px = 0;
			}
			if( px >= width)
			{
				px = width - 1;
			}
			int py = (int)(dy * (float)height);
			if( py < 0)
			{
				py = 0;
			}
			if( py >= height)
			{
				py = height - 1;
			}
			return panoramaColors[ px + width * (height - py - 1)];
		}
		static int VerifyTexture( Texture2D texture)
		{
			if( texture == null)
			{
				return -1;
			}
			if( texture.width != texture.height)
			{
				return -2;
			}
			if( IsPowerOfTwo( texture.width) == false
			||	IsPowerOfTwo( texture.height) == false)
			{
				return -4;
			}
			return texture.width;
		}
		static bool IsPowerOfTwo( int value)
		{
			return (value <= 0)? false : (value & (value - 1)) == 0;
		}
		static string GetBuildTargetPlatformName()
		{
			switch( EditorUserBuildSettings.activeBuildTarget)
			{
				case BuildTarget.StandaloneWindows64:
				case BuildTarget.StandaloneWindows:
				case BuildTarget.StandaloneLinux64:
			#if !UNITY_2019_2_OR_NEWER
				case BuildTarget.StandaloneLinuxUniversal:
				case BuildTarget.StandaloneLinux:
			#endif
				case BuildTarget.StandaloneOSX:
				{
					return "Standalone";
				}
				case BuildTarget.iOS:
				{
					return "iPhone";
				}
				case BuildTarget.Android:
				{
					return "Android";
				}
				case BuildTarget.tvOS:
				{
					return "tvOS";
				}
				case BuildTarget.WebGL:
				{
					return "WebGL";
				}
				case BuildTarget.WSAPlayer:
				{
					return "Windows Store Apps";
				}
				case BuildTarget.PS4:
				{
					return "PS4";
				}
				case BuildTarget.XboxOne:
				{
					return "XboxOne";
				}
			}
			return string.Empty;
		}
		static Vector3[] GetCubemapDirections( CubemapFace face)
		{
			switch( face)
			{
				case CubemapFace.PositiveX:
				{
					return kDirections[ 0];
				}
				case CubemapFace.NegativeX:
				{
					return kDirections[ 1];
				}
				case CubemapFace.PositiveY:
				{
					return kDirections[ 2];
				}
				case CubemapFace.NegativeY:
				{
					return kDirections[ 3];
				}
				case CubemapFace.PositiveZ:
				{
					return kDirections[ 4];
				}
				case CubemapFace.NegativeZ:
				{
					return kDirections[ 5];
				}
			}
			return null;
		}
		static readonly Vector3[][] kDirections = new Vector3[][]
		{
			/* Right */
			new Vector3[]
			{
				new Vector3( -1.0f, -1.0f,  1.0f),
				new Vector3( -1.0f, -1.0f, -1.0f),
				new Vector3( -1.0f,  1.0f,  1.0f),
				new Vector3( -1.0f,  1.0f, -1.0f)
			},
			/* Left */
			new Vector3[]
			{
				new Vector3(  1.0f, -1.0f, -1.0f),
				new Vector3(  1.0f, -1.0f,  1.0f),
				new Vector3(  1.0f,  1.0f, -1.0f),
				new Vector3(  1.0f,  1.0f,  1.0f)
			},
			/* Top */
			new Vector3[]
			{
				new Vector3( -1.0f,  1.0f, -1.0f),
				new Vector3(  1.0f,  1.0f, -1.0f),
				new Vector3( -1.0f,  1.0f,  1.0f),
				new Vector3(  1.0f,  1.0f,  1.0f)
			},
			/* Bottom */
			new Vector3[]
			{
				new Vector3( -1.0f, -1.0f,  1.0f),
				new Vector3(  1.0f, -1.0f,  1.0f),
				new Vector3( -1.0f, -1.0f, -1.0f),
				new Vector3(  1.0f, -1.0f, -1.0f)
			},
			/* Front */
			new Vector3[]
			{
				new Vector3( -1.0f, -1.0f, -1.0f),
				new Vector3(  1.0f, -1.0f, -1.0f),
				new Vector3( -1.0f,  1.0f, -1.0f),
				new Vector3(  1.0f,  1.0f, -1.0f)
			},
			/* Back */
			new Vector3[]
			{
				new Vector3(  1.0f, -1.0f, 1.0f),
				new Vector3( -1.0f, -1.0f, 1.0f),
				new Vector3(  1.0f,  1.0f, 1.0f),
				new Vector3( -1.0f,  1.0f, 1.0f)
			}
		};
		
		[SerializeField]
		internal Texture2D panoramaTexture = default;
		[SerializeField]
		internal Texture2D rightTexture = default;
		[SerializeField]
		internal Texture2D leftTexture = default;
		[SerializeField]
		internal Texture2D topTexture = default;
		[SerializeField]
		internal Texture2D bottomTexture = default;
		[SerializeField]
		internal Texture2D frontTexture = default;
		[SerializeField]
		internal Texture2D backTexture = default;
		[SerializeField]
		internal float direction = 0.0f;
		[SerializeField]
		internal bool exrToLinear = true;
	}
}
