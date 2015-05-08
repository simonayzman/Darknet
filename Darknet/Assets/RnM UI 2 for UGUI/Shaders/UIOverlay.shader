Shader "Custom/UI Overlay"
{
   	Properties 
   	{
       	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
    }
 
    SubShader
    {
    	LOD 110
    	
       	Tags {
       		"Queue" = "Transparent"
       		"IgnoreProjector" = "True"
       		"RenderType" = "Transparent"
       	}
       	
       ZWrite Off
       Lighting Off
       Cull Off
       Fog { Mode Off }
       ZTest [unity_GUIZTestMode]
       Blend DstColor SrcColor
 
       Pass 
       {
         	CGPROGRAM
         	#pragma vertex vert_vct
         	#pragma fragment frag_mult 
         	#pragma fragmentoption ARB_precision_hint_fastest
         	#include "UnityCG.cginc"
 
        	sampler2D _MainTex;
         	float4 _MainTex_ST;
 
			struct vin_vct 
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};
 
         	struct v2f_vct
         	{
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				half2 texcoord : TEXCOORD0;
         	};
 
         	v2f_vct vert_vct(vin_vct v)
         	{
				v2f_vct o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.color = v.color;
				o.texcoord = v.texcoord;
				return o;
         	}
 
         	float4 frag_mult(v2f_vct i) : COLOR
         	{
          		float4 tex = tex2D(_MainTex, i.texcoord);
 
          		float4 final;          
          		final.rgb = i.color.rgb * tex.rgb * 2;
          		final.a = i.color.a * tex.a;
         		return lerp(float4(0.5f,0.5f,0.5f,0.5f), final, final.a);
         	}
 
         	ENDCG
    	}
	}
}