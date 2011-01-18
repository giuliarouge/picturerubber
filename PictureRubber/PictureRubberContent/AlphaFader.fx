// AlphaFader for PictureRubber
// a project for SK GDV @ FH Erfurt WS 2010/11
// 
// Stefan Benischke, Eric Jahn, Erik Müller

//variables
texture imageTexture;
texture compareTexture;
texture mouseTexture;
int textureIndex;
int alphaAmount;
int adjustment;
float2 delta;

sampler2D Image = sampler_state
{
  AddressU = Clamp;
  AddressV = Clamp;
  Texture = (imageTexture);
};

sampler2D CompareImage = sampler_state
{
  AddressU = Clamp;
  AddressV = Clamp;
  Texture = (compareTexture);
};

sampler2D MouseImage = sampler_state
{
  AddressU = Clamp;
  AddressV = Clamp;
  Texture = (mouseTexture);
};

struct Input
{
    float4 Position : POSITION0;
    float2 TexCoord0 : TEXCOORD0;
};

bool compareColors(float4 _color, float _value)
{
	bool isEqual = false;
	if(_color.r == _value &&
		_color.g == _value &&
		_color.b == _value &&
		_color.a == _value)
	{
		isEqual = true;
	}
	return isEqual;
}

Input VS_Main(Input input)
{
	//code from Frank Nagl, SBIP, http://code.google.com/p/sbip/
	 Input output;	 
	 output.Position = float4(sign(input.Position.xy), 0.0f, 1.0f);	 
	 output.TexCoord0 = input.TexCoord0 + adjustment * delta * 0.125;
	 return output;
}

float4 PS_AlphaFader(Input input) : COLOR0
{  
	//work on first layer
	float4 textureColor, mouseColor, compareColor;

	//get pixelcolor
	textureColor = tex2D(Image, input.TexCoord0);
	mouseColor = tex2D(MouseImage, input.TexCoord0);

	if(textureIndex == 0)
	{
		if(compareColors(mouseColor, 1.0f) &&
		!compareColors(textureColor, 0.0f))
		{
			//reduce saturation
			textureColor -= alphaAmount/255.0;
		}
	}
	else
	{
		compareColor = tex2D(CompareImage, input.TexCoord0);
		if(compareColors(mouseColor, 1.0f) &&
			compareColors(compareColor, 0.0f) &&
			!compareColors(textureColor, 0.0f))
		{
			//reduce saturation
			textureColor -= alphaAmount/255.0;
		}
	}	
    
    return textureColor;
}

technique AlphaFader
{
   pass Pass_0
   {
		VertexShader = compile vs_2_0 VS_Main();
		PixelShader = compile ps_2_0 PS_AlphaFader();
   }
}

