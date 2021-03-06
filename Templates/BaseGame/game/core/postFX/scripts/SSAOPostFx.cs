//-----------------------------------------------------------------------------
// Copyright (c) 2012 GarageGames, LLC
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to
// deal in the Software without restriction, including without limitation the
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or
// sell copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
// IN THE SOFTWARE.
//-----------------------------------------------------------------------------


///
$SSAOPostFx::overallStrength = 2.0;

// TODO: Add small/large param docs.

// The small radius SSAO settings.
$SSAOPostFx::sRadius = 0.1;
$SSAOPostFx::sStrength = 6.0;
$SSAOPostFx::sDepthMin = 0.1;
$SSAOPostFx::sDepthMax = 1.0;
$SSAOPostFx::sDepthPow = 1.0;
$SSAOPostFx::sNormalTol = 0.0;
$SSAOPostFx::sNormalPow = 1.0;

// The large radius SSAO settings.
$SSAOPostFx::lRadius = 1.0;
$SSAOPostFx::lStrength = 10.0;
$SSAOPostFx::lDepthMin = 0.2;
$SSAOPostFx::lDepthMax = 2.0;
$SSAOPostFx::lDepthPow = 0.2;
$SSAOPostFx::lNormalTol = -0.5;
$SSAOPostFx::lNormalPow = 2.0;

/// Valid values: 0, 1, 2
$SSAOPostFx::quality = 0;

///
$SSAOPostFx::blurDepthTol = 0.001;

/// 
$SSAOPostFx::blurNormalTol = 0.95;

///
$SSAOPostFx::targetScale = "0.5 0.5";


function SSAOPostFx::onAdd( %this )
{  
   %this.wasVis = "Uninitialized";
   %this.quality = "Uninitialized";
   
   PostFXManager.registerPostEffect(%this);
}

function SSAOPostFx::preProcess( %this )
{   
   if ( $SSAOPostFx::quality !$= %this.quality )
   {
      %this.quality = mClamp( mRound( $SSAOPostFx::quality ), 0, 2 );
      
      %this.setShaderMacro( "QUALITY", %this.quality );      
   }      
   
   %this.targetScale = $SSAOPostFx::targetScale;
}

function SSAOPostFx::setShaderConsts( %this )
{      
   %this.setShaderConst( "$overallStrength", $SSAOPostFx::overallStrength );

   // Abbreviate is s-small l-large.   
   
   %this.setShaderConst( "$sRadius",      $SSAOPostFx::sRadius );
   %this.setShaderConst( "$sStrength",    $SSAOPostFx::sStrength );
   %this.setShaderConst( "$sDepthMin",    $SSAOPostFx::sDepthMin );
   %this.setShaderConst( "$sDepthMax",    $SSAOPostFx::sDepthMax );
   %this.setShaderConst( "$sDepthPow",    $SSAOPostFx::sDepthPow );
   %this.setShaderConst( "$sNormalTol",   $SSAOPostFx::sNormalTol );
   %this.setShaderConst( "$sNormalPow",   $SSAOPostFx::sNormalPow );
   
   %this.setShaderConst( "$lRadius",      $SSAOPostFx::lRadius );
   %this.setShaderConst( "$lStrength",    $SSAOPostFx::lStrength );
   %this.setShaderConst( "$lDepthMin",    $SSAOPostFx::lDepthMin );
   %this.setShaderConst( "$lDepthMax",    $SSAOPostFx::lDepthMax );
   %this.setShaderConst( "$lDepthPow",    $SSAOPostFx::lDepthPow );
   %this.setShaderConst( "$lNormalTol",   $SSAOPostFx::lNormalTol );
   %this.setShaderConst( "$lNormalPow",   $SSAOPostFx::lNormalPow );
   
   %blur = %this->blurY;
   %blur.setShaderConst( "$blurDepthTol", $SSAOPostFx::blurDepthTol );
   %blur.setShaderConst( "$blurNormalTol", $SSAOPostFx::blurNormalTol );   
   
   %blur = %this->blurX;
   %blur.setShaderConst( "$blurDepthTol", $SSAOPostFx::blurDepthTol );
   %blur.setShaderConst( "$blurNormalTol", $SSAOPostFx::blurNormalTol );   
   
   %blur = %this->blurY2;
   %blur.setShaderConst( "$blurDepthTol", $SSAOPostFx::blurDepthTol );
   %blur.setShaderConst( "$blurNormalTol", $SSAOPostFx::blurNormalTol );
      
   %blur = %this->blurX2;
   %blur.setShaderConst( "$blurDepthTol", $SSAOPostFx::blurDepthTol );
   %blur.setShaderConst( "$blurNormalTol", $SSAOPostFx::blurNormalTol );         
}

function SSAOPostFx::onEnabled( %this )
{
   // This tells the AL shaders to reload and sample
   // from our #ssaoMask texture target. 
   $AL::UseSSAOMask = true;
   
   return true;
}

function SSAOPostFx::onDisabled( %this )
{
  $AL::UseSSAOMask = false;
}

function SSAOPostFx::populatePostFXSettings(%this)
{
   PostEffectEditorInspector.startGroup("SSAO - General");
   PostEffectEditorInspector.addField("$PostFXManager::Settings::EnabledSSAO", "Enabled", "bool", "Low,Medium,High", $PostFXManager::PostFX::EnableSSAO, "");
   PostEffectEditorInspector.addField("$PostFXManager::Settings::SSAO::quality", "Quality", "list", "Low,Medium,High", $SSAOPostFx::quality, "");
   PostEffectEditorInspector.addField("$PostFXManager::Settings::SSAO::overallStrength", "Overall Strength", "float", "", $SSAOPostFx::overallStrength, "");
   PostEffectEditorInspector.addField("$PostFXManager::Settings::SSAO::blurDepthTol", "Blur (Softness)", "float", "", $SSAOPostFx::blurDepthTol, "");
   PostEffectEditorInspector.addField("$PostFXManager::Settings::SSAO::blurNormalTol", "Blur (Normal Maps)", "float", "", $SSAOPostFx::blurNormalTol, "");
   PostEffectEditorInspector.endGroup();
   
   PostEffectEditorInspector.startGroup("SSAO - Near");
   PostEffectEditorInspector.addField("$PostFXManager::Settings::SSAO::sRadius", "Radius", "list", "Low,Medium,High", $SSAOPostFx::sRadius, "");
   PostEffectEditorInspector.addField("$PostFXManager::Settings::SSAO::sStrength", "Strength", "float", "", $SSAOPostFx::sStrength, "");
   PostEffectEditorInspector.addField("$PostFXManager::Settings::SSAO::sDepthMin", "Depth Min", "float", "", $SSAOPostFx::sDepthMin, "");
   PostEffectEditorInspector.addField("$PostFXManager::Settings::SSAO::sDepthMax", "Depth Max", "float", "", $SSAOPostFx::sDepthMax, "");
   PostEffectEditorInspector.addField("$PostFXManager::Settings::SSAO::sNormalTol", "Normal Map Tolerance", "float", "", $SSAOPostFx::sNormalTol, "");
   PostEffectEditorInspector.addField("$PostFXManager::Settings::SSAO::sNormalPow", "Normal Map Power", "float", "", $SSAOPostFx::sNormalPow, "");
   PostEffectEditorInspector.endGroup();
   
   PostEffectEditorInspector.startGroup("SSAO - Far");
   PostEffectEditorInspector.addField("$PostFXManager::Settings::SSAO::lRadius", "Radius", "list", "Low,Medium,High", $SSAOPostFx::lRadius, "");
   PostEffectEditorInspector.addField("$PostFXManager::Settings::SSAO::lStrength", "Strength", "float", "", $SSAOPostFx::lStrength, "");
   PostEffectEditorInspector.addField("$PostFXManager::Settings::SSAO::lDepthMin", "Depth Min", "float", "", $SSAOPostFx::lDepthMin, "");
   PostEffectEditorInspector.addField("$PostFXManager::Settings::SSAO::lDepthMax", "Depth Max", "float", "", $SSAOPostFx::lDepthMax, "");
   PostEffectEditorInspector.addField("$PostFXManager::Settings::SSAO::lNormalTol", "Normal Map Tolerance", "float", "", $SSAOPostFx::lNormalTol, "");
   PostEffectEditorInspector.addField("$PostFXManager::Settings::SSAO::lNormalPow", "Normal Map Power", "float", "", $SSAOPostFx::lNormalPow, "");
   PostEffectEditorInspector.endGroup();
}

function SSAOPostFx::applyFromPreset(%this)
{
   //SSAO Settings
   $PostFXManager::PostFX::EnableSSAO  = $PostFXManager::Settings::EnabledSSAO;
   $SSAOPostFx::blurDepthTol           = $PostFXManager::Settings::SSAO::blurDepthTol;
   $SSAOPostFx::blurNormalTol          = $PostFXManager::Settings::SSAO::blurNormalTol;
   $SSAOPostFx::lDepthMax              = $PostFXManager::Settings::SSAO::lDepthMax;
   $SSAOPostFx::lDepthMin              = $PostFXManager::Settings::SSAO::lDepthMin;
   $SSAOPostFx::lDepthPow              = $PostFXManager::Settings::SSAO::lDepthPow;
   $SSAOPostFx::lNormalPow             = $PostFXManager::Settings::SSAO::lNormalPow;
   $SSAOPostFx::lNormalTol             = $PostFXManager::Settings::SSAO::lNormalTol;
   $SSAOPostFx::lRadius                = $PostFXManager::Settings::SSAO::lRadius;
   $SSAOPostFx::lStrength              = $PostFXManager::Settings::SSAO::lStrength;
   $SSAOPostFx::overallStrength        = $PostFXManager::Settings::SSAO::overallStrength;
   $SSAOPostFx::quality                = $PostFXManager::Settings::SSAO::quality;
   $SSAOPostFx::sDepthMax              = $PostFXManager::Settings::SSAO::sDepthMax;
   $SSAOPostFx::sDepthMin              = $PostFXManager::Settings::SSAO::sDepthMin;
   $SSAOPostFx::sDepthPow              = $PostFXManager::Settings::SSAO::sDepthPow;
   $SSAOPostFx::sNormalPow             = $PostFXManager::Settings::SSAO::sNormalPow;
   $SSAOPostFx::sNormalTol             = $PostFXManager::Settings::SSAO::sNormalTol;
   $SSAOPostFx::sRadius                = $PostFXManager::Settings::SSAO::sRadius;
   $SSAOPostFx::sStrength              = $PostFXManager::Settings::SSAO::sStrength;  
   
   if($PostFXManager::PostFX::EnableSSAO)
      %this.enable();
   else
      %this.disable();
}

function SSAOPostFx::settingsApply(%this)
{
   $PostFXManager::Settings::EnabledSSAO                   = $PostFXManager::PostFX::EnableSSAO ;
   
   $PostFXManager::Settings::SSAO::blurDepthTol             = $SSAOPostFx::blurDepthTol;
   $PostFXManager::Settings::SSAO::blurNormalTol            = $SSAOPostFx::blurNormalTol;
   $PostFXManager::Settings::SSAO::lDepthMax                = $SSAOPostFx::lDepthMax;
   $PostFXManager::Settings::SSAO::lDepthMin                = $SSAOPostFx::lDepthMin;
   $PostFXManager::Settings::SSAO::lDepthPow                = $SSAOPostFx::lDepthPow;
   $PostFXManager::Settings::SSAO::lNormalPow               = $SSAOPostFx::lNormalPow;
   $PostFXManager::Settings::SSAO::lNormalTol               = $SSAOPostFx::lNormalTol;
   $PostFXManager::Settings::SSAO::lRadius                  = $SSAOPostFx::lRadius;
   $PostFXManager::Settings::SSAO::lStrength                = $SSAOPostFx::lStrength;
   $PostFXManager::Settings::SSAO::overallStrength          = $SSAOPostFx::overallStrength;
   $PostFXManager::Settings::SSAO::quality                  = $SSAOPostFx::quality;
   $PostFXManager::Settings::SSAO::sDepthMax                = $SSAOPostFx::sDepthMax;
   $PostFXManager::Settings::SSAO::sDepthMin                = $SSAOPostFx::sDepthMin;
   $PostFXManager::Settings::SSAO::sDepthPow                = $SSAOPostFx::sDepthPow;
   $PostFXManager::Settings::SSAO::sNormalPow               = $SSAOPostFx::sNormalPow;
   $PostFXManager::Settings::SSAO::sNormalTol               = $SSAOPostFx::sNormalTol;
   $PostFXManager::Settings::SSAO::sRadius                  = $SSAOPostFx::sRadius;
   $PostFXManager::Settings::SSAO::sStrength                = $SSAOPostFx::sStrength;
}
//-----------------------------------------------------------------------------
// GFXStateBlockData / ShaderData
//-----------------------------------------------------------------------------

singleton GFXStateBlockData( SSAOStateBlock : PFX_DefaultStateBlock )
{   
   samplersDefined = true;
   samplerStates[0] = SamplerClampPoint;
   samplerStates[1] = SamplerWrapLinear;
   samplerStates[2] = SamplerClampPoint;
};

singleton GFXStateBlockData( SSAOBlurStateBlock : PFX_DefaultStateBlock )
{   
   samplersDefined = true;
   samplerStates[0] = SamplerClampLinear;
   samplerStates[1] = SamplerClampPoint;
};

singleton ShaderData( SSAOShader )
{   
   DXVertexShaderFile 	= $Core::CommonShaderPath @ "/postFX/postFxV.hlsl";
   DXPixelShaderFile 	= $Core::CommonShaderPath @ "/postFX/ssao/SSAO_P.hlsl";            
   
   OGLVertexShaderFile  = $Core::CommonShaderPath @ "/postFX/gl/postFxV.glsl";
   OGLPixelShaderFile   = $Core::CommonShaderPath @ "/postFX/ssao/gl/SSAO_P.glsl";

   samplerNames[0] = "$deferredMap";
   samplerNames[1] = "$randNormalTex";
   samplerNames[2] = "$powTable";
   
   pixVersion = 3.0;
};

singleton ShaderData( SSAOBlurYShader )
{
   DXVertexShaderFile 	= $Core::CommonShaderPath @ "/postFX/ssao/SSAO_Blur_V.hlsl";
   DXPixelShaderFile 	= $Core::CommonShaderPath @ "/postFX/ssao/SSAO_Blur_P.hlsl";   
   
   OGLVertexShaderFile  = $Core::CommonShaderPath @ "/postFX/ssao/gl/SSAO_Blur_V.glsl";
   OGLPixelShaderFile   = $Core::CommonShaderPath @ "/postFX/ssao/gl/SSAO_Blur_P.glsl";
   
   samplerNames[0] = "$occludeMap";
   samplerNames[1] = "$deferredMap";

   pixVersion = 3.0;      
   
   defines = "BLUR_DIR=float2(0.0,1.0)";         
};

singleton ShaderData( SSAOBlurXShader : SSAOBlurYShader )
{
   defines = "BLUR_DIR=float2(1.0,0.0)";
};

//-----------------------------------------------------------------------------
// PostEffects
//-----------------------------------------------------------------------------

singleton PostEffect( SSAOPostFx )
{     
   allowReflectPass = false;
     
   renderTime = "PFXBeforeBin";
   renderBin = "ProbeBin";   
   renderPriority = 10;
   
   shader = SSAOShader;
   stateBlock = SSAOStateBlock;
         
   texture[0] = "#deferred";         
   texture[1] = "core/postFX/images/noise.png";
   texture[2] = "#ssao_pow_table";
   
   target = "$outTex";
   targetScale = "0.5 0.5";
   targetViewport = "PFXTargetViewport_NamedInTexture0";
   
   singleton PostEffect()
   {
      internalName = "blurY";
      
      shader = SSAOBlurYShader;
      stateBlock = SSAOBlurStateBlock;
      
      texture[0] = "$inTex";
      texture[1] = "#deferred";
      
      target = "$outTex"; 
   };
      
   singleton PostEffect()
   {
      internalName = "blurX";
      
      shader = SSAOBlurXShader;
      stateBlock = SSAOBlurStateBlock;
      
      texture[0] = "$inTex";
      texture[1] = "#deferred";
      
      target = "$outTex"; 
   };   
   
   singleton PostEffect()
   {
      internalName = "blurY2";
      
      shader = SSAOBlurYShader;
      stateBlock = SSAOBlurStateBlock;
            
      texture[0] = "$inTex";
      texture[1] = "#deferred";
      
      target = "$outTex"; 
   };
   
   singleton PostEffect()
   {
      internalName = "blurX2";
            
      shader = SSAOBlurXShader;
      stateBlock = SSAOBlurStateBlock;
            
      texture[0] = "$inTex";
      texture[1] = "#deferred";
            
      // We write to a mask texture which is then
      // read by the lighting shaders to mask ambient.
      target = "#ssaoMask";   
   };  
};


/// Just here for debug visualization of the 
/// SSAO mask texture used during lighting.
singleton PostEffect( SSAOVizPostFx )
{      
   allowReflectPass = false;
        
   shader = PFX_PassthruShader;
   stateBlock = PFX_DefaultStateBlock;
   
   texture[0] = "#ssaoMask";
   
   target = "$backbuffer";
};

singleton ShaderData( SSAOPowTableShader )
{
   DXVertexShaderFile 	= $Core::CommonShaderPath @ "/postFX/ssao/SSAO_PowerTable_V.hlsl";
   DXPixelShaderFile 	= $Core::CommonShaderPath @ "/postFX/ssao/SSAO_PowerTable_P.hlsl";            
   
   OGLVertexShaderFile  = $Core::CommonShaderPath @ "/postFX/ssao/gl/SSAO_PowerTable_V.glsl";
   OGLPixelShaderFile   = $Core::CommonShaderPath @ "/postFX/ssao/gl/SSAO_PowerTable_P.glsl";   
   
   pixVersion = 2.0;
};

singleton PostEffect( SSAOPowTablePostFx )
{
   shader = SSAOPowTableShader;
   stateBlock = PFX_DefaultStateBlock;
   
   renderTime = "PFXTexGenOnDemand";
   
   target = "#ssao_pow_table"; 
   
   targetFormat = "GFXFormatR16F";   
   targetSize = "256 1";
};