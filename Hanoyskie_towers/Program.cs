using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Hanoyskie_towers
{
    class Program
    {
        static void Main()
        {
            var arr = new Stack<int>[3];
            for (int i = 0; i < arr.Length; i++)
                arr[i] = new Stack<int>();
            for (int i = 4; i >= 1; i--)
            {
                arr[0].Push(i);
            }
            Towers towers = new Towers(arr);
            Console.CursorVisible = false;

            
            BeginAlg(towers, arr[0].Count);
        }
        static void BeginAlg(Towers towers, int towerHeight)
        {
            Console.Clear();
            towers.PrintTowers(0, 1);
            towers.PrintUsedCommandsAt(10, 1);
            Console.SetCursorPosition(0, 0);
            towers.SolutionHanoibns(towerHeight, 'a', 'b', 'c');
        }
    }
    class Towers
    {
        Stack<int>[] towers;
        List<string> usedCommands;
        public Towers(Stack<int>[] towers)
        {
            this.towers = towers;
            usedCommands = new List<string>();
        }
        public void InputCommand(char from, char to)
        {
            int fr = CharParser(from);
            int t = CharParser(to);
            if (towers[fr].Count >= 1)
                if (towers[t].Count == 0)
                {
                    Move(fr, t);
                    usedCommands.Add($"{ from} -> {to}");
                }
                else if (towers[fr].Peek() <= towers[t].Peek())
                {
                    Move(fr, t);
                    usedCommands.Add($"{ from} -> {to}");
                }
        }
        private int CharParser(char letter)
        {
            return (int)letter - (int)'a';
        }
        private void Move(int from, int to)
        {
            towers[to].Push(towers[from].Pop());
        }
        public void PrintUsedCommandsAt(int xCords, int yCords)
        {
            Console.SetCursorPosition(xCords, yCords);
            Console.Write("Used commands");
            for (int i = 0; i < usedCommands.Count; i++)
            {
                Console.SetCursorPosition(xCords, i + yCords + 1);
                Console.Write(usedCommands[i]);
            }
        }
        public void PrintTowers(int xCords, int yCords)
        {
            int bottom = 0;
            foreach(var tower in towers)
                if (tower.Count > bottom)
                    bottom = tower.Count;

            for (int t = 0; t < towers.Length; t++)
            {
                int[] currTower = towers[t].Reverse().ToArray();
                if (currTower.Length > 0)
                    for (int y = 0; y < currTower.Length; y++)
                    {
                        Console.SetCursorPosition(xCords + t * 3, yCords + bottom - y - 1);
                        Console.Write(currTower[y]);
                    }
            }

        }
        public void SolutionHanoibns(int k, char a, char b, char c)
        {
            if (k > 1) SolutionHanoibns(k - 1, a, c, b);
            Move(CharParser(a), CharParser(b));
            usedCommands.Add($"{a} -> {b}");

            Thread.Sleep(200);
            Console.Clear();
            PrintTowers(0, 1);
            PrintUsedCommandsAt(10, 1);

            if (k > 1) SolutionHanoibns(k - 1, c, b, a);
        }
    }
}
