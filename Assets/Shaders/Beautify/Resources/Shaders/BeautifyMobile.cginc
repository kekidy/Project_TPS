	#include "UnityCG.cginc"

	uniform sampler2D      _MainTex;
	uniform sampler2D_half _CameraDepthTexture;
	uniform sampler2D      _OverlayTex;
	uniform sampler2D      _ScreenLum;
	uniform sampler2D      _BloomTex;
	
	uniform half4 _MainTex_TexelSize;
	uniform half4 _MainTex_ST;
	uniform half4 _ColorBoost; // x = Brightness, y = Contrast, z = Saturate, w = Daltonize;
	uniform half4 _Sharpen;
	uniform half4 _Dither;
	uniform half4 _FXColor;
	uniform half4 _Vignetting;
	uniform half4 _Frame;
	uniform half4 _Outline;
	uniform half4 _Dirt;		
    uniform half3 _Bloom;
    uniform half4 _CompareParams;
	uniform half  _VignettingAspectRatio;
	
    struct appdata {
    	float4 vertex : POSITION;
		half2 texcoord : TEXCOORD0;
    };
    
	struct v2f {
	    float4 pos : SV_POSITION;
	    half2 uv: TEXCOORD0;
    	half2 depthUV : TEXCOORD1;	 
    	#if DIRT   
	    half2 uvNonStereo: TEXCOORD2;
	    #endif
	};

	v2f vert(appdata v) {
    	v2f o;
    	o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
   		o.uv = UnityStereoScreenSpaceUVAdjust(v.texcoord, _MainTex_ST);
    	o.depthUV = o.uv;
    	#if DIRT
   		o.uvNonStereo = v.texcoord;
   		#endif
    	#if UNITY_UV_STARTS_AT_TOP
    	if (_MainTex_TexelSize.y < 0) {
	        // Depth texture is inverted WRT the main texture
    	    o.depthUV.y = 1.0 - o.depthUV.y;
    	}
    	#endif
    	return o;
	}

	half getLuma(half3 rgb) { 
		const half3 lum = half3(0.299, 0.587, 0.114);
		return dot(rgb, lum);
	}
	
	half3 getNormal(half depth, half depth1, half depth2, half2 offset1, half2 offset2) {
  		float3 p1 = float3(offset1, depth1 - depth);
  		float3 p2 = float3(offset2, depth2 - depth);
  		float3 normal = cross(p1, p2);
	  	return normalize(normal);
	}
		
	half getRandomFast(half2 uv) {
		const float2 OFFSET = float2( 26.0, 161.0 );
		const float DOMAIN = 71.0;
		const float ONE_DIV_DOMAIN = 0.01408450704; // 1 / DOMAIN
		const float ONE_DIV_SOMELARGEFLOAT = 0.001051374728; //1.0 / 951.135664;
		float2 p = float2(uv) + _Time.yy;
		p -= floor(p * ONE_DIV_DOMAIN) * DOMAIN;    
		p += OFFSET.xy;                                
		p *= p;                                          
		return frac(p.x * p.y * ONE_DIV_SOMELARGEFLOAT);
	}
	
	void beautifyPassFast(v2f i, inout half3 rgbM) {
		
	    const half3 halves = half3(0.5, 0.5, 0.5);
	    const half4 ones   = half4(1.0,1.0,1.0,1.0);

		// Grab scene info
		half3 uvInc      = half3(_MainTex_TexelSize.x, _MainTex_TexelSize.y, 0);
		half  depthN     = Linear01Depth(UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, i.depthUV + uvInc.zy)));
		half  depthW     = Linear01Depth(UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, i.depthUV - uvInc.xz)));
		half  lumaM      = getLuma(rgbM);
		
		#if !NIGHT_VISION && !THERMAL_VISION
		// 0. RGB Dither		
		half3 dither     = dot(half2(171.0, 231.0), i.uv * _ScreenParams.xy).xxx;
		      dither     = frac(dither / half3(103.0, 71.0, 97.0)) - halves;
		      rgbM      *= 1.0 + step(_Dither.y, depthW) * dither * _Dither.x;

		// 1. Daltonize
		#if DALTONIZE
		half3 rgb0       = ones.xyz - saturate(rgbM.rgb);
		      rgbM.r    *= 1.0 + rgbM.r * rgb0.g * rgb0.b * _ColorBoost.w;
			  rgbM.g    *= 1.0 + rgbM.g * rgb0.r * rgb0.b * _ColorBoost.w;
			  rgbM.b    *= 1.0 + rgbM.b * rgb0.r * rgb0.g * _ColorBoost.w;	
			  rgbM      *= lumaM / getLuma(rgbM);
		#endif
		
		// 2. Sharpen
		half  depthClamp = abs(depthW - _Dither.z) < _Dither.w;
		half  maxDepth   = max(depthN, depthW);
		half  minDepth   = min(depthN, depthW);
		half  dDepth     = maxDepth - minDepth;
		half  lumaDepth  = saturate(_Sharpen.y / dDepth);
	    half3 rgbN       = tex2D(_MainTex, i.uv + uvInc.zy).rgb;
		half3 rgbS       = tex2D(_MainTex, i.uv - uvInc.zy).rgb;
	    half3 rgbW       = tex2D(_MainTex, i.uv - uvInc.xz).rgb;
    	half  lumaN      = getLuma(rgbN);
    	half  lumaW      = getLuma(rgbW);
    	half  lumaS      = getLuma(rgbS);
    	half  maxLuma    = max(lumaN,lumaS);
    	      maxLuma    = max(maxLuma, lumaW);
	    half  minLuma    = min(lumaN,lumaS);
	          minLuma    = min(minLuma, lumaW) - 0.000001;
	    half  lumaPower  = 2 * lumaM - minLuma - maxLuma;
		half  lumaAtten  = saturate(_Sharpen.w / (maxLuma - minLuma));
		      rgbM      *= 1.0 + clamp(lumaPower * lumaAtten * lumaDepth * _Sharpen.x, -_Sharpen.z, _Sharpen.z) * depthClamp;
		
		// 3. Vibrance
		half3 maxComponent = max(rgbM.r, max(rgbM.g, rgbM.b));
 		half3 minComponent = min(rgbM.r, min(rgbM.g, rgbM.b));
 		half  sat        = saturate(maxComponent - minComponent);
		      rgbM      *= 1.0 + _ColorBoost.z * (1.0 - sat) * (rgbM - getLuma(rgbM));

  		#endif	// night & thermal vision exclusion

   		// 4. Tinting
   		#if TINT
  		      rgbM       = lerp(rgbM, rgbM * _FXColor.rgb, _FXColor.a);
  		#endif

   	 	// 5. Bloom
   	 	#if BLOOM
   		#if UNITY_COLORSPACE_GAMMA
		rgbM = GammaToLinearSpace(rgbM);
		#endif
		rgbM += tex2D(_BloomTex, i.uv).rgb * _Bloom.xxx;
   		#if UNITY_COLORSPACE_GAMMA
		rgbM = LinearToGammaSpace(rgbM);
		#endif
		#endif
		
 	 	// 6. Lens Dirt
		#if DIRT
		half  curLum     = tex2D(_ScreenLum, halves.xy).a;
   	 	half  scrLum     = saturate((curLum - _Dirt.z) * _Dirt.y);
  	 	       rgbM      = lerp(rgbM, (1.0 - (1.0-rgbM) * (1.0-tex2D(_OverlayTex, i.uv))), scrLum);
	   	#endif

   	 	// 7. Night Vision
   	 	#if NIGHT_VISION
   	 	       lumaM      = getLuma(rgbM);	// updates luma
		half   depth      = Linear01Depth(UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, i.depthUV)));
   		half3  normalNW   = getNormal(depth, depthN, depthW, uvInc.zy, -uvInc.xz);
   		half   nvbase     = saturate(normalNW.z - 0.8); // minimum ambient self radiance (useful for pitch black)
   			   nvbase    += lumaM;						// adds current lighting
   			   nvbase    *= nvbase * (0.5 + nvbase);	// increase contrast
   			   rgbM	      = nvbase * _FXColor.rgb;
   		half2  uvs        = floor(i.uv.xy * _ScreenParams.xy);
   			   rgbM      *= frac(uvs.y*0.25)>0.4;	// scan lines
   			   rgbM	     *= 1.0 + getRandomFast(uvs) * 0.3 - 0.15;				// noise
	 	#endif
   
  	 	// 8. Thermal Vision
   	 	#if THERMAL_VISION
   	 	       lumaM      = getLuma(rgbM);	// updates luma
    	half3 tv0 	     = lerp(half3(0.0,0.0,1.0), half3(1.0,1.0,0.0), lumaM * 2.0);
    	half3 tv1	     = lerp(half3(1.0,1.0,0.0), half3(1.0,0.0,0.0), lumaM * 2.0 - 1.0);
    		  rgbM       = lerp(tv0, tv1, lumaM >= 0.5);
   		half2 uvs        = floor(i.uv.xy * _ScreenParams.xy);
   			  rgbM      *= 0.2 + frac(uvs.y*0.25)>0.4;	// scan lines
   			  rgbM		*= 1.0 + getRandomFast(uvs) * 0.2 - 0.1;				// noise
	 	#endif

		// 9. Final contrast + brightness
  			  rgbM       = (rgbM - halves) * _ColorBoost.y + halves;
  			  rgbM      *= _ColorBoost.x;

  		// 10. Colored vignetting
  		#if VIGNETTING
  		half2 vd         = half2(i.uv.x - 0.5, (i.uv.y - 0.5) * _VignettingAspectRatio);
  		      rgbM       = lerp(rgbM, lumaM * _Vignetting.rgb, saturate(_Vignetting.a * dot(vd,vd)));
  		#endif

   		// 11. Sepia
   		#if SEPIA
   		      rgbM.r     = dot(rgbM, half3(0.393, 0.769, 0.189));
              rgbM.g     = dot(rgbM, half3(0.349, 0.686, 0.168));   
              rgbM.b     = dot(rgbM, half3(0.272, 0.534, 0.131));
        #endif

   		// 12. Outline
   		#if OUTLINE
   		#if !NIGHT_VISION
   		half depth       = Linear01Depth(UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, i.depthUV)));
   		half3 normalNW   = getNormal(depth, depthN, depthW, uvInc.zy, -uvInc.xz);
   		#endif
		half  depthE     = Linear01Depth(UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, i.depthUV + uvInc.xz)));		
		half  depthS     = Linear01Depth(UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, i.depthUV - uvInc.zy)));
   		half3 normalSE   = getNormal(depth, depthS, depthE, -uvInc.zy, uvInc.xz);
		half  dnorm      = dot(normalNW, normalSE);
   		rgbM             = lerp(rgbM, _Outline.rgb, dnorm  < _Outline.a);
	 	#endif
	 	  		
  		// 13. Border
  		if (_Frame.a) {
  		      rgbM       = lerp(rgbM, _Frame.rgb, saturate( (max(abs(i.uv.x - 0.5), abs(i.uv.y - 0.5)) - _Frame.a) * 50.0));
   		}
	}

	half4 fragBeautifyFast (v2f i) : SV_TARGET {
   		half4 pixel = tex2D(_MainTex, i.uv);
   		beautifyPassFast(i, pixel.rgb);
   		return pixel;
	}
	
	half4 fragCompareFast (v2f i) : SV_TARGET {
		// separator line + antialias
		half2 dd     = i.uv - 0.5.xx;
		half  co     = dot(_CompareParams.xy, dd);
		half  dist   = distance( _CompareParams.xy * co, dd );
		half4 aa     = saturate( (_CompareParams.w - dist) / abs(_MainTex_TexelSize.y) );

		half4 pixel  = tex2D(_MainTex, i.uv);

		// are we on the beautified side?
		half s       = dot(dd, _CompareParams.yz);
		if (s>0) beautifyPassFast(i, pixel.rgb);

		return pixel + aa;

	}
