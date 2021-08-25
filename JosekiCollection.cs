using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Baduk;


namespace JosekiMaster
{
    class BadukJoseki
    {
        public List<BadukMove> Moves;
        public List<BadukStone> StartingStones;
        public int StartingStonesCount = 0;

        public BadukJoseki()
        {
            Moves = new List<BadukMove>();
            StartingStones = new List<BadukStone>();
        }

        public void AddMove(int pX, int pY, int pColor)
        {
            Moves.Add(new BadukMove(pColor, pX, pY));
        }

        public void ImportFromText(string pText)
        {
            StartingStones.Clear();
            Moves.Clear();
            string[] parts = pText.Split(';');
            for(int i = 0; i < parts.Count(); i++)
            {
                string[] elements = parts[i].Split(',');
                int NumElements = elements.Count();
                if(NumElements == 2)
                {
                    bool check;
                    int CommandParam;
                    check = Int32.TryParse(elements[1], out CommandParam);
                    if (check == false)
                    {
                        continue;
                    }
                    if(elements[0] == "s")
                    {
                        StartingStonesCount = CommandParam;
                    }
                }
                if (NumElements == 3)
                {
                    bool check;
                    int tempColor, temp_X, temp_Y;
                    check = Int32.TryParse(elements[0], out tempColor);
                    if (check == false)
                    {
                        continue;
                    }
                    check = Int32.TryParse(elements[1], out temp_X);
                    if (check == false)
                    {
                        continue;
                    }
                    check = Int32.TryParse(elements[2], out temp_Y);
                    if (check == false)
                    {
                        continue;
                    }
                    if(StartingStones.Count >= StartingStonesCount)
                    {
                        Moves.Add(new BadukMove(tempColor, temp_X, temp_Y));
                    }
                    else
                    {
                        StartingStones.Add(new BadukStone(temp_X, temp_Y, tempColor));
                    }
                }
            }
        }
    }

    class JosekiCollection
    {
        public List<BadukJoseki> JosekiList;

        public JosekiCollection()
        {
            JosekiList = new List<BadukJoseki>();
        }

        public void Import(string pFileName)
        {
            JosekiList.Clear();
            StreamReader sr = new StreamReader(pFileName);
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    BadukJoseki TempJoseki = new BadukJoseki();
                    TempJoseki.ImportFromText(line);
                    JosekiList.Add(TempJoseki);
                }
            }
            sr.Close();
        }

        public void Export(string pFileName)
        {
            List<string> lines = new List<string>();
            foreach(BadukJoseki CurrentJoseki in JosekiList)
            {
                string current_line = "";
                if(CurrentJoseki.StartingStones.Count > 0)
                {
                    current_line += "s," + CurrentJoseki.StartingStones.Count + ";";
                }
                for(int i = 0; i < CurrentJoseki.StartingStones.Count; i++)
                {
                    current_line += CurrentJoseki.StartingStones[i].Color.ToString() + ',' + CurrentJoseki.StartingStones[i].X.ToString() + ',' + CurrentJoseki.StartingStones[i].Y.ToString() + ";";
                }
                foreach(BadukMove CurrentMove in CurrentJoseki.Moves)
                {
                    current_line += CurrentMove.Color.ToString() + ',' +  CurrentMove.X.ToString() + ',' + CurrentMove.Y.ToString() + ";";
                }
                current_line = current_line.Trim(';');
                lines.Add(current_line);
            }

            StreamWriter outputFile = new StreamWriter(pFileName);
            foreach (string line in lines)
            {
                outputFile.WriteLine(line);
            }
            outputFile.Close();
        }

        public void Remove(int param)
        {
            if(param >= 0 && param <= JosekiList.Count - 1)
            {
                JosekiList.RemoveAt(param);
            }
        }

        public void Add(BadukJoseki pJoseki)
        {
            if(pJoseki.Moves.Count > 0)
            {
                JosekiList.Add(pJoseki);
            }
        } 
    }
}
