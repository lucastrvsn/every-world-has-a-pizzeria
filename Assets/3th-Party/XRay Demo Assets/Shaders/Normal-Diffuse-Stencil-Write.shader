Shader "XRay Shaders/Diffuse-Stencil-Write"
{
	Properties
	{
		_Color("Main Color", Color) = (1, 1, 1, 1)
		_MainTex("Base (RGB)", 2D) = "white" {}
		_Range("Range", Float) = 5.0
	}

	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 200

		Stencil
		{
			Ref 1
			Comp Always
			Pass Replace
			ZFail Keep
		}

		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _MainTex;
		float _Range;
		fixed4 _Color;

		struct Input {
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutput o)
		{
			fixed4 c = lerp(tex2D(_MainTex, IN.uv_MainTex), Luminance(tex2D(_MainTex, IN.uv_MainTex)), _Range) * _Color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
	
	Fallback "Legacy Shaders/VertexLit"
}
