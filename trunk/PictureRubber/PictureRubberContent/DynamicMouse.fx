// DynamicMouse Shader for PictureRubber
// a project for SK GDV @ FH Erfurt WS 2010/11
// 
// Stefan Benischke, Eric Jahn, Erik Müller

//variables
texture backgroundTexture;
texture mouseTexture;
float2 topLeft, bottomRight, dimensions;
int adjustment;
float2 delta;

sampler2D BackgroundImage = sampler_state
{
  AddressU = Clamp;
  AddressV = Clamp;
  Texture = (backgroundTexture);
};

sampler2D MouseImage = sampler_state
{
  AddressU = Clamp;
  AddressV = Clamp;
  Texture = (mouseTexture);
};

bool isPointInRange(float2 _position)
{
	bool isInRange = false;
	if(_position.x >= topLeft.x &&
		_position.x <= bottomRight.x &&
		_position.y >= topLeft.y &&
		_position.y <= bottomRight.y)
	{
		isInRange = true;
	}
	return isInRange;
}

float2 calculateMouseCoordinate(float2 _position)
{
	float x = (_position.x - topLeft.x) / (bottomRight.x - topLeft.x);
	float y = (_position.y - topLeft.y) / (bottomRight.y - topLeft.y);
	return float2(x, y);
}

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
	 output.TexCoord0 = input.TexCoord0 + adjustment * delta * 0.125;
	 return output;
}

float4 PS_DynamicMouse(Input input) : COLOR0
{  
	float4 mouseColor, textureColor;

	//get pixelcolor
	textureColor = tex2D(BackgroundImage, input.TexCoord0);
	
	float2 actualShaderPosition = input.TexCoord0;
	if(isPointInRange(actualShaderPosition))
	{
		float2 texturCoordinate = calculateMouseCoordinate(actualShaderPosition);
		mouseColor = tex2D(MouseImage, texturCoordinate);
		if(mouseColor.a != 0.0f &&
			compareColors(textureColor, 0.0f))
		{
			textureColor = 1.0f;
		}
	}

    return textureColor;
}

technique DynamicMouse
{
   pass Pass_0
   {
		VertexShader = compile vs_2_0 VS_Main();
		PixelShader = compile ps_2_0 PS_DynamicMouse();
   }
}

