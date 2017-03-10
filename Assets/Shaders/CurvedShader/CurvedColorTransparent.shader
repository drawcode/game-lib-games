Shader "Curved/CurvedColorTransparent" {
	Properties {
		_ColorTint("Tint", Color) = (1.0, 0.6, 0.6, 1.0)
		_MainTex("Color (RGB) Alpha (A)", 2D) = "white"
		_QOffset ("Offset", Vector) = (0,0,0,0)
		_Dist ("Distance", Float) = 100.0
	}
	SubShader { 
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }          
		Lighting Off
		cull off
		//ztest always
		Zwrite off
		Fog{ Mode Off }
		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

            sampler2D _MainTex;
			float4 _QOffset;
			float _Dist;
			float4 _MainTex_ST;
			float4 _ColorTint;

			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f vert (appdata_base v)
			{
			    v2f o;
			    float4 vPos = mul (UNITY_MATRIX_MV, v.vertex);
			    float zOff = vPos.z/_Dist;
			    vPos += _QOffset*zOff*zOff;
				
			    o.pos = mul (UNITY_MATRIX_P, vPos);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex); //v.texcoord;
				
			    return o;
			}

			half4 frag(v2f i) : COLOR
			{
				half4 col = tex2D(_MainTex, i.uv.xy) * _ColorTint;
				//col.rgb = _ColorTint.rgb;
				//col.a = _ColorTint.a;
			    return col;
			}
			ENDCG
		}
		
	}

	FallBack "Diffuse"
}
