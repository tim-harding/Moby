Shader "Custom/LitBend" {
	Properties{
		_Width ("Width", Int) = 128
		_Offset ("Offset", Float) = 0
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows vertex:vert addshadow
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
			float3 worldNormal;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		uint _Width;
		float _Offset;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void vert(inout appdata_full v) {
			static const float PI = 3.1415926535f;
			float4 wPos = mul(unity_ObjectToWorld, v.vertex);
			wPos.x += _Offset;
			float fraction = (wPos.x + _Width / 2) / _Width;

			float halfTheta = fraction * PI * 4;
			float theta = halfTheta * 2;
			float3x3 yAxisRot = {
				cos(-theta), 0, sin(-theta),
				0, 1, 0,
				-sin(-theta), 0, cos(-theta)
			};
			float3x3 zAxisRot = {
				1, 0, 0,
				0, cos(halfTheta), -sin(halfTheta),
				0, sin(halfTheta), cos(halfTheta)
			};
			float3x3 rot = mul(yAxisRot, zAxisRot);
			
			wPos.x = 0;
			wPos.xyz = mul(rot, wPos);

			float radius = _Width / PI / 8;
			wPos.x += -1 * radius * sin(-theta);
			wPos.z += -1 * radius * cos(-theta);

			v.normal = mul(unity_ObjectToWorld, v.normal);
			v.normal = mul(rot, v.normal); 
			v.normal = mul(unity_WorldToObject, v.normal);

			v.vertex = mul(unity_WorldToObject, wPos);
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			//o.Albedo = float3(0, 0, 0);
			//o.Emission = IN.worldNormal;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
