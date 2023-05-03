string g_ScreenAlignedQuad : ModelData = "C:/Program Files/ATI Research Inc/RenderMonkey 1.5/Examples/Media/Models/ScreenAlignedQuad.3ds";

const float4x4 g_MatViewProjection : ViewProjection;

//const float g_fInverseViewportWidth;
//const float g_fInverseViewportHeight;

struct VS_OUTPUT
{
   float4 pos       : POSITION0;
   float2 texCoord  : TEXCOORD0;
};

VS_OUTPUT DeinterlaceVS( float4 inPos: POSITION, float2 inTex: TEXCOORD  )
{
   VS_OUTPUT o = (VS_OUTPUT) 0;

   o.pos = mul( g_MatViewProjection, inPos );
   o.texCoord = inTex;
      
   return o;
}

texture g_Texture<string ResourceName = ".\interlaceTest.tga";>;

sampler2D Texture0 = sampler_state
{
   Texture = (g_Texture);
   ADDRESSU = CLAMP;
   ADDRESSV = CLAMP;
   //ADDRESSW = CLAMP;
   //BORDERCOLOR = 0x0;
   MAGFILTER = LINEAR;
   MINFILTER = LINEAR;
};

//const float3 kernel_20[9] = 
//    {
//    -1,-1,  1/16.0f,  0,-1, 2/16.0f,  1,-1, 1/16.0f,
//    -1, 0,  2/16.0f,  0,0,  4/16.0f,  1,0,  2/16.0f,
//    -1, 1,  1/16.0f,  0,1,  2/16.0f,  1,1,  1/16.0f
//    };

//const float2 kernel[3] = 
//    {
//    -1,  1/4.0f,
//     0,  2/4.0f,
//     1,  1/4.0f,
//    };      

float4 DeinterlacePS( float2 texCoord0  : TEXCOORD0, 
					  float2 texCoord1  : TEXCOORD1, 
					  float2 texCoord2  : TEXCOORD2 ) : COLOR
{
    float4 sumPix = 0.0f;
//    float2 tx;

//    for (int i = 0; i < 3; i++)
//    {
//        tx.x = texCoord0.x;
//        tx.y = texCoord0.y + g_fInverseViewportHeight * kernel[i].x;
//        sumPix += tex2D( Texture0, tx ) * kernel[i].y;
//    }       

	sumPix += tex2D( Texture0, texCoord0 ) * 0.5f;
	sumPix += tex2D( Texture0, texCoord1 ) * 0.25f;
	sumPix += tex2D( Texture0, texCoord2 ) * 0.25f;

    return sumPix;
    //return tex2D( Texture0, texCoord );
}

//--------------------------------------------------------------//
// De-interlace for Pixel Shader 1.4 and above
//--------------------------------------------------------------//
technique DeinterlacePS_14
{
   pass P0
   {
      CULLMODE = NONE;

      VertexShader = compile vs_1_1 DeinterlaceVS();
      PixelShader = compile ps_1_4 DeinterlacePS();
   }

}

