Shader "Hidden/ScanlineShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Scale ("Scaling", float) = 1.0
		_LightAmount ("Light Amount", float) = 0.2
		_Offset ("Offset", float) = 10
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always
		Pass
		{
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			float _Scale;
			float _LightAmount;
			float _Offset;
			
			fixed4 frag (v2f i) : SV_Target
			{
			    float4 array = float4(-1, 0, 1, 0);
				fixed4 col = tex2D(_MainTex, i.uv);
				// just invert the colors
				
				float offset = _Offset * _SinTime.x / 8;
				
				float f = array[ (int)( (i.uv.y + offset) * _Scale)% 4 ] *_LightAmount;
				col.r += f;
				col.g += f;
				col.b += f;
												
				return col;
			}
			ENDCG
		}
	}
}
