// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

Shader "Custom/GridShader T" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_DetailTex ("Detail Texture", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		
		_DetailOffset("DetailOffset", Vector) = (0, 0, 0 ,0)
		
		//[MaterialToggle] _isObjectSpace("isObjectSpace", Float) = 0

	}
	SubShader {
		Tags { "RenderType"="Transparent" "IgnoreProjector" = "True" "RenderType" = "Fade"}
		LOD 200
		
		ZTest Always
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows alpha:blend

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0


		sampler2D _MainTex;
		sampler2D _DetailTex;
		float4 _DetailOffset;

		struct Input {
			float2 uv_MainTex : TEXCOORD0;
			float3 worldPos;

			float alpha;
		};

		half _Glossiness;
		half _Metallic;
		
		half _StartY;
		half _Falloff;
		half _QuadraticFallOff;

		fixed4 _Color;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			
			fixed4 c_detail = tex2D (_DetailTex, float2(IN.worldPos.x + _DetailOffset.x, IN.worldPos.z + _DetailOffset.y));

			// c.a = 1;
			o.Albedo = c_detail.rgb * c_detail.r + c.rgb * (1 - c_detail.r);
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;

		}
		ENDCG
	}
	FallBack "Diffuse"
}
