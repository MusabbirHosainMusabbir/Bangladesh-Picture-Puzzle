// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

     Shader "Custom/Puzzle Cell" {
         Properties {
             _MainTex ("Base layer (RGB)", 2D) = "white" {}
             _Color("Color", COLOR) = (1,1,1,1)
                        _Side1 ("Side1", 2D) = "white" {}
       [MaterialToggle] _Invert1 ("Invert1", Float) = 0.0
                        _Side2 ("Side2", 2D) = "white" {}
       [MaterialToggle] _Invert2 ("Invert2", Float) = 0.0
                        _Side3 ("Side3", 2D) = "white" {}
       [MaterialToggle] _Invert3 ("Invert3", Float) = 0.0
                        _Side4 ("Side4", 2D) = "white" {}
       [MaterialToggle] _Invert4 ("Invert4", Float) = 0.0
       
         }
         
         SubShader {
             Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
             
             Lighting Off 
             Fog { Mode Off }
             ZWrite Off
             Blend SrcAlpha OneMinusSrcAlpha
             
             CGINCLUDE
             #include "UnityCG.cginc"
             sampler2D _MainTex;
             sampler2D _Side1;
             sampler2D _Side2;
             sampler2D _Side3;
             sampler2D _Side4;
         
             float4 _MainTex_ST;
             float4 _Side1_ST;
             float4 _Side2_ST;
             float4 _Side3_ST;
             float4 _Side4_ST;
             
        		float _Invert1;
        		float _Invert2;
        		float _Invert3;
        		float _Invert4;
        		
             struct v2f {
                 float4 pos : SV_POSITION;
                 fixed4 color : TEXCOORD0;        
                 float2 uv : TEXCOORD1;
                 float2 uv1 : TEXCOORD2;
                 float2 uv2 : TEXCOORD3;
                 float2 uv3 : TEXCOORD4;
                 float2 uv4 : TEXCOORD5;
             };
             
       		half4 _Color;
             float _RotationDegrees;
             
             v2f vert (appdata_full v)
             {
             
             appdata_full v1 = v;
             appdata_full v2 = v;
             appdata_full v3 = v;
             appdata_full v4 = v;
             
             // Side 1
             if(_Invert1 == 1)
				_RotationDegrees = 180;
				else
				_RotationDegrees = 0;
				
			v1.texcoord.x = v1.texcoord.x - 0.5;
			v1.texcoord.y = v1.texcoord.y - 0.5 - 0.25;

            float s = sin ( -_RotationDegrees * 3.14159 / 180);
            float c = cos ( -_RotationDegrees * 3.14159 / 180);
           
            float2x2 rotationMatrix = float2x2( c, -s, s, c);
            rotationMatrix *=0.5;
            rotationMatrix +=0.5;
            rotationMatrix = rotationMatrix * 2 - 1;
            v1.texcoord.xy = mul ( v1.texcoord.xy, rotationMatrix );

   			v1.texcoord.x = v1.texcoord.x + 0.5;
			v1.texcoord.y = v1.texcoord.y + 0.5;

             // Side 2
             if(_Invert2 == 1)
				_RotationDegrees = -90;
				else
				_RotationDegrees = 90;
			
			v2.texcoord.x = v2.texcoord.x - 0.5 - 0.25;
			v2.texcoord.y = v2.texcoord.y - 0.5;

            s = sin ( -_RotationDegrees * 3.14159 / 180);
            c = cos ( -_RotationDegrees * 3.14159 / 180);
           
            rotationMatrix = float2x2( c, -s, s, c);
            rotationMatrix *=0.5;
            rotationMatrix +=0.5;
            rotationMatrix = rotationMatrix * 2 - 1;
            v2.texcoord.xy = mul ( v2.texcoord.xy, rotationMatrix );

   			v2.texcoord.x = v2.texcoord.x + 0.5;
			v2.texcoord.y = v2.texcoord.y + 0.5;

             // Side 3
             if(_Invert3 == 1)
				_RotationDegrees = 0;
				else
				_RotationDegrees = 180;
			
			v3.texcoord.x = v3.texcoord.x - 0.5;
			v3.texcoord.y = v3.texcoord.y - 0.5 + 0.25;

            s = sin ( -_RotationDegrees * 3.14159 / 180);
            c = cos ( -_RotationDegrees * 3.14159 / 180);
           
            rotationMatrix = float2x2( c, -s, s, c);
            rotationMatrix *=0.5;
            rotationMatrix +=0.5;
            rotationMatrix = rotationMatrix * 2 - 1;
            v3.texcoord.xy = mul ( v3.texcoord.xy, rotationMatrix );

   			v3.texcoord.x = v3.texcoord.x + 0.5;
			v3.texcoord.y = v3.texcoord.y + 0.5;

             // Side 4
             if(_Invert4 == 1)
				_RotationDegrees = 90;
				else
				_RotationDegrees = -90;
			
			v4.texcoord.x = v4.texcoord.x - 0.5 + 0.25;
			v4.texcoord.y = v4.texcoord.y - 0.5;

            s = sin ( -_RotationDegrees * 3.14159 / 180);
            c = cos ( -_RotationDegrees * 3.14159 / 180);
           
            rotationMatrix = float2x2( c, -s, s, c);
            rotationMatrix *=0.5;
            rotationMatrix +=0.5;
            rotationMatrix = rotationMatrix * 2 - 1;
            v4.texcoord.xy = mul ( v4.texcoord.xy, rotationMatrix );

   			v4.texcoord.x = v4.texcoord.x + 0.5;
			v4.texcoord.y = v4.texcoord.y + 0.5;

                 v2f o;
                 o.pos = UnityObjectToClipPos(v.vertex);
                 o.uv = TRANSFORM_TEX(v.texcoord.xy,_MainTex);
                 o.uv1 = TRANSFORM_TEX(v1.texcoord.xy,_Side1);
                 o.uv2 = TRANSFORM_TEX(v2.texcoord.xy,_Side2);
                 o.uv3 = TRANSFORM_TEX(v3.texcoord.xy,_Side3);
                 o.uv4 = TRANSFORM_TEX(v4.texcoord.xy,_Side4);
                 o.color = fixed4(_Color.r, _Color.g, _Color.b, _Color.a);
         
                 return o;
             }
             
             ENDCG
         
         
             Pass {
                 CGPROGRAM
                 #pragma vertex vert
                 #pragma fragment frag
                 #pragma fragmentoption ARB_precision_hint_fastest        

                 fixed4 frag (v2f i) : COLOR
                 {
                     fixed4 o;
                     fixed4 tex = tex2D (_MainTex, i.uv);
                     fixed4 tex1 = tex2D (_Side1, i.uv1);
                     fixed4 tex2 = tex2D (_Side2, i.uv2);
                     fixed4 tex3 = tex2D (_Side3, i.uv3);
                     fixed4 tex4 = tex2D (_Side4, i.uv4);
                     
                     float a = 0;
                     
					 if(_Invert1 == 1)
                     	a = 1 - tex1.r;
					 else
                      	a = tex1.r;

					if(tex.a > a)
 						tex.a = a;

					 if(_Invert2 == 1)
                     	a = 1 - tex2.r;
					 else
                      a = tex2.r;

					if(tex.a > a)
						tex.a = a;

					 if(_Invert3 == 1)
                     	a = 1 - tex3.r;
					 else
                      a = tex3.r;

					if(tex.a > a)
						tex.a = a;

					 if(_Invert4 == 1)
                     	a = 1 - tex4.r;
					 else
                      a = tex4.r;

					if(tex.a > a)
						tex.a = a;

                     o = tex * i.color;
                     return o;
                 }
                 

			ENDCG
		}
    }       
}