Shader "Curved/CurvedDiffuse" {
	Properties {
		//[NoScaleOffset] _MainTex("Texture", 2D) = "white" {}
		//_MainTex ("Base (RGB)", 2D) = "white" {}
		_MainTex("Texture", 2D) = "white" {}
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

			// make fog work
			#pragma multi_compile_fog

			#include "UnityCG.cginc"
			#include "UnityLightingCommon.cginc" // for _LightColor0

            sampler2D _MainTex;
			float4 _QOffset;
			float _Dist;
			float4 _MainTex_ST;

			struct v2f {
				float4 vertex : SV_POSITION;
				fixed4 diff : COLOR0; // diffuse lighting color
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
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
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex); //v.texcoord;
															//o.uv = v.texcoord;
				UNITY_TRANSFER_FOG(o,o.vertex);
			    return o;
			}

			half4 frag (v2f i) : COLOR
			{
			    half4 col = tex2D(_MainTex, i.uv.xy);
				// multiply by lighting
				col *= i.diff;
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
			    return col;
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
