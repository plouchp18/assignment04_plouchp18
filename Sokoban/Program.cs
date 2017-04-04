using System;
using System.Windows.Forms;

namespace Sokoban
{
    public class Program : Engine
    {

        public static SlideSprite player;
        public static SlideSprite[,] goals;
        public static SlideSprite[,] walls;
        public static SlideSprite[,] blocks;
        public static int height;
        public static int width;
        public static String map;
        public static int x;
        public static int y;
        public static Box winScreen = new Box(0, 0, 0, 0, 0);
        public static TextSprite winText = new TextSprite(0, 0, "       You Win!\nPress R to restart or N to play a new map");
        public static String[] levels = new String[] { Properties.Resources.level1, Properties.Resources.level2 };
        public static int level = 0;



        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Right)
            {
                if (canMoveTo(x + 1, y, 1, 0)) x++;
                if (blocks[x, y] != null) moveBlock(x, y, 1, 0);
            }
            if (e.KeyCode == Keys.Left)
            {
                if (canMoveTo(x - 1, y, -1, 0)) x--;
                if (blocks[x, y] != null) moveBlock(x, y, -1, 0);
            }
            if (e.KeyCode == Keys.Up)
            {
                if (canMoveTo(x, y - 1, 0, -1)) y--;
                if (blocks[x, y] != null) moveBlock(x, y, 0, -1);
            }
            if (e.KeyCode == Keys.Down)
            {
                if (canMoveTo(x, y + 1, 0, 1)) y++;
                if (blocks[x, y] != null) moveBlock(x, y, 0, 1);
            }
            if (e.KeyCode == Keys.R) 
            {
                Reset();
            }
            if (e.KeyCode == Keys.N) 
            {
                NewMap();
            }
            if (e.KeyCode == Keys.P) 
            {
                jukebox.PlayLooping();
            }
            if (e.KeyCode == Keys.M)
            {
                jukebox.Stop();
            }
            player.TargetX = x * 100;
            player.TargetY = y * 100;
            setWinText();
        }

        public void moveBlock(int i, int j, int dx, int dy)
        {
            blocks[i + dx, j + dy] = blocks[i, j];
            blocks[i, j] = null;

            blocks[i + dx, j + dy].TargetX = (i + dx) * 100;
            blocks[i + dx, j + dy].TargetY = (j + dy) * 100;
            if (goals[i + dx, j + dy] != null) blocks[i + dx, j + dy].image = Properties.Resources.final;
            else blocks[i + dx, j + dy].image = Properties.Resources.box;

        }

        public Boolean canMoveTo(int i, int j, int dx, int dy)
        {
            if (isFinished()) return false;
            if (walls[i, j] == null && blocks[i, j] == null) return true;
            if (walls[i, j] != null) return false;
            if (blocks[i, j] != null && blocks[i + dx, j + dy] == null && walls[i + dx, j + dy] == null) return true;
            return false;

        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            fixScale();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            fixScale();
        }

        private void fixScale()
        {
            parent.Scale = Math.Min(ClientSize.Width, ClientSize.Height) / (Math.Max(height,width)*100.0f);
            parent.X = (ClientSize.Width - (100 * width * parent.Scale)) / 2;
            parent.Y = (ClientSize.Height - (100 * height * parent.Scale)) / 2;
            //more code here
        }

        protected Boolean isFinished()
        {
            for (int i = 0; i < width; i++) 
            {
                for (int j = 0; j < height; j++) 
                {
                    if (goals[i, j] != null ^ blocks[i, j] != null) return false;
                }
            }
            return true;
        }

        protected void setWinText()
        {
            if(isFinished())
            {
                winScreen.setDimensions(ClientSize.Width*2, ClientSize.Height*2);
                winScreen.setOpacity(200);
                winScreen.setVisibility(true);
                winText.changeLocation(ClientSize.Width / 2 - 175, ClientSize.Height / 2 - 75);
                if (winText.x < 50) winText.x = 50;
                winText.fontResize(ClientSize.Width, ClientSize.Height);
                winText.setVisibility(true);
            }
            else
            {
                winScreen.setVisibility(false);
                winText.setVisibility(false);
            }
        }

        protected static void Setup()
        {
            map = levels[level];
            String[] lines = map.Split('\n');
            width = lines[0].Length-1;
            height = lines.Length;
            goals = new SlideSprite[width, height];
            walls = new SlideSprite[width, height];
            blocks = new SlideSprite[width, height];
            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    if (lines[j][i] == 'g' || lines[j][i] == 'B')
                    {
                        goals[i, j] = new SlideSprite(Properties.Resources.goal, i * 100, j * 100);
                        Program.parent.add(goals[i, j]);
                    }
                    if (lines[j][i] == 'w')
                    {
                        walls[i, j] = new SlideSprite(Properties.Resources.wall, i * 100, j * 100);
                        Program.parent.add(walls[i, j]);
                    }
                    if (lines[j][i] == 'b' || lines[j][i] == 'B')
                    {
                        blocks[i, j] = new SlideSprite(Properties.Resources.box, i * 100, j * 100);
                        if (lines[j][i] == 'B') blocks[i, j].image = Properties.Resources.final;

                    }
                    if (lines[j][i] == 'c')
                    {
                        player = new SlideSprite(Properties.Resources.player, i * 100, j * 100);

                        x = i;
                        y = j;

                    }

                }

            }
            for (int j = 0; j < height; j++)
                for (int i = 0; i < width; i++)
                    if (blocks[i, j] != null) Program.parent.add(blocks[i, j]);
            Program.parent.add(player);
            winScreen.setVisibility(false);
            winText.setVisibility(false);
            win.add(winScreen);
            win.add(winText);
        }

        protected static void Reset()
        {
            parent.RemoveAll();
            win.RemoveAll();
            Setup();
        }

        protected static void ChangeMap()
        {
            level = (level + 1) % levels.Length;
        }

        protected static void NewMap()
        {
            ChangeMap();
            Reset();
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            map = Properties.Resources.level1;
            Setup();
            Application.Run(new Program());
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // Program
            // 
            this.ClientSize = new System.Drawing.Size(282, 253);
            this.Name = "Program";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.ResumeLayout(false);

        }
    }
}