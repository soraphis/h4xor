// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

Shader "Custom/YFadeOutShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_FogColor ("FogColor", Color) = (0,0,0,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		
		_StartY("StartY", Float) = -1.0
		_Falloff("Falloff", Float) = -5.0
		//[MaterialToggle] _isObjectSpace("isObjectSpace", Float) = 0

	}
	SubShader {
		//Tags { "RenderType" = "Opaque" }
		//Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Fade" }
		//Cull Back
		ZWrite On
		//ZTest Less
		ColorMask 0
		
		Blend SrcAlpha OneMinusSrcAlpha
		
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows alpha:blend

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

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
		fixed4 _FogColor;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			
			float y = -IN.worldPos.y + _StartY;

			//float f = 0;
			//if(_isObjectSpace < 0.5f){
				float f = min(1, max(0, y * _Falloff));
			//}else {
			//	// f = min(1, max(0, IN.uv_MainTex.y -0.5));
			//	c.r = min(1, max(0, IN.uv_MainTex.y - 0.25));
			//}
			// if (_QuadraticFallOff > 0.5f) f = 1 - (1-f)*(1-f);

			c.a = 1-(f*f*f);
			o.Albedo = c.rgb * (1-f)* (1-f) + _FogColor * (f)* (f);
			// o.Albedo = fixed3(f, 0, 0);
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic *(1-f);
			o.Smoothness = _Glossiness *(1-f);
			o.Alpha = c.a;

		}
		ENDCG
	}
	FallBack "Diffuse"
}
