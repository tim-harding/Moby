// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/BendShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Width ("Width", Int) = 128
		_Height ("Height", Int) = 4
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			Cull Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float3 normal : TEXCOORD1;
				float3 view : TEXCOORD2;
				float fraction : TEXCOORD3;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			int _Width;
			int _Height;
			
			v2f vert (appdata v)
			{
				v2f o;

				const float PI = 3.1415926535;
				float4 wPos = mul(unity_ObjectToWorld, v.vertex);
				float width = (float)_Width;
				float height = (float)_Height;
				float fraction = (wPos.x + width / 2) / width;
				o.fraction = fraction;

				float theta = fraction * PI * 8;
				float3x3 yAxisRot = {
					cos(theta), 0, sin(theta),
					0, 1, 0,
					-sin(theta), 0, cos(theta)
				};
				theta = fraction * PI * 4;
				float3x3 zAxisRot = {
					1, 0, 0,
					0, cos(theta), -sin(theta),
					0, sin(theta), cos(theta)
				};
				float3x3 rot = mul(yAxisRot, zAxisRot);

				wPos.x = 0;
				wPos.xyz = mul(rot, wPos);

				float radius = _Width / PI / 8;
				theta = fraction * PI * 8;
				wPos.x += radius * sin(theta);
				wPos.z += radius * cos(theta);

				o.normal = mul(rot, v.normal);
				o.view = WorldSpaceViewDir(v.vertex);
				o.vertex = UnityObjectToClipPos(mul(unity_WorldToObject, wPos));
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{	
				int pattern = (int)(i.uv.x * 2) ^ (int)(i.uv.y * 2);
				float remap = pattern ? 0.75 : 0.25;

				float facingRatio = abs(dot(i.normal, normalize(i.view)));
				//float4 col = { facingRatio, facingRatio, facingRatio, 1 };
				float4 col = { 1, 1, 1, 1 };
				col *= remap;
				col *= float4((i.normal.xyz + 1) / 2, 1);
				return col;
			}
			ENDCG
		}
	}
}