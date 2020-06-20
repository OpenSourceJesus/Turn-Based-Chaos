using UnityEngine;
using System.Collections.Generic;
using Extensions;
using UnityEngine.UI;

namespace GridGame
{
	[RequireComponent(typeof(RawImage))]
	// [RequireComponent(typeof(Image))]
	public class AnimatedBackground : MonoBehaviour, IUpdatable
	{
		public bool PauseWhileUnfocused
		{
			get
			{
				return true;
			}
		}
		public RawImage rawImage;
		// public Image image;
		Texture2D texture;
		public Vector2Int textureSize;
		// public float pixelsPerUnit;
		Color[] colors = new Color[0];
		public FloatRange pixelChangeRate;
		public float maxPixelChangeRateDelta;
		public Gradient gradient;
		Pixel[] pixels = new Pixel[0];
		// Vector2 perlinOffset;
		// public float maxPerlinOffset;
		// public float maxPerlinOffsetChange;
		// public Vector2 perlinScale;
		public int blurRange;
		public float scale;

		void OnEnable ()
		{
			pixels = new Pixel[textureSize.x * textureSize.y];
			// perlinOffset = Random.insideUnitCircle * maxPerlinOffset;
			colors = new Color[textureSize.x * textureSize.y];
			for (int x = 0; x < textureSize.x; x ++)
			{
				for (int y = 0; y < textureSize.y; y ++)
				{
					Pixel pixel = new Pixel();
					// pixel.gradientPosition = Mathf.PerlinNoise(x / textureSize.x * perlinScale.x + perlinOffset.x, y / textureSize.y * perlinScale.y + perlinOffset.y);
					pixel.gradientPosition = Random.value;
					pixel.gradientPositionChangeRate = pixelChangeRate.Get(Random.value);
					colors[x + y * textureSize.x] = gradient.Evaluate(pixel.gradientPosition);
					pixels[x + y * textureSize.x] = pixel;
				}
			}
			texture = new Texture2D(textureSize.x, textureSize.y);
			texture.SetPixels(colors);
			texture.Apply();
			rawImage.texture = texture;
			// image.sprite = Sprite.Create(texture, Rect.MinMaxRect(0, 0, textureSize.x, textureSize.y), Vector2.one / 2, pixelsPerUnit);
			GameManager.updatables = GameManager.updatables.Add(this);
		}

		public void DoUpdate ()
		{
			for (int x = 0; x < textureSize.x; x ++)
			{
				for (int y = 0; y < textureSize.y; y ++)
				{
					Pixel pixel = pixels[x * textureSize.x + y];
					pixel.gradientPosition = Mathf.Clamp01(pixel.gradientPosition + pixel.gradientPositionChangeRate * Time.deltaTime);
					if (pixel.gradientPosition == 0 || pixel.gradientPosition == 1)
					{
						pixel.gradientPositionChangeRate *= -1;
						pixel.gradientPositionChangeRate += Random.Range(-maxPixelChangeRateDelta, maxPixelChangeRateDelta);
						pixel.gradientPositionChangeRate = Mathf.Clamp(pixel.gradientPositionChangeRate, pixelChangeRate.min, pixelChangeRate.max);
					}
					colors[x + y * textureSize.x] = gradient.Evaluate(pixel.gradientPosition);
					pixels[x + y * textureSize.x] = pixel;
				}
			}
			Blur (blurRange);
			texture.SetPixels(colors);
			texture.Apply();
		}

		void Blur (int blurRange)
		{
			Color[] newColors = new Color[colors.Length];
			for (int x = 0; x < textureSize.x; x ++)
			{
				for (int y = 0; y < textureSize.y; y ++)
				{
					List<Color> _colors = new List<Color>();
					for (int x2 = Mathf.Clamp(x - blurRange, 0, textureSize.x - 1); x2 <= Mathf.Clamp(x + blurRange, 0, textureSize.x - 1); x2 ++)
					{
						for (int y2 = Mathf.Clamp(y - blurRange, 0, textureSize.y - 1); y2 <= Mathf.Clamp(y + blurRange, 0, textureSize.y - 1); y2 ++)
							_colors.Add(colors[x2 + y2 * textureSize.x]);
					}
					newColors[x + y * textureSize.x] = ColorExtensions.GetAverage(_colors.ToArray());
				}
			}
			colors = newColors;
		}

		void OnDisable ()
		{
			GameManager.updatables = GameManager.updatables.Remove(this);
		}

		public class Pixel
		{
			public float gradientPosition;
			public float gradientPositionChangeRate;
		}
	}
}