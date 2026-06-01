using System.IO;
using UnityEngine;

namespace Softwyx.CareerLog.Ui.Records.Menu;

/// <summary>Loads <c>assets/icon_records_screen.png</c> for the Records screen caption.</summary>
internal static class ScreenIcon{
    private const string FileName = "icon_records_screen.png";

    // Match vanilla handbook caption sprites (icon_button_encyclopedia_idle uses PPU 100).
    private const float SpritePixelsPerUnit = 100f;

    private static Sprite _sprite;

    public static Sprite Get(){
        if(_sprite) return _sprite;

        var texture = LoadTexture();

        if(!texture) return null;

        _sprite = CreateSprite(texture, "CareerLog_RecordsScreenIcon");

        return _sprite;
    }

    private static Texture2D LoadTexture(){
        var path = PluginPaths.AssetFile(FileName);

        if(!File.Exists(path)){
            CareerLogPlugin.Log?.LogWarning(PluginInfo.Format($"Screen icon not found: {path}"));

            return null;
        }

        var fileData = File.ReadAllBytes(path);
        var texture  = new Texture2D(2, 2, TextureFormat.ARGB32, false);

        if(!texture.LoadImage(fileData)){
            CareerLogPlugin.Log?.LogWarning(PluginInfo.Format($"Failed to decode screen icon: {path}"));

            Object.Destroy(texture);

            return null;
        }

        texture.name       = "CareerLog_RecordsScreenIcon";
        texture.filterMode = FilterMode.Bilinear;
        texture.wrapMode   = TextureWrapMode.Clamp;
        texture.Apply();

        CareerLogPlugin.Log?.LogInfo(PluginInfo.Format($"Screen icon loaded from {path}."));

        return texture;
    }

    private static Sprite CreateSprite(Texture2D texture, string spriteName){
        var sprite = Sprite.Create(
                                   texture,
                                   new Rect(0f, 0f, texture.width, texture.height),
                                   new Vector2(0.5f, 0.5f),
                                   SpritePixelsPerUnit
                                  );
        sprite.name = spriteName;

        return sprite;
    }
}
