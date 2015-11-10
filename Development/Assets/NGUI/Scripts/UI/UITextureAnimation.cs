using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UITextureAnimation : UISpriteAnimation {
	
	public List<Texture> textures;
	UITexture mTexture;
	int startingIndex = 0;

	class TexurerComparer: IComparer<Texture>
	{
	    public int Compare(Texture x, Texture y)
		{
			return x.name.CompareTo(y.name);
		}
	}
	
	void Awake()
	{
		TexurerComparer tc = new TexurerComparer();
		textures.Sort(tc);	
	}
	
	void Start()
	{
		RebuildSpriteList();
		if (playOnStart)
			UIPlay();
	}
	
	override protected void RebuildSpriteList ()
	{
		if (mTexture == null) mTexture = GetComponent<UITexture>();
		mSpriteNames.Clear();
		startingIndex = -1;
		
		if (mTexture != null && textures != null)
		{
			for (int i = 0, imax = textures.Count; i < imax; ++i)
			{
				Texture texture = textures[i];

				if (string.IsNullOrEmpty(mPrefix) || texture.name.StartsWith(mPrefix))
				{
					if (startingIndex < 0) startingIndex = i;
					mSpriteNames.Add(texture.name);
				}
			}
			mSpriteNames.Sort();
		}
		
	}
	
	override protected void ChangeSprite (int index)
	{
		mTexture.mainTexture = textures[startingIndex + index];
	}
}
