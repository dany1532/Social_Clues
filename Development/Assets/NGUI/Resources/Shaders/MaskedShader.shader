Shader "Custom/MaskedShader"
{
    Properties
    {
      _Color ("Main Color", Color) = (1, 1, 1, 1)
      _Color2 ("Secondary Color", Color) = (1, 1, 1, 1)
      _MainTex ("Base (RGBA)", 2D) = "white" {}
      _Mask ("Culling Mask", 2D) = "white" {}
      _Cutoff ("Alpha cutoff", Range (0,1)) = 0.1
    }

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}
		
		LOD 100
		Cull Off
		Lighting Off
		ZWrite On
		Fog { Mode Off }
		ColorMask RGB
		AlphaTest GEqual [_Cutoff]
		Blend SrcAlpha OneMinusSrcAlpha
		
		Pass
		{
			ColorMaterial AmbientAndDiffuse
			SetTexture[_MainTex]
			{
				Combine Texture * Primary
			}
			
			SetTexture [_MainTex]
			{
				ConstantColor [_Color]
				Combine Previous * Constant
			}
		}
				
		Pass
		{
			ColorMaterial AmbientAndDiffuse
			
          	SetTexture [_Mask]
          	{
				ConstantColor [_Color2]
				Combine Texture * Constant
          	}
		}
	}
}