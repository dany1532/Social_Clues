using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Blend two textures from one to the other, using along with the appropriate shader
/// We always blend from one texture to another (0 to 1) and update the target texture in the appropriate end
/// </summary>
public class BlendTextures : MonoBehaviour {
	// Frequency between each update of the texture
	public float blendFrequency = 0.1f;
	// Blending step during each update
	public float blendStep = 0.01f;
		
	// Current blend position
	float blendPosition = 0.0f;
	
	// If we are blending towards texure1 (true) or texture2 (false)
	bool setTexture1 = false;
	
	public UITexture textureUI;
	
	// Initial textures for the demo
	public Texture texture1;
	public Texture texture2;
	
	// Use this for initialization
	void Start () {
		SetTexture(texture1, 1);
		SetTexture(texture2, 2);
		
		if (textureUI != null)
		{
			textureUI.material.SetFloat( "_Blend", blendPosition );
		}
		else
			// Set initial blending value
			this.renderer.material.SetFloat( "_Blend", blendPosition );
	}
	
	/// <summary>
	/// Sets specific texture on material
	/// </summary>
	/// <param name='texture'>
	/// New texture
	/// </param>
	/// <param name='textureId'>
	/// Texture position in materal (either 1 or 2)
	/// </param>
	public void SetTexture(Texture texture, int textureId)
	{
		if (textureUI != null){
			textureUI.enabled = true;
			textureUI.material.SetTexture( "_Texture" + textureId, texture );
		}
		else
			// Update target texture to iven texture
			this.renderer.material.SetTexture( "_Texture" + textureId, texture );
	}
	
	public void ClearTexture(){
		textureUI.enabled = false;	
	}
	
	/// <summary>
	/// Blend from current texture to given texture
	/// </summary>
	/// <param name='texture'>
	/// New texture to blend to
	/// </param>
	public void SwitchToTexture(Texture texture)
	{
		if (textureUI != null){
			textureUI.enabled = true;
			textureUI.material.SetTexture( "_Texture" + ((setTexture1) ? 1 : 2), texture );
		}
		else
			// Update target texture to iven texture
			this.renderer.material.SetTexture( "_Texture" + ((setTexture1) ? 1 : 2), texture );
		// Start invoking blending function
		InvokeRepeating("BlendTexture",0, blendFrequency);
	}
	
	/// <summary>
	/// Blends from current to target texture
	/// </summary>
	void BlendTexture ()
	{
		// Calculate current blending position
		blendPosition = blendPosition + ((setTexture1) ? -blendStep : blendStep);
		if (textureUI != null)
			textureUI.material.SetFloat( "_Blend", blendPosition );
		else
			// Update blending position on shader
			this.renderer.material.SetFloat( "_Blend", blendPosition );
		
		// if we finished blending process
		if(blendPosition < 0 || blendPosition > 1) {
			// stop blending process
			CancelInvoke("BlendTexture");
			// and update target texture
			setTexture1 = !setTexture1;
		}
	}
}
