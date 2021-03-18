Shader "Unlit/2 Band Shader"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Texture", 2D) = "white" {}
    }
        SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100

        Pass
        {
            // from https://roystan.net/articles/toon-shader.html
            Tags
            {
                "LightMode" = "ForwardBase"
                "PassFlags" = "OnlyDirectional"
            }



            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
        // make fog work
        #pragma multi_compile_fog

        #include "UnityCG.cginc"

        struct appdata
        {
            float4 vertex : POSITION;
            float2 uv : TEXCOORD0;
            // Inside the appdata struct. from link
            float3 normal : NORMAL;
        };

        struct v2f
        {
            float2 uv : TEXCOORD0;
            UNITY_FOG_COORDS(1)
            float4 vertex : SV_POSITION;

            // Inside the v2f struct. from link
            float3 worldNormal : NORMAL;
        };

        sampler2D _MainTex;
        float4 _MainTex_ST;

        v2f vert(appdata v)
        {
            v2f o;
            o.vertex = UnityObjectToClipPos(v.vertex);
            o.uv = TRANSFORM_TEX(v.uv, _MainTex);
            UNITY_TRANSFER_FOG(o,o.vertex);
            o.worldNormal = UnityObjectToWorldNormal(v.normal); // from link
            return o;
        }

        float4 _Color;//from link

        fixed4 frag(v2f i) : SV_Target
        {
            // At the top of the fragment shader. from link
            float3 normal = normalize(i.worldNormal);
            float NdotL = dot(_WorldSpaceLightPos0, normal);

            // Below the NdotL declaration.
            float lightIntensity = NdotL > 0 ? 1 : 0;

            // sample the texture
            fixed4 col = tex2D(_MainTex, i.uv);
            // apply fog
            UNITY_APPLY_FOG(i.fogCoord, col);

            float4 sample = tex2D(_MainTex, i.uv); //from link

            // Modify the existing return line. from link
            return _Color * sample * lightIntensity;
            //return col; // original line
        }
        ENDCG
    }
        // Insert just after the closing curly brace of the existing Pass. from link
        UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
    }
}
