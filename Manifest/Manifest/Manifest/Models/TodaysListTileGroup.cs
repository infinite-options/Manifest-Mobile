using System;
using System.Collections.ObjectModel;

namespace Manifest.Models
{
    class TodaysListTileGroup : ObservableCollection<TodaysListTile>
    {
        public string Name { get; set; }
        public string GroupIcon { get; set; }

        public TodaysListTileGroup(string Name, ObservableCollection<TodaysListTile> Tiles) : base(Tiles)
        {
            this.Name = Name;
        }

        public TodaysListTileGroup(string Name, string GroupIcon)
        {
            this.Name = Name;
            this.GroupIcon = GroupIcon;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
