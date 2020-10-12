using Manifest.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Manifest.Controls
{
    public class TodaysListTileSelector : DataTemplateSelector
    {
        public DataTemplate EventTodaysListTemplate { get; set; }
        public DataTemplate OccuranceTodaysListTemplate { get; set; }
        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            TodaysListTile displayObject = item as TodaysListTile;
            if (displayObject.Type == TileType.Occurance)
            {
                return OccuranceTodaysListTemplate;
            }
            else
            {
                return EventTodaysListTemplate;
            }
        }
    }
}
