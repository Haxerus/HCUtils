using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Achievements;
using Terraria.Graphics.Shaders;
using Terraria.Localization;
using Terraria.ModLoader;

namespace HCUtils
{
    class HCPlayer : ModPlayer
    {

        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            if (this.player.whoAmI != Main.myPlayer)
                return true;

            CustomKill(damageSource, damage, hitDirection, pvp);

            return false;
        }

        /*
         * This is more or less a copy of the Vanilla Player.KillMe() function with death messages and death X's made toggleable
         */
        public void CustomKill(PlayerDeathReason damageSource, double damage, int hitDirection, bool pvp)
        {
            Player player = this.player;
            bool hideDeaths = ModContent.GetInstance<HCConfig>().hideDeaths;

            var log = ModContent.GetInstance<HCUtils>().Logger;

            if (player.dead)
                return;
            if (pvp)
                player.pvpDeath = true;
            if (player.trapDebuffSource)
                AchievementsHelper.HandleSpecialEvent(player, 4);
            player.lastDeathPostion = player.Center;
            player.lastDeathTime = DateTime.Now;

            /*
             * Hide death X's locally
             */
            if (hideDeaths)
            {
                player.showLastDeath = false;
            } else
            {
                player.showLastDeath = true;
            }

            bool overFlowing;
            int coinsOwned = (int)Utils.CoinsCount(out overFlowing, player.inventory);
            if (Main.myPlayer == player.whoAmI)
            {
                player.lostCoins = coinsOwned;
                player.lostCoinString = Main.ValueToCoins(player.lostCoins);
            }
            if (Main.myPlayer == player.whoAmI)
                Main.mapFullscreen = false;
            if (Main.myPlayer == player.whoAmI)
            {
                player.trashItem.SetDefaults(0, false);
                if ((int)player.difficulty == 0)
                {
                    for (int index = 0; index < 59; ++index)
                    {
                        if ((player.inventory[index].stack <= 0 ? 0 : (player.inventory[index].type < 1522 || player.inventory[index].type > 1527 ? (player.inventory[index].type == 3643 ? 1 : 0) : 1)) != 0)
                        {
                            int number = Item.NewItem((int)player.position.X, (int)player.position.Y, player.width, player.height, player.inventory[index].type, 1, false, 0, false, false);
                            Main.item[number].netDefaults(player.inventory[index].netID);
                            Main.item[number].Prefix((int)player.inventory[index].prefix);
                            Main.item[number].stack = player.inventory[index].stack;
                            Main.item[number].velocity.Y = (float)((double)Main.rand.Next(-20, 1) * 0.200000002980232);
                            Main.item[number].velocity.X = (float)((double)Main.rand.Next(-20, 21) * 0.200000002980232);
                            Main.item[number].noGrabDelay = 100;
                            Main.item[number].favorited = false;
                            Main.item[number].newAndShiny = false;
                            if (Main.netMode == 1)
                                NetMessage.SendData(21, -1, -1, (NetworkText)null, number, 0.0f, 0.0f, 0.0f, 0, 0, 0);
                            player.inventory[index].SetDefaults(0, false);
                        }
                    }
                }
                else if ((int)player.difficulty == 1)
                    player.DropItems();
                else if ((int)player.difficulty == 2)
                {
                    player.DropItems();
                    player.KillMeForGood();
                }
            }
            Main.PlaySound(5, (int)player.position.X, (int)player.position.Y, 1, 1f, 0.0f);
            player.headVelocity.Y = (float)((double)Main.rand.Next(-40, -10) * 0.100000001490116);
            player.bodyVelocity.Y = (float)((double)Main.rand.Next(-40, -10) * 0.100000001490116);
            player.legVelocity.Y = (float)((double)Main.rand.Next(-40, -10) * 0.100000001490116);
            player.headVelocity.X = (float)((double)Main.rand.Next(-20, 21) * 0.100000001490116 + (double)(2 * hitDirection));
            player.bodyVelocity.X = (float)((double)Main.rand.Next(-20, 21) * 0.100000001490116 + (double)(2 * hitDirection));
            player.legVelocity.X = (float)((double)Main.rand.Next(-20, 21) * 0.100000001490116 + (double)(2 * hitDirection));
            if (player.stoned)
            {
                player.headPosition = Vector2.Zero;
                player.bodyPosition = Vector2.Zero;
                player.legPosition = Vector2.Zero;
            }
            for (int index1 = 0; index1 < 100; ++index1)
            {
                if (player.stoned)
                    Dust.NewDust(player.position, player.width, player.height, 1, (float)(2 * hitDirection), -2f, 0, default(Color), 1f);
                else if (player.frostArmor)
                {
                    int index2 = Dust.NewDust(player.position, player.width, player.height, 135, (float)(2 * hitDirection), -2f, 0, default(Color), 1f);
                    Main.dust[index2].shader = GameShaders.Armor.GetSecondaryShader(player.ArmorSetDye(), player);
                }
                else if (player.boneArmor)
                {
                    int index2 = Dust.NewDust(player.position, player.width, player.height, 26, (float)(2 * hitDirection), -2f, 0, default(Color), 1f);
                    Main.dust[index2].shader = GameShaders.Armor.GetSecondaryShader(player.ArmorSetDye(), player);
                }
                else
                    Dust.NewDust(player.position, player.width, player.height, 5, (float)(2 * hitDirection), -2f, 0, default(Color), 1f);
            }
            player.mount.Dismount(player);
            player.dead = true;
            player.respawnTimer = 600;
            bool flag = false;
            if (Main.netMode != 0 && !pvp)
            {
                for (int index = 0; index < 200; ++index)
                {
                    if (Main.npc[index].active && (Main.npc[index].boss || Main.npc[index].type == 13 || (Main.npc[index].type == 14 || Main.npc[index].type == 15)) && (double)Math.Abs((float)(player.Center.X - Main.npc[index].Center.X)) + (double)Math.Abs((float)(player.Center.Y - Main.npc[index].Center.Y)) < 4000.0)
                    {
                        flag = true;
                        break;
                    }
                }
            }
            if (flag)
                player.respawnTimer = player.respawnTimer + 600;
            if (Main.expertMode)
                player.respawnTimer = (int)((double)player.respawnTimer * 1.5);
            player.immuneAlpha = 0;
            player.palladiumRegen = false;
            player.iceBarrier = false;
            player.crystalLeaf = false;

            NetworkText deathText = damageSource.GetDeathText(player.name);
            if (Main.netMode == 2)
            {
                if (!hideDeaths)
                    NetMessage.BroadcastChatMessage(deathText, new Color(225, 25, 25), -1);
            }
            else if (Main.netMode == 0)
            {
                if (!hideDeaths)
                    Main.NewText(deathText.ToString(), (byte)225, (byte)25, (byte)25, false);
            }
                
            if (Main.netMode == 1 && player.whoAmI == Main.myPlayer)
            {
                if (!hideDeaths)
                    NetMessage.SendPlayerDeath(player.whoAmI, damageSource, (int)damage, hitDirection, pvp, -1, -1);
            }
                


            if (player.whoAmI == Main.myPlayer && (int)player.difficulty == 0)
            {
                if (!pvp)
                {
                    player.DropCoins();
                }
                else
                {
                    player.lostCoins = 0;
                    player.lostCoinString = Main.ValueToCoins(player.lostCoins);
                }
            }

            player.DropTombstone(coinsOwned, deathText, hitDirection);

            if (player.whoAmI == Main.myPlayer)
            {
                try
                {
                    WorldGen.saveToonWhilePlaying();
                }
                catch
                {
                }
            }
        }
    }
}
