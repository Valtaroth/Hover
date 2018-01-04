Shader "Custom/Terrain/Fading" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_FadeMinDistance ("Fade Min Distance", Float) = 500
		_FadeMaxDistance ("Fade Max Distance", Float) = 1000
		_FadeMinColor ("Fade Color in Minimal", Color) = (1, 1, 1, 1)
		_FadeMaxColor ("Fade Color in Maximum", Color) = (0, 0, 0, 0)
	}
	SubShader {
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }
		LOD 200

		Blend SrcAlpha OneMinusSrcAlpha

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows alpha:fade

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
			half4 color : COLOR;
			float3 worldPos;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		float _FadeMinDistance;
		float _FadeMaxDistance;
		half4 _FadeMinColor;
		half4 _FadeMaxColor;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_CBUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_CBUFFER_END

		void surf (Input IN, inout SurfaceOutputStandard o) {
			float dist = distance(_WorldSpaceCameraPos, IN.worldPos);
			half weight = saturate((dist - _FadeMinDistance) / (_FadeMaxDistance - _FadeMinDistance));
			half4 fadeColor = lerp(_FadeMinColor, _FadeMaxColor, weight);

			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb * IN.color.rgb * fadeColor.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a * IN.color.a * fadeColor.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
