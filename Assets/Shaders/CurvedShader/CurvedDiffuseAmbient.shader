Shader "Curved/CurvedDiffuseAmbient" {
	Properties {
		//_MainTex ("Base (RGB)", 2D) = "white" {}
		[NoScaleOffset] _MainTex("Texture", 2D) = "white" {}
		_QOffset ("Offset", Vector) = (0,0,0,0)
		_Dist ("Distance", Float) = 100.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" "LightMode" = "ForwardBase" }
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "UnityLightingCommon.cginc" // for _LightColor0

            sampler2D _MainTex;
			float4 _QOffset;
			float _Dist;
			
			struct v2f {
			    float4 vertex : SV_POSITION;
				fixed4 diff : COLOR0; // diffuse lighting color
			    float4 uv : TEXCOORD0;
			};

			v2f vert (appdata_base v)
			{
			    v2f o;

				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord;
				// get vertex normal in world space
				half3 worldNormal = UnityObjectToWorldNormal(v.normal);
				// dot product between normal and light direction for
				// standard diffuse (Lambert) lighting
				half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
				// factor in the light color
				o.diff = nl * _LightColor0;

			    float4 vPos = mul (UNITY_MATRIX_MV, v.vertex);
			    float zOff = vPos.z/_Dist;
			    vPos += _QOffset*zOff*zOff;
			    o.vertex = mul (UNITY_MATRIX_P, vPos);
			    o.uv = v.texcoord;

				// the only difference from previous shader:
				// in addition to the diffuse lighting from the main light,
				// add illumination from ambient or light probes
				// ShadeSH9 function from UnityCG.cginc evaluates it,
				// using world space normal
				o.diff.rgb += ShadeSH9(half4(worldNormal, 1));

			    return o;
			}

			half4 frag (v2f i) : COLOR
			{
			    half4 col = tex2D(_MainTex, i.uv.xy);
				// multiply by lighting
				col *= i.diff;
			    return col;
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
