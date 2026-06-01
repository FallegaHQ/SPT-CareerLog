using System.IO;
using UnityEngine;

namespace Softwyx.CareerLog.Ui.Records.Menu;

/// <summary>Loads <c>assets/icon_records.png</c>.</summary>
internal static class MenuIcon{
    private const string FileName = "icon_records.png";

    private const float SpritePixelsPerUnit = 200f;

    private static Sprite _sprite;
    private static Sprite _spriteMenuOverhaul;

    public static Sprite Get(bool menuOverhaulLayout){
        return menuOverhaulLayout ? GetMenuOverhaul() : GetVanilla();
    }

    private static Sprite GetVanilla(){
        if(_sprite) return _sprite;

        var texture = LoadTexture(false);

        if(!texture) return null;

        _sprite = CreateSprite(texture, "CareerLog_RecordsMenuIcon");

        return _sprite;
    }

    private static Sprite GetMenuOverhaul(){
        if(_spriteMenuOverhaul) return _spriteMenuOverhaul;

        var texture = LoadTexture(true);

        if(!texture) return null;

        _spriteMenuOverhaul = CreateSprite(texture, "CareerLog_RecordsMenuIcon_MenuOverhaul");

        return _spriteMenuOverhaul;
    }

    private static Texture2D LoadTexture(bool invertLineArt){
        var path = PluginPaths.AssetFile(FileName);

        if(!File.Exists(path)){
            CareerLogPlugin.Log?.LogWarning(PluginInfo.Format($"Menu icon not found: {path}"));

            return null;
        }

        var fileData = File.ReadAllBytes(path);
        var texture  = new Texture2D(2, 2, TextureFormat.ARGB32, false);

        if(!texture.LoadImage(fileData)){
            CareerLogPlugin.Log?.LogWarning(PluginInfo.Format($"Failed to decode menu icon: {path}"));

            Object.Destroy(texture);

            return null;
        }

        if(invertLineArt) InvertLineArt(texture);

        texture.name       = invertLineArt ? "CareerLog_RecordsMenuIcon_Inverted" : "CareerLog_RecordsMenuIcon";
        texture.filterMode = FilterMode.Bilinear;
        texture.wrapMode   = TextureWrapMode.Clamp;
        texture.Apply();

        CareerLogPlugin.Log?.LogInfo(
                                     PluginInfo.Format(
                                                       invertLineArt
                                                           ? $"Menu icon loaded (Menu Overhaul invert) from {path}."
                                                           : $"Menu icon loaded from {path}."
                                                      )
                                    );

        return texture;
    }

    private static void InvertLineArt(Texture2D texture){
        var pixels = texture.GetPixels();

        for(var i = 0; i < pixels.Length; i++){
            var pixel = pixels[i];

            if(pixel.a < 0.01f) continue;

            pixel.r   = 1f - pixel.r;
            pixel.g   = 1f - pixel.g;
            pixel.b   = 1f - pixel.b;
            pixels[i] = pixel;
        }

        texture.SetPixels(pixels);
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
