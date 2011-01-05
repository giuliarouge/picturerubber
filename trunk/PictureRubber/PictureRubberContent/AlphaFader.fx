// AlphaFader for PictureRubber
// a project for SK GDV @ FH Erfurt WS 2010/11
// 
// Stefan Benischke, Eric Jahn, Erik Müller

//variables
texture ImageTexture;
texture MouseTexture;
int alphaAmount;
int Adjustment;
float2 Delta;

sampler2D Image = sampler_state
{
  AddressU = Clamp;
  AddressV = Clamp;
  Texture = (ImageTexture);
};

sampler2D MouseImage = sampler_state
{
  AddressU = Clamp;
  AddressV = Clamp;
  Texture = (MouseTexture);
};

struct Input
{
    float4 Position : POSITION0;
    float2 TexCoord0 : TEXCOORD0;
};

Input VS_Main(Input input)
{
	//code from Frank Nagl, SBIP, http://code.google.com/p/sbip/
	 Input output;	 
	 output.Position = float4(sign(input.Position.xy), 0.0f, 1.0f);	 
	 output.TexCoord0 = input.TexCoord0 + Adjustment * Delta * 0.125;
	 return output;
}

float4 PS_AlphaFader(Input input) : COLOR0
{  
	float4 textureColor = tex2D( Image, input.TexCoord0 );
	float4 mouseColor = tex2D( Image, input.TexCoord0 );
	float4 white = float4(1.0f, 1.0f, 1.0f, 1.0f);

	if(mouseColor == white)
	{
		//reduce saturation
		//textureColor *= saturate(textureColor.a - alphaAmount);
	}
    
    return tex;
}

technique AlphaFader
{
   pass Pass_0
   {
		VertexShader = compile vs_2_0 VS_Main();
		PixelShader = compile ps_2_0 PS_AlphaFader();
   }
}

