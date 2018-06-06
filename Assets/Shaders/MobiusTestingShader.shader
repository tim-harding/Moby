Shader "Custom/MobiusTestingShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		Cull Off

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
			float3 viewDir;
			float3 worldNormal; INTERNAL_DATA
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			fixed4 color = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = color.rgb;
			o.Alpha = color.a;

			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;

			o.Normal = dot(IN.viewDir, float3(0, 0, 1)) > 0 ? float3(0, 0, 1) : float3(0, 0, -1); 
		}
		ENDCG
	}
	FallBack "Diffuse"
}
