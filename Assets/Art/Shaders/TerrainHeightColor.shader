Shader "Custom/TerrainHeightColor"
{
	Properties
	{
		_MinHeight ("Min Height", Float) = 0
		_MaxHeight ("Max Height", Float) = 150

		_ColorLow    ("Plain Color", Color) = (0.1, 0.4, 0.1, 1)   // 平原 - 深绿
		_ColorMid    ("Hill Color", Color)  = (0.6, 0.5, 0.3, 1)   // 丘陵 - 棕灰
		_ColorHigh   ("Mountain Color", Color) = (0.9, 0.9, 0.9, 1) // 山顶 - 白
	}

	SubShader
	{
		Tags
		{ 
			"RenderPipeline"="UniversalRenderPipeline"
			"RenderType"="Opaque" 
		}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct a2v
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float3 worldPos : TEXCOORD0;
			};

			// ===== Uniform 参数 =====
			float _MinHeight;
			float _MaxHeight;
			float4 _ColorLow;
			float4 _ColorMid;
			float4 _ColorHigh;

			v2f vert(a2v v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				// 归一化高度 [0,1]
				// saturate(x)：内建函数，将x限制在[0,1]范围
				float h = saturate((i.worldPos.y - _MinHeight) / (_MaxHeight - _MinHeight));

				// 分段平滑插值 smoothstep(edge0, edge1, x)
				// 在两个边界间进行平滑过渡（S型曲线）
				float lowToMid = smoothstep(0.2, 0.6, h); // 平原->丘陵
				float midToHigh = smoothstep(0.6, 1.0, h); // 丘陵->山顶

				// 颜色插值 lerp(a,b,t)：在a,b之间线性插值
				float4 colLowMid = lerp(_ColorLow, _ColorMid, lowToMid);
				float4 finalCol = lerp(colLowMid, _ColorHigh, midToHigh);

				return finalCol;
			}
			ENDCG
		}
	}

	FallBack Off
}
