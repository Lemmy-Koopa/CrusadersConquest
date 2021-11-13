
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using CrusadersConquest.Content.Projectiles;

namespace CrusadersConquest.Content.Items.Weapons
{
    public class AscusSpreader : ModItem
    {
		public override void SetStaticDefaults()
		{
			Item.staff[item.type] = true;
			Tooltip.SetDefault("Casts sticky ooey gooey shrooms");
		}

		public override void SetDefaults()
		{
			item.useTime = item.useAnimation = 26;
			item.damage = 5;
			item.knockBack = 3f;
			item.width = 50;
			item.height = 46;

			item.autoReuse = true;
			item.noMelee = true;
			item.magic = true;

			item.useStyle = ItemUseStyleID.HoldingOut;	
			item.value = Item.buyPrice(0, 0, 15, 0);
			item.rare = ItemRarityID.Green;
			item.mana = 8;
			item.UseSound = SoundID.Item21;
			
			item.shoot = ModContent.ProjectileType<StickyShroomProj>();
			item.shootSpeed = 12.5f;
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			Vector2 value = Vector2.Normalize(new Vector2(speedX, speedY)) * 16.5f;
			if (Collision.CanHit(position, 0, 0, position + value, 0, 0)){
				position += value;
			}
			Projectile.NewProjectile(position.X, position.Y, value.X, value.Y, type, damage, knockBack, player.whoAmI, 0f, 0f);
			return false;
		}
	}
	//Crafted With Wood, Mushrooms, Wildnature Element.
}

