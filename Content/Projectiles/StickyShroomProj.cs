using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CrusadersConquest.Content.Projectiles
{
    public class StickyShroomProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 16;

            projectile.friendly = true;
            projectile.magic = true;

            projectile.penetrate = 8;
        }

        public override void DrawBehind(int index, List<int> drawCacheProjsBehindNPCsAndTiles, List<int> drawCacheProjsBehindNPCs, List<int> drawCacheProjsBehindProjectiles, List<int> drawCacheProjsOverWiresUI)
        {
            if (projectile.ai[0] == 1f)
            {
                int npcIndex = (int)projectile.ai[1];
                if (npcIndex >= 0 && npcIndex < 200 && Main.npc[npcIndex].active)
                {
                    if (Main.npc[npcIndex].behindTiles)
                    {
                        drawCacheProjsBehindNPCsAndTiles.Add(index);
                    }
                    else drawCacheProjsBehindNPCs.Add(index);
                    return;
                }
            }
            drawCacheProjsBehindProjectiles.Add(index);
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
        {
            width = height = 14;
            return true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            projectile.penetrate--;
            if (projectile.penetrate <= 0)
            {
                projectile.Kill();
            }
            else
            {
                if (projectile.velocity.X != oldVelocity.X)
                {
                    projectile.velocity.X = -oldVelocity.X;
                }
                if (projectile.velocity.Y != oldVelocity.Y)
                {
                    projectile.velocity.Y = -oldVelocity.Y;
                }
                Main.PlaySound(SoundID.Item10, projectile.position);
                projectile.velocity /= 1.25f;
            }
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (targetHitbox.Width > 12 && targetHitbox.Height > 12)
            {
                targetHitbox.Inflate(-targetHitbox.Width / 12, -targetHitbox.Height / 12);
            }
            return projHitbox.Intersects(targetHitbox);
        }

        public bool BeSticking
        {
            get => projectile.ai[0] == 1f;
            set => projectile.ai[0] = value ? 1f : 0f;
        }

        public int TargetWhoAmI
        {
            get => (int)projectile.ai[1];
            set => projectile.ai[1] = value;
        }

        bool onceDone = false;
        int projDmg;
        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (onceDone == false)
            {
                projDmg = projectile.damage;
                BeSticking = true;
                TargetWhoAmI = target.whoAmI;
                projectile.netUpdate = true;
                UpdateStickyShrooms(target);
                onceDone = true;
            }
            projectile.damage = 0;
        }

        private const int shroomAmount = 5;
        private readonly Point[] shroomSticking = new Point[shroomAmount];
        private void UpdateStickyShrooms(NPC target)
        {
            int shroomI = 0;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile thisProj = Main.projectile[i];
                if (i != projectile.whoAmI && thisProj.active && thisProj.owner == Main.myPlayer && thisProj.type == projectile.type && thisProj.modProjectile is StickyShroomProj stickyProj && stickyProj.BeSticking && stickyProj.TargetWhoAmI == target.whoAmI)
                {
                    shroomSticking[shroomI++] = new Point(i, thisProj.timeLeft);
                    if (shroomI >= shroomSticking.Length)
                    {
                        break;
                    }
                }
            }

            if (shroomI >= shroomAmount)
            {
                int BeSticking = 0;
                for (int i = 1; i < shroomAmount; i++)
                {
                    if (shroomSticking[i].Y < shroomSticking[BeSticking].Y)
                    {
                        BeSticking = i;
                    }
                }
                Main.projectile[shroomSticking[BeSticking].X].Kill();
            }
        }

        public override void AI()
        {
            UpdateAlpha();
            if (BeSticking)
            {
                StickyAI();
            }
            else
            {
                NormalAI();
            }
        }

        private void UpdateAlpha()
        {
            if (projectile.alpha > 0)
            {
                projectile.alpha -= 20;
            }

            if (projectile.alpha < 0)
            {
                projectile.alpha = 0;
            }
        }

        private void NormalAI()
        {
            projectile.rotation += 0.125f;
            TargetWhoAmI++;
            if (TargetWhoAmI >= 20)
            {
                const float velXmult = 0.99f;
                const float velYmult = 0.67f;
                TargetWhoAmI = 20;
                projectile.velocity.X *= velXmult;
                projectile.velocity.Y += velYmult;
            }
            if (Main.rand.Next(35) == 0)
            {
                int num = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 138, 0f, 0f, 0, default, 1f);
                Main.dust[num].velocity *= 0.95f;
                Main.dust[num].scale *= 0.8f;
                Main.dust[num].noGravity = true;

            }
        }

        private void StickyAI()
        {
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            const int aiFactor = 1;
            projectile.localAI[0] += 1f;

            if (Main.rand.Next(25) == 0)
            {
                int num = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 138, 0f, 0f, 0, default, 1f);
                Main.dust[num].velocity *= 1.1f;
                Main.dust[num].scale *= 0.95f;
                Main.dust[num].noGravity = true;
            }
            bool hitEffect = projectile.localAI[0] % 30f == 0f;
            int projTargetIndex = TargetWhoAmI;
            if (projectile.localAI[0] >= 60 * aiFactor || projTargetIndex < 0 || projTargetIndex >= 200)
            {
                projectile.Kill();
            }
            else if (Main.npc[projTargetIndex].active && !Main.npc[projTargetIndex].dontTakeDamage)
            {
                projectile.Center = Main.npc[projTargetIndex].Center - projectile.velocity * 2f;
                projectile.gfxOffY = Main.npc[projTargetIndex].gfxOffY;
                if (hitEffect)
                {
                    projectile.damage = projDmg;
                    projectile.Damage();
                }
            }
            else
            {
                projectile.Kill();
            }
        }

        public override void Kill(int timeLeft)
        {

            if (BeSticking)
            {
                for (int i = 0; i < 3; i++)
                {
                    int num = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 138, 0f, 0f, 0, Color.Orange, 1f);
                    Main.dust[num].velocity *= 1.25f;
                    Main.dust[num].scale *= 0.9f;
                }
                for (int i = 0; i < Main.rand.Next(1, 3); i++)
                {
                    float xStuff = Main.rand.Next(-7, 7);
                    float yStuff = Main.rand.Next(-7, 7);
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, xStuff, yStuff, mod.ProjectileType("ShroomyCloudProj"), projectile.damage / 2, projectile.knockBack, projectile.owner, 0f, 0f);
                }
            }
            else
            {
                for (int i = 0; i < 2; i++)
                {
                    int num = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 138, 0f, 0f, 0, Color.Orange, 1f);
                    Main.dust[num].velocity *= 0.85f;
                    Main.dust[num].scale *= 0.75f;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color color)
        {
            Vector2 vector = new Vector2(projectile.width * 0.5f, projectile.height * 0.5f);
            float trailSize = projectile.scale;
            for (int i = 0; i < 5; i++)
            {
                trailSize *= 0.91f;
                color = new Color(255, 255, 255) * 0.5f;
                Vector2 position = projectile.oldPos[i] - Main.screenPosition + vector + new Vector2(0f, projectile.gfxOffY);
                Color trailColor = projectile.GetAlpha(color) * ((float)(projectile.oldPos.Length - i) / projectile.oldPos.Length);
                spriteBatch.Draw(Main.projectileTexture[projectile.type], position, null, trailColor, projectile.rotation, new Vector2(Main.projectileTexture[projectile.type].Width * 0.5f, Main.projectileTexture[projectile.type].Height * 0.5f), trailSize, SpriteEffects.None, 0f);
            }
            return false;
        }
    }

    public class ShroomyCloudProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 26;

            projectile.magic = true;
            projectile.friendly = true;
            projectile.tileCollide = false;

            projectile.penetrate = -1;
            projectile.timeLeft = 75;


            projectile.alpha = 35;
        }

        public override void AI()
        {
            projectile.rotation += 0.005f;
            projectile.velocity *= 0.94f;
            if (Main.rand.Next(30) == 0)
            {
                int num = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, DustID.Amber, 0f, 0f, 0, default, 1f);
                Main.dust[num].velocity *= 1.1f;
                Main.dust[num].scale *= 0.9f;
                Main.dust[num].noGravity = true;
            }

            if (projectile.timeLeft <= 20)
            {
                projectile.alpha += 15;
            }
            if (projectile.alpha >= 255)
            {
                projectile.alpha = 255;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color color)
        {
            Vector2 vector = new Vector2(projectile.width * 0.5f, projectile.height * 0.5f);
            float trailSize = projectile.scale;
            for (int i = 0; i < 5; i++)
            {
                trailSize *= 1.035f;
                color = new Color(255, 255, 255) * 0.25f;
                Vector2 position = projectile.oldPos[i] - Main.screenPosition + vector + new Vector2(0f, projectile.gfxOffY);
                Color trailColor = projectile.GetAlpha(color) * ((float)(projectile.oldPos.Length - i) / projectile.oldPos.Length);
                spriteBatch.Draw(Main.projectileTexture[projectile.type], position, null, trailColor, projectile.rotation, new Vector2(Main.projectileTexture[projectile.type].Width * 0.5f, Main.projectileTexture[projectile.type].Height * 0.5f), trailSize, SpriteEffects.None, 0f);
            }
            return false;
        }
    }
}