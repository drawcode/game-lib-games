Shader "Curved/CurvedTransparent" {
	Properties {
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

			// make fog work
			#pragma multi_compile_fog

			#include "UnityCG.cginc"

            sampler2D _MainTex;
			float4 _QOffset;
			float _Dist;
			float4 _MainTex_ST;

			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
			};

			v2f vert (appdata_base v)
			{
			    v2f o;
			    float4 vPos = mul (UNITY_MATRIX_MV, v.vertex);
			    float zOff = vPos.z/_Dist;
			    vPos += _QOffset*zOff*zOff;
			    o.pos = mul (UNITY_MATRIX_P, vPos);
				//o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex); //v.texcoord;
															//o.uv = v.texcoord;
				//o.Alpha = tex2D(_MainTex, v.uv_MainTex).a;
				UNITY_TRANSFER_FOG(o,vPos);
			    return o;
			}

			half4 frag (v2f i) : COLOR
			{
			    half4 col = tex2D(_MainTex, i.uv.xy);
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
			    return col;
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
