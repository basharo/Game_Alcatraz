using Alcatraz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alcatraz
{
    public class Move
    {
        private Player player;
        private Prisoner prisoner;
        private int rowOrCol;
        private int row;
        private int col;

        public Move(Player player, Prisoner prisoner, int rowOrCol, int row, int col)
        {
            this.player = player;
            this.prisoner = prisoner;
            this.rowOrCol = rowOrCol;
            this.row = row;
            this.col = col;
        }

        public Player getPLayer()
        {
            return this.player;
        }
        public Prisoner getPrisoner()
        {
            return this.prisoner;
        }
        public int getRowOrCol()
        {
            return this.rowOrCol;
        }
        public int getRow()
        {
            return this.row;
        }
        public int getCol()
        {
            return this.col;
        }
    }
}
