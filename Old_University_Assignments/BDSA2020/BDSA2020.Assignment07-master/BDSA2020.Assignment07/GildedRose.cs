using System.Collections.Generic;

namespace GildedRose
{
    public class GildedRose
    {
        private IList<Item> Items { get; }

        public GildedRose(IList<Item> items)
        {
            Items = items;
        }

        public IList<Item> getItems()
        {
             return Items;
        }

        public void UpdateNormal(Item item)
        {
            item.SellIn--;
            if (item.Quality <= 50 && item.Quality > 0) 
            {
                item.Quality--;
                if(item.SellIn < 0 && item.Quality > 0) item.Quality--; 
            }
        }
        public void UpdateBrie(Item item)
        {
            item.SellIn--;
            if (item.Quality < 50) 
            {
                item.Quality++;
                if(item.SellIn < 0 && item.Quality < 50) item.Quality++; 
            }
        }

        public void UpdateBackstage(Item item)
        {
            item.SellIn--;
            if (item.Quality < 50 && item.SellIn >=0) 
            {
                item.Quality++;
                if(item.SellIn <= 10 && item.Quality < 50) item.Quality++; 
                if(item.SellIn <= 5 && item.Quality < 50) item.Quality++; 
            }
            if(item.SellIn < 0) item.Quality = 0; 
        }

        public void UpdateConjured(Item item)
        {
            item.SellIn--;
            int newQuali = item.Quality;
            if (newQuali > 0)
            {
               newQuali -= 2; 
               if(item.SellIn <=0) newQuali-=2;
            } 
            if (newQuali < 0) newQuali = 0;
            item.Quality = newQuali;
        }
        public void UpdateQuality()
        {
            for (var i = 0; i < Items.Count; i++)
            {
                var item = Items[i];
                UpdateQualityItem(item); 
            }
        }

        private void UpdateQualityItem(Item item) {
            switch (item.Name)
                {
                    case "Aged Brie":
                        UpdateBrie(item);
                        break;
                    case "Sulfuras, Hand of Ragnaros":
                        //Do nothing
                        break;
                    case "Backstage passes to a TAFKAL80ETC concert":
                        UpdateBackstage(item);
                        break;
                    case "Conjured Mana Cake":
                        UpdateConjured(item);
                        break;
                    default:
                        UpdateNormal(item);
                        break;
                }
        }
    }
}
