Shader "Curved/Curved" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
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

                        sampler2D _MainTex;
			float4 _QOffset;
			float _Dist;
			
			struct v2f {
			    float4 pos : SV_POSITION;
			    float4 uv : TEXCOORD0;
			};

			v2f vert (appdata_base v)
			{
			    v2f o;
			    float4 vPos = mul (UNITY_MATRIX_MV, v.vertex);
			    float zOff = vPos.z/_Dist;
			    vPos += _QOffset*zOff*zOff;
			    o.pos = mul (UNITY_MATRIX_P, vPos);
			    o.uv = v.texcoord;
			    return o;
			}

			half4 frag (v2f i) : COLOR
			{
			    half4 col = tex2D(_MainTex, i.uv.xy);
			    return col;
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
