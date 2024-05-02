// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "MapShader"
{
    Properties
    {
        _ColorMapTexture ("My texture", 2D) = "white" {}
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
        }
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _ColorMapTexture;
            half4 _CountryColors[43];
            float _LastVisit[43];
            float _CurrentTime;

            struct appdata {
                float4 vertex : POSITION; // vertex position  
                float2 uv : TEXCOORD0; // texture coordinate  
            };  
        
            struct v2f {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0; // texture coordinate  
            };
        
            v2f vert(appdata input) {
                v2f o;
                o.vertex = UnityObjectToClipPos(input.vertex);
                o.uv = input.uv;
                return o;
            }
        
            half4 frag(v2f output) : COLOR {
                float4 color = tex2D(_ColorMapTexture, output.uv.xy);
                float temp = color.r * 255.0;
                uint id = round( color.r * 255.0);
                float4 actualColor = _CountryColors[id];
                float r = float(id / 255);

                actualColor.r = actualColor.r + (0.2 * (max(0.0, _LastVisit[id] - _CurrentTime + 0.75) / 0.75)) + (color.g * 5.0f);
                actualColor.g = actualColor.g + (0.2 * (max(0.0, _LastVisit[id] - _CurrentTime + 0.75) / 0.75)) + (color.g * 5.0f);
                actualColor.b = actualColor.b + (0.2 * (max(0.0, _LastVisit[id] - _CurrentTime + 0.75) / 0.75)) + (color.g * 5.0f);

                return half4(actualColor.r, actualColor.g, actualColor.b, 1.0); 
                // return half4(1.0,0.0,0.0,1.0)
            }
            ENDCG
        }
    }
}