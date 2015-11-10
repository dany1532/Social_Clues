using UnityEngine;
using System.Collections;

[System.Serializable]
public class UIScaledSprite {
	
	public UISprite sprite;
	public UIStretch stretch;
	private Transform transform;
	
	/// <summary>
	/// Initializes a new instance of the <see cref="UIScaledSprite"/> class.
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
		transform = sprite.transform;
	}
	
	/// <summary>
	/// Enable this instance.
	/// </summary>
	public void Enable ()
	{
		sprite.gameObject.SetActive(true);
	}
	
	/// <summary>
	/// Disable this instance.
	/// </summary>
	public void Disable ()
	{
		sprite.gameObject.SetActive(false);
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
	public void SetComponents(UISprite _sprite, UIStretch _stretch)
	{
		sprite = _sprite;
		transform = sprite.transform;
		
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
	public void SetSprite (UIAtlas atlas, string filename, Color color)
	{
		if (sprite != null)
		{
			sprite.atlas = atlas;
			sprite.spriteName = filename;
			sprite.color = color;
			
			if (stretch != null)
			{
				stretch.initialSize = new Vector2(sprite.mInner.width, sprite.mInner.height);
			}
		}
	}
	
	/// <summary>
	/// Updates the atlas.
	/// </summary>
	/// <param name='atlas'>
	/// Atlas.
	/// </param>
	public void UpdateSprite(string filename)
	{
		if (sprite != null)
		{
			sprite.spriteName = filename;
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
	public void UpdateSprite(string filename, Color color)
	{
		if (sprite != null)
		{
			sprite.spriteName = filename;
			sprite.color = color;
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
	public void UpdateColor(UIAtlas atlas, string filename, Color color)
	{
		if (sprite != null)
		{
			sprite.atlas = atlas;
			sprite.spriteName = filename;
			sprite.color = color;
		}
	}
	
	/// <summary>
	/// Updates the color.
	/// </summary>
	/// <param name='atlas'>
	/// Atlas.
	/// </param>
	/// <param name='color'>
	/// Color.
	/// </param>
	public void UpdateColor(UIAtlas atlas, Color color)
	{
		if (sprite != null)
		{
			sprite.atlas = atlas;
			sprite.color = color;
		}
	}
	
	/// <summary>
	/// Updates the color.
	/// </summary>
	/// <param name='color'>
	/// Color.
	/// </param>
	public void UpdateColor(Color color)
	{
		if (sprite != null)
		{
			sprite.color = color;
		}
	}
	
	/// <summary>
	/// Updates the atlas.
	/// </summary>
	/// <param name='atlas'>
	/// Atlas.
	/// </param>
	public void UpdateAtlas(UIAtlas atlas)
	{
		if (sprite != null)
		{
			sprite.atlas = atlas;
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
			stretch.initialSize = new Vector2(sprite.mInner.width, sprite.mInner.height);
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
			stretch.initialSize = new Vector2(sprite.mInner.width, sprite.mInner.height);
			stretch.relativeSize = newScale;
			stretch.UpdateStretch();
		}
	}
	
	/// <summary>
	/// Updates the stretching of the sprite
	/// </summary>
	/// <param name='filename'>
	/// Filename.
	/// </param>
	public void UpdateStretching(string filename)
	{
		if (filename != string.Empty)
		{
			if (sprite != null)
			{
				sprite.spriteName = filename;
			
				if (stretch != null)
				{
					stretch.initialSize = new Vector2(sprite.mInner.width, sprite.mInner.height);
				}
			}
		}
		else
		{
			UpdateStretching();
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
