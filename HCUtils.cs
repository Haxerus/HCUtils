using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI;

namespace HCUtils
{
	public class HCUtils : Mod
	{
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
			bool hide = ModContent.GetInstance<HCConfig>().hidePlayerNames;
			if (hide)
            {
				layers.Insert(20, new LegacyGameInterfaceLayer("HCUtils : MP Player Names", DrawCustomLayer, InterfaceScaleType.UI));
				for (int i = 0; i < layers.Count; i++)
				{
					if (layers[i].Name.Equals("Vanilla: MP Player Names"))
						layers[i].Active = false;
				}
			}
        }

        private bool DrawCustomLayer()
        {
			PlayerInput.SetZoom_World();
			int screenW = Main.screenWidth;
			int screenH = Main.screenHeight;
			Vector2 screenPos = Main.screenPosition;
			PlayerInput.SetZoom_UI();
			float uIScale = Main.UIScale;
			for (int i = 0; i < 255; i++)
			{
				if (Main.player[i].active && Main.myPlayer != i && !Main.player[i].dead && Main.player[Main.myPlayer].team > 0 && Main.player[Main.myPlayer].team == Main.player[i].team)
				{
					string text = Main.player[i].name;
					if (Main.player[i].statLife < Main.player[i].statLifeMax2)
					{
						object obj = text;
						text = obj + ": " + Main.player[i].statLife + "/" + Main.player[i].statLifeMax2;
					}

					Vector2 textPos = Main.fontMouseText.MeasureString(text);
					float textOffset = 0f;

					if (Main.player[i].chatOverhead.timeLeft > 0)
					{
						textOffset = -textPos.Y;
					}

					Vector2 screenCenter = new Vector2(screenW / 2 + screenPos.X, screenH / 2 + screenPos.Y);
					Vector2 position = Main.player[i].position;
					position += (position - screenCenter) * (Main.GameViewMatrix.Zoom - Vector2.One);

					float normColor = Main.mouseTextColor / 255f;
					Color color = new Color((byte)(Main.teamColor[Main.player[i].team].R * normColor), (byte)(Main.teamColor[Main.player[i].team].G * normColor), (byte)(Main.teamColor[Main.player[i].team].B * normColor), Main.mouseTextColor);
					
					float newX = position.X + (float)(Main.player[i].width / 2) - screenCenter.X;
					float newY = position.Y - screenPos.Y - 2f + textOffset - screenCenter.Y;
					float num8 = (float)Math.Sqrt((double)(newX * newX + newY * newY));

					int smallerDim = screenW > screenH ? screenW : screenH;
					smallerDim = smallerDim / 2 - 30;

					if (smallerDim < 100)
					{
						smallerDim = 100;
					}

					textPos.X = position.X + (float)(Main.player[i].width / 2) - textPos.X / 2f - screenPos.X;
					textPos.Y = position.Y - textPos.Y - 2f + textOffset - screenPos.Y;

					if (Main.player[Main.myPlayer].gravDir == -1f)
					{
						textPos.Y = (float)screenH - textPos.Y;
					}
					textPos *= 1f / uIScale;

					Vector2 mouseTextPos = Main.fontMouseText.MeasureString(text);
					textPos += mouseTextPos * (1f - uIScale) / 4f;

					Main.spriteBatch.DrawString(Main.fontMouseText, text, new Vector2(textPos.X - 2f, textPos.Y), Color.Black, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
					Main.spriteBatch.DrawString(Main.fontMouseText, text, new Vector2(textPos.X + 2f, textPos.Y), Color.Black, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
					Main.spriteBatch.DrawString(Main.fontMouseText, text, new Vector2(textPos.X, textPos.Y - 2f), Color.Black, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
					Main.spriteBatch.DrawString(Main.fontMouseText, text, new Vector2(textPos.X, textPos.Y + 2f), Color.Black, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
					Main.spriteBatch.DrawString(Main.fontMouseText, text, textPos, color, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
				}
			}

			return true;
		}
    }
}