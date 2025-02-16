Shader "Unlit/UnlitBase" {
    Properties { // input data
        _ColorA ("Color A", Color ) = (1,1,1,1)
        _Float ("Color Start", Range(0,1) ) = 0
        _MainTex("Main Texture", 2D) = "white" { }
        
    }
    SubShader {
        // subshader tags
        Tags {
            "RenderType"="Opaque" // tag to inform the render pipeline of what type this is
        }
        Pass {
            // pass tags
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"
            
            
            struct appdata
            {
                // common attribute container naming for per-vertex that can be pass to vert function
                float4 vertex : POSITION; // position in object space
                float3 normal : NORMAL; // normals in object space
                float2 uv : TEXCOORD0; // the first (num 0 ) texture coordinates in the mat
                
                //more attributes
                float4 tang: TANGENT; // tangent direction (xyz) tangent sign (w)
                float4 color : COLOR; 
            };

            struct v2g
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
                float3 worldPos: TEXCOORD1;
            };

            struct g2f
            {
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
                float2 UV : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
            };
            
           

            

            float4 _ColorA;
            float _Float;
            sampler2D _MainTex; 
            float4 _MainTex_ST; // Stores tiling (x, y) and offset (z, w).
            float _Extruded;
            
            // vertex func - first function in the flow
            v2g vert (appdata v)
            {
                v2g o;
                //o.vertex = UnityObjectToClipPos(v.vertex); // from object directly to clip (camera) pos
                o.vertex = v.vertex;
                //o.normal = UnityObjectToWorldNormal(v.normal);
                o.normal = v.normal;
                o.worldPos = mul(UNITY_MATRIX_M, float4(v.vertex.xyz,1)); // from object to world pos
                o.uv = v.uv;
                return o;
            }

            //[maxvertcount(6)]
            //void geom

            



            float4 frag( v2g i ) : SV_Target {
                 float2 uv = i.uv * _MainTex_ST.xy + _MainTex_ST.zw;
                fixed4 texColor = tex2D(_MainTex, uv);
                return texColor;
                
            }
            
            ENDCG
        }
    }
}
