Shader "Flunity/Default"
{
	Properties
	{
		_MainTex ("Particle Texture", 2D) = "white" {}
	}

	Category
	{
		Tags
		{
			"Queue"="Transparent"
			"IgnoreProjector"="True"
			"RenderType"="Transparent"
		}

		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off
		Lighting Off
		ZWrite Off

		SubShader
		{
			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile_particles
				
				#include "UnityCG.cginc"

				struct appdata_t
				{
					float4 vertex : POSITION;
					float4 color : COLOR;
					float2 texcoord0 : TEXCOORD0;
					float2 texcoord1 : TEXCOORD1;
				};

				struct v2f
				{
					float4 vertex : SV_POSITION;
					fixed4 color : COLOR0;
					fixed4 tint : COLOR1;
					half2 texcoord : TEXCOORD0;
				};
				
				v2f vert(appdata_t IN)
				{
					v2f OUT;
					OUT.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);
					OUT.texcoord = IN.texcoord0;
					OUT.color = IN.color;

					int gi = IN.texcoord1.x;
					int ai = IN.texcoord1.y;

					OUT.tint.r = IN.texcoord1.x - gi;
					OUT.tint.g = (gi - 1) / 255.0;
					OUT.tint.b = IN.texcoord1.y - ai;
					OUT.tint.a = (ai - 1) / 255.0;

					return OUT;
				}

				sampler2D _MainTex;

				fixed4 frag(v2f IN) : SV_Target
				{
					fixed4 c = tex2D(_MainTex, IN.texcoord);
					c.r = c.r * IN.color.r + IN.tint.r;
					c.g = c.g * IN.color.g + IN.tint.g;
					c.b = c.b * IN.color.b + IN.tint.b;
					c.a = c.a * IN.color.a + IN.tint.a;
					return c;
				}
				ENDCG 
			}
		}	
	}
}
