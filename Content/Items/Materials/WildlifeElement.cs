
using Terraria.ID;
using Terraria.ModLoader;

namespace CrusadersConquest.Content.Items.Materials
{
    public class WildlifeElement : ModItem
    {
        public override void SetStaticDefaults() => DisplayName.SetDefault("Wildlife Element");
 
        public override void SetDefaults()
        {
            item.width = 14;
            item.height = 20;
            item.maxStack = 999;
            item.value = 100;
            item.rare = ItemRarityID.Blue;
        }
    }
}