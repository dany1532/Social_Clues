Shader "Custom/PuzzlePieceShader" {
	Properties
	{
		_MainTex ("MainTex(RGBA)", 2D) = "white" {}
		_OutlineTex ("OutlineTex(RGBA)", 2D) = "white" {}
		_MainColor("MainColor", COLOR) = (1,1,1,1)
		_OutlineColor("OutlineColor", COLOR) = (0,0,0,1)
	}
	
	SubShader
	{
		LOD 100

		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}
		
		Cull Off
		Lighting Off
		ZWrite Off
		Fog { Mode Off }
		Offset -1, -1
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				
				#include "UnityCG.cginc"
	
				struct appdata_t
				{
					float4 vertex : POSITION;
					float2 texcoordOutline : TEXCOORD0;
					float2 texcoordMain: TEXCOORD1;
					fixed4 color : COLOR;
				};
	
				struct v2f
				{
					float4 vertex : SV_POSITION;
					half2 texcoordOutline : TEXCOORD0;
					half2 texcoordMain : TEXCOORD1;
					fixed4 color : COLOR;
				};
	
				sampler2D _OutlineTex;
				sampler2D _MainTex;
				float4 _MainColor;
				float4 _OutlineColor;
				
				float4 _MainTex_ST;
				float4 _OutlineTex_ST;
				
				v2f vert (appdata_t v)
				{
					v2f o;
					o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
					o.texcoordOutline = TRANSFORM_TEX(v.texcoordOutline, _OutlineTex);
					o.texcoordMain = TRANSFORM_TEX(v.texcoordMain, _MainTex);
					
					o.texcoordOutline = o.texcoordOutline.xy * _OutlineTex_ST.xy + _OutlineTex_ST.zw;
					o.texcoordMain = o.texcoordMain.xy * _MainTex_ST.xy + _MainTex_ST.zw;
					
					o.color = v.color;
					return o;
				}
				
				fixed4 frag (v2f i) : COLOR
				{
					fixed4 colOutline = tex2D(_OutlineTex, i.texcoordOutline);// * i.color;
					fixed4 colorPuzzle = tex2D(_MainTex, i.texcoordMain) * i.color;
					
					if(colOutline.r != _MainColor.r || colOutline.g != _MainColor.g || colOutline.b != _MainColor.b){
						colorPuzzle.a = 0;
					}
					return colorPuzzle;
				}
			ENDCG
		}
	}
}
