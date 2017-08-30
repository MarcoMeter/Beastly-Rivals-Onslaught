// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Shader by Sergio Santos
// http://www.knittedpixels.com

Shader "Powerbot/Powerbot_Shader" {
    Properties {
        _DIF_color ("DIF_color", Color) = (1,1,1,1)
        _DIF ("DIF", 2D) = "gray" {}
        _GlossPower ("GlossPower", Float ) = 0.5
        _Gloss ("Gloss", 2D) = "white" {}
        _SPEC_color ("SPEC_color", Color) = (0.7,0.87,1,1)
        _RIM_Exp ("RIM_Exp", Float ) = 3
        _RIM ("RIM", Color) = (0.7,0.87,1,1)
        _SI ("SI", Float ) = 0.5
        _Reflection_Amm ("Reflection_Amm", Float ) = 1
        _Reflection ("Reflection", 2D) = "black" {}
        _NM ("NM", 2D) = "bump" {}
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "ForwardBase"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _DIF; uniform float4 _DIF_ST;
            uniform sampler2D _NM; uniform float4 _NM_ST;
            uniform float4 _RIM;
            uniform float _RIM_Exp;
            uniform float _SI;
            uniform float4 _SPEC_color;
            uniform sampler2D _Gloss; uniform float4 _Gloss_ST;
            uniform float _GlossPower;
            uniform sampler2D _Reflection; uniform float4 _Reflection_ST;
            uniform float _Reflection_Amm;
            uniform float4 _DIF_color;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 binormalDir : TEXCOORD4;
                LIGHTING_COORDS(5,6)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.texcoord0;
                o.normalDir = mul(float4(v.normal,0), unity_WorldToObject).xyz;
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.binormalDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
/////// Normals:
                float2 node_471 = i.uv0;
                float3 node_61 = UnpackNormal(tex2D(_NM,TRANSFORM_TEX(node_471.rg, _NM)));
                float3 normalLocal = node_61.rgb;
                float3 normalDirection =  normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
/////// Diffuse:
                float NdotL = dot( normalDirection, lightDirection );
                float3 diffuse = max( 0.0, NdotL)*InvPi * attenColor + UNITY_LIGHTMODEL_AMBIENT.rgb;
////// Emissive:
                float4 node_2 = tex2D(_DIF,TRANSFORM_TEX(node_471.rg, _DIF));
                float3 node_453 = (_DIF_color.rgb*node_2.rgb);
                float node_13 = (1.0-max(0,dot(normalDirection, viewDirection)));
                float3 node_195 = lerp(node_453,pow(node_453,_RIM_Exp),node_13);
                float node_21 = viewReflectDirection.g;
                float3 node_29 = (float3(float2(node_21,node_21),node_21)*node_13);
                float3 node_38 = lerp(node_195,(_RIM.rgb+node_195),saturate((node_29+node_29)));
                float3 emissive = (node_38*_SI);
///////// Gloss:
                float gloss = (_GlossPower*tex2D(_Gloss,TRANSFORM_TEX(node_471.rg, _Gloss)).g);
                float specPow = exp2( gloss * 10.0+1.0);
////// Specular:
                NdotL = max(0.0, NdotL);
                float3 specularColor = (_SPEC_color.rgb*node_2.a);
                float specularMonochrome = dot(specularColor,float3(0.3,0.59,0.11));
                float normTerm = (specPow + 8.0 ) / (8.0 * Pi);
                float3 specular = (floor(attenuation) * _LightColor0.xyz) * pow(max(0,dot(halfDirection,normalDirection)),specPow) * specularColor*normTerm;
                float3 finalColor = 0;
                float3 diffuseLight = diffuse;
                diffuseLight *= 1-specularMonochrome;
                float2 node_291 = (reflect(viewReflectDirection,node_61.rgb).rg+i.uv0.rg);
                finalColor += diffuseLight * (((tex2D(_Reflection,TRANSFORM_TEX(node_291, _Reflection)).rgb*node_13)*_Reflection_Amm)+node_38);
                finalColor += specular;
                finalColor += emissive;
/// Final Color:
                return fixed4(finalColor,1);
            }
            ENDCG
        }
        Pass {
            Name "ForwardAdd"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            
            
            Fog { Color (0,0,0,0) }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _DIF; uniform float4 _DIF_ST;
            uniform sampler2D _NM; uniform float4 _NM_ST;
            uniform float4 _RIM;
            uniform float _RIM_Exp;
            uniform float _SI;
            uniform float4 _SPEC_color;
            uniform sampler2D _Gloss; uniform float4 _Gloss_ST;
            uniform float _GlossPower;
            uniform sampler2D _Reflection; uniform float4 _Reflection_ST;
            uniform float _Reflection_Amm;
            uniform float4 _DIF_color;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 binormalDir : TEXCOORD4;
                LIGHTING_COORDS(5,6)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.texcoord0;
                o.normalDir = mul(float4(v.normal,0), unity_WorldToObject).xyz;
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.binormalDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
/////// Normals:
                float2 node_472 = i.uv0;
                float3 node_61 = UnpackNormal(tex2D(_NM,TRANSFORM_TEX(node_472.rg, _NM)));
                float3 normalLocal = node_61.rgb;
                float3 normalDirection =  normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
/////// Diffuse:
                float NdotL = dot( normalDirection, lightDirection );
                float3 diffuse = max( 0.0, NdotL)*InvPi * attenColor;
///////// Gloss:
                float gloss = (_GlossPower*tex2D(_Gloss,TRANSFORM_TEX(node_472.rg, _Gloss)).g);
                float specPow = exp2( gloss * 10.0+1.0);
////// Specular:
                NdotL = max(0.0, NdotL);
                float4 node_2 = tex2D(_DIF,TRANSFORM_TEX(node_472.rg, _DIF));
                float3 specularColor = (_SPEC_color.rgb*node_2.a);
                float specularMonochrome = dot(specularColor,float3(0.3,0.59,0.11));
                float normTerm = (specPow + 8.0 ) / (8.0 * Pi);
                float3 specular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),specPow) * specularColor*normTerm;
                float3 finalColor = 0;
                float3 diffuseLight = diffuse;
                diffuseLight *= 1-specularMonochrome;
                float2 node_291 = (reflect(viewReflectDirection,node_61.rgb).rg+i.uv0.rg);
                float node_13 = (1.0-max(0,dot(normalDirection, viewDirection)));
                float3 node_453 = (_DIF_color.rgb*node_2.rgb);
                float3 node_195 = lerp(node_453,pow(node_453,_RIM_Exp),node_13);
                float node_21 = viewReflectDirection.g;
                float3 node_29 = (float3(float2(node_21,node_21),node_21)*node_13);
                float3 node_38 = lerp(node_195,(_RIM.rgb+node_195),saturate((node_29+node_29)));
                finalColor += diffuseLight * (((tex2D(_Reflection,TRANSFORM_TEX(node_291, _Reflection)).rgb*node_13)*_Reflection_Amm)+node_38);
                finalColor += specular;
/// Final Color:
                return fixed4(finalColor * 1,0);
            }
            ENDCG
        }
    }
    FallBack "Bumped Specular"
    CustomEditor "ShaderForgeMaterialInspector"
}
