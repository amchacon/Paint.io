// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:32719,y:32712,varname:node_3138,prsc:2|emission-3665-OUT;n:type:ShaderForge.SFN_Color,id:7241,x:31898,y:32408,ptovrint:False,ptlb:FirstColor,ptin:_FirstColor,varname:node_7241,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.07843138,c2:0.3921569,c3:0.7843137,c4:1;n:type:ShaderForge.SFN_Color,id:1292,x:31898,y:32585,ptovrint:False,ptlb:SecondColor,ptin:_SecondColor,varname:node_1292,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.7205882,c2:0.826572,c3:1,c4:1;n:type:ShaderForge.SFN_ScreenPos,id:1900,x:31319,y:33003,varname:node_1900,prsc:2,sctp:0;n:type:ShaderForge.SFN_RemapRange,id:5483,x:31501,y:33042,varname:node_5483,prsc:2,frmn:-1,frmx:1,tomn:0,tomx:1|IN-1900-V;n:type:ShaderForge.SFN_Set,id:6593,x:31670,y:33042,varname:ScreenPosition,prsc:2|IN-5483-OUT;n:type:ShaderForge.SFN_Get,id:951,x:31929,y:32742,varname:node_951,prsc:2|IN-6593-OUT;n:type:ShaderForge.SFN_Lerp,id:3665,x:32128,y:32611,varname:node_3665,prsc:2|A-1292-RGB,B-7241-RGB,T-951-OUT;proporder:7241-1292;pass:END;sub:END;*/

Shader "Shader Forge/2_Colors_Background" {
    Properties {
        _FirstColor ("FirstColor", Color) = (0.07843138,0.3921569,0.7843137,1)
        _SecondColor ("SecondColor", Color) = (0.7205882,0.826572,1,1)
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x 
            #pragma target 3.0
            uniform float4 _FirstColor;
            uniform float4 _SecondColor;
            struct VertexInput {
                float4 vertex : POSITION;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 projPos : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.pos = UnityObjectToClipPos( v.vertex );
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float2 sceneUVs = (i.projPos.xy / i.projPos.w);
////// Lighting:
////// Emissive:
                float ScreenPosition = ((sceneUVs * 2 - 1).g*0.5+0.5);
                float3 emissive = lerp(_SecondColor.rgb,_FirstColor.rgb,ScreenPosition);
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
