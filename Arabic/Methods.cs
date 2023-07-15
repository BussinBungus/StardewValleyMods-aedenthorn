﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System;
using System.Text;
using System.Xml;
using StardewValley.Buildings;

namespace Arabic
{
    public partial class ModEntry
    {
        private void ReloadFonts()
        {
            dialogueFont = MakeFont("dialogueFont");
            smallFont = MakeFont("smallFont");
            tinyFont = MakeFont("tinyFont");
        }

        private SpriteFont MakeFont(string name)
        {
            Monitor.Log($"Making font {name}");
            var arabicTexture = Helper.ModContent.Load<Texture2D>($"assets/{name}.png");
            var arabicMap = new Dictionary<string, Mapping>();
            XmlReader xmlReader = XmlReader.Create(Path.Combine(Helper.DirectoryPath, "assets", $"{name}.fnt"));
            Monitor.Log("reading xml font");
            while (xmlReader.Read())
            {
                //keep reading until we see your element
                if (xmlReader.Name.Equals("char") && (xmlReader.NodeType == XmlNodeType.Element))
                {
                    arabicMap[xmlReader.GetAttribute("id")] = new Mapping()
                    {
                        x = int.Parse(xmlReader.GetAttribute("x")),
                        y = int.Parse(xmlReader.GetAttribute("y")),
                        width = int.Parse(xmlReader.GetAttribute("width")),
                        height = int.Parse(xmlReader.GetAttribute("height")),
                        xo = int.Parse(xmlReader.GetAttribute("xoffset")),
                        yo = int.Parse(xmlReader.GetAttribute("yoffset")),
                        xa = int.Parse(xmlReader.GetAttribute("xadvance")),
                    };
                }
            }
            var fontObject = new SpriteFontMapping()
            {
                DefaultCharacter = '؟',
                LineSpacing = 24,
                Spacing = -1
            };
            var glyphs = new List<Rectangle>();
            var cropping = new List<Rectangle>();
            var charMap = new List<char>();
            var kerning = new List<Vector3>();
            foreach (var m in arabicMap)
            {
                var ch = (char)int.Parse(Convert.ToString(int.Parse(m.Key), 16), NumberStyles.HexNumber);
                var glyph = new Rectangle(m.Value.x, m.Value.y, m.Value.width, m.Value.height);
                var crop = new Rectangle(m.Value.xo, m.Value.yo, m.Value.width, m.Value.height);
                var kern = new Vector3(-m.Value.xa + m.Value.width, m.Value.width, 0);
                fontObject.Characters.Add(ch);
                fontObject.Glyphs.Add(ch, new SpriteFont.Glyph()
                {
                    BoundsInTexture = glyph,
                    Cropping = crop,
                    Character = ch,
                    LeftSideBearing = kern.X,
                    RightSideBearing = kern.Z,
                    Width = kern.Y,
                    WidthIncludingBearings = kern.X + kern.Y + kern.Z
                });
                glyphs.Add(glyph);
                cropping.Add(crop);
                charMap.Add(ch);
                kerning.Add(kern);
            }
            //File.WriteAllText(Path.Combine(Helper.DirectoryPath, "assets", "SpriteFont1.ar.json"), JsonConvert.SerializeObject(fontObject, Newtonsoft.Json.Formatting.Indented));
            return new SpriteFont(arabicTexture, glyphs, cropping, charMap, 32, -1, kerning, '؟');
            //var spriteFont = SHelper.GameContent.Load<SpriteFont>("Fonts/SpriteFont1.ar-AR");
        }
        private static void FixForArabic(ref SpriteFont spriteFont, ref string text, ref Vector2 position)
        {
            if (!Config.ModEnabled || text?.Length == 0 || LocalizedContentManager.CurrentLanguageCode != LocalizedContentManager.LanguageCode.mod || LocalizedContentManager.CurrentModLanguage.LanguageCode != "ar")
                return;
            if (spriteFont == Game1.smallFont)
            {
                spriteFont = smallFont;
            }
            else if (spriteFont == Game1.tinyFont)
            {
                spriteFont = tinyFont;
            }
            else if (spriteFont == Game1.dialogueFont)
            {
                spriteFont = dialogueFont;
            }
            else
            {
                return;
            }
            if (!spriteFont.Characters.Contains(text[0]))
                return;
            string inter = "";
            for (int i = text.Length - 1; i >= 0; i--)
            {
                inter += text[i];
            }
            text = inter;
        }
        private static void FixForArabic(ref SpriteFont spriteFont, ref StringBuilder text, ref Vector2 position)
        {
            if (!Config.ModEnabled || text?.Length == 0 || LocalizedContentManager.CurrentLanguageCode != LocalizedContentManager.LanguageCode.mod || LocalizedContentManager.CurrentModLanguage.LanguageCode != "ar")
                return;
            if (spriteFont == Game1.smallFont)
            {
                spriteFont = smallFont;
            }
            else if (spriteFont == Game1.tinyFont)
            {
                spriteFont = tinyFont;
            }
            else if (spriteFont == Game1.dialogueFont)
            {
                spriteFont = dialogueFont;
            }
            else
            {
                return;
            }
            if (!spriteFont.Characters.Contains(text[0]))
                return;
            StringBuilder inter = new StringBuilder();
            for (int i = text.Length - 1; i >= 0; i--)
            {
                inter.Append(text[i]);
            }
            text = inter;
        }

    }
}