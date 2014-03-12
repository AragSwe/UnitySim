Shader "Custom/test" {
	Properties {
      _MainTex ("Texture", 2D) = "white" {}
      _Color ("Color (RGBA)", Color) = (0, 0, 0, 1)
    }
    SubShader {
      Tags { "RenderType" = "Opaque" }
      CGPROGRAM
      #pragma surface surf Lambert
      struct Input {
          float2 uv_MainTex;
      };
      
      sampler2D _MainTex;
      float4 _Color;
      
      void surf (Input IN, inout SurfaceOutput o) {
          o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb;
          if(o.Albedo.r == o.Albedo.g == o.Albedo.b == 0)
          	o.Albedo += _Color.rgb;
      }
      ENDCG
    } 
    Fallback "Diffuse"
}
