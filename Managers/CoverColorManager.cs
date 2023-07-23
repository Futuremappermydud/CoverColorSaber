using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Concurrent;
using ColorThief;
using CoverColorSaber.Models;
using JetBrains.Annotations;
using CoverColorSaber.Settings;
using Zenject;
using Color = UnityEngine.Color;
using BS_Utils.Utilities;
using IPA.Utilities.Async;
using System;

namespace CoverColorSaber.Managers
{
	public class CoverColorManager
    {
        private CacheManager _cacheManager;
        private SettingsMenu _settingsMenu;

		[Inject]
		public void Construct(CacheManager cacheManager, SettingsMenu settingsMenu)
        {
            _cacheManager = cacheManager;
			_settingsMenu = settingsMenu;

			BSEvents.levelSelected += LevelSelected;
		}
		private async void LevelSelected(LevelCollectionViewController lcvc, IPreviewBeatmapLevel level)
		{
			Texture2D tex = null;
			Sprite sprite = (await level.GetCoverImageAsync(System.Threading.CancellationToken.None));
			try
			{
				tex = sprite.texture;
			}
			catch (Exception)
			{
				await UnityMainThreadTaskScheduler.Factory.StartNew(() =>
				{
					if (level is not CustomPreviewBeatmapLevel)
					{
						tex = GetFromUnreadable(sprite.texture, InvertAtlas(sprite.textureRect));
					}
					else
					{
						tex = GetFromUnreadable(sprite.texture, sprite.textureRect);
					}
				});
			}
			if (tex == null) return;

			var scheme = new ColorScheme("CoverSaber", "Cover Saber", true, "Cover Saber", false, Color.white, Color.white, Color.white, Color.white, true, Color.white, Color.white, Color.white);
			var colors = new List<QuantizedColor>();

			await Task.Run(async () => {
				var data = await GetSchemeFromCoverImage(tex, level.levelID);
				scheme = _cacheManager.GetOrAdd(level.levelID, data).Scheme;
				colors = data.Colors;
			});

			_settingsMenu.SongName = level.songName;
			_settingsMenu.SetColors(colors, scheme, level.levelID);
		}

		public async Task<ColorDataResult> GetSchemeFromCoverImage(Texture2D tex, string levelID)
        {
			Plugin.Log.Info("x");
			Plugin.Log.Info((tex is null).ToString());
			if (_cacheManager.TryGetCache(levelID, out ColorDataResult result))
			{
				return result;
			}
			result = new ColorDataResult();

			var thief = new ColorThief.ColorThief();
            var colors = new List<QuantizedColor>();
			Plugin.Log.Info("x2");

			await Task.Run(() => colors = thief.GetPalette(tex));
			Plugin.Log.Info("x3");
			colors.Sort((x, y) => y.Population.CompareTo(x.Population));
			Plugin.Log.Info("x4");
			Plugin.Log.Info((colors is null).ToString());

			var leftColor = colors[0].UnityColor;
            var rightColor = colors[1].UnityColor;
            var obsColor = colors[2].UnityColor;
			Plugin.Log.Info("x4.5");


			result.Scheme = new ColorScheme("CoverSaber", "Cover Saber", true, "Cover Saber", true, leftColor, rightColor, leftColor * 1.3f, rightColor * 1.3f, false, leftColor * 1.5f, rightColor * 1.5f, obsColor);
			result.Colors = colors;
			Plugin.Log.Info("x5");

			_cacheManager.SetCache(levelID, result);
			Plugin.Log.Info("x6");
			return result;
        }

		private static Texture2D GetFromUnreadable(Texture2D tex, Rect rect)
		{
			var tmp = RenderTexture.GetTemporary(
									tex.width,
									tex.height,
									0,
									RenderTextureFormat.Default,
									RenderTextureReadWrite.Linear);
			Graphics.Blit(tex, tmp);
			var previous = RenderTexture.active;
			RenderTexture.active = tmp;
			var myTexture2D = new Texture2D((int)rect.width, (int)rect.height);
			myTexture2D.ReadPixels(rect, 0, 0);
			myTexture2D.Apply();
			RenderTexture.active = previous;
			RenderTexture.ReleaseTemporary(tmp);
			return myTexture2D;
		}

		private static Rect InvertAtlas(Rect i)
		{
			Rect o = new(i.x, i.y, i.width, i.height);
			o.y = 2048 - o.y - 160;
			return o;
		}
	}
}
