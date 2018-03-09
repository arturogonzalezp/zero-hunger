Shader "Week9/GrabPass"
{
	Properties
	{
		_TintColor("Color", Color) = (1.0,1.0,1.0,1.0)
		_NormalMap("Normal map", 2D) = "bump" {}
		_DistortionFactor("Distortion factor", Range(0.0, 0.1)) = 0.1
		
		// Color to tint the mesh
		// normal map to disturb your grab pass texture => default value = "bump"
		// Distortion Factor => slider between 0 & 0.1
	}
	SubShader
	{
		Tags { "Queue"="Transparent" }
		

		// First pass
		// Unity "takes a screenshot of what is covered by the mesh".
		GrabPass {}
		
		// Second pass
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			sampler2D _GrabTexture;
			sampler2D _NormalMap;
			fixed4 _TintColor;
			half _DistortionFactor;

			
			struct appdata
			{
				float4 position : POSITION;
				float2 uv: TEXCOORD0;
			};

			struct v2f
			{
				//To do
				float4 clipPosition: SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 uv_GrabPass : TEXCOORD1;
			};
			
			v2f vert (appdata v)
			{
				v2f o;

				// To do...
				// UnityObjectToClipPos(vertex in object space)  => and returns vertex position in clip space

				o.clipPosition = UnityObjectToClipPos(v.position);
				o.uv = v.uv;
				o.uv_GrabPass=ComputeGrabScreenPos(o.clipPosition);
				// Use the function ComputeGrabScreenPos to compute the uv of the grab texture
				// The parameter of this method is the vertex in CLIP SPACE.

				return o;
			
			}
			
			fixed4 frag (v2f i) : SV_Target
			{		
				// 1.
				// Get the vector from the normal map and save it in a half4 variable. => the w component of a vector = 0.0
				// Call it normalDistortion
				// Use the default uvs to read the texture.
				half4 normalDistortion = half4(UnpackNormal(tex2D(_NormalMap, i.uv + float2(0.0, (_Time.y)/2))), 0.0);
			
				// 2.
				// Disturb the uvs of the Grab texture by adding an offset => normalDistortion & the slide value
				i.uv_GrabPass += (normalDistortion * _DistortionFactor);
				
				// Read and return the _GrabTexture texel color.
				// Read it with the function: tex2Dproj(sampler2D NameOfTheTexture, half4 uvs) as we project the texture on the mesh.
				return tex2Dproj(_GrabTexture, i.uv_GrabPass)*_TintColor;
			}
			
			ENDCG
		}
	}
}
