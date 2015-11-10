using UnityEngine;
using System.Collections;

[System.Serializable]
public class UIScaledTexture {
	
	public UITexture texture;
	public UIStretch stretch;
	private Transform transform;
	
	/// <summary>
	/// Initializes a new instance of the <see cref="UIScaledTexture"/> class.
	/// </summary>
	public void Start()
	{
		Initialize();
	}

	/// <summary>
	/// Initialize this instance.
	/// </summary>
	public void Initialize()
	{
		transform = texture.transform;
	}
	
	/// <summary>
	/// Enable this instance.
	/// </summary>
	public void Enable ()
	{
		texture.gameObject.SetActive(true);
	}
	
	/// <summary>
	/// Disable this instance.
	/// </summary>
	public void Disable ()
	{
		texture.gameObject.SetActive(false);
	}
	
	/// <summary>
	/// Sets the components.
	/// </summary>
	/// <param name='_sprite'>
	/// _sprite.
	/// </param>
	/// <param name='_stretch'>
	/// _stretch.
	/// </param>
	public void SetComponents(UITexture _texture, UIStretch _stretch)
	{
		texture = _texture;
		transform = texture.transform;
		
		stretch = _stretch;
	}
	
	/// <summary>
	/// Sets a new sprite
	/// </summary>
	/// <param name='atlas'>
	/// Atlas.
	/// </param>
	/// <param name='filename'>
	/// Filename.
	/// </param>
	/// <param name='color'>
	/// Color.
	/// </param>
	public void SetTexture (Texture _texture, Color color)
	{
		if (_texture != null)
		{
			texture.mainTexture = _texture;
			texture.color = color;
			
			if (stretch != null)
			{
				stretch.initialSize = new Vector2(_texture.width, _texture.height);
			}
		}
	}
	
	/// <summary>
	/// Sets a new sprite
	/// </summary>
	/// <param name='atlas'>
	/// Atlas.
	/// </param>
	/// <param name='filename'>
	/// Filename.
	/// </param>
	/// <param name='color'>
	/// Color.
	/// </param>
	public void SetTexture (Texture _texture)
	{
		if (_texture != null)
		{
			texture.mainTexture = _texture;
			
			if (stretch != null)
			{
				stretch.initialSize = new Vector2(_texture.width, _texture.height);
			}
		}
	}
	/// <summary>
	/// Updates the atlas.
	/// </summary>
	/// <param name='atlas'>
	/// Atlas.
	/// </param>
	public void UpdateTexture(Texture _texture)
	{
		if (texture != null)
		{
			texture.mainTexture = _texture;
		}
	}
	
	/// <summary>
	/// Updates the sprite.
	/// </summary>
	/// <param name='filename'>
	/// Filename.
	/// </param>
	/// <param name='color'>
	/// Color.
	/// </param>
	public void UpdateTexture(Texture _texture, Color color)
	{
		if (texture != null)
		{
			texture.mainTexture = _texture;
			texture.color = color;
		}
	}
	
	/// <summary>
	/// Updates the color.
	/// </summary>
	/// <param name='atlas'>
	/// Atlas.
	/// </param>
	/// <param name='sprite'>
	/// Sprite.
	/// </param>
	/// <param name='color'>
	/// Color.
	/// </param>
	public void UpdateColor(Color color)
	{
		if (texture != null)
		{
			texture.color = color;
		}
	}
	
	/// <summary>
	/// Updates the stretching of the sprite
	/// </summary>
	/// <param name='filename'>
	/// Filename.
	/// </param>
	public void UpdateStretching()
	{
		if (stretch != null)
		{
			stretch.initialSize = new Vector2(texture.mainTexture.width, texture.mainTexture.height);
		}
	}
	
	/// <summary>
	/// Updates the stretching of the sprite
	/// </summary>
	/// <param name='filename'>
	/// Filename.
	/// </param>
	public void UpdateStretching(Vector2 newScale)
	{
		if (stretch != null)
		{
			stretch.initialSize = new Vector2(texture.mainTexture.width, texture.mainTexture.height);
			stretch.relativeSize = newScale;
			stretch.UpdateStretch();
		}
	}
	
	/// <summary>
	/// Gets the position.
	/// </summary>
	/// <returns>
	/// The position.
	/// </returns>
	public Vector3 GetPosition ()
	{
		return transform.position;
	}
	
	/// <summary>
	/// Gets the scale.
	/// </summary>
	/// <returns>
	/// The scale.
	/// </returns>
	public Vector3 GetScale ()
	{
		return transform.localScale;
	}
	
	/// <summary>
	/// Gets the relative scale.
	/// </summary>
	/// <returns>
	/// The relative scale.
	/// </returns>
	public Vector2 GetRelativeScale ()
	{
		return stretch.relativeSize;
	}
}
