using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;

namespace CrusadersConquest.Content.Items.Armour.Mushroom
{
	// Mushroom Helmet
	//     Also does Set Bonus
	[AutoloadEquip(EquipType.Head)]
	public class MushroomHelmet : ModItem //Missed chance to call it a MushroomCap, tbh
	{
		public override void SetDefaults()
		{
			item.width = 34; //Texture width (x2 for export)
			item.height = 28; //Texture height (x2 for export)
			item.rare = 2; //ItemRarityID.Green
			item.value = 1500 * 5; //15 s, 0 c
			item.defense = 1;
		}
		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			return body.type == ModContent.ItemType<MushroomBreastplate>() && legs.type == ModContent.ItemType<MushroomBoots>();
		}
		public override void UpdateArmorSet(Player player)
		{
			player.setBonus = "Enhanced effectiveness of healing items";
			//If you want to change set bonus text, just change the string to something else
			player.GetModPlayer<CCPlayer>().shroomHeal = true; //Field in modplayer, to allow this set's gimmick to work
			player.potionDelayTime -= 300;
		}

		public override void AddRecipes()
		{
			//Modify this as necessary
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Mushroom, 8);
			recipe.AddIngredient(ItemID.Wood, 40);
			recipe.AddIngredient(ModContent.ItemType<Materials.WildlifeElement>(), 5);
			recipe.AddTile(TileID.Benches);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}

	// Mushroom Breastplate
	[AutoloadEquip(EquipType.Body)]
	public class MushroomBreastplate : ModItem //Missed chance to call it a MushroomCap, tbh
	{
		public override void SetDefaults()
		{
			item.width = 38; //Texture width (x2 for export)
			item.height = 26; //Texture height (x2 for export)
			item.rare = 2; //ItemRarityID.Green
			item.value = 2000 * 5; //20 s, 0 c
			item.defense = 2;
		}

		public override void AddRecipes()
		{
			//Modify this as necessary
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Mushroom, 5);
			recipe.AddIngredient(ItemID.Wood, 60);
			recipe.AddIngredient(ModContent.ItemType<Materials.WildlifeElement>(), 10);
			recipe.AddTile(TileID.Benches);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}

	// Mushroom Boots
	[AutoloadEquip(EquipType.Legs)]
	public class MushroomBoots : ModItem
	{
		public override void SetDefaults()
		{
			item.width = 22; //Texture width (x2 for export)
			item.height = 18; //Texture height (x2 for export)
			item.rare = 2; //ItemRarityID.Green
			item.value = 1000 * 5; //10 s, 0 c
			item.defense = 1;
		}

		public override void AddRecipes()
		{
			//Modify this as necessary
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Mushroom, 10);
			recipe.AddIngredient(ItemID.Wood, 40);
			recipe.AddIngredient(ModContent.ItemType<Materials.WildlifeElement>(), 5);
			recipe.AddTile(TileID.Benches);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}

	// Temporary ModPlayer location
	//     Recommend moving to a more globally-accessible location (and in a different file)
	//     because this will inevitably end up being abused later on.
	public class CCPlayer : ModPlayer
    {
		// This region will get progressively larger as we add to ModPlayer.
        #region ModPlayer Fields
        public bool shroomHeal;

        public override void ResetEffects()
        {
			shroomHeal = false;
        }
        public override void UpdateDead()
        {
			shroomHeal = false;
        }
        #endregion
        public override void GetHealLife(Item item, bool quickHeal, ref int healValue)
        {
			if(shroomHeal)
            {
				healValue += 10;
            }
            base.GetHealLife(item, quickHeal, ref healValue);
        }
    }
}
