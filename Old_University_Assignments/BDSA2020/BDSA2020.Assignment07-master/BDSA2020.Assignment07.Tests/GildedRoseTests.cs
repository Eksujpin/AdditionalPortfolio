using System.ComponentModel;
using Xunit;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO; 

namespace GildedRose
{
    public class GildedRoseTests
    {
        GildedRose app;
        IList<Item> Items = new List<Item>();

        public GildedRoseTests()
        {
            List<Item> TestItems = new List<Item>{
                new Item {Name = "+5 Dexterity Vest", SellIn = 10, Quality = 20},
                new Item {Name = "Aged Brie", SellIn = 1, Quality = 0},
                new Item {Name = "Elixir of the Mongoose", SellIn = 5, Quality = 7},
                new Item {Name = "Sulfuras, Hand of Ragnaros", SellIn = 0, Quality = 80},
                new Item {Name = "Sulfuras, Hand of Ragnaros", SellIn = -1, Quality = 80},
                new Item
                {
                    Name = "Backstage passes to a TAFKAL80ETC concert",
                    SellIn = 15,
                    Quality = 20
                },
				// this conjured item does not work properly yet
				new Item {Name = "Conjured Mana Cake", SellIn = 3, Quality = 6}
            };

            foreach (Item item in TestItems)
            {
                Items.Add(new Item
                {
                    Name = item.Name,
                    SellIn = item.SellIn,
                    Quality = item.Quality
                });
            }

            app = new GildedRose(TestItems);
        }

        [Fact]
        public void Sell_date_passed_quality_degrade_test()
        {
            app.UpdateQuality();

            for (int i = 0; i < Items.Count; i++)
            {
                var itemAfter = app.getItems()[i];
                var itemBefore = Items[i];
                if (itemAfter.Quality < itemBefore.Quality)
                {
                    if (itemBefore.Name.StartsWith("Conjured"))
                    {
                        if (itemAfter.SellIn <= 0)
                        {
                            Assert.Equal(itemAfter.Quality, (itemBefore.Quality - 4));
                        }
                        else
                        {
                            Assert.Equal(itemAfter.Quality, (itemBefore.Quality - 2));
                        }
                    }
                    else
                    {
                        if (itemAfter.SellIn <= 0)
                        {
                            Assert.Equal(itemAfter.Quality, (itemBefore.Quality - 2));
                        }
                        else
                        {
                            Assert.Equal(itemAfter.Quality, (itemBefore.Quality - 1));
                        }
                    }
                }
            }
        }

        [Fact]
        public void Quality_Negative_Test()
        {
            app.UpdateQuality();

            bool QualityNegVal = false;
            foreach (Item I in app.getItems())
            {
                if (I.Quality < 0)
                {
                    QualityNegVal = true;
                }
            }
            Assert.Equal("False", QualityNegVal.ToString());
        }

        [Fact]
        public void Aged_Brie_increasing()
        {
            app.UpdateQuality();
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i].Name.StartsWith("Aged"))
                {
                    Assert.Equal(app.getItems()[i].Quality, Items[i].Quality+1);
                }
            }
        }
        [Fact]
        public void Aged_Brie_increasing_Double()
        {
            for (int i = 0; i < 3; i++)
            {
                app.UpdateQuality();
            }
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i].Name.StartsWith("Aged"))
                {
                    Assert.Equal(app.getItems()[i].Quality, Items[i].Quality+5);
                }
            }
        }

        [Fact]
        public void Quality_never_more_then_50()
        {
            for (int i = 0; i < 62; i++)
            {
                app.UpdateQuality();
            }

            foreach (Item item in app.getItems())
            {
                if (item.Quality != 80)
                {
                    Assert.True(item.Quality <= 50);
                }
            }

        }

        [Fact]
        public void Sulfuras_never_Degrade()
        {
            app.UpdateQuality();
            app.UpdateQuality();
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i].Name.StartsWith("Sulfuras"))
                {
                    Assert.Equal(app.getItems()[i].Quality, Items[i].Quality);
                    Assert.Equal(80, Items[i].Quality);
                }
            }
        }

        [Fact]
        public void Backstage_passes_test()
        {
            //Should have SellIn = 15, quality at 20
            var backstagepass = app.getItems().Where(item => item.Name.Contains("Backstage")).FirstOrDefault();

            for (int i = 0; i < 5; i++)
            {
                app.UpdateQuality();
            }
            //Should have SellIn = 10 after 5 ticks, quality gone up by 6 so it should be at 26. 
            Assert.Equal(26, backstagepass.Quality);

            for (int i = 0; i < 5; i++)
            {
                app.UpdateQuality();
            }
            //Should have SellIn = 5 after 5 ticks, quality gone up by 4 * 2 and 3 * 1 so it should be at 37. 
            Assert.Equal(37, backstagepass.Quality);
            app.UpdateQuality();
            //Should have SellIn = 4 after 1 tick, quality gone up by 1 * 3 so it should be at 40.
            Assert.Equal(40, backstagepass.Quality);
            for (int i = 0; i < 5; i++)
            {
                app.UpdateQuality();
            }
            //Should have SellIn = -1 after 5 ticks, quality should be 0.
            Assert.Equal(0, backstagepass.Quality);
        }

        [Fact]
        public void Conjured_degrades_twice_as_fast()
        {
            var qualityBeforeUpdate = Items.Where(item => item.Name.StartsWith("Conjured")).FirstOrDefault().Quality;
            app.UpdateQuality();
            var qualityAfterUpdate = app.getItems().Where(item => item.Name.StartsWith("Conjured")).FirstOrDefault().Quality;
            var difference = qualityBeforeUpdate - qualityAfterUpdate;
            Assert.Equal(2, difference);
        }
        [Fact]
        public void coverageTest()
        {
            Program.Main(new string[2]);
        }
    }
}